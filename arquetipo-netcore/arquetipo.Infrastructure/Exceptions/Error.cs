using arquetipo.Infrastructure.Exceptions;

namespace arquetipo.Entity.Models
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
        
        public Error() {}

        public Error(CrAutoExcepcion exception)
        {
            Code = exception.Code;
            Message = exception.Message;
            Details = exception.Details;
        }
    }
}
