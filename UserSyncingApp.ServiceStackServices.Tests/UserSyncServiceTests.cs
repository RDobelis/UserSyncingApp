using NSubstitute;
using UserSyncingApp.ServiceInterface;
using UserSyncingApp.ServiceModel.Types;
using FluentAssertions;

namespace UserSyncingApp.ServiceStackServices.Tests;

[TestFixture]
public class UserSyncServiceTests
{
    private UserSyncService _userSyncService;
    private IUserService _userService;
    
    [SetUp]
    public void SetUp()
    {
        _userService = Substitute.For<IUserService>();
        _userSyncService = new UserSyncService(_userService);
    }
    
    [TearDown]
    public void TearDown()
    {
        _userService.ClearReceivedCalls();
        _userSyncService.Dispose();
    }
    
    [Test]
    public async Task Any_SyncRemoteToLocal_ShouldCallSyncRemoteToLocalAsync()
    {
        var request = new SyncRemoteToLocal();
        
        var result = await _userSyncService.Any(request) as SyncResponse;
        
        await _userService.Received(1).SyncRemoteToLocalAsync();
        result.Should().NotBeNull();
        result!.Result.Should().Be("Sync from remote to local completed");
    }
    
    [Test]
    public async Task Any_SyncLocalToRemote_ShouldCallSyncLocalToRemoteAsync()
    {
        var request = new SyncLocalToRemote();
        
        var result = await _userSyncService.Any(request) as SyncResponse;
        
        await _userService.Received(1).SyncLocalToRemoteAsync();
        result.Should().NotBeNull();
        result!.Result.Should().Be("Sync from local to remote completed");
    }

    [Test]
    public void Any_UpdateUserEmail_ShouldCallUpdateUserEmail()
    {
        var request = new UpdateUserEmail { UserId = 1, NewEmail = "newEmail@example.com" };

        var result = _userSyncService.Any(request) as UpdateResponse;

        _userService.Received(1).UpdateUserEmail(1, "newEmail@example.com");
        result.Should().NotBeNull();
        result!.Result.Should().Be("User email updated");
    }
    
    [Test]
    public void Any_DeleteUser_ShouldCallDeleteUser()
    {
        var request = new DeleteUser { UserId = 1 };

        var result = _userSyncService.Any(request) as DeleteResponse;

        _userService.Received(1).DeleteUser(1);
        result.Should().NotBeNull();
        result!.Result.Should().Be("User deleted");
    }
}