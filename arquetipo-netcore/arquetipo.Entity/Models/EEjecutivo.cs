namespace arquetipo.Entity.Models
{
    public class EEjecutivo
    {
        public Guid Id { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string TelefonoConvencional { get; set; }
        public string Celular { get; set; }
        public Guid PatioId { get; set; }
        public short Edad { get; set; }

        public EEjecutivo(Guid id,
            string identificacion,
            string nombres,
            string apellidos,
            string direccion,
            string telefonoConvencional,
            string celular,
            Guid patioId,
            short edad)
        {
            Id = id;
            Identificacion = identificacion;
            Nombres = nombres;
            Apellidos = apellidos;
            Direccion = direccion;
            TelefonoConvencional = telefonoConvencional;
            Celular = celular;
            PatioId = patioId;
            Edad = edad;
        }
    }
}
