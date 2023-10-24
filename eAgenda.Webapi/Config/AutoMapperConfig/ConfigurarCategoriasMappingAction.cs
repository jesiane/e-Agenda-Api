using AutoMapper;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Webapi.ViewModels.ModuloDespesa;
using System.Linq;

namespace eAgenda.Webapi.Config.AutoMapperConfig
{
    public class ConfigurarCategoriasInsertMappingAction : IMappingAction<FormsDespesaViewModel, Despesa>
    {
        private readonly IRepositorioCategoria repositorioCategoria;

        public ConfigurarCategoriasInsertMappingAction(IRepositorioCategoria repositorioCategoria)
        {
            this.repositorioCategoria = repositorioCategoria;
        }

        public void Process(FormsDespesaViewModel despesaVM, Despesa despesa, ResolutionContext context)
        {
            despesa.Categorias = repositorioCategoria.SelecionarMuitos(despesaVM.CategoriasSelecionadas);
        }
    }

    public class ConfigurarCategoriasEditMappingAction : IMappingAction<Despesa, FormsDespesaViewModel>
    {
        public void Process(Despesa despesa, FormsDespesaViewModel despesaVM, ResolutionContext context)
        {
            despesaVM.CategoriasSelecionadas = despesa.Categorias
                .Select(categoria => categoria.Id)
                .ToList();
        }
    }
}