using eAgenda.Aplicacao.ModuloDespesa;
using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eAgenda.Infra.Orm.ModuloDespesa;
using eAgenda.Aplicacao.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloCategoria;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloCompromisso;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApi.ViewModels.ModuloDespesa;

namespace eAgenda.WebApi.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriaController : Controller
    {
        private ServicoCategoria servicoCategoria;

        public CategoriaController()
        {
            IConfiguration configuracao = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var connectionString = configuracao.GetConnectionString("SqlServer");

            var builder = new DbContextOptionsBuilder<eAgendaDbContext>();

            builder.UseSqlServer(connectionString);

            var contextoPersistencia = new eAgendaDbContext(builder.Options);

            var repositorioCategoria = new RepositorioCategoriaOrm(contextoPersistencia);

            servicoCategoria = new ServicoCategoria(repositorioCategoria, contextoPersistencia);
        }

        [HttpGet]
        public List<ViewModels.ModuloCategoria.ListarCategoriaViewModel> SeleciontarTodos()
        {
            var categorias = servicoCategoria.SelecionarTodos().Value;

            var categoriasViewModel = new List<ListarCategoriaViewModel>();

            foreach (var categoria in categorias)
            {
                var contatoViewModel = new ListarCategoriaViewModel
                {
                    Id = categoria.Id,
                    Titulo = categoria.Titulo,
                };

                categoriasViewModel.Add(contatoViewModel);
            }

            return categoriasViewModel;
        }

        [HttpGet("visualizacao-completa/{id}")]
        public VisualizarCategoriaViewModel SeleciontarPorId(Guid id)
        {
            var categoria = servicoCategoria.SelecionarPorId(id).Value;

            var categoriaViewModel = new VisualizarCategoriaViewModel
            {
                Id = categoria.Id,
                Titulo = categoria.Titulo,
            };

            foreach (var c in categoria.Despesas)
            {
                var despesaViewModel = new ListarDespesaViewModel
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Valor = c.Valor,
                    FormaPagamento = nameof(c.FormaPagamento)
                };

                categoriaViewModel.Despesas.Add(despesaViewModel);
            }

            return categoriaViewModel;
        }

        [HttpPost]
        public string Inserir(InserirCategoriaViewModel categoriaViewModel)
        {
            var categoria = new Categoria();

            var resultado = servicoCategoria.Inserir(categoria);

            if (resultado.IsSuccess)
                return "Contato inserido com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpPut("{id}")]
        public string Editar(Guid id, EditarCategoriaViewModel categoriaViewModel)
        {
            var categoria = servicoCategoria.SelecionarPorId(id).Value;

            categoria.Titulo = categoriaViewModel.Titulo;
            

            var resultado = servicoCategoria.Editar(categoria);

            if (resultado.IsSuccess)
                return "Contato editado com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoCategoria.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {
                string[] errosNaBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosNaBusca);
            }

            var categoria = resultadoBusca.Value;

            var resultado = servicoCategoria.Excluir(categoria);

            if (resultado.IsSuccess)
                return "Contato excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }
    }
}
