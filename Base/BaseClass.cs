using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace QLKSMVC.Base
{
    public class BaseClass
    {
        public string hashString(string strHash)
        {
            string strResult = "";
            foreach (var s in strHash)
            {
                if (Char.IsDigit(s))
                {
                    if ((int)s < 53 && (int)s >= 48 && (int)s % 2 != 0)
                    {
                        strResult += (char)((int)s + 65) + "mcrs";
                    }
                    else
                    {
                        strResult += (char)((int)s + 65) + "losc";
                    }
                }
                else if (s.Equals(';'))
                {
                    strResult += (char)(182) + "comec";
                }
                else
                {
                    if ((int)s > 96 && (int)s < 123)
                    {
                        if ((int)s > 109)
                        {
                            strResult += (char)((int)s - 45) + "lops";
                        }
                        else
                        {
                            strResult += (char)((int)s - 19) + "lecs";
                        }
                    }
                }
            }
            return strResult;
        }

        public string splitString(string s, string strSplit)
        {
            string strResult = "";
            string[] kq = s.Split(strSplit);
            for (int i = 0; i < kq.Length; i++)
            {
                strResult += kq[i];
            }
            return strResult;
        }

        public string deCodeHash(string strCode)
        {
            string strResult = "";
            strResult = splitString(strCode, "mcrs");
            strResult = splitString(strResult, "comec");
            strResult = splitString(strResult, "losc");
            strResult = splitString(strResult, "lops");
            strResult = splitString(strResult, "lecs");
            string kq = "";
            foreach (var s in strResult)
            {
                if ((int)s >= 78 && (int)s <= 90)
                {
                    kq += (char)((int)s + 19);
                }
                else if ((int)s >= 65 && (int)s <= 77)
                {
                    kq += (char)((int)s + 45);
                }
                else if ((int)s == 182)
                {
                    kq += ";";
                }
                else if ((int)s >= 97 && (int)s <= 122)
                {
                    kq += (char)((int)s - 65);
                }
            }
            return kq;
        }


        public string randEmailCode()
        {
            Random rd = new Random();
            string strCode = "";
            for (int i = 0; i < 6; i++)
            {
                if (i % 3 == 0)
                {
                    strCode += (char)(rd.Next(65, 90));
                }
                else if (i % 2 == 0)
                {
                    strCode += (char)(rd.Next(48, 57));
                }
                else
                {
                    strCode += (char)(rd.Next(97, 122));
                }

            }
            return strCode;
        }


        public bool sendEmailGetCode(string strCode, string email)
        {
            EmailSendCode sendCode = new EmailSendCode();
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress(sendCode.EmailSender);
                mail.Subject = "Password Recovery";
                string Body = strCode;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(sendCode.EmailSender, sendCode.Password); // Enter seders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string randomToken()
        {
            string result = "";
            Random rd = new Random();
            for (int i = 1 ; i <= 255 ; i++)
            {
                result += (char)(rd.Next(48,127));
            }
            return result;
        }
    }
}