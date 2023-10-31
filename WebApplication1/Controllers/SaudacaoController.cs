using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaudacaoController : ControllerBase
    {
        [HttpGet("bom-dia")]
        public Saudacao BomDia()
        {
             return new Saudacao
            {
                Data = DateTime.Now,
                Mensagem = "Bom dia Galera da Academia"
            };
        }
        [HttpGet("boa-tarde")]
        public Saudacao BoaTarde()
        {
            return new Saudacao
            {
                Data = DateTime.Now,
                Mensagem = "Boa tarde Galera da Academia"
            };
        }
    }

    public class Saudacao
    {
        public DateTime Data { get; set; }
        public string Mensagem { get; set; }
    }
}
