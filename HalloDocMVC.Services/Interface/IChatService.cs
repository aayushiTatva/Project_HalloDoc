using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Services.Interface
{
    public interface IChatService
    {
        public bool SetConnectionId(string ConnectionString, string AspId);
        public string getConnectionId(string AspId);
        public string CreateTextFile(ChatUsersModel user);
        public List<ChatJsonObject> ReadTextFile(string fileName);
        public bool AddText(ChatUsersModel user, string msg);
        public Task<List<ChatJsonObject>> CheckHistory(ChatUsersModel user);
    }
}
