using System;
using System.Runtime.Serialization;

namespace XMAS2019.Domain.Exceptions
{
    [Serializable]
    public class AttemptVersionMismatchException : Exception
    {
        public AttemptVersionMismatchException(long actualVersion, int expectedVersion)
            : this($"Expected version {expectedVersion} but actual version is: {actualVersion}")
        {
        }

        public AttemptVersionMismatchException()
        {
        }

        public AttemptVersionMismatchException(string message) : base(message)
        {
        }

        public AttemptVersionMismatchException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AttemptVersionMismatchException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}