using Microsoft.EntityFrameworkCore;
using PartesReuniao.Data;
using PartesReuniao.Models;
using PartesReuniao.Models.Enums;

namespace PartesReuniao.Services;

public class DesignacaoMecanicaService
{
    private readonly AppDbContext _context;
    private readonly Random _random = new Random();
    
    public DesignacaoMecanicaService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<DesignacaoMecanica>> GerarDesignacoesMecanicas(int mesInicial, int anoInicial, DiasReuniao diasReuniao)
    {
        var designacoes = new List<DesignacaoMecanica>();
        var datas = ObterDatasReunioes(mesInicial, anoInicial, diasReuniao);
        var pessoasDesignadasNosPeriodo = new HashSet<int>();
        
        foreach (var data in datas)
        {
            var designacao = new DesignacaoMecanica
            {
                Data = data,
                MesInicial = mesInicial,
                AnoInicial = anoInicial
            };
            
            // INDICADORES (3 pessoas)
            var ind1 = await ObterProximaPessoa(TipoParte.Indicadores, pessoasDesignadasNosPeriodo);
            if (ind1 != null) { designacao.Indicador1Id = ind1.Id; pessoasDesignadasNosPeriodo.Add(ind1.Id); }
            
            var ind2 = await ObterProximaPessoa(TipoParte.Indicadores, pessoasDesignadasNosPeriodo);
            if (ind2 != null) { designacao.Indicador2Id = ind2.Id; pessoasDesignadasNosPeriodo.Add(ind2.Id); }
            
            var ind3 = await ObterProximaPessoa(TipoParte.Indicadores, pessoasDesignadasNosPeriodo);
            if (ind3 != null) { designacao.Indicador3Id = ind3.Id; pessoasDesignadasNosPeriodo.Add(ind3.Id); }
            
            // VOLANTE (2 pessoas)
            var vol1 = await ObterProximaPessoa(TipoParte.Volante, pessoasDesignadasNosPeriodo);
            if (vol1 != null) { designacao.Volante1Id = vol1.Id; pessoasDesignadasNosPeriodo.Add(vol1.Id); }
            
            var vol2 = await ObterProximaPessoa(TipoParte.Volante, pessoasDesignadasNosPeriodo);
            if (vol2 != null) { designacao.Volante2Id = vol2.Id; pessoasDesignadasNosPeriodo.Add(vol2.Id); }
            
            // ÁUDIO E VÍDEO (2 pessoas)
            var av1 = await ObterProximaPessoa(TipoParte.AudioVideo, pessoasDesignadasNosPeriodo);
            if (av1 != null) { designacao.AudioVideo1Id = av1.Id; pessoasDesignadasNosPeriodo.Add(av1.Id); }
            
            var av2 = await ObterProximaPessoa(TipoParte.AudioVideo, pessoasDesignadasNosPeriodo);
            if (av2 != null) { designacao.AudioVideo2Id = av2.Id; pessoasDesignadasNosPeriodo.Add(av2.Id); }
            
            designacoes.Add(designacao);
        }
        
        return designacoes;
    }
    
