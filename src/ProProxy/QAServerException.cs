/// <summary>
/// Summary description for QAServerException
/// </summary>
namespace Experian.Qas.Proweb
{
    using System;

    [global::System.Serializable]
    public class QAServerException : ApplicationException
    {
        public QAServerException()
        {
        }

        public QAServerException(string message)
            : base(message)
        {
        }

        public QAServerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected QAServerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}