using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace HR_Automation_System.Classes
{
    // Класс отправки электронной почты
    public class EmailUtility
    {
        private MailAddress _senderAddress;  // Адрес отправителя (в нашем случае bot.robot.92@mail.ru) 
        private MailAddress _receiverAddress;  // Адрес получателя
        private SmtpClient _smtpClient; // Клиент для отправки почты

        public EmailUtility(string receiverAddress)
        {
            _senderAddress = new MailAddress("bot.robot.92@mail.ru", "Bot");
            _receiverAddress = new MailAddress(receiverAddress);
            _smtpClient = new SmtpClient("smtp.mail.ru", 587); // Адрес и порт SMTP-сервера
            // Логин и пароль для отправок писем
            _smtpClient.Credentials = new NetworkCredential("bot.robot.92@mail.ru", "5sv-H5d-bUy-PLS");
            _smtpClient.EnableSsl = true;
        }

        // Создание и отправка сообщения об аттестации
        public bool SendGraduationMail(string name, string date)
        {
            try
            {
                // Cоздаем объект сообщения
                MailMessage m = new MailMessage(_senderAddress, _receiverAddress);
                // Тема письма        
                m.Subject = "Аттестация";
                // Текст письма
                m.Body = string.Format("Здравствуйте, {0}!<br><br> " +
                    "Вам назначена аттестация на {1}.<br><br>" +
                    "С Уважением,<br>" +
                    "Команда HR.", name, date);
                m.IsBodyHtml = true;            
                _smtpClient.Send(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Во время отправки сообщения произошла ошибка: \n{0}", ex.Message),
                    "Ошибка при отправке Email",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            MessageBox.Show("Письмо с уведомлением об аттестации успешно отправлено",
                "Аттестация", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
    }
}
