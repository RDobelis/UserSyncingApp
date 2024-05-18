using System;

namespace UserSyncingApp.ServiceModel.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(int userId) : base($"User with ID {userId} not found") { }
}