namespace arquetipo.Entity.Models
{
    public class EClientePatio
    {
        public Guid ClienteId { get; set; }
        public Guid PatioId { get; set; }
        public DateTime FechaAsignacion { get; set; }

        public EClientePatio(Guid clienteId, Guid patioId, DateTime fechaAsignacion)
        {
            ClienteId = clienteId;
            PatioId = patioId;
            FechaAsignacion = fechaAsignacion;
        }
    }
}
