using FoodioAPI.DTOs.Search;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodioAPI.Database.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserByRefreshTokenAsync(string refreshToken);
}

