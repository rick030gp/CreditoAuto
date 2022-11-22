namespace arquetipo.Entity.DTOs
{
    public class ECrearVehiculoDto
    {
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string NumeroChasis { get; set; }
        public string Marca { get; set; }
        public string? Tipo { get; set; }
        public float Cilindraje { get; set; }
        public decimal Avaluo { get; set; }
    }
}
