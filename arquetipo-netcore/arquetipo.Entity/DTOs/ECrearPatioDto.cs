using System.ComponentModel.DataAnnotations;

namespace arquetipo.Entity.DTOs
{
    public class ECrearPatioDto
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public short NumeroPuntoVenta { get; set; }
    }
}
