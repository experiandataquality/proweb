/// <summary>
/// Summary description for QAServerException
/// </summary>
namespace Experian.Qas.Proweb
{
    using System;

    public class QAServerException : Exception
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

    }
}