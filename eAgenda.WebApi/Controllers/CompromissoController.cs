﻿using eAgenda.Aplicacao.ModuloContato;
using eAgenda.Infra.Orm.ModuloContato;
using eAgenda.Infra.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eAgenda.Infra.Orm.ModuloCompromisso;
using eAgenda.Aplicacao.ModuloCompromisso;
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

            var repositorioContato = new RepositorioContatoOrm(contextoPersistencia);

            servicoCompromisso = new ServicoCompromisso(repositorioCompromisso, contextoPersistencia);

            servicoContato = new ServicoContato(repositorioContato, contextoPersistencia);
        }
        [HttpGet]
        public List<ListarCompromissoViewModel> SeleciontarTodos()
        {
            var compromissos = servicoCompromisso.SelecionarTodos().Value;

            var compromissosViewModel = new List<ListarCompromissoViewModel>();

            foreach (var c in compromissos)
            {
                var compromissoViewModel = new ListarCompromissoViewModel
                {
                    Id = c.Id,
                    Assunto = c.Assunto,
                    Data = c.Data,
                    HoraInicio = c.HoraInicio.ToString(@"hh\:mm\:ss"),
                    HoraTermino = c.HoraTermino.ToString(@"hh\:mm\:ss"),
                //    NomeContato = c.Contato.NomeContato,
                };

                compromissosViewModel.Add(compromissoViewModel);
            }

            return compromissosViewModel;
        }

        [HttpGet("{id}")]
        public InserirCompromissoViewModel SeleciontarPorId(Guid id)
        {
            var compromisso = servicoCompromisso.SelecionarPorId(id).Value;

            var compromissoViewModel = new InserirCompromissoViewModel
            {
                Assunto = compromisso.Assunto,
                Data = compromisso.Data,
                Local = compromisso.Local,
                TipoLocalizacao = compromisso.TipoLocalizacao,
                Link = compromisso.Link,
                HoraInicio = compromisso.HoraInicio.ToString(@"hh\:mm\:ss"),
                HoraTermino = compromisso.HoraTermino.ToString(@"hh\:mm\:ss"),
                ContatoId = compromisso.Contato.Id
            };

            return compromissoViewModel;
        }

        [HttpGet("visualizacao-completa/{id}")]
        public VisualizarCompromissoViewModel SeleciontarPorIdCompleto(Guid id)
        {
            var compromisso = servicoCompromisso.SelecionarPorId(id).Value;

            var compromissoViewModel = new VisualizarCompromissoViewModel
            {
                Id = compromisso.Id,
                Assunto = compromisso.Assunto,
                Data = compromisso.Data,
                Local = compromisso.Local,
                TipoLocalizacao = compromisso.TipoLocalizacao,
                Link = compromisso.Link,
                HoraInicio = compromisso.HoraInicio.ToString(@"hh\:mm\:ss"),
                HoraTermino = compromisso.HoraTermino.ToString(@"hh\:mm\:ss"),
            };


            //if (compromisso.Contato != null)
            //{
            //    var contato = compromisso.Contato;

            //    var contatoViewModel = new ListarContatoViewModel
            //    {
            //        Id = contato.Id,
            //        Nome = contato.Nome,
            //        Cargo = contato.Cargo,
            //        Empresa = contato.Empresa,
            //        Email = contato.Email,
            //        Telefone = contato.Telefone,
            //    };

            //    compromissoViewModel.Contato = contatoViewModel;
            //}

            return compromissoViewModel;
        }

        [HttpPost]
        public string Inserir(InserirCompromissoViewModel compromissoViewModel)
        {
            Compromisso compromisso = new Compromisso
            {
                Assunto = compromissoViewModel.Assunto,
                Data = compromissoViewModel.Data,
                Local = compromissoViewModel.Local,
                TipoLocalizacao = compromissoViewModel.TipoLocalizacao,
                Link = compromissoViewModel.Link,
                HoraInicio = (Convert.ToDateTime(compromissoViewModel.HoraInicio)).TimeOfDay,
                HoraTermino = (Convert.ToDateTime(compromissoViewModel.HoraTermino)).TimeOfDay
            };

            if (compromissoViewModel.ContatoId != null)
            {
                var resultadoBusca =
                    servicoContato.SelecionarPorId(compromissoViewModel.ContatoId);

                if (resultadoBusca.IsFailed)
                {

                    string[] errosBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                    return string.Join("\r\n", errosBusca);
                }

                compromisso.Contato = resultadoBusca.Value;
            }

            var resultado = servicoCompromisso.Inserir(compromisso);

            if (resultado.IsSuccess)
                return "Compromisso inserido com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();
            return string.Join("\r\n", erros);
        }

        [HttpPut("{id}")]
        public string Editar(Guid id, InserirCompromissoViewModel compromissoViewModel)
        {
            var resultadoBusca = servicoCompromisso.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {
                string[] errosBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosBusca);
            }

            var compromisso = resultadoBusca.Value;

            compromisso.Assunto = compromissoViewModel.Assunto;
            compromisso.Data = compromissoViewModel.Data;
            compromisso.Local = compromissoViewModel.Local;
            compromisso.TipoLocalizacao = compromissoViewModel.TipoLocalizacao;
            compromisso.Link = compromissoViewModel.Link;
            compromisso.HoraInicio = (Convert.ToDateTime(compromissoViewModel.HoraInicio)).TimeOfDay;
            compromisso.HoraTermino = (Convert.ToDateTime(compromissoViewModel.HoraTermino)).TimeOfDay;

            if (compromissoViewModel.ContatoId != null)
            {
                var resultadoBuscaContato =
                    servicoContato.SelecionarPorId(compromissoViewModel.ContatoId);

                if (resultadoBuscaContato.IsFailed)
                {

                    string[] errosBuscaContato = resultadoBuscaContato.Errors.Select(x => x.Message).ToArray();

                    return string.Join("\r\n", errosBuscaContato);
                }

                compromisso.Contato = resultadoBuscaContato.Value;
            }

            var resultado = servicoCompromisso.Editar(compromisso);

            if (resultado.IsSuccess)
                return "Compromisso editado com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }


        [HttpDelete("{id}")]
        public string Excluir(Guid id)
        {
            var resultadoBusca = servicoCompromisso.SelecionarPorId(id);

            if (resultadoBusca.IsFailed)
            {

                string[] errosBusca = resultadoBusca.Errors.Select(x => x.Message).ToArray();

                return string.Join("\r\n", errosBusca);
            }

            var resultado = servicoCompromisso.Excluir(id);

            if (resultado.IsSuccess)
                return "Compromisso excluído com sucesso";

            string[] erros = resultado.Errors.Select(x => x.Message).ToArray();

            return string.Join("\r\n", erros);
        }
    }
}
