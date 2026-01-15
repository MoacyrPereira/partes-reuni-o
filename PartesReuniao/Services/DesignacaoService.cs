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
    
    private readonly Random _random = new Random();
    
    /// <summary>
    /// Gera designações automáticas para todas as semanas do mês
    /// </summary>
    public async Task<List<DesignacaoSemana>> GerarDesignacoesMesCompleto(int mes, int ano)
    {
        var semanas = new List<DesignacaoSemana>();
        var quintas = ObterQuintasFeiras(mes, ano);
        
        // Controle de pessoas já designadas no mês (para qualquer parte)
        var pessoasDesignadasNoMes = new HashSet<int>();
        
        for (int i = 0; i < quintas.Count; i++)
        {
            var semana = new DesignacaoSemana
            {
                Mes = mes,
                Ano = ano,
                NumeroSemana = i + 1,
                PeriodoTexto = ObterPeriodoTexto(quintas[i]),
                CanticoInicial = 1,
                CanticoMeio = 1,
                CanticoFinal = 1
            };
            
            // PRESIDENTE
            var presidente = await ObterProximaPessoaParaParte(TipoParte.Presidentes, mes, ano, pessoasDesignadasNoMes);
            if (presidente != null)
            {
                semana.ComentariosFinaisPessoaId = presidente.Id;
            }
            
            // ORAÇÃO INICIAL
            var oracaoInicial = await ObterProximaPessoaParaParte(TipoParte.OracaoInicialFinal, mes, ano, pessoasDesignadasNoMes);
            if (oracaoInicial != null)
            {
                semana.OracaoInicialPessoaId = oracaoInicial.Id;
                pessoasDesignadasNoMes.Add(oracaoInicial.Id);
            }
            
            // TESOUROS DA PALAVRA
            var tesouros = await ObterProximaPessoaParaParte(TipoParte.TesourosPalavra, mes, ano, pessoasDesignadasNoMes);
            if (tesouros != null)
            {
                semana.TesourosPalavraPessoaId = tesouros.Id;
                pessoasDesignadasNoMes.Add(tesouros.Id);
            }
            
            // ENCONTRE JOIAS
            var joias = await ObterProximaPessoaParaParte(TipoParte.EncontreJoias, mes, ano, pessoasDesignadasNoMes);
            if (joias != null)
            {
                semana.JoiasEspirituaisPessoaId = joias.Id;
                pessoasDesignadasNoMes.Add(joias.Id);
            }
            
            // LEITURA DA BÍBLIA
            var leitura = await ObterProximaPessoaParaParte(TipoParte.LeituraBiblia, mes, ano, pessoasDesignadasNoMes);
            if (leitura != null)
            {
                semana.LeituraBibliaPessoaId = leitura.Id;
                pessoasDesignadasNoMes.Add(leitura.Id);
            }
            
            // FAÇA SEU MELHOR NO MINISTÉRIO
            // Priorizar Campo F (2 partes) + Campo M (1 parte)
            semana.PartesMinisterio = new List<ParteMinisterio>();

            // PARTE 1 - CAMPO FEMININO (3 minutos)
            var campoF1_P1 = await ObterProximaPessoaCampoFeminino(mes, ano, pessoasDesignadasNoMes);
            int? campoF1_P1Id = null;
            if (campoF1_P1 != null)
            {
                campoF1_P1Id = campoF1_P1.Id;
                pessoasDesignadasNoMes.Add(campoF1_P1.Id);
            }

            var campoF1_P2 = await ObterProximaPessoaCampoFeminino(mes, ano, pessoasDesignadasNoMes);
            int? campoF1_P2Id = null;
            if (campoF1_P2 != null)
            {
                campoF1_P2Id = campoF1_P2.Id;
                pessoasDesignadasNoMes.Add(campoF1_P2.Id);
            }

            semana.PartesMinisterio.Add(new ParteMinisterio
            {
                Ordem = 1,
                Minutos = 3,
                Titulo = "Iniciando conversas",
                Subtitulo = "TESTEMUNHO INFORMAL",
                Pessoa1Id = campoF1_P1Id,
                Pessoa2Id = campoF1_P2Id
            });

            // PARTE 2 - CAMPO MASCULINO (4 minutos)
            var campoM_P1 = await ObterProximaPessoaCampoMasculino(mes, ano, pessoasDesignadasNoMes);
            int? campoM_P1Id = null;
            if (campoM_P1 != null)
            {
                campoM_P1Id = campoM_P1.Id;
                pessoasDesignadasNoMes.Add(campoM_P1.Id);
            }

            var campoM_P2 = await ObterProximaPessoaCampoMasculino(mes, ano, pessoasDesignadasNoMes);
            int? campoM_P2Id = null;
            if (campoM_P2 != null)
            {
                campoM_P2Id = campoM_P2.Id;
                pessoasDesignadasNoMes.Add(campoM_P2.Id);
            }

            semana.PartesMinisterio.Add(new ParteMinisterio
            {
                Ordem = 2,
                Minutos = 4,
                Titulo = "Iniciando conversas",
                Subtitulo = "DE CASA EM CASA",
                Pessoa1Id = campoM_P1Id,
                Pessoa2Id = campoM_P2Id
            });

            // PARTE 3 - CAMPO FEMININO (5 minutos)
            var campoF2_P1 = await ObterProximaPessoaCampoFeminino(mes, ano, pessoasDesignadasNoMes);
            int? campoF2_P1Id = null;
            if (campoF2_P1 != null)
            {
                campoF2_P1Id = campoF2_P1.Id;
                pessoasDesignadasNoMes.Add(campoF2_P1.Id);
            }

            var campoF2_P2 = await ObterProximaPessoaCampoFeminino(mes, ano, pessoasDesignadasNoMes);
            int? campoF2_P2Id = null;
            if (campoF2_P2 != null)
            {
                campoF2_P2Id = campoF2_P2.Id;
                pessoasDesignadasNoMes.Add(campoF2_P2.Id);
            }

            semana.PartesMinisterio.Add(new ParteMinisterio
            {
                Ordem = 3,
                Minutos = 5,
                Titulo = "Iniciando conversas",
                Subtitulo = "TESTEMUNHO INFORMAL",
                Pessoa1Id = campoF2_P1Id,
                Pessoa2Id = campoF2_P2Id
            });
            
            // NOSSA VIDA CRISTÃ (1 parte inicial)
            semana.PartesVidaCrista = new List<ParteVidaCrista>();
            var vidaCrista = await ObterProximaPessoaParaParte(TipoParte.NossaVidaCrista, mes, ano, pessoasDesignadasNoMes);
            semana.PartesVidaCrista.Add(new ParteVidaCrista
            {
                Ordem = 1,
                Minutos = 15,
                Titulo = "NECESSIDADES LOCAIS",
                PessoaId = vidaCrista?.Id
            });
            if (vidaCrista != null)
            {
                pessoasDesignadasNoMes.Add(vidaCrista.Id);
            }
            
            // ESTUDO DE LIVRO
            var estudoDirigente = await ObterProximaPessoaParaParte(TipoParte.EstudoLivro, mes, ano, pessoasDesignadasNoMes);
            if (estudoDirigente != null)
            {
                semana.EstudoLivroDirigentePessoaId = estudoDirigente.Id;
                pessoasDesignadasNoMes.Add(estudoDirigente.Id);
            }
            
            var estudoLeitor = await ObterProximaPessoaParaParte(TipoParte.LeitorLivro, mes, ano, pessoasDesignadasNoMes);
            if (estudoLeitor != null)
            {
                semana.EstudoLivroLeitorPessoaId = estudoLeitor.Id;
                pessoasDesignadasNoMes.Add(estudoLeitor.Id);
            }
            
            // COMENTÁRIOS FINAIS (mesmo que presidente, pode ser outra pessoa se presidente já foi usado)
            // COMENTÁRIOS FINAIS (mesmo que presidente, pode ser outra pessoa se presidente já foi usado)
            var comentarios = await ObterProximaPessoaParaParte(TipoParte.Presidentes, mes, ano, pessoasDesignadasNoMes);
            if (comentarios != null)
            {
                semana.ComentariosFinaisPessoaId = comentarios.Id;
                pessoasDesignadasNoMes.Add(comentarios.Id);
            }
            
            // ORAÇÃO FINAL
            var oracaoFinal = await ObterProximaPessoaParaParte(TipoParte.OracaoInicialFinal, mes, ano, pessoasDesignadasNoMes);
            if (oracaoFinal != null)
            {
                semana.OracaoFinalPessoaId = oracaoFinal.Id;
                pessoasDesignadasNoMes.Add(oracaoFinal.Id);
            }
            
            semanas.Add(semana);
        }
        
        return semanas;
    }
    
   /// <summary>
    /// Obtém próxima pessoa para uma parte específica (ALEATÓRIO)
    /// </summary>
    private async Task<Pessoa?> ObterProximaPessoaParaParte(TipoParte tipoParte, int mes, int ano, HashSet<int> pessoasJaDesignadasNoMes)
    {
        // Busca todas as pessoas habilitadas para esta parte
        var pessoasHabilitadas = await _context.PessoasPartes
            .Include(pp => pp.Pessoa)
            .Where(pp => pp.TipoParte == tipoParte && pp.Habilitado && pp.Pessoa.Ativo)
            .Select(pp => pp.Pessoa)
            .ToListAsync();
        
        if (!pessoasHabilitadas.Any())
            return null;
        
        // Busca histórico de designações para esta parte específica
        var historicos = await _context.HistoricoDesignacoes
            .Where(h => h.TipoParte == tipoParte)
            .ToListAsync();
        
        // PRIORIDADE 1: Pessoas habilitadas que NÃO foram designadas no mês atual
        var pessoasDisponiveisNoMes = pessoasHabilitadas
            .Where(p => !pessoasJaDesignadasNoMes.Contains(p.Id))
            .ToList();
        
        // Se ninguém está disponível (todos já foram designados), usar todos os habilitados
        var pessoasParaEscolher = pessoasDisponiveisNoMes.Any() 
            ? pessoasDisponiveisNoMes 
            : pessoasHabilitadas;
        
        // PRIORIDADE 2: Quem nunca fez esta parte específica
        var pessoasQueNuncaFizeram = pessoasParaEscolher
            .Where(p => !historicos.Any(h => h.PessoaId == p.Id))
            .ToList();
        
        if (pessoasQueNuncaFizeram.Any())
        {
            // ALEATÓRIO entre os que nunca fizeram
            return pessoasQueNuncaFizeram[_random.Next(pessoasQueNuncaFizeram.Count)];
        }
        
        // PRIORIDADE 3: Quem fez há mais tempo
        var pessoasComHistorico = pessoasParaEscolher
            .Select(p => new
            {
                Pessoa = p,
                UltimaData = historicos
                    .Where(h => h.PessoaId == p.Id)
                    .Max(h => h.Data)
            })
            .OrderBy(x => x.UltimaData)
            .ToList();
        
        if (!pessoasComHistorico.Any())
            return null;
        
        // Pega a data mais antiga
        var dataMaisAntiga = pessoasComHistorico.First().UltimaData;
        
        // Filtra todos que têm a mesma data mais antiga
        var pessoasComMesmaDataAntiga = pessoasComHistorico
            .Where(x => x.UltimaData == dataMaisAntiga)
            .Select(x => x.Pessoa)
            .ToList();
        
        // ALEATÓRIO entre os que fizeram há mais tempo
        return pessoasComMesmaDataAntiga[_random.Next(pessoasComMesmaDataAntiga.Count)];
    }

    /// <summary>
    /// Obtém próxima pessoa genérica (para ministério que aceita qualquer pessoa) - ALEATÓRIO
    /// </summary>
    private async Task<Pessoa?> ObterProximaPessoaGenerica(int mes, int ano, HashSet<int> pessoasJaDesignadasNoMes)
    {
        // Busca todas as pessoas ativas
        var pessoasAtivas = await _context.Pessoas
            .Where(p => p.Ativo)
            .ToListAsync();
        
        if (!pessoasAtivas.Any())
            return null;
        
        // Pessoas que NÃO foram designadas no mês
        var pessoasDisponiveisNoMes = pessoasAtivas
            .Where(p => !pessoasJaDesignadasNoMes.Contains(p.Id))
            .ToList();
        
        // Se ninguém está disponível, usar todos
        var pessoasParaEscolher = pessoasDisponiveisNoMes.Any() 
            ? pessoasDisponiveisNoMes 
            : pessoasAtivas;
        
        // ALEATÓRIO
        return pessoasParaEscolher[_random.Next(pessoasParaEscolher.Count)];
    }
    
    /// <summary>
    /// Salva designação no histórico
    /// </summary>
    public async Task SalvarHistorico(DesignacaoSemana semana)
    {
        var data = new DateTime(semana.Ano, semana.Mes, 1);
        
        // Salvar todas as designações no histórico
        var historicos = new List<HistoricoDesignacao>();
        
        if (semana.PresidentePessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.PresidentePessoaId.Value, TipoParte.Presidentes, data, semana.NumeroSemana));
        
        if (semana.OracaoInicialPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.OracaoInicialPessoaId.Value, TipoParte.OracaoInicialFinal, data, semana.NumeroSemana));
        
        if (semana.TesourosPalavraPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.TesourosPalavraPessoaId.Value, TipoParte.TesourosPalavra, data, semana.NumeroSemana));
        
        if (semana.JoiasEspirituaisPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.JoiasEspirituaisPessoaId.Value, TipoParte.EncontreJoias, data, semana.NumeroSemana));
        
        if (semana.LeituraBibliaPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.LeituraBibliaPessoaId.Value, TipoParte.LeituraBiblia, data, semana.NumeroSemana));
        
        if (semana.EstudoLivroDirigentePessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.EstudoLivroDirigentePessoaId.Value, TipoParte.EstudoLivro, data, semana.NumeroSemana));
        
        if (semana.EstudoLivroLeitorPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.EstudoLivroLeitorPessoaId.Value, TipoParte.LeitorLivro, data, semana.NumeroSemana));
        
        if (semana.ComentariosFinaisPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.ComentariosFinaisPessoaId.Value, TipoParte.Presidentes, data, semana.NumeroSemana));
        
        if (semana.OracaoFinalPessoaId.HasValue)
            historicos.Add(CriarHistorico(semana.OracaoFinalPessoaId.Value, TipoParte.OracaoInicialFinal, data, semana.NumeroSemana));
        
        foreach (var parte in semana.PartesVidaCrista)
        {
            if (parte.PessoaId.HasValue)
                historicos.Add(CriarHistorico(parte.PessoaId.Value, TipoParte.NossaVidaCrista, data, semana.NumeroSemana));
        }
        
        _context.HistoricoDesignacoes.AddRange(historicos);
        await _context.SaveChangesAsync();
    }
    
    private HistoricoDesignacao CriarHistorico(int pessoaId, TipoParte tipoParte, DateTime data, int semana)
    {
        return new HistoricoDesignacao
        {
            PessoaId = pessoaId,
            TipoParte = tipoParte,
            Data = data,
            Semana = semana,
            Mes = data.Month,
            Ano = data.Year
        };
    }
    
    private List<DateTime> ObterQuintasFeiras(int mes, int ano)
    {
        var quintas = new List<DateTime>();
        var primeiroDia = new DateTime(ano, mes, 1);
        var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);
        
        for (var data = primeiroDia; data <= ultimoDia; data = data.AddDays(1))
        {
            if (data.DayOfWeek == DayOfWeek.Thursday)
                quintas.Add(data);
        }
        
        return quintas;
    }
    
    private string ObterPeriodoTexto(DateTime data)
    {
        var fimSemana = data.AddDays(6);
        var meses = new[] { "", "JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO",
                           "JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO" };
        return $"{data.Day:00} A {fimSemana.Day:00} DE {meses[data.Month]}";
    }
    
    /// <summary>
    /// Obtém próxima pessoa habilitada para Campo Masculino
    /// </summary>
    private async Task<Pessoa?> ObterProximaPessoaCampoMasculino(int mes, int ano, HashSet<int> pessoasJaDesignadasNoMes)
    {
        return await ObterProximaPessoaParaParte(TipoParte.FacaSeuMelhorCampoMasculino, mes, ano, pessoasJaDesignadasNoMes);
    }

    /// <summary>
    /// Obtém próxima pessoa habilitada para Campo Feminino
    /// </summary>
    private async Task<Pessoa?> ObterProximaPessoaCampoFeminino(int mes, int ano, HashSet<int> pessoasJaDesignadasNoMes)
    {
        return await ObterProximaPessoaParaParte(TipoParte.FacaSeuMelhorCampoFeminino, mes, ano, pessoasJaDesignadasNoMes);
    }
}