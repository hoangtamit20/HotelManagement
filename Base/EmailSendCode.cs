using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKSMVC.Base
{
    public class EmailSendCode
    {
        private readonly string password = "kudiwwwxjypoffra";
        private readonly string emailSender = "thesenegalteam@gmail.com";

        public EmailSendCode()
        {
        }

        public string Password => password;

        public string EmailSender => emailSender;
    }
}