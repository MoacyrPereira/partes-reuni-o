using Microsoft.EntityFrameworkCore;
using PartesReuniao.Data;
using PartesReuniao.Models;
using PartesReuniao.Models.Enums;

namespace PartesReuniao.Services;

public class DesignacaoService
{
    private readonly AppDbContext _context;
    
    public DesignacaoService(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Busca a próxima pessoa disponível para uma parte específica
    /// Prioridade: quem nunca fez > quem fez há mais tempo
    /// Evita designar a mesma pessoa no mesmo mês
    /// </summary>
    public async Task<Pessoa?> ObterProximaPessoaParaParte(TipoParte tipoParte, int mes, int ano)
    {
        // Busca todas as pessoas habilitadas para esta parte
        var pessoasHabilitadas = await _context.PessoasPartes
            .Include(pp => pp.Pessoa)
            .Where(pp => pp.TipoParte == tipoParte && pp.Habilitado && pp.Pessoa.Ativo)
            .Select(pp => pp.Pessoa)
            .ToListAsync();
        
        if (!pessoasHabilitadas.Any())
            return null;
        
        // Busca histórico de designações para esta parte
        var historicos = await _context.HistoricoDesignacoes
            .Where(h => h.TipoParte == tipoParte)
            .ToListAsync();
        
        // Busca pessoas já designadas neste mês (para qualquer parte)
        var pessoasDesignadasNoMes = await _context.HistoricoDesignacoes
            .Where(h => h.Mes == mes && h.Ano == ano)
            .Select(h => h.PessoaId)
            .Distinct()
            .ToListAsync();
        
        // Filtra pessoas que ainda não foram designadas neste mês
        var pessoasDisponiveisNoMes = pessoasHabilitadas
            .Where(p => !pessoasDesignadasNoMes.Contains(p.Id))
            .ToList();
        
        // Se todas as pessoas já foram designadas no mês, usa todas as habilitadas
        var pessoasParaEscolher = pessoasDisponiveisNoMes.Any() 
            ? pessoasDisponiveisNoMes 
            : pessoasHabilitadas;
        
        // Separa quem nunca fez esta parte
        var pessoasQuNuncaFizeram = pessoasParaEscolher
            .Where(p => !historicos.Any(h => h.PessoaId == p.Id))
            .ToList();
        
        if (pessoasQuNuncaFizeram.Any())
        {
            // Retorna primeira pessoa que nunca fez (ordem alfabética)
            return pessoasQuNuncaFizeram.OrderBy(p => p.Nome).First();
        }
        
        // Se todos já fizeram, retorna quem fez há mais tempo
        var pessoaComHistorico = pessoasParaEscolher
            .Select(p => new
            {
                Pessoa = p,
                UltimaData = historicos
                    .Where(h => h.PessoaId == p.Id)
                    .Max(h => h.Data)
            })
            .OrderBy(x => x.UltimaData)
            .ThenBy(x => x.Pessoa.Nome)
            .First();
        
        return pessoaComHistorico.Pessoa;
    }
    
    /// <summary>
    /// Salva designação no histórico
    /// </summary>
    public async Task SalvarDesignacao(int pessoaId, TipoParte tipoParte, DateTime data, int semana)
    {
        var historico = new HistoricoDesignacao
        {
            PessoaId = pessoaId,
            TipoParte = tipoParte,
            Data = data,
            Semana = semana,
            Mes = data.Month,
            Ano = data.Year
        };
        
        _context.HistoricoDesignacoes.Add(historico);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Gera designações automáticas para um mês inteiro
    /// </summary>
    public async Task<Dictionary<int, Dictionary<TipoParte, Pessoa?>>> GerarDesignacoesMes(int mes, int ano)
    {
        var resultado = new Dictionary<int, Dictionary<TipoParte, Pessoa?>>();
        var todasAsPartes = Enum.GetValues<TipoParte>();
        
        // Calcula número de semanas no mês (considerando quintas-feiras)
        var numeroDeSemanas = ObterNumeroDeSemanas(mes, ano);
        
        for (int semana = 1; semana <= numeroDeSemanas; semana++)
        {
            var designacoesDaSemana = new Dictionary<TipoParte, Pessoa?>();
            
            foreach (var parte in todasAsPartes)
            {
                var pessoa = await ObterProximaPessoaParaParte(parte, mes, ano);
                designacoesDaSemana[parte] = pessoa;
            }
            
            resultado[semana] = designacoesDaSemana;
        }
        
        return resultado;
    }
    
    /// <summary>
    /// Calcula número de quintas-feiras (reuniões) em um mês
    /// </summary>
    private int ObterNumeroDeSemanas(int mes, int ano)
    {
        var primeiroDia = new DateTime(ano, mes, 1);
        var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);
        
        int contador = 0;
        for (var data = primeiroDia; data <= ultimoDia; data = data.AddDays(1))
        {
            if (data.DayOfWeek == DayOfWeek.Thursday) // Quinta-feira
                contador++;
        }
        
        return contador;
    }
    
    public async Task<List<Pessoa>> ObterPessoasHabilitadasParaParte(TipoParte tipoParte)
    {
        return await _context.PessoasPartes
            .Include(pp => pp.Pessoa)
            .Where(pp => pp.TipoParte == tipoParte && pp.Habilitado && pp.Pessoa.Ativo)
            .Select(pp => pp.Pessoa)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
}