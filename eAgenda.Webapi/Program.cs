using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

namespace eAgenda.Webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            try
            {
                var app = CreateHostBuilder(args)
                    .ConfigureServices(x =>
                    
                        x.AddLogging(a => { 

                            a.ClearProviders();
                            a.AddSerilog(); 
                        }))

                    .Build();                

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var db = services.GetRequiredService<eAgendaDbContext>();

                    Log.Logger.Information("Atualizando a banco de dados do e-Agenda...");

                    var migracaoRealizada = MigradorBancoDadoseAgenda.AtualizarBancoDados(db);

                    if (migracaoRealizada)
                        Log.Logger.Information("Banco de dados atualizado");
                    else
                        Log.Logger.Information("Nenhuma migração pendente");
                }

                Log.Logger.Information("Iniciando o servidor da aplicação e-Agenda...");

                app.Run();

            }
            catch (Exception exc)
            {
                Log.Logger.Fatal(exc, "O servidor da aplicação e-Agenda parou inesperadamente.");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}