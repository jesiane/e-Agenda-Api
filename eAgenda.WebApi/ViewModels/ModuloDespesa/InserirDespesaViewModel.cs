﻿using eAgenda.Dominio.ModuloDespesa;

namespace eAgenda.WebApi.ViewModels.ModuloDespesa
{
    public class InserirDespesaViewModel
    {
        public InserirDespesaViewModel()
        { 
            CategoriasSelecionadas = new List<Guid>();
        }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public FormaPgtoDespesaEnum FormaPagamento { get; set;}
        public List<Guid> CategoriasSelecionadas { get; set; }
    }
}
