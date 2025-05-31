using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using FoodioAPI.DTOs.Configs;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FoodioAPI.Services.Implements;
public class EmailService : IEmailSender
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

}