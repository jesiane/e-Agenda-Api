using eAgenda.Aplicacao.ModuloContato;
using eAgenda.Dominio.ModuloContato;

using eAgenda.Infra.Orm;
using eAgenda.Infra.Orm.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloCompromisso;
using eAgenda.WebApi.ViewModels.ModuloContato;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.WebApi.Controllers
{
    [ApiController]
    [Route("api/contatos")]
    public class ContatoController : ControllerBase
    {
        private ServicoContato servicoContato;

        public ContatoController()
        {
            IConfiguration configuracao = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            var connectionString = configuracao.GetConnectionString("SqlServer");

            var builder = new DbContextOptionsBuilder<eAgendaDbContext>();

            builder.UseSqlServer(connectionString);

            var contextoPersistencia = new eAgendaDbContext(builder.Options);

            var repositorioContato = new RepositorioContatoOrm(contextoPersistencia);

            servicoContato = new ServicoContato(repositorioContato, contextoPersistencia);
        }

        [HttpGet]
        public List<ListarContatoViewModel> SeleciontarTodos(StatusFavoritoEnum statusFavorito)
        {
            var contatos = servicoContato.SelecionarTodos(statusFavorito).Value;

            var contatosViewModel = new List<ListarContatoViewModel>();

            foreach (var contato in contatos)
            {
                var contatoViewModel = new ListarContatoViewModel
                {
                    Id = contato.Id,
                    Nome = contato.Nome,
                    Empresa = contato.Empresa,
                    Cargo = contato.Cargo,
                    Telefone = contato.Telefone
                };

                contatosViewModel.Add(contatoViewModel);
            }

            return contatosViewModel;
        }

        [HttpGet("visualizacao-completa/{id}")]
        public VisualizarContatoViewModel SeleciontarPorId(Guid id)
        {
            var contato = servicoContato.SelecionarPorId(id).Value;

            var contatoViewModel = new VisualizarContatoViewModel
            {
                Id = contato.Id,
                Nome = contato.Nome,
                Empresa = contato.Empresa,
                Cargo = contato.Cargo,
                Email = contato.Email,
                Telefone = contato.Telefone
            };

            foreach (var c in contato.Compromissos)
            {
                var compromissoViewModel = new ListarCompromissoViewModel
                {
                    Id = c.Id,
                    Assunto = c.Assunto,
                    Data = c.Data,
                    HoraInicio = c.HoraInicio.ToString(@"hh\:mm\:ss"),
                    HoraTermino = c.HoraTermino.ToString(@"hh\:mm\:ss")
                };

                contatoViewModel.Compromissos.Add(compromissoViewModel);
            }

            return contatoViewModel;
        }

        [HttpPost]
        public string Inserir(FormsContatoViewModel contatoViewModel)
        {
            var contato = new Contato(contatoViewModel.Nome, contatoViewModel.Email, contatoViewModel.Telefone,
                contatoViewModel.Empresa, contatoViewModel.Cargo);

            var resultado = servicoContato.Inserir(contato);

            if (resultado.IsSuccess)
                return "Contato inserido com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpPut("{id}")]
        public string Editar(Guid id, FormsContatoViewModel contatoViewModel)
        {
            var contato = servicoContato.SelecionarPorId(id).Value;

            contato.Nome = contatoViewModel.Nome;
            contato.Email = contatoViewModel.Email;
            contato.Telefone = contatoViewModel.Telefone;
            contato.Empresa = contatoViewModel.Empresa;
            contato.Cargo = contatoViewModel.Cargo;

            var resultado = servicoContato.Editar(contato);

            if (resultado.IsSuccess)
                return "Contato editado com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoContato.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {
                string[] errosNaBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosNaBusca);
            }

            var contato = resultadoBusca.Value;

            var resultado = servicoContato.Excluir(contato);

            if (resultado.IsSuccess)
                return "Contato excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

    }
}
