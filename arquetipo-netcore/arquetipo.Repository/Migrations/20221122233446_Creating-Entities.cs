using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arquetipo.Repository.Migrations
{
    public partial class CreatingEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Edad = table.Column<short>(type: "smallint", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EstadoCivil = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IdentificacionConyugue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NombreConyugue = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EsSujetoCredito = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NumeroPuntoVenta = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NumeroChasis = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    MarcaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Cilindraje = table.Column<float>(type: "real", nullable: false),
                    Avaluo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehiculo_Marca_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientePatio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientePatio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientePatio_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientePatio_Patio_PatioId",
                        column: x => x.PatioId,
                        principalTable: "Patio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ejecutivo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TelefonoConvencional = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Edad = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejecutivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ejecutivo_Patio_PatioId",
                        column: x => x.PatioId,
                        principalTable: "Patio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudCredito",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaElaboracion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MesesPlazo = table.Column<short>(type: "smallint", nullable: false),
                    Cuotas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Entrada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EjecutivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudCredito_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudCredito_Ejecutivo_EjecutivoId",
                        column: x => x.EjecutivoId,
                        principalTable: "Ejecutivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudCredito_Patio_PatioId",
                        column: x => x.PatioId,
                        principalTable: "Patio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudCredito_Vehiculo_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Identificacion",
                table: "Cliente",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientePatio_ClienteId_PatioId",
                table: "ClientePatio",
                columns: new[] { "ClienteId", "PatioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientePatio_PatioId",
                table: "ClientePatio",
                column: "PatioId");

            migrationBuilder.CreateIndex(
                name: "IX_Ejecutivo_Identificacion",
                table: "Ejecutivo",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ejecutivo_PatioId",
                table: "Ejecutivo",
                column: "PatioId");

            migrationBuilder.CreateIndex(
                name: "IX_Marca_Nombre",
                table: "Marca",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patio_NumeroPuntoVenta",
                table: "Patio",
                column: "NumeroPuntoVenta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudCredito_ClienteId",
                table: "SolicitudCredito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudCredito_EjecutivoId",
                table: "SolicitudCredito",
                column: "EjecutivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudCredito_PatioId",
                table: "SolicitudCredito",
                column: "PatioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudCredito_VehiculoId",
                table: "SolicitudCredito",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_MarcaId",
                table: "Vehiculo",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_Placa",
                table: "Vehiculo",
                column: "Placa",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientePatio");

            migrationBuilder.DropTable(
                name: "SolicitudCredito");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Ejecutivo");

            migrationBuilder.DropTable(
                name: "Vehiculo");

            migrationBuilder.DropTable(
                name: "Patio");

            migrationBuilder.DropTable(
                name: "Marca");
        }
    }
}
