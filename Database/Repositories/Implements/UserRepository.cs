using FoodioAPI.Entities;
using FoodioAPI.Database.Repositories;
using FoodioAPI.Database.Repositories.Implements;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.DTOs.Search;
using FoodioAPI.Helpers;
using FoodioAPI.Database;
using Google;


namespace FoodioAPI.Database.Repositories.Implements;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> FindUserByRefreshTokenAsync(string refreshToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}

