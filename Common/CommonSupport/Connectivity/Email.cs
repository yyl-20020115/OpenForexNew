using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace CommonSupport
{
    /// <summary>
    /// Email helper class.
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// Send general email, fully configurable.
        /// </summary>
        /// <returns></returns>
        public static bool SendEmailAsync(string username, string password, string smtpServer, int port, string from, string to,
            string subject, string body, SendCompletedEventHandler handler)
        {
            SmtpClient client = new SmtpClient(smtpServer, port);
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(username, password);

            MailMessage message = new MailMessage(from, to);
            message.Body = body;
            message.Subject = subject;

            try
            {
                client.SendAsync(message, null);
                if (handler != null)
                {
                    client.SendCompleted += handler;
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to send mail.", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send email trough GMAIL SSL.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="handler"></param>
        public static bool SendGmailSSLAsync(string username, string password, string from, string to, 
            string subject, string body, SendCompletedEventHandler handler)
        {
            return SendEmailAsync(username, password, "smtp.gmail.com", 587, from, to, subject, body, handler);
        }

    }
}
