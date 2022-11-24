namespace arquetipo.Entity.Models
{
    public class EPatio
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public short NumeroPuntoVenta { get; set; }
        public List<EEjecutivo> Ejecutivos { get; set; }

        public EPatio(
            Guid id,
            string nombre,
            string direccion,
            string telefono,
            short numeroPuntoVenta)
        {
            Id = id;
            Nombre = nombre;
            Direccion = direccion;
            Telefono = telefono;
            NumeroPuntoVenta = numeroPuntoVenta;
            Ejecutivos = new List<EEjecutivo>();
        }
    }
}
