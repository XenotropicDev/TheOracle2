using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.Exceptions
{
    internal class MissingEmbedException : ArgumentException
    {
        public MissingEmbedException()
        {
        }

        public MissingEmbedException(string message) : base(message)
        {
        }

        public MissingEmbedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MissingEmbedException(string message, string paramName) : base(message, paramName)
        {
        }

        public MissingEmbedException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected MissingEmbedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
