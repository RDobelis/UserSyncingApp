using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UserSyncingApp.Data;
using UserSyncingApp.ServiceModel.Exceptions;
using UserSyncingApp.ServiceModel.Types;

namespace UserSyncingApp.ServiceInterface
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly string _userDataEndpoint;

        public UserService(HttpClient httpClient, AppDbContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _userDataEndpoint = configuration["UserDataEndpoint"];
        }

        public async Task SyncRemoteToLocalAsync()
        {
            var users = await FetchRemoteUsersAsync();
            var existingUserIds = _context.Users.AsNoTracking().Select(u => u.Id).ToHashSet();

            var newUsers = GetNewUsers(users, existingUserIds);
            var updatedUsers = GetUpdatedUsers(users, existingUserIds);

            DetachExistingUsers();
            await AddNewUsersAsync(newUsers);
            UpdateExistingUsers(updatedUsers);

            await _context.SaveChangesAsync();
        }

        public async Task SyncLocalToRemoteAsync()
        {
            var localUsers = await _context.Users.ToListAsync();
            var remoteUsers = await FetchRemoteUsersAsync();

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
            var user = ValidateLocalUser(userId);
            
            user.Email = newEmail;
            
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var user = ValidateLocalUser(userId);
            
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        private async Task<List<User>> FetchRemoteUsersAsync()
        {
            var response = await _httpClient.GetStringAsync(_userDataEndpoint);
            
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        private static List<User> GetNewUsers(IEnumerable<User> users, HashSet<int> existingUserIds) => 
            users.Where(u => !existingUserIds.Contains(u.Id)).ToList();

        private static List<User> GetUpdatedUsers(IEnumerable<User> users, HashSet<int> existingUserIds) => 
            users.Where(u => existingUserIds.Contains(u.Id)).ToList();

        private void DetachExistingUsers()
        {
            var existingUsers = _context.Users.ToList();
            
            foreach (var existingUser in existingUsers)
            {
                _context.Entry(existingUser).State = EntityState.Detached;
            }
        }

        private async Task AddNewUsersAsync(IEnumerable<User> newUsers) => 
            await _context.Users.AddRangeAsync(newUsers);

        private void UpdateExistingUsers(IEnumerable<User> updatedUsers)
        {
            foreach (var updatedUser in updatedUsers)
            {
                _context.Users.Attach(updatedUser);
                _context.Entry(updatedUser).State = EntityState.Modified;
            }
        }

        private User ValidateLocalUser(int userId)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            
            return user;
        }
    }
}
