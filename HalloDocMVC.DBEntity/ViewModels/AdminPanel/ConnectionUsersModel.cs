using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ConnectionUsersModel
    {
        public static List<ChatUsersModel> activeUsers = new List<ChatUsersModel>();
    }
    public class ChatUsersModel
    {
        public string ConnectionId { get; set; }
        public int RequestId { get; set; }
        public string SenderAspId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderType { get; set; }
        public string ReceiverAspId { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverType { get; set; }
    }

    public class ChatJsonObject
    {
        public int AdminId { get; set; }
        public int PhysicianId { get; set; }
        public int RequestId { get; set; }
        public string Message { get; set; }
        public DateTime Datetime { get; set; }
    }
}
