using eAgenda.Aplicacao.ModuloDespesa;
using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eAgenda.Infra.Orm.ModuloDespesa;

using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApi.ViewModels.ModuloDespesa;
using eAgenda.WebApi.ViewModels.ModuloCategoria;

namespace eAgenda.WebApi.Controllers
{
    [ApiController]
    [Route("api/despesas")]
    public class DespesaController : Controller
    {
        private ServicoDespesa servicoDespesa;
        private ServicoCategoria servicoCategoria;
     
        public DespesaController()
        {
            IConfiguration configuracao = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuracao.GetConnectionString("SqlServer");

            var builder = new DbContextOptionsBuilder<eAgendaDbContext>();

            builder.UseSqlServer(connectionString);

            var contextoPersistencia = new eAgendaDbContext(builder.Options);

            var repositorioDespesa = new RepositorioDespesaOrm(contextoPersistencia);

            var repositorioCategoria = new RepositorioCategoriaOrm(contextoPersistencia);

            servicoDespesa = new ServicoDespesa(repositorioDespesa, contextoPersistencia);

            servicoCategoria = new ServicoCategoria(repositorioCategoria, contextoPersistencia);
        }

        [HttpGet]
        public List<ListarDespesaViewModel> SeleciontarTodos()
        {
            var despesas = servicoDespesa.SelecionarTodos().Value;

            var despesasViewModel = new List<ListarDespesaViewModel>();

            foreach (var c in despesas)
            {
                var despesaViewModel = new ListarDespesaViewModel
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Valor = c.Valor,
                    Data = c.Data,
                    FormaPagamento = nameof(c.FormaPagamento)
                };

                despesasViewModel.Add(despesaViewModel);
            }

            return despesasViewModel;
        }

        [HttpGet("{id}")]
        public FormsDespesaViewModel SeleciontarPorId(Guid id)
        {
            var despesa = servicoDespesa.SelecionarPorId(id).Value;

            var despesaViewModel = new FormsDespesaViewModel
            {
                Descricao = despesa.Descricao,
                Valor = despesa.Valor,
                FormaPagamento = despesa.FormaPagamento,
         
            };

            foreach (var d in despesa.Categorias)
            {
                despesaViewModel.CategoriasSelecionadas.Add(d.Id);
            }
            return despesaViewModel;
        }


        [HttpGet("visualizacao-completa/{id}")]
        public VisualizarDespesaViewModel SeleciontarPorIdCompleto(Guid id)
        {
            var despesa = servicoDespesa.SelecionarPorId(id).Value;

            var despesaViewModel = new VisualizarDespesaViewModel
            {
                Id = despesa.Id,
                Descricao = despesa.Descricao,
                Valor = despesa.Valor,
                FormaPagamento = nameof(despesa.FormaPagamento)
                
            };


            foreach (var d in despesa.Categorias)
            {
                var categoriaViewModel = new ListarCategoriaViewModel 
                {
                    Id = d.Id,
                    Titulo = d.Titulo
                };
                despesaViewModel.Categorias.Add(categoriaViewModel);
            }

            return despesaViewModel;
        }


        [HttpPost]
        public string Inserir(FormsDespesaViewModel despesaViewModel)
        {
            Despesa despesa = new Despesa
            {
                Descricao = despesaViewModel.Descricao,
                Valor = despesaViewModel.Valor,
                FormaPagamento = despesaViewModel.FormaPagamento
            };

            foreach (var d in despesaViewModel.CategoriasSelecionadas)
            {
                var resultadoBusca = servicoCategoria.SelecionarPorId(d);

                if (resultadoBusca.IsFailed)
                {
                    string[] errosBusca = resultadoBusca.Errors.Select(e => e.Message).ToArray();

                    return string.Join(", ", errosBusca);
                }

                var categoria = resultadoBusca.Value;

                despesa.Categorias.Add(categoria);
            }
            var resultado = servicoDespesa.Inserir(despesa);

            if (resultado.IsSuccess)
                return "Despesa inserida com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n ", erros);
        }

        [HttpPut("{id}")]
        public string Editar(Guid id, FormsDespesaViewModel despesaViewModel)
        {
            var resultadoBusca = servicoDespesa.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {

                string[] errosBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosBusca);
            }

            var despesa = resultadoBusca.Value;

            despesa.Descricao = despesaViewModel.Descricao;
            despesa.Valor = despesaViewModel.Valor;
            despesa.FormaPagamento = despesaViewModel.FormaPagamento;

            despesa.Categorias.Clear();
            foreach (var c in despesaViewModel.CategoriasSelecionadas)
            {
                var resultadoBuscaCategoria = servicoCategoria.SelecionarPorId(c);

                if (resultadoBuscaCategoria.IsFailed)
                {

                    string[] errosBuscaCategoria = resultadoBuscaCategoria.Errors.Select(x => x.Message).ToArray();

                    return string.Join("\r\n", errosBuscaCategoria);
                }

                var categoria = resultadoBuscaCategoria.Value;

                despesa.Categorias.Add(categoria);
            }

            var resultado = servicoDespesa.Editar(despesa);

            if (resultado.IsSuccess)
                return "Despesa editada com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }


        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoDespesa.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {

                string[] errosBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosBusca);
            }

            var resultado = servicoDespesa.Excluir(id);

            if (resultado.IsSuccess)
                return "Compromisso excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }
    }
}
