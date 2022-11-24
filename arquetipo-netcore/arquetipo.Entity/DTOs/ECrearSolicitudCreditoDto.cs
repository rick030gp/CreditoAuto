namespace arquetipo.Entity.DTOs
{
    public class ECrearSolicitudCreditoDto
    {
        public DateTime FechaElaboracion { get; set; } = DateTime.Now;
        public string IdentificacionCliente { get; set; }
        public Guid PatioId { get; set; }
        public string PlacaVehiculo { get; set; }
        public short MesesPlazo { get; set; }
        public decimal Cuotas { get; set; }
        public decimal Entrada { get; set; }
        public Guid EjecutivoId { get; set; }
        public string? Observacion { get; set; }
    }
}
