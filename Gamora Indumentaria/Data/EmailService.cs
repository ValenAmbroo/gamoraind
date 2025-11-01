using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Gamora_Indumentaria.Data
{
    public static class EmailService
    {
        private static SmtpClient BuildClient()
        {
            string host = ConfigurationManager.AppSettings["SmtpHost"] ?? "";
            int port = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var p) ? p : 587;
            bool enableSsl = bool.TryParse(ConfigurationManager.AppSettings["SmtpEnableSsl"], out var s) ? s : true;
            string user = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
            string pass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";

            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("SMTP no configurado (SmtpHost vacío). Configure App.config.");

            var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            if (!string.IsNullOrWhiteSpace(user))
            {
                client.Credentials = new NetworkCredential(user, pass);
            }

            return client;
        }

        public static void Send(string toAddressesCsv, string subject, string body, bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toAddressesCsv))
                throw new ArgumentException("Destinatarios vacíos");

            string from = ConfigurationManager.AppSettings["SmtpFrom"] ?? (ConfigurationManager.AppSettings["SmtpUser"] ?? "no-reply@localhost");

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(ExtractEmail(from), from);
                foreach (var to in toAddressesCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(to.Trim());
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                using (var client = BuildClient())
                {
                    client.Send(message);
                }
            }
        }

        private static string ExtractEmail(string input)
        {
            // Permite "Nombre <correo@dominio>" o solo correo
            try
            {
                var addr = new MailAddress(input);
                return addr.Address;
            }
            catch { return input; }
        }
    }
}
