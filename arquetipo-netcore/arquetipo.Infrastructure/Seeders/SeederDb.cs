using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using CsvHelper;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace arquetipo.Infrastructure.Seeders
{
    [ExcludeFromCodeCoverage]
    public class SeederDb
    {
        public static void Initialize(CrAutoDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();
            string? tempValue = null;

            #region Clientes
            if (!dbContext.Clientes.Any())
            {
                using var stream = new StreamReader("Files/Clientes.csv");
                using (var csvReader = new CsvReader(stream, CultureInfo.CurrentCulture))
                {
                    csvReader.Read();
                    while (csvReader.Read())
                    {
                        string identificacion = csvReader.GetField(0);
                        if (dbContext.Clientes.Local.Any(c => c.Identificacion == identificacion))
                            throw new Exception($"El archivo tiene clientes duplicados ({identificacion})");

                        var cliente = new ECliente(
                            Guid.NewGuid(),
                            identificacion,
                            csvReader.GetField(1),
                            csvReader.GetField(2),
                            Convert.ToInt16(csvReader.GetField(3)),
                            Convert.ToDateTime(csvReader.GetField(4)),
                            csvReader.GetField(5),
                            csvReader.GetField(6),
                            csvReader.GetField(7),
                            csvReader.TryGetField<string>(8, out tempValue) ? tempValue : null,
                            csvReader.TryGetField<string>(9, out tempValue) ? tempValue : null,
                            Convert.ToBoolean(csvReader.GetField(10)));

                        dbContext.Clientes.Add(cliente);
                    }
                }

            };
            #endregion

            #region Marcas
            if (!dbContext.Marcas.Any())
            {
                using var stream = new StreamReader("Files/Marcas.csv");
                using (var csvReader = new CsvReader(stream, CultureInfo.CurrentCulture))
                {
                    csvReader.Read();
                    while (csvReader.Read())
                    {
                        string marca = csvReader.GetField(0);
                        if (dbContext.Marcas.Local.Any(c => c.Nombre == marca))
                            throw new Exception($"El archivo tiene marcas duplicadas ({marca})");
                        dbContext.Marcas.Add(new EMarca(Guid.NewGuid(), marca));
                    }
                }

            };
            #endregion

            #region Patios
            if (!dbContext.Patios.Any())
            {
                
                var patio1Id = Guid.NewGuid();
                var patio2Id = Guid.NewGuid();
                var patio3Id = Guid.NewGuid();

                dbContext.Patios.AddRange(new List<EPatio>
                {
                    new EPatio(patio1Id, "Patio 1", "Quito Norte", "0228876735", 1),
                    new EPatio(patio2Id, "Patio 2", "Quito Centro", "0228876736", 2),
                    new EPatio(patio3Id, "Patio 3", "Quito Sur", "0228876737", 3)
                });
            }
            #endregion

            #region Ejecutivos
            if (!dbContext.Ejecutivos.Any())
            {
                using var stream = new StreamReader("Files/Ejecutivos.csv");
                using (var csvReader = new CsvReader(stream, CultureInfo.CurrentCulture))
                {
                    csvReader.Read();
                    while (csvReader.Read())
                    {
                        string identificacion = csvReader.GetField(0);
                        if (dbContext.Ejecutivos.Local.Any(c => c.Identificacion == identificacion))
                            throw new Exception($"El archivo tiene ejecutivos duplicados ({identificacion})");

                        var patio = dbContext.Patios.FirstOrDefault(p => p.Nombre == csvReader.GetField(6)) ??
                            dbContext.Patios.Local.FirstOrDefault(p => p.Nombre == csvReader.GetField(6));
                        if (patio == null)
                            throw new Exception($"El patio asociado no existe ({csvReader.GetField(6)})");

                        var ejecutivo = new EEjecutivo(
                            Guid.NewGuid(),
                            identificacion,
                            csvReader.GetField(1),
                            csvReader.GetField(2),
                            csvReader.GetField(3),
                            csvReader.GetField(4),
                            csvReader.GetField(5),
                            patio.Id,
                            Convert.ToInt16(csvReader.GetField(7))
                        );

                        dbContext.Ejecutivos.Add(ejecutivo);
                    }
                }
            };
            #endregion

            dbContext.SaveChanges();
        }
    }
}
