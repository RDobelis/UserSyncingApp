using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using UserSyncingApp.Data;
using UserSyncingApp.ServiceModel.Exceptions;
using UserSyncingApp.ServiceModel.Types;
using UserSyncingApp.Tests.Mocks;

namespace UserSyncingApp.ServiceInterface.Tests;

[TestFixture]
public class UserServiceTests
{
    private UserService _userService;
    private HttpClient _httpClient;
    private AppDbContext _context;
    private MockHttpMessageHandler _httpMessageHandler;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new AppDbContext(options);

        var responseMessage = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(new List<User>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new() { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            }))
        };

        _httpMessageHandler = new MockHttpMessageHandler(responseMessage);
        _httpClient = new HttpClient(_httpMessageHandler);
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { { "UserDataEndpoint", "https://example.com/users" } })
            .Build();

        _userService = new UserService(_httpClient, _context, configuration);
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _httpClient.Dispose();
    }

    [Test]
    public async Task SyncRemoteToLocalAsync_WhenCalledAndNoLocalUsers_ShouldInsertUsers()
    {
        await _userService.SyncRemoteToLocalAsync();

        _context.Users.Count().Should().Be(2);
        _context.Users.Should().Contain(u => u.Name == "John Doe" && u.Email == "john@example.com");
        _context.Users.Should().Contain(u => u.Name == "Jane Doe" && u.Email == "jane@example.com");
    }
    
    [Test]
    public async Task SyncRemoteToLocalAsync_WhenCalledWithExistingLocalUsers_ShouldUpdateUsers()
    {
        var existingUser = new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        await _userService.SyncRemoteToLocalAsync();

        var users = await _context.Users.ToListAsync();
        users.Should().Contain(u => u.Name == "John Doe" && u.Email == "john@example.com");
    }
    
    [Test]
    public async Task SyncLocalToRemoteAsync_WhenCalledWithUpdatedValue_ShouldUpdateRemoteUser()
    {
        var localUser = new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
        await _context.Users.AddAsync(localUser);
        await _context.SaveChangesAsync();
        await using var sw = new StringWriter();
        Console.SetOut(sw);
        
        await _userService.SyncLocalToRemoteAsync();

        var output = sw.ToString();
        output.Should().Contain("UPDATE");
        output.Should().Contain("john.doe@example.com");
    }

    [Test]
    public void UpdateUser_WhenCalled_ShouldUpdateUserEmail()
    {
        var localUser = new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
        _context.Users.Add(localUser);
        _context.SaveChanges();

        _userService.UpdateUserEmail(1, "new.email@example.com");

        var updatedUser = _context.Users.SingleOrDefault(u => u.Id == 1);
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Be("new.email@example.com");
    }
    
    [Test]
    public void UpdateUser_WhenUserNotFound_ShouldThrowUserNotFoundException()
    {
        var act = () => _userService.UpdateUserEmail(1, "test@example.com");
        
        act.Should().Throw<UserNotFoundException>().WithMessage("User with id 1 not found");
    }

    [Test]
    public void DeleteUser_WhenCalled_ShouldDeleteUser()
    {
        var user = new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
        _context.Users.Add(user);
        _context.SaveChanges();

        _userService.DeleteUser(1);

        var deletedUser = _context.Users.SingleOrDefault(u => u.Id == 1);
        deletedUser.Should().BeNull();
    }
    
    [Test]
    public void DeleteUser_WhenUserNotFound_ShouldThrowUserNotFoundException()
    {
        var act = () => _userService.DeleteUser(1);
        
        act.Should().Throw<UserNotFoundException>().WithMessage("User with id 1 not found");
    }
}