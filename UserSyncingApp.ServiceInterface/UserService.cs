using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.ServiceModel.Types;
using Newtonsoft.Json;
using UserSyncingApp.Data;
using UserSyncingApp.ServiceModel.Exceptions;

namespace UserSyncingApp.ServiceInterface;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;

    public UserService(HttpClient httpClient, AppDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task SyncRemoteToLocalAsync()
    {
        var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users");
        var users = JsonConvert.DeserializeObject<List<User>>(response);

        var existingUsers = _context.Users.ToList();
        var existingUserIds = existingUsers.Select(u => u.Id).ToHashSet();

        var newUsers = users.Where(u => !existingUserIds.Contains(u.Id)).ToList();
        var updatedUsers = users.Where(u => existingUserIds.Contains(u.Id)).ToList();

        foreach (var existingUser in existingUsers)
        {
            _context.Entry(existingUser).State = EntityState.Detached;
        }

        await _context.Users.AddRangeAsync(newUsers);

        foreach (var updatedUser in updatedUsers)
        {
            _context.Users.Attach(updatedUser);
            _context.Entry(updatedUser).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
    }


    public async Task SyncLocalToRemoteAsync()
    {
        var localUsers = _context.Users.ToList();
        var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users");
        var remoteUsers = JsonConvert.DeserializeObject<List<User>>(response);

        foreach (var localUser in localUsers)
        {
            var remoteUser = remoteUsers.SingleOrDefault(u => u.Id == localUser.Id);
            Console.WriteLine(remoteUser == null
                ? $"PUT: {JsonConvert.SerializeObject(localUser)}"
                : $"UPDATE: {JsonConvert.SerializeObject(localUser)}");
        }
    }

    public void UpdateUser(int userId, string newEmail)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        
        user.Email = newEmail;
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void DeleteUser(int userId)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        
        _context.Users.Remove(user);
        _context.SaveChanges();
    }
}

