using System;

namespace RoutingSimulator.Core.Exceptions
{
    public class PathDoesNotExistException : Exception
    {
        public PathDoesNotExistException() : base()
        { }

        public PathDoesNotExistException(string message) : base(message)
        { }

        public PathDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
