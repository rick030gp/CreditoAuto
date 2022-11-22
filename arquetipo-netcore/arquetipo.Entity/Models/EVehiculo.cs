namespace arquetipo.Entity.Models
{
    public class EVehiculo
    {
        public Guid Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string NumeroChasis { get; set; }
        public Guid MarcaId { get; set; }
        public string? Tipo { get; set; }
        public float Cilindraje { get; set; }
        public decimal Avaluo { get; set; }

        public EVehiculo(
            Guid id,
            string placa,
            string modelo,
            string numeroChasis,
            Guid marcaId,
            string? tipo,
            float cilindraje,
            decimal avaluo)
        {
            Id = id;
            Placa = placa;
            Modelo = modelo;
            NumeroChasis = numeroChasis;
            MarcaId = marcaId;
            Tipo = tipo;
            Cilindraje = cilindraje;
            Avaluo = avaluo;
        }
    }
}
