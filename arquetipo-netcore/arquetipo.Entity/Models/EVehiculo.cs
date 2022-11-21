namespace arquetipo.Entity.Models
{
    public class EVehiculo
    {
        public Guid Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string NumeroChasis { get; set; }
        public Guid MarcaId { get; set; }
        public string Tipo { get; set; }
        public float Cilindraje { get; set; }
        public decimal Avaluo { get; set; }
    }
}
