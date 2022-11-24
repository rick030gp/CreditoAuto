using arquetipo.Entity.Models;

namespace arquetipo.Infrastructure.Exceptions
{
    public class CrAutoErrores
    {
        public static readonly Error ActualizacionDatosVaciosError = new()
        {
            Code = "EGL001",
            Message = "Datos a actualizar vacíos"
        };

        public static readonly Error EliminarRelacionesExistentesError = new()
        {
            Code = "EGL002",
            Message = "No se puede eliminar el objeto porque tiene relaciones existentes"
        };

        public static readonly Error ClienteYaExisteError = new()
        {
            Code = "ECL001",
            Message = "El cliente ya existe"
        };

        public static readonly Error ClienteNoExisteError = new()
        {
            Code = "ECL002",
            Message = "El cliente no existe"
        };

        public static readonly Error PatioYaExisteError = new()
        {
            Code = "EPT001",
            Message = "El patio ya existe"
        };

        public static readonly Error PatioNoExisteError = new()
        {
            Code = "EPT002",
            Message = "El patio no existe"
        };

        public static readonly Error VehiculoYaExisteError = new()
        {
            Code = "EVH001",
            Message = "El vehículo ya existe"
        };

        public static readonly Error VehiculoNoExisteError = new()
        {
            Code = "EVH002",
            Message = "El vehículo no existe"
        };

        public static readonly Error MarcaNoExisteError = new()
        {
            Code = "EMC002",
            Message = "La marca de vehículo no existe"
        };

        public static readonly Error AsociacionClientePatioNoExiste = new()
        {
            Code = "ECP001",
            Message = "La asociación entre cliente y patio no existe"
        };

        public static readonly Error ClienteYaTieneSolicitudError = new()
        {
            Code = "ESC001",
            Message = "El cliente ya tiene una solicitud de crédito registrada"
        };

        public static readonly Error EjecutivoNoDisponibleEnPatioError = new()
        {
            Code = "ESC002",
            Message = "El ejecutivo ingresado no pertenece al patio ingresado"
        };

        public static readonly Error VehiculoEnReservaError = new()
        {
            Code = "ESC003",
            Message = "El vehículo ya se encuentra reservado"
        };
    }
}
