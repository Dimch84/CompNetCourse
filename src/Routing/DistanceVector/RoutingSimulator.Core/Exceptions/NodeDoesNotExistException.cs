using System;

namespace RoutingSimulator.Core.Exceptions
{
    public class NodeDoesNotExistException : Exception
    {
        public NodeDoesNotExistException() : base()
        { }

        public NodeDoesNotExistException(string message) : base(message)
        { }

        public NodeDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
