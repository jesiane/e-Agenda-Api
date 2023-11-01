using eAgenda.Aplicacao.ModuloTarefa;
using eAgenda.Infra.Orm.ModuloTarefa;
using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eAgenda.Aplicacao.ModuloContato;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloContato;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.WebApi.ViewModels.ModuloTarefa;
using eAgenda.WebApi.ViewModels.ModuloCompromisso;

namespace eAgenda.WebApi.Controllers
{
    [ApiController]
    [Route("api/tarefas")]
    public class TarefaController : Controller
    {
        private ServicoTarefa servicoTarefa;
        public TarefaController()
        {
            IConfiguration configuracao = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuracao.GetConnectionString("SqlServer");

            var builder = new DbContextOptionsBuilder<eAgendaDbContext>();

            builder.UseSqlServer(connectionString);

            var contextoPersistencia = new eAgendaDbContext(builder.Options);

            var repositorioTarefa = new RepositorioTarefaOrm(contextoPersistencia);

            servicoTarefa = new ServicoTarefa(repositorioTarefa, contextoPersistencia);
        }

        [HttpGet]
        public List<ListarTarefaViewModel> SeleciontarTodos(StatusTarefaEnum statusTarefa)
        {
            var tarefas = servicoTarefa.SelecionarTodos(statusTarefa).Value;

            var tarefasViewModel = new List<ListarTarefaViewModel>();

            foreach (var tarefa in tarefas)
            {
                var tarefaViewModel = new ListarTarefaViewModel
                {
                    Id = tarefa.Id,                  
                };

                tarefasViewModel.Add(tarefaViewModel);
            }

            return tarefasViewModel;
        }

        [HttpGet("visualizacao-completa/{id}")]
        public VisualizarTarefaViewModel SeleciontarPorId(Guid id)
        {
            var tarefa = servicoTarefa.SelecionarPorId(id).Value;

            var tarefasViewModel = new VisualizarTarefaViewModel
            {
                Id = tarefa.Id,
            };
            return tarefasViewModel;
        }


        [HttpPost]
        public string Inserir(InserirTarefaViewModel contatoViewModel)
        {
            var tarefa = new Tarefa();

            var resultado = servicoTarefa.Inserir(tarefa);

            if (resultado.IsSuccess)
                return "Contato inserido com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }


        [HttpPut("{id}")]
        public string Editar(Guid id, EditarTarefaViewModel tarefaViewModel)
        {
            var tarefa = servicoTarefa.SelecionarPorId(id).Value;

            tarefa.Titulo = tarefaViewModel.Titulo;
        
            var resultado = servicoTarefa.Editar(tarefa);

            if (resultado.IsSuccess)
                return "Contato editado com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoTarefa.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {
                string[] errosNaBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosNaBusca);
            }

            var contato = resultadoBusca.Value;

            var resultado = servicoTarefa.Excluir(contato);

            if (resultado.IsSuccess)
                return "Contato excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }
    }
}
