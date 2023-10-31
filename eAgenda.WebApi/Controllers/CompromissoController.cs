using eAgenda.Aplicacao.ModuloContato;
using eAgenda.Infra.Orm.ModuloContato;
using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eAgenda.Infra.Orm.ModuloCompromisso;
using eAgenda.Aplicacao.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApi.ViewModels.ModuloContato;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.WebApi.ViewModels.ModuloCompromisso;

namespace eAgenda.WebApi.Controllers
{
    [ApiController]
    [Route("api/compromissos")]
    public class CompromissoController : Controller
    {
       private ServicoCompromisso servicoCompromisso;
       private ServicoContato servicoContato;

        public CompromissoController()
        {
            IConfiguration configuracao = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            var connectionString = configuracao.GetConnectionString("SqlServer");

            var builder = new DbContextOptionsBuilder<eAgendaDbContext>();

            builder.UseSqlServer(connectionString);

            var contextoPersistencia = new eAgendaDbContext(builder.Options);

            var repositorioCompromisso = new RepositorioCompromissoOrm(contextoPersistencia);

            servicoCompromisso = new ServicoCompromisso(repositorioCompromisso, contextoPersistencia);
        }

        [HttpGet]
        public List<ListarCompromissoViewModel> SeleciontarTodos(StatusCompromissoEnum statusCompromisso)
        {
            var compromissos = servicoCompromisso.SelecionarTodos(statusCompromisso).Value;

            var compromissosViewModel = new List<ListarCompromissoViewModel>();

            foreach (var compromisso in compromissos)
            {
                var compromissoViewModel = new ListarCompromissoViewModel
                {
                    Id = compromisso.Id,
                    Assunto = compromisso.Assunto,
                    Data = compromisso.Data,
                    HoraInicio = compromisso.HoraInicio.ToString(@"hh\:mm\:ss"),
                    HoraTermino = compromisso.HoraTermino.ToString(@"hh\:mm\:ss"),
                    NomeContato = compromisso.Contato,
            };
                compromissosViewModel.Add(compromissoViewModel);
            }
            return compromissosViewModel;
        }
        
          [HttpGet("visualizacao-completa/{id}")]
        public VisualizarCompromissoViewModel SelecionarPorId(Guid id)
        {
            var compromisso = servicoCompromisso.SelecionarPorId(id).Value;

            var compromissoViewModel = new VisualizarCompromissoViewModel
            {
                Id = compromisso.Id,
                Assunto = compromisso.Assunto,
                Local = compromisso.Local,
                Link = compromisso.Link,
                Data = compromisso.Data,
                Telefone = compromisso.Telefone,
                HoraInicio = compromisso.HoraInicio.ToString(@"dd\:mm\:ss"),
                HoraTermino = compromisso.HoraTermino.ToString(@"dd\:mm\:ss"),
            };
            var contatoViewModel = new ListarContatoViewModel
            {
                Id = compromisso.Id,
                Nome = compromisso.Contato.Nome,
                Email = compromisso.Contato.Email,
                Telefone = compromisso.Contato.Telefone,
                Empresa = compromisso.Contato.Empresa,
                Cargo = compromisso.Contato.Cargo,
            };

       //     compromissoViewModel.Contatos.Add(contatoViewModel);

            return compromissoViewModel;
        }

        [HttpPost]
        public string Inserir(InserirCompromissoViewModel compromissoViewModel)
        {
            var compromisso = new Compromisso(compromissoViewModel.Assunto,
                compromissoViewModel.Local, compromissoViewModel.TipoLocalizacao,
                compromissoViewModel.Link, compromissoViewModel.Data,
                compromissoViewModel.HoraInicio, compromissoViewModel.HoraTermino, Contato);

            var resultado = servicoCompromisso.Inserir(compromisso);

            if (resultado.IsSuccess)
                return "Compromisso inserido com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }


        //AQUI

        [HttpPut("{id}")]
        public string Editar(Guid id, EditarCompromissoViewModel compromissoViewModel)
        {
            var compromisso = servicoCompromisso.SelecionarPorId(id).Value;

            compromisso.Assunto = compromissoViewModel.Assunto;
            compromisso.Local = compromissoViewModel.Local;
            compromisso.Link = compromissoViewModel.Link;
            compromisso.Data = compromissoViewModel.Data;
            compromisso.HoraInicio = compromissoViewModel.HoraInicio.ToString;
            compromisso.HoraTermino = compromissoViewModel.HoraTermino.ToString;

            var resultado = servicoCompromisso.Editar(compromisso);

            if (resultado.IsSuccess)
                return "Contato editado com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }

        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoCompromisso.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {
                string[] errosNaBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosNaBusca);
            }

            var compromisso = resultadoBusca.Value;

            var resultado = servicoCompromisso.Excluir(compromisso);

            if (resultado.IsSuccess)
                return "Compromisso excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }
    }
}
