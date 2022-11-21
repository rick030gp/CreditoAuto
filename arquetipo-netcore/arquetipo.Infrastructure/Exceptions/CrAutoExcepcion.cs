using arquetipo.Entity.Models;

namespace arquetipo.Infrastructure.Exceptions
{
    public class CrAutoExcepcion : Exception
    {
        public string Code { get; set; }
        public string? Details { get; set; }

        public CrAutoExcepcion(Error error, Exception? innerException = null) 
            : base(error.Message, innerException)
        {
            Code = error.Code;
            Details = innerException?.StackTrace;
        }
    }
}
