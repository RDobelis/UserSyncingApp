using System.Threading.Tasks;

namespace UserSyncingApp.ServiceInterface;

public interface IUserService
{
    Task SyncRemoteToLocalAsync();
    Task SyncLocalToRemoteAsync();
    void UpdateUserEmail(int userId, string newEmail);
    void DeleteUser(int userId);
}