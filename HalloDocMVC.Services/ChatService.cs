using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepository<ChatMessage> _chatMessageRepository;
        private readonly IGenericRepository<ChatConnection> _chatConnectionRepository;

        public ChatService(IGenericRepository<ChatMessage> iChatMessageRepository, IGenericRepository<ChatConnection> iChatConnectionRepository)
        {
            _chatMessageRepository = iChatMessageRepository;
            _chatConnectionRepository = iChatConnectionRepository;
        }

        public bool SetConnectionId(string ConnectionString, string AspId)
        {
            var cdo = _chatConnectionRepository.GetAll().Where(e => e.Aspnetuserid == AspId).FirstOrDefault();
            if(cdo != null)
            {
                cdo.ConnectionString = ConnectionString;
                _chatConnectionRepository.Update(cdo);
            }
            else{
                ChatConnection cd = new ChatConnection();
                cd.ConnectionString = ConnectionString;
                cd.Aspnetuserid = AspId;
                _chatConnectionRepository.Add(cd);
            }
            return true;
        }
        public string getConnectionId(string AspId)
        {
            var result = _chatConnectionRepository.GetAll().Where(e => e.Aspnetuserid == AspId).Select(e => e.ConnectionString).FirstOrDefault();
            return result;
        }
        public string CreateTextFile(ChatUsersModel user)
        {
            string fileName = user.SenderId + user.SenderType + "_" + user.ReceiverId + user.ReceiverType + "_" + user.RequestId + ".txt";
            string FilePath = "wwwroot\\Upload\\ChatFile\\" + fileName;
            try
            {

                if (File.Exists(FilePath))
                {

                    File.Delete(FilePath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                // Create a new file
                using (StreamWriter sw = File.CreateText(FilePath))
                {

                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            return fileName;
        }

        public List<ChatJsonObject> ReadTextFile(string fileName)
        {
            List<ChatJsonObject> chatList = new List<ChatJsonObject>();

            using (StreamReader sr = File.OpenText("wwwroot\\Upload\\ChatFile\\" + fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ChatJsonObject chat = JsonConvert.DeserializeObject<ChatJsonObject>(line);

                    chatList.Add(chat);
                }
            }
            return chatList;
        }

        public bool AddText(ChatUsersModel user, string msg)
        {
            try
            {
                var data = _chatMessageRepository.GetAll().Where(e => e.RequestId == user.RequestId && e.RecipientId == user.ReceiverId && e.SenderId == user.SenderId ||
                               e.RequestId == user.RequestId && e.RecipientId == user.SenderId && e.SenderId == user.ReceiverId).FirstOrDefault();
                    ChatMessage message = new ChatMessage
                    {
                        Message = msg,
                        SenderName = user.SenderName,
                        SenderType = user.SenderType,
                        RecipientName = user.ReceiverName,
                        RecipientType = user.ReceiverType,
                        RequestId = user.RequestId,
                        RecipientId = user.ReceiverId,
                        SenderId = user.SenderId,
                        CreatedDate = DateTime.Now,
                    }; _chatMessageRepository.Add(message);
              

                ChatJsonObject chatJsonObject = new ChatJsonObject
                {
                    Message = msg,
                    Datetime = DateTime.Now,
                    RequestId = user.RequestId
                };

                if (user.SenderType == "Admin")
                {
                    chatJsonObject.AdminId = user.SenderId;
                }
                else if (user.SenderType == "Provider")
                {
                    chatJsonObject.PhysicianId = user.SenderId;
                }
                if(user.SenderType == "Patient")
                {
                    chatJsonObject.AdminId = user.ReceiverId;
                }
                string json = JsonConvert.SerializeObject(chatJsonObject);

                using (StreamWriter sw = File.AppendText("wwwroot\\Upload\\ChatFile\\" + user.RequestId))
                {
                    sw.WriteLine(json);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            return true;
        }

        public async Task<List<ChatJsonObject>> CheckHistory(ChatUsersModel user)
        {
            try
            {
                var data = _chatMessageRepository.GetAll()
                            .Where(
                            e => e.RequestId == user.RequestId && e.RecipientId == user.ReceiverId && e.SenderId == user.SenderId
                                                                         ||
                                e.RequestId == user.RequestId && e.RecipientId == user.SenderId && e.SenderId == user.ReceiverId
                            ).FirstOrDefault();
                /*if (data == null)
                {
                    var ChatMessage = new ChatMessage();
                    ChatMessage.RequestId = user.RequestId;
                    ChatMessage.SenderName = user.SenderName;
                    ChatMessage.SenderType = user.SenderType;
                    ChatMessage.SenderId = user.SenderId;
                    ChatMessage.RecipientType = user.ReceiverType;
                    ChatMessage.RecipientId = user.ReceiverId;
                    ChatMessage.RecipientName = user.ReceiverName;*//*
                    ChatMessage.FilePath = CreateTextFile(user);*//*
                    ChatMessage.CreatedDate = DateTime.Now;
                    _chatMessageRepository.Add(ChatMessage);

                    return ReadTextFile(ChatMessage.FilePath);
                }
                else
                {
                    return ReadTextFile(data.FilePath);
                }*/
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            return new List<ChatJsonObject> { };
        }
    }
}
