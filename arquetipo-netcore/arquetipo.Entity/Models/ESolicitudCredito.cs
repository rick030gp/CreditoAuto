namespace arquetipo.Entity.Models
{
    public class ESolicitudCredito
    {
        public Guid Id { get; set; }
        public DateTime FechaElaboracion { get; set; }
        public Guid ClienteId { get; set; }
        public Guid PatioId { get; set; }
        public Guid VehiculoId { get; set; }
        public short MesesPlazo { get; set; }
        public decimal Cuotas { get; set; }
        public decimal Entrada { get; set; }
        public Guid EjecutivoId { get; set; }
        public string Observacion { get; set; }
        public EstadoSolicitud Estado { get; set; }
    }

    public enum EstadoSolicitud
    {
        Registrada,
        Despachada,
        Cancelada
    }
}
