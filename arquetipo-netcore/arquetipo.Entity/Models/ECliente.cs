namespace arquetipo.Entity.Models
{
    public class ECliente
    {
        public Guid Id { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public short Edad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string EstadoCivil { get; set; }
        public string IdentificacionConyugue { get; set; }
        public string NombreConyugue { get; set; }
        public bool EsSujetoCredito { get; set; }

        public ECliente(
            Guid id,
            string identificacion,
            string nombres,
            string apellidos,
            short edad,
            DateTime fechaNacimiento,
            string direccion,
            string telefono,
            string estadoCivil,
            string identificacionConyugue,
            string nombreConyugue,
            bool esSujetoCredito)
        {
            Id = id;
            Identificacion = identificacion;
            Nombres = nombres;
            Apellidos = apellidos;
            Edad = edad;
            FechaNacimiento = fechaNacimiento;
            Direccion = direccion;
            Telefono = telefono;
            EstadoCivil = estadoCivil;
            IdentificacionConyugue = identificacionConyugue;
            NombreConyugue = nombreConyugue;
            EsSujetoCredito = esSujetoCredito;
        }
    }
}
