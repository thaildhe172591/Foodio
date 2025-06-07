using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodioAPI.Services
{
    public interface IStorageService
    {
        Task<Uri> UploadFileAsync(string name, IFormFile file);
    }
}