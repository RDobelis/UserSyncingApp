using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyApp.ServiceModel.Types;
using Newtonsoft.Json;
using UserSyncingApp.Data;

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

        foreach (var user in users)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                _context.Users.Add(user);
            }
            else
            {
                _context.Entry(existingUser).CurrentValues.SetValues(user);
            }
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
            if (remoteUser == null)
            {
                Console.WriteLine($"Prepare to insert: {JsonConvert.SerializeObject(localUser)}");
            }
            else
            {
                Console.WriteLine($"Prepare to update: {JsonConvert.SerializeObject(localUser)}");
            }
        }
    }

    public void UpdateUser(int userId, string newEmail)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        
        if (user == null)
        {
            return;
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
            return;
        }
        
        _context.Users.Remove(user);
        _context.SaveChanges();
    }
}

