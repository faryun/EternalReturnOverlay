using System;

namespace EternalReturnOverlay
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message)
        {
        }
    }
}