    private async Task<Pessoa?> ObterProximaPessoa(TipoParte tipoParte, HashSet<int> pessoasJaDesignadas)
    {
        var pessoasHabilitadas = await _context.PessoasPartes
            .Include(pp => pp.Pessoa)
            .Where(pp => pp.TipoParte == tipoParte && pp.Habilitado && pp.Pessoa.Ativo)
            .Select(pp => pp.Pessoa)
            .ToListAsync();
        
        if (!pessoasHabilitadas.Any()) return null;
        
        var historicos = await _context.HistoricoDesignacoes
            .Where(h => h.TipoParte == tipoParte)
            .ToListAsync();
        
        var disponiveis = pessoasHabilitadas.Where(p => !pessoasJaDesignadas.Contains(p.Id)).ToList();
        var paraEscolher = disponiveis.Any() ? disponiveis : pessoasHabilitadas;
        
        var nuncaFizeram = paraEscolher.Where(p => !historicos.Any(h => h.PessoaId == p.Id)).ToList();
        if (nuncaFizeram.Any()) return nuncaFizeram[_random.Next(nuncaFizeram.Count)];
        
        var comHistorico = paraEscolher
            .Select(p => new { Pessoa = p, UltimaData = historicos.Where(h => h.PessoaId == p.Id).Max(h => h.Data) })
            .OrderBy(x => x.UltimaData)
            .ToList();
        
        if (!comHistorico.Any()) return null;
        
        var dataMaisAntiga = comHistorico.First().UltimaData;
        var maisAntigos = comHistorico.Where(x => x.UltimaData == dataMaisAntiga).Select(x => x.Pessoa).ToList();
        
        return maisAntigos[_random.Next(maisAntigos.Count)];
    }
    
    private List<DateTime> ObterDatasReunioes(int mesInicial, int anoInicial, DiasReuniao diasReuniao)
    {
        var datas = new List<DateTime>();
        var dataInicio = new DateTime(anoInicial, mesInicial, 1);
        var dataFim = dataInicio.AddMonths(2).AddDays(-1);
        
        var (dia1, dia2) = diasReuniao switch
        {
            DiasReuniao.QuartaSabado => (DayOfWeek.Wednesday, DayOfWeek.Saturday),
            DiasReuniao.QuintaSabado => (DayOfWeek.Thursday, DayOfWeek.Saturday),
            DiasReuniao.QuartaDomingo => (DayOfWeek.Wednesday, DayOfWeek.Sunday),
            DiasReuniao.QuintaDomingo => (DayOfWeek.Thursday, DayOfWeek.Sunday),
            _ => (DayOfWeek.Thursday, DayOfWeek.Sunday)
        };
        
        for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
        {
            if (data.DayOfWeek == dia1 || data.DayOfWeek == dia2)
                datas.Add(data);
        }
        
        return datas.OrderBy(d => d).ToList();
    }
    
    public async Task SalvarHistorico(DesignacaoMecanica designacao)
    {
        var historicos = new List<HistoricoDesignacao>();
        
        if (designacao.Indicador1Id.HasValue)
            historicos.Add(Criar(designacao.Indicador1Id.Value, TipoParte.Indicadores, designacao.Data));
        if (designacao.Indicador2Id.HasValue)
            historicos.Add(Criar(designacao.Indicador2Id.Value, TipoParte.Indicadores, designacao.Data));
        if (designacao.Indicador3Id.HasValue)
            historicos.Add(Criar(designacao.Indicador3Id.Value, TipoParte.Indicadores, designacao.Data));
        if (designacao.Volante1Id.HasValue)
            historicos.Add(Criar(designacao.Volante1Id.Value, TipoParte.Volante, designacao.Data));
        if (designacao.Volante2Id.HasValue)
            historicos.Add(Criar(designacao.Volante2Id.Value, TipoParte.Volante, designacao.Data));
        if (designacao.AudioVideo1Id.HasValue)
            historicos.Add(Criar(designacao.AudioVideo1Id.Value, TipoParte.AudioVideo, designacao.Data));
        if (designacao.AudioVideo2Id.HasValue)
            historicos.Add(Criar(designacao.AudioVideo2Id.Value, TipoParte.AudioVideo, designacao.Data));
        
        _context.HistoricoDesignacoes.AddRange(historicos);
        await _context.SaveChangesAsync();
    }
    
    private HistoricoDesignacao Criar(int pessoaId, TipoParte tipo, DateTime data)
    {
        return new HistoricoDesignacao
        {
            PessoaId = pessoaId,
            TipoParte = tipo,
            Data = data,
            Semana = 0,
            Mes = data.Month,
            Ano = data.Year
        };
    }
}