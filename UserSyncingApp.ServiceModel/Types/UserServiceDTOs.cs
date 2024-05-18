using ServiceStack;

namespace UserSyncingApp.ServiceModel.Types
{
    [Route("/users/sync-remote-to-local", "POST")]
    public class SyncRemoteToLocal : IReturn<SyncResponse> { }

    [Route("/users/sync-local-to-remote", "POST")]
    public class SyncLocalToRemote : IReturn<SyncResponse> { }

    [Route("/users/update-email", "PUT")]
    public class UpdateUserEmail : IReturn<UpdateResponse>
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; }
    }

    [Route("/users/delete", "DELETE")]
    public class DeleteUser : IReturn<DeleteResponse>
    {
        public int UserId { get; set; }
    }

    public class SyncResponse
    {
        public string Result { get; set; }
    }

    public class UpdateResponse
    {
        public string Result { get; set; }
    }

    public class DeleteResponse
    {
        public string Result { get; set; }
    }
}