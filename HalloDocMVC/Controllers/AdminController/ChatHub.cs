using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ChatHub : Hub
    {
        private readonly IChatService _ChatService;
        private readonly INotyfService _NotyfService;
        private readonly IGenericRepository<ChatMessage> _chatMessageRepository;
        private readonly IGenericRepository<ChatConnection> _chatConnectionRepository;

        public ChatHub(IChatService chatService, INotyfService notyfService, IGenericRepository<ChatMessage> iChatMessageRepository, IGenericRepository<ChatConnection> iChatConnectionRepository)
        {
            _ChatService = chatService;
            _NotyfService = notyfService;
            _chatMessageRepository = iChatMessageRepository;
            _chatConnectionRepository = iChatConnectionRepository;
        }
        public override Task OnConnectedAsync()
        {
            ChatUsersModel users = new ChatUsersModel();
            users.ConnectionId = Context.ConnectionId;
            users.SenderAspId = CV.ID();
            users.SenderType = CV.role();
            if (!ConnectionUsersModel.activeUsers.Any(e => e.ConnectionId == Context.ConnectionId))
            {
                ConnectionUsersModel.activeUsers.Add(users);
                _ChatService.SetConnectionId(Context.ConnectionId, CV.ID());

            }
            return base.OnConnectedAsync();
        }
        public async Task SendToUser(string user, string receiver, string message, string requestid, string receiverid, string receiverType, string receivername)
        {
            var receiverConnectionId = _ChatService.getConnectionId(receiver);
            ChatUsersModel chatusers = ConnectionUsersModel.activeUsers.Where(x => x.SenderAspId == CV.ID()).FirstOrDefault();
            chatusers.ReceiverId = Convert.ToInt32(receiverid);
            chatusers.ReceiverType = receiverType;
            chatusers.ReceiverName = receivername;
            chatusers.RequestId = Convert.ToInt32(requestid);
            chatusers.SenderId = Convert.ToInt32(CV.UserID());
            chatusers.SenderName = CV.UserName();
            _ChatService.AddText(chatusers, message);
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", user, message, requestid);
                      
        }

        public string GetConnectionId() => Context.ConnectionId;

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ChatUsersModel users = ConnectionUsersModel.activeUsers.Where(e => e.ConnectionId == Context.ConnectionId).FirstOrDefault();

            ConnectionUsersModel.activeUsers.Remove(users);

            return base.OnDisconnectedAsync(exception);
        }
        public async Task<List<ChatJsonObject>> CheckHistory(string requestId, string RecieverId, string RecieverName, string RecieverType)
        {
            try
            {
                ChatUsersModel chatUser = ConnectionUsersModel.activeUsers.FirstOrDefault(u => u.SenderAspId == CV.ID());
                if (chatUser != null)
                {
                    chatUser.ReceiverType = RecieverType;
                    chatUser.ReceiverId = Convert.ToInt32(RecieverId);
                    chatUser.ReceiverName = RecieverName;
                    chatUser.RequestId = Convert.ToInt32(requestId);
                    chatUser.SenderId = Convert.ToInt32(CV.UserID());
                    chatUser.SenderName = CV.UserName();
                    return await _ChatService.CheckHistory(chatUser);
                }
                else
                {
                    // Handle case where ChatUser is not found
                    return new List<ChatJsonObject>();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error in CheckHistory: {ex.Message}");
                throw; // Rethrow the exception to propagate it further
            }
        }

    }
}
