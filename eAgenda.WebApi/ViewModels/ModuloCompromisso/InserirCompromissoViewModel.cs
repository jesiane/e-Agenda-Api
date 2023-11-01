using eAgenda.Dominio.ModuloCompromisso;

namespace eAgenda.WebApi.ViewModels.ModuloCompromisso
{
    public class InserirCompromissoViewModel
    {
       // public Guid Id { get; set; }
        public string Assunto { get; set; }
        public string Local { get; set; }
        public TipoLocalizacaoCompromissoEnum TipoLocalizacao { get;set; }  
        public string? Link { get; set; }
        public DateTime Data { get; set; }
        public string HoraInicio { get; set; }
        public string HoraTermino { get; set; }
        public Guid ContatoId { get; set; } 

    }
}
