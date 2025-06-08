using SendGrid;
using SendGrid.Helpers.Mail;
using FoodioAPI.Configs;
using Microsoft.Extensions.Options;
using FoodioAPI.Services;

namespace FoodioAPI.Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;

        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string verificationLink)
        {
            var apiKey = _emailConfig.Key;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_emailConfig.OwnerMail, _emailConfig.Company);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, verificationLink);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendTemplateEmailAsync(string toEmail, string templateId, object dynamicData)
        {
            var apiKey = _emailConfig.Key;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_emailConfig.OwnerMail, _emailConfig.Company);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
    }
}