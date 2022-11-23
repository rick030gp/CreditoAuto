namespace arquetipo.Entity.Models
{
    public class EClientePatio
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid PatioId { get; set; }
        public DateTime? FechaAsignacion { get; set; }

        public EClientePatio(
            Guid id,
            Guid clienteId,
            Guid patioId,
            DateTime? fechaAsignacion = null)
        {
            Id = id;
            ClienteId = clienteId;
            PatioId = patioId;
            FechaAsignacion = fechaAsignacion;
        }
    }
}
