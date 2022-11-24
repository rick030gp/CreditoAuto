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
        public string? Observacion { get; set; }
        public EstadoSolicitud Estado { get; set; }

        public ESolicitudCredito(
            Guid id,
            DateTime fechaElaboracion,
            Guid clienteId,
            Guid patioId,
            Guid vehiculoId,
            short mesesPlazo,
            decimal cuotas,
            decimal entrada,
            Guid ejecutivoId,
            string? observacion = null)
        {
            Id = id;
            FechaElaboracion = fechaElaboracion;
            ClienteId = clienteId;
            PatioId = patioId;
            VehiculoId = vehiculoId;
            MesesPlazo = mesesPlazo;
            Cuotas = cuotas;
            Entrada = entrada;
            EjecutivoId = ejecutivoId;
            Observacion = observacion;
            Estado = EstadoSolicitud.Registrada;
        }
    }

    public enum EstadoSolicitud
    {
        Registrada,
        Despachada,
        Cancelada
    }
}
