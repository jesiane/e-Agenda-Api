using eAgenda.Aplicacao.ModuloCompromisso;
using eAgenda.Aplicacao.ModuloContato;
using eAgenda.Aplicacao.ModuloDespesa;
using eAgenda.Aplicacao.ModuloTarefa;
using eAgenda.Dominio;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
//using eAgenda.Infra.Configs;
using eAgenda.Infra.Orm;
using eAgenda.Infra.Orm.ModuloCompromisso;
using eAgenda.Infra.Orm.ModuloContato;
using eAgenda.Infra.Orm.ModuloDespesa;
using eAgenda.Infra.Orm.ModuloTarefa;
using eAgenda.Webapi.Config.AutoMapperConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eAgenda.Webapi.Config
{
    public static class DependencyInjectionConfig
    {
        public static void ConfigurarInjecaoDependencia(this IServiceCollection services, IConfiguration configuracao)
        {
            services.AddDbContext<eAgendaDbContext>(opt =>
            {
                opt.UseSqlServer(configuracao.GetConnectionString("Postgres"), 
                    opt => opt.EnableRetryOnFailure(3))
                .EnableDetailedErrors(detailedErrorsEnabled: true);
            });

            services.AddScoped<IContextoPersistencia, eAgendaDbContext>();

            services.AddScoped<IRepositorioTarefa, RepositorioTarefaOrm>();
            services.AddTransient<ServicoTarefa>();

            services.AddScoped<IRepositorioContato, RepositorioContatoOrm>();
            services.AddTransient<ServicoContato>();

            services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoOrm>();
            services.AddTransient<ServicoCompromisso>();

            services.AddScoped<IRepositorioDespesa, RepositorioDespesaOrm>();
            services.AddTransient<ServicoDespesa>();

            services.AddScoped<IRepositorioCategoria, RepositorioCategoriaOrm>();
            services.AddTransient<ServicoCategoria>();

            services.AddTransient<ConfigurarCategoriasInsertMappingAction>();
        }
    }
}