﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using arquetipo.Repository.Context;

#nullable disable

namespace arquetipo.Repository.Migrations
{
    [DbContext(typeof(CrAutoDbContext))]
    partial class CrAutoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("arquetipo.Entity.Models.ECliente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<short>("Edad")
                        .HasColumnType("smallint");

                    b.Property<bool>("EsSujetoCredito")
                        .HasColumnType("bit");

                    b.Property<string>("EstadoCivil")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<string>("Identificacion")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("IdentificacionConyugue")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("NombreConyugue")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Nombres")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("Identificacion")
                        .IsUnique();

                    b.ToTable("Cliente", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EClientePatio", b =>
                {
                    b.Property<Guid>("ClienteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PatioId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("FechaAsignacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.HasKey("ClienteId", "PatioId");

                    b.HasIndex("PatioId");

                    b.ToTable("ClientePatio", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EEjecutivo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<short>("Edad")
                        .HasColumnType("smallint");

                    b.Property<string>("Identificacion")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Nombres")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid>("PatioId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TelefonoConvencional")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("Identificacion")
                        .IsUnique();

                    b.HasIndex("PatioId");

                    b.ToTable("Ejecutivo", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EMarca", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Marca", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EPatio", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<short>("NumeroPuntoVenta")
                        .HasColumnType("smallint");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("NumeroPuntoVenta")
                        .IsUnique();

                    b.ToTable("Patio", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.ESolicitudCredito", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClienteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Cuotas")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("EjecutivoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Entrada")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("FechaElaboracion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<short>("MesesPlazo")
                        .HasColumnType("smallint");

                    b.Property<string>("Observacion")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<Guid>("PatioId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VehiculoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId");

                    b.HasIndex("EjecutivoId");

                    b.HasIndex("PatioId");

                    b.HasIndex("VehiculoId");

                    b.ToTable("SolicitudCredito", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EVehiculo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Avaluo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<float>("Cilindraje")
                        .HasColumnType("real");

                    b.Property<Guid>("MarcaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("NumeroChasis")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Placa")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Tipo")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("MarcaId");

                    b.HasIndex("Placa")
                        .IsUnique();

                    b.ToTable("Vehiculo", (string)null);
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EClientePatio", b =>
                {
                    b.HasOne("arquetipo.Entity.Models.ECliente", null)
                        .WithMany()
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("arquetipo.Entity.Models.EPatio", null)
                        .WithMany()
                        .HasForeignKey("PatioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EEjecutivo", b =>
                {
                    b.HasOne("arquetipo.Entity.Models.EPatio", null)
                        .WithMany()
                        .HasForeignKey("PatioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("arquetipo.Entity.Models.ESolicitudCredito", b =>
                {
                    b.HasOne("arquetipo.Entity.Models.ECliente", null)
                        .WithMany()
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("arquetipo.Entity.Models.EEjecutivo", null)
                        .WithMany()
                        .HasForeignKey("EjecutivoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("arquetipo.Entity.Models.EPatio", null)
                        .WithMany()
                        .HasForeignKey("PatioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("arquetipo.Entity.Models.EVehiculo", null)
                        .WithMany()
                        .HasForeignKey("VehiculoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("arquetipo.Entity.Models.EVehiculo", b =>
                {
                    b.HasOne("arquetipo.Entity.Models.EMarca", null)
                        .WithMany()
                        .HasForeignKey("MarcaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
