using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class EmailConfiguration
    {
        //details of sender's mail
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        #region SendMail
        //details of reciever's mail
        public async Task<bool> SendMail(String To, String Subject, String Body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", From));
                message.To.Add(new MailboxAddress("", To));
                message.Subject = Subject;
                message.Body = new TextPart("html")
                {
                    Text = Body
                };
                using (var client = new MailKit.Net.Smtp.SmtpClient()) //steps of mail sending
                {
                    await client.ConnectAsync(SmtpServer, Port, false);
                    await client.AuthenticateAsync(UserName, Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        #endregion

        #region Encode
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        #endregion

        #region Decode
        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion
    }
}
