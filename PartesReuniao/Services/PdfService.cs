using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PartesReuniao.Models;
using Microsoft.EntityFrameworkCore;
using PartesReuniao.Data;

namespace PartesReuniao.Services;

public class PdfService
{
    private readonly AppDbContext _context;
    
    public PdfService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<byte[]> GerarPdfMes(int mes, int ano)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var semanas = await _context.DesignacoesSemanas
            .Include(s => s.OracaoInicialPessoa)
            .Include(s => s.PresidentePessoa)
            .Include(s => s.TesourosPalavraPessoa)
            .Include(s => s.JoiasEspirituaisPessoa)
            .Include(s => s.LeituraBibliaPessoa)
            .Include(s => s.PartesMinisterio).ThenInclude(p => p.Pessoa1)
            .Include(s => s.PartesMinisterio).ThenInclude(p => p.Pessoa2)
            .Include(s => s.PartesVidaCrista).ThenInclude(p => p.Pessoa)
            .Include(s => s.EstudoLivroDirigentePessoa)
            .Include(s => s.EstudoLivroLeitorPessoa)
            .Include(s => s.ComentariosFinaisPessoa)
            .Include(s => s.OracaoFinalPessoa)
            .Where(s => s.Mes == mes && s.Ano == ano)
            .OrderBy(s => s.NumeroSemana)
            .ToListAsync();
        
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
                
