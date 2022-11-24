using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.SolicitudCredito
{
    public class SolicitudCreditoRepositorio : GenericRepositorio<ESolicitudCredito, Guid>, ISolicitudCreditoRepositorio
    {
        private readonly CrAutoDbContext _context;
        public SolicitudCreditoRepositorio(CrAutoDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<ESolicitudCredito?> ObtenerActivaPorVehiculoAsync(Guid vehiculoId)
        {
            return await _context.SolicitudesCredito.FirstOrDefaultAsync(
                sc => sc.VehiculoId == vehiculoId && sc.Estado == EstadoSolicitud.Registrada);
        }

        public async Task<ESolicitudCredito?> ObtenerActivaPorClienteFechaAsync(Guid clienteId, DateTime fecha)
        {
            var solicitudesFecha = _context.SolicitudesCredito.AsEnumerable()
                .Where(sc => sc.Estado == EstadoSolicitud.Registrada && sc.FechaElaboracion.ToString("yyyy-MM-dd") == fecha.ToString("yyyy-MM-dd"));

            return solicitudesFecha.FirstOrDefault(s => s.ClienteId == clienteId);
        }
    }
}