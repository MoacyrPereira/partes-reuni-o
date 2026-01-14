namespace PartesReuniao.Models;

public class ParteMinisterio
{
    public int Id { get; set; }
    public int DesignacaoSemanaId { get; set; }
    public DesignacaoSemana DesignacaoSemana { get; set; } = null!;
    
    public int Ordem { get; set; }
    public int Minutos { get; set; } = 3;
    public string Titulo { get; set; } = "Iniciando conversas";
    public string Subtitulo { get; set; } = "TESTEMUNHO INFORMAL";
    
    public int? Pessoa1Id { get; set; }
    public Pessoa? Pessoa1 { get; set; }
    
    public int? Pessoa2Id { get; set; }
    public Pessoa? Pessoa2 { get; set; }
}