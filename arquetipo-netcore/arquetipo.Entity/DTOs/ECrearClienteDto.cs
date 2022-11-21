using System.ComponentModel.DataAnnotations;

namespace arquetipo.Entity.DTOs
{
    public class ECrearClienteDto
    {
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public short Edad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string EstadoCivil { get; set; }
        public string? IdentificacionConyugue { get; set; }
        public string? NombreConyugue { get; set; }
        public bool EsSujetoCredito { get; set; } = true;
    }
}
