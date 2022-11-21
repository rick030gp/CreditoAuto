namespace arquetipo.Entity.Models
{
    public class EMarca
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public EMarca(Guid id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
