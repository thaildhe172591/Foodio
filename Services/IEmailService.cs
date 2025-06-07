using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodioAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}