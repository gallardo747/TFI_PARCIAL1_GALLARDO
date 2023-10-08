using System;

namespace VETLY_BLL.BusinessExceptions
{
    public class EnvioMQErroneaException : Exception
    {
        public EnvioMQErroneaException(string message) : base(message)
        {

        }
    }
}
