using eAgenda.Dominio.ModuloTarefa;

namespace eAgenda.WebApi.ViewModels.ModuloTarefa
{
    public class InserirTarefaViewModel
    {
        public string Titulo { get; set; }
        public PrioridadeTarefaEnum Prioridade { get; set; }

    }
}
