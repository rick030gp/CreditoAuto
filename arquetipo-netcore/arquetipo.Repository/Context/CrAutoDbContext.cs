using arquetipo.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Context
{
    public class CrAutoDbContext : DbContext
    {
        public virtual DbSet<ECliente>? Clientes { get; set; }
        public virtual DbSet<EPatio>? Patios { get; set; }
        public virtual DbSet<EMarca>? Marcas { get; set; }
        public virtual DbSet<EEjecutivo>? Ejecutivos { get; set; }
        public virtual DbSet<EVehiculo>? Vehiculos { get; set; }
        public virtual DbSet<ESolicitudCredito>? SolicitudesCredito { get; set; }
        public virtual DbSet<EClientePatio>? ClientePatios { get; set; }

        public CrAutoDbContext(DbContextOptions options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ECliente>(cliente =>
            {
                cliente.ToTable(EConstante.CLIENTE_TABLENAME);
                cliente.HasKey(cl => cl.Id);
                cliente.HasIndex(cl => cl.Identificacion).IsUnique();
                cliente.Property(cl => cl.Identificacion).HasMaxLength(EConstante.IDENTIFICACION_MAXLENGTH);
                cliente.Property(cl => cl.Nombres).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                cliente.Property(cl => cl.Apellidos).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                cliente.Property(cl => cl.Direccion).HasMaxLength(EConstante.DIRECCION_MAXLENGTH);
                cliente.Property(cl => cl.Telefono).HasMaxLength(EConstante.TELEFONO_MAXLENGTH);
                cliente.Property(cl => cl.EstadoCivil).HasMaxLength(EConstante.CODIGO_MAXLENGTH);
                cliente.Property(cl => cl.IdentificacionConyugue).HasMaxLength(EConstante.IDENTIFICACION_MAXLENGTH);
                cliente.Property(cl => cl.NombreConyugue).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
            });

            modelBuilder.Entity<EPatio>(patio =>
            {
                patio.ToTable(EConstante.PATIO_TABLENAME);
                patio.HasKey(pt => pt.Id);
                patio.Property(pt => pt.Nombre).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                patio.Property(pt => pt.Direccion).HasMaxLength(EConstante.DIRECCION_MAXLENGTH);
                patio.Property(pt => pt.Telefono).HasMaxLength(EConstante.TELEFONO_MAXLENGTH);
                patio.HasIndex(pt => pt.NumeroPuntoVenta).IsUnique();
            });

            modelBuilder.Entity<EMarca>(marca =>
            {
                marca.ToTable(EConstante.MARCA_TABLENAME);
                marca.HasKey(m => m.Id);
                marca.Property(m => m.Nombre).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                marca.HasIndex(m => m.Nombre).IsUnique();
            });

            modelBuilder.Entity<EEjecutivo>(ejecutivo =>
            {
                ejecutivo.ToTable(EConstante.EJECUTIVO_TABLENAME);
                ejecutivo.HasKey(e => e.Id);
                ejecutivo.Property(e => e.Identificacion).HasMaxLength(EConstante.IDENTIFICACION_MAXLENGTH);
                ejecutivo.HasIndex(e => e.Identificacion).IsUnique();
                ejecutivo.Property(e => e.Nombres).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                ejecutivo.Property(e => e.Apellidos).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                ejecutivo.Property(e => e.Direccion).HasMaxLength(EConstante.DIRECCION_MAXLENGTH);
                ejecutivo.Property(e => e.TelefonoConvencional).HasMaxLength(EConstante.TELEFONO_MAXLENGTH);
                ejecutivo.Property(e => e.Celular).HasMaxLength(EConstante.TELEFONO_MAXLENGTH);
                ejecutivo.HasOne<EPatio>().WithMany(p => p.Ejecutivos)
                    .HasForeignKey(e => e.PatioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EVehiculo>(vehiculo =>
            {
                vehiculo.ToTable(EConstante.VEHICULO_TABLENAME);
                vehiculo.HasKey(v => v.Id);
                vehiculo.Property(v => v.Placa).HasMaxLength(EConstante.CODIGO_MAXLENGTH);
                vehiculo.HasIndex(v => v.Placa).IsUnique();
                vehiculo.Property(v => v.Modelo).HasMaxLength(EConstante.NOMBRES_MAXLENGTH);
                vehiculo.Property(v => v.NumeroChasis).HasMaxLength(EConstante.CODIGO_MAXLENGTH);
                vehiculo.Property(v => v.Tipo).HasMaxLength(EConstante.CODIGO_MAXLENGTH);
                vehiculo.Property(v => v.Cilindraje);
                vehiculo.Property(v => v.Avaluo).HasColumnType("decimal(18,2)");
                vehiculo.HasOne<EMarca>().WithMany()
                    .HasForeignKey(v => v.MarcaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ESolicitudCredito>(solicitud =>
            {
                solicitud.ToTable(EConstante.SOLICITUD_CREDITO_TABLENAME);
                solicitud.HasKey(s => s.Id);
                solicitud.Property(s => s.FechaElaboracion).HasDefaultValueSql("getdate()");
                solicitud.Property(s => s.MesesPlazo);
                solicitud.Property(s => s.Cuotas).HasColumnType("decimal(18,2)");
                solicitud.Property(s => s.Entrada).HasColumnType("decimal(18,2)");
                solicitud.Property(s => s.Observacion).HasMaxLength(EConstante.DESCRIPCION_MAXLENGTH);
                solicitud.Property(s => s.Estado)
                    .HasMaxLength(EConstante.CODIGO_MAXLENGTH)
                    .HasConversion(e => e.ToString(), e => (EstadoSolicitud)Enum.Parse(typeof(EstadoSolicitud), e));
                solicitud.HasOne<ECliente>().WithMany()
                    .HasForeignKey(s => s.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
                solicitud.HasOne<EPatio>().WithMany()
                    .HasForeignKey(s => s.PatioId)
                    .OnDelete(DeleteBehavior.Restrict);
                solicitud.HasOne<EVehiculo>().WithMany()
                    .HasForeignKey(s => s.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);
                solicitud.HasOne<EEjecutivo>().WithMany()
                    .HasForeignKey(s => s.EjecutivoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EClientePatio>(clientePatio =>
            {
                clientePatio.ToTable(EConstante.CLIENTE_PATIO_TABLENAME);
                clientePatio.HasKey(cp => cp.Id);
                clientePatio.Property(cp => cp.FechaAsignacion).HasDefaultValueSql("getdate()");
                clientePatio.HasIndex(cp => new { cp.ClienteId, cp.PatioId }).IsUnique();
            });

            modelBuilder.Entity<EClientePatio>(clientePatio =>
            {
                clientePatio.HasOne<ECliente>().WithMany()
                    .HasForeignKey(cp => cp.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EClientePatio>(clientePatio =>
            {
                clientePatio.HasOne<EPatio>().WithMany()
                    .HasForeignKey(cp => cp.PatioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
