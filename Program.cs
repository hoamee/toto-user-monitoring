using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace toto_monitoring
{
    class Program
    {
        private static IConfiguration _iconfiguration;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Canteen management database monitoring");
            GetAppSettingsFile();
            var dal = new Dal(_iconfiguration);

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);
            while (true)
            {
                var delayTask = Task.Delay(30000);
                var userList = dal.GetList();
                if (userList != null)
                {
                    Email(userList);
                    Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ": Detected " + userList.Count + " users expired. Email sent!");
                }
                await delayTask;
            }

        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }


        private static void Email(List<UserModel> ls)
        {
            try
            {
                string body = "<table><tr><th>Staff code</th><th>Name</th><th>Start date</th><th>Expire date</th><th>Created Date</th></tr>";
                ls.ForEach(item =>
                {
                    body += ("<tr><td>" + item.UsrId + "</td><td>" + item.Name + "</td><td>" + item.StDate + "</td><td>"
                    + item.ExpDate + "</td><td>" + item.CreateDate + "</td></tr>");
                });
                body += "</table>";
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("hieult212@gmail.com");
                message.To.Add(new MailAddress("le.thanh.hieu@netmarks.com.vn"));
                // message.Bcc.Add("le.thanh.hieu@netmarks.com.vn");
                message.Subject = "TOTO warning";
                message.IsBodyHtml = true;
                message.Body = "<b>User changes alert</b>"
                + "<br/> <br/>"
                + "User expire date detected. Detail: <br/>"
                + body;

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("hieult212@gmail.com", "<01001011001100/>");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        }
    }
}
