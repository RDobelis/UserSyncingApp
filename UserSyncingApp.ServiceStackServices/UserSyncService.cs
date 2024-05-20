using ServiceStack;
using UserSyncingApp.ServiceInterface;
using UserSyncingApp.ServiceModel.Types;

namespace UserSyncingApp.ServiceStackServices
{
    public class UserSyncService : Service
    {
        private readonly IUserService _userService;

        public UserSyncService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<object> Any(SyncRemoteToLocal request)
        {
            await _userService.SyncRemoteToLocalAsync();
            
            return new SyncResponse { Result = "Sync from remote to local completed" };
        }

        public async Task<object> Any(SyncLocalToRemote request)
        {
            await _userService.SyncLocalToRemoteAsync();
            
            return new SyncResponse { Result = "Sync from local to remote completed" };
        }

        public object Any(UpdateUserEmail request)
        {
            _userService.UpdateUserEmail(request.UserId, request.NewEmail);
            
            return new UpdateResponse { Result = $"User with ID {request.UserId} was updated with e-mail: {request.NewEmail}" };
        }

        public object Any(DeleteUser request)
        {
            _userService.DeleteUser(request.UserId);
            
            return new DeleteResponse { Result = $"User with ID {request.UserId} was deleted" };
        }
    }
}