using System;
using System.Runtime.Serialization;

namespace Cthulhu.Serialization
{
    public class WorldParseException : Exception
    {
        public WorldParseException()
        {
        }

        public WorldParseException(string message) : base(message)
        {
        }

        public WorldParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WorldParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}