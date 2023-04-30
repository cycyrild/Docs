using Org.BouncyCastle.Crypto;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using System;
using static DocsWASM.Server.Mailing.Template;
namespace DocsWASM.Server.Mailing
{
    public class AccountVerification
    {
		private static Random random { get; set; }
        public Dictionary<string, (string email, DateTime deliverTime)> EmailToVerify = new();

		public AccountVerification(Random r)
		{
			random = r;
		}

		private SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
		{
			Port = 587,
			Credentials = new NetworkCredential("docsresourceforstudents@gmail.com", "mqbhifqdqkbncpdt"),
			EnableSsl = true,
		};
		public event Action<string> AccountVerified;

		private static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public async Task<string> GenerateAndSend(string mail)
        {
			var code = RandomString(5);
			EmailToVerify[code] = (mail, DateTime.Now);
			var msg = new MailMessage("Doc's <docsresourceforstudents@gmail.com>",
				mail,
				"[Doc's] Vérification de votre adresse e-mail - Code de vérification",
				VerificationCode(code));
			msg.IsBodyHtml= true;
			
			await smtpClient.SendMailAsync(msg);
			return code;
		}



    }
}