                page.Content().Column(column =>
                {
                    foreach (var semana in semanas)
                    {
                        // CABEÇALHO
                        column.Item().BorderBottom(2).BorderColor(Colors.Black).PaddingBottom(5)
                            .AlignCenter().Text("NOSSA VIDA E MINISTÉRIO CRISTÃO")
                            .Bold().FontSize(16);
                        
                        column.Item().PaddingTop(5).PaddingBottom(5)
                            .Border(1).BorderColor(Colors.Black)
                            .AlignCenter().Text(semana.PeriodoTexto)
                            .FontSize(11);
                        
                        // SEÇÃO 1: ABERTURA
                        column.Item().PaddingTop(5).Border(1).BorderColor(Colors.Black).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });
                            
                            // Cântico e Oração
                            table.Cell().Padding(5).Text("Cântico").Bold();
                            table.Cell().Padding(5).AlignCenter().Text(semana.CanticoInicial.ToString()).Bold();
                            table.Cell().Padding(5).Text("Oração Inicial:").Bold();
                            table.Cell().Padding(5).Text(semana.OracaoInicialPessoa?.Nome ?? "").Bold();
                        });
                        
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });
                            
                            // Presidente
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosPresidente} Minuto" + (semana.MinutosPresidente > 1 ? "s" : ""));
                            table.Cell().Padding(5).Text("Presidente:").Bold();
                            table.Cell().Padding(5).Text("Comentários Iniciais");
                            table.Cell().Padding(5).Text(semana.PresidentePessoa?.Nome ?? "").Bold();
                        });
                        
                        // SEÇÃO 2: TESOUROS DA PALAVRA DE DEUS
                        column.Item().PaddingTop(10)
                            .Background(Colors.Black)
                            .Padding(8)
                            .AlignCenter()
                            .Text("TESOUROS DA PALAVRA DE DEUS")
                            .FontColor(Colors.White).Bold().FontSize(12);
                        
                        // Tesouros da Palavra
                        column.Item().Border(1).BorderColor(Colors.Black).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosTesourosPalavra} Minutos");
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.TesourosPalavraPessoa?.Nome ?? "").Bold();
                                col.Item().PaddingTop(2).Text(t =>
                                {
                                    t.Span("Tesouros da Palavra de Deus: ").Bold();
                                    t.Span(semana.TesourosTema);
                                });
                            });
                        });
                        
                        // Joias Espirituais
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosJoiasEspirituais} Minutos");
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.JoiasEspirituaisPessoa?.Nome ?? "").Bold();
                                col.Item().PaddingTop(2).Text(t =>
                                {
                                    t.Span("Joias espirituais: ").Bold();
                                    t.Span(semana.JoiasTema);
                                });
                            });
                        });
                        
                        // Leitura da Bíblia
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosLeituraBiblia} Minutos");
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.LeituraBibliaPessoa?.Nome ?? "").Bold();
                                col.Item().PaddingTop(2).Text(t =>
                                {
                                    t.Span("Leitura da Bíblia: ").Bold();
                                    t.Span(semana.LeituraTema);
                                });
                            });
                        });
                        
                        // SEÇÃO 3: FAÇA SEU MELHOR NO MINISTÉRIO
                        column.Item().PaddingTop(10)
                            .Background(Colors.Black)
                            .Padding(8)
                            .AlignCenter()
                            .Text("FAÇA SEU MELHOR NO MINISTÉRIO")
                            .FontColor(Colors.White).Bold().FontSize(12);
                        
                        foreach (var parte in semana.PartesMinisterio.OrderBy(p => p.Ordem))
                        {
                            column.Item().Border(1).BorderColor(Colors.Black).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(70);
                                    columns.RelativeColumn();
                                });
                                
                                table.Cell().Padding(5).AlignCenter().Text($"{parte.Minutos} Minutos");
                                table.Cell().Padding(5).Column(col =>
                                {
                                    col.Item().Text(t =>
                                    {
                                        if (parte.Pessoa1 != null)
                                        {
                                            t.Span(parte.Pessoa1.Nome).Bold();
                                            if (parte.Pessoa2 != null)
                                            {
                                                t.Span(" E ");
                                                t.Span(parte.Pessoa2.Nome).Bold();
                                            }
                                        }
                                    });
                                    col.Item().PaddingTop(2).Text(t =>
                                    {
                                        t.Span(parte.Titulo + ": ").Bold();
                                        t.Span(parte.Subtitulo);
                                    });
                                });
                            });
                        }
                        
                        // SEÇÃO 4: NOSSA VIDA CRISTÃ
                        column.Item().PaddingTop(10)
                            .Background(Colors.Black)
                            .Padding(8)
                            .AlignCenter()
                            .Text("NOSSA VIDA CRISTÃ")
                            .FontColor(Colors.White).Bold().FontSize(12);
                        
                        // Cântico
                        column.Item().Border(1).BorderColor(Colors.Black).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).Text("Cântico:").Bold();
                            table.Cell().Padding(5).AlignCenter().Text(semana.CanticoMeio.ToString()).Bold();
                        });
                        
                        // Partes Vida Cristã
                        foreach (var parte in semana.PartesVidaCrista.OrderBy(p => p.Ordem))
                        {
                            column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(70);
                                    columns.RelativeColumn();
                                });
                                
                                table.Cell().Padding(5).AlignCenter().Text($"{parte.Minutos} Minutos");
                                table.Cell().Padding(5).Column(col =>
                                {
                                    col.Item().Text(parte.Titulo).Bold();
                                    if (parte.Pessoa != null)
                                    {
                                        col.Item().PaddingTop(2).Text(parte.Pessoa.Nome);
                                    }
                                });
                            });
                        }
                        
                        // Estudo de Livro
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(120);
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosEstudoLivro} Minutos");
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.EstudoLivroDirigentePessoa?.Nome ?? "").Bold();
                            });
                            table.Cell().Padding(5).Text("Leitor:").Bold();
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.EstudoLivroLeitorPessoa?.Nome ?? "").Bold();
                                col.Item().PaddingTop(2).Text(t =>
                                {
                                    t.Span("Livro: ").Bold();
                                    t.Span(semana.EstudoLivroTema);
                                });
                            });
                        });
                        
                        // Comentários Finais
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).AlignCenter().Text($"{semana.MinutosComentariosFinais} Minutos");
                            table.Cell().Padding(5).Column(col =>
                            {
                                col.Item().Text(semana.ComentariosFinaisPessoa?.Nome ?? "").Bold();
                                col.Item().PaddingTop(2).Text("Comentários finais").Bold();
                            });
                        });
                        
                        // Encerramento
                        column.Item().Border(1).BorderColor(Colors.Black).BorderTop(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });
                            
                            table.Cell().Padding(5).Text("Cântico:").Bold();
                            table.Cell().Padding(5).AlignCenter().Text(semana.CanticoFinal.ToString()).Bold();
                            table.Cell().Padding(5).Text("Oração final:").Bold();
                            table.Cell().Padding(5).Text(semana.OracaoFinalPessoa?.Nome ?? "").Bold();
                        });
                        
                        // Espaço entre semanas
                        if (semana != semanas.Last())
                        {
                            column.Item().PageBreak();
                        }
                    }
                });
                
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        });
        
        return pdf.GeneratePdf();
    }
    
    public async Task<byte[]> GerarPdfDesignacoesMecanicas(int mesInicial, int anoInicial, List<DesignacaoMecanica> designacoes)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var designacoesCompletas = new List<DesignacaoMecanica>();
        foreach (var d in designacoes)
        {
            var completa = await _context.DesignacoesMecanicas
                .Include(x => x.Indicador1)
                .Include(x => x.Indicador2)
                .Include(x => x.Indicador3)
                .Include(x => x.Volante1)
                .Include(x => x.Volante2)
                .Include(x => x.AudioVideo1)
                .Include(x => x.AudioVideo2)
                .FirstOrDefaultAsync(x => x.Id == d.Id);
            
            if (completa != null)
                designacoesCompletas.Add(completa);
        }
        
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));
                
                page.Content().Column(column =>
                {
                    // Header CONGREGAÇÃO CAPARROZ
                    column.Item().AlignCenter()
                        .Background(Colors.Black)
                        .Padding(10)
                        .Text("CONGREGAÇÃO CAPARROZ")
                        .FontColor(Colors.White)
                        .Bold()
                        .FontSize(18);
                    
                    // Tabela
                    column.Item().PaddingTop(0).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);   // DATA
                            columns.RelativeColumn(3.5f); // INDICADORES
                            columns.RelativeColumn(2.5f); // VOLANTE
                            columns.RelativeColumn(2f);   // ÁUDIO E VIDEO
                        });
                        
                        // Header da tabela (fundo preto)
                        table.Cell().Border(2).BorderColor(Colors.Black).Background(Colors.Black)
                            .Padding(5).AlignCenter().Text("DATA").Bold().FontColor(Colors.White).FontSize(10);
                        table.Cell().Border(2).BorderColor(Colors.Black).Background(Colors.Black)
                            .Padding(5).AlignCenter().Text("INDICADORES").Bold().FontColor(Colors.White).FontSize(10);
                        table.Cell().Border(2).BorderColor(Colors.Black).Background(Colors.Black)
                            .Padding(5).AlignCenter().Text("VOLANTE").Bold().FontColor(Colors.White).FontSize(10);
                        table.Cell().Border(2).BorderColor(Colors.Black).Background(Colors.Black)
                            .Padding(5).AlignCenter().Text("ÁUDIO E VÍDEO").Bold().FontColor(Colors.White).FontSize(10);
                        
                        // Rows
                        foreach (var d in designacoesCompletas.OrderBy(x => x.Data))
                        {
                            // DATA
                            table.Cell().Border(2).BorderColor(Colors.Black)
                                .Padding(5).AlignCenter()
                                .Text(d.Data.ToString("d-MMM.", new System.Globalization.CultureInfo("pt-BR")))
                                .FontSize(9);
                            
                            // INDICADORES
                            table.Cell().Border(2).BorderColor(Colors.Black).Padding(5).AlignCenter().Column(col =>
                            {
                                if (d.Indicador1 != null) col.Item().Text(d.Indicador1.Nome.ToUpper()).FontSize(9);
                                if (d.Indicador2 != null) col.Item().Text(d.Indicador2.Nome.ToUpper()).FontSize(9);
                                if (d.Indicador3 != null) col.Item().Text(d.Indicador3.Nome.ToUpper()).FontSize(9);
                            });
                            
                            // VOLANTE
                            table.Cell().Border(2).BorderColor(Colors.Black).Padding(5).AlignCenter().Column(col =>
                            {
                                if (d.Volante1 != null) col.Item().Text(d.Volante1.Nome.ToUpper()).FontSize(9);
                                if (d.Volante2 != null) col.Item().Text(d.Volante2.Nome.ToUpper()).FontSize(9);
                            });
                            
                            // ÁUDIO E VIDEO
                            table.Cell().Border(2).BorderColor(Colors.Black).Padding(5).AlignCenter().Column(col =>
                            {
                                if (d.AudioVideo1 != null) col.Item().Text(d.AudioVideo1.Nome.ToUpper()).FontSize(9);
                                if (d.AudioVideo2 != null) col.Item().Text(d.AudioVideo2.Nome.ToUpper()).FontSize(9);
                            });
                        }
                    });
                });
            });
        });
        
        return pdf.GeneratePdf();
    }
}