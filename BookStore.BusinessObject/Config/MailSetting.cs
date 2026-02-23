using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.Config
{
    public class MailSetting
    {
        /*
        Purpose : Cấu hình Mail Settings
        */
        public class MailSettings
        {
            public string SmtpServer { get; set; } = "";
            public int Port { get; set; }
            public string SenderName { get; set; } = "";
            public string SenderEmail { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Password { get; set; } = "";
            public bool EnableSSL { get; set; } 

            public required string BaseUrl { get; set; } // thêm vào đây

        }
    }
}
