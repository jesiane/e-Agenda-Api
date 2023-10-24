using eAgenda.Dominio.Compartilhado;
using System;
using System.Collections.Generic;

namespace eAgenda.Dominio.ModuloDespesa
{
    public interface IRepositorioDespesa : IRepositorio<Despesa>
    {
        List<Despesa> SelecionarDespesasAntigas(DateTime data, Guid usuarioId);
        List<Despesa> SelecionarDespesasUltimos30Dias(DateTime dataBase, Guid usuarioId);
    }
}
