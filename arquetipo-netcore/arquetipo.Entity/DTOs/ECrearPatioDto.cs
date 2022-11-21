using System.ComponentModel.DataAnnotations;

namespace arquetipo.Entity.DTOs
{
    public class ECrearPatioDto
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required]
        public short NumeroPuntoVenta { get; set; }
    }
}
