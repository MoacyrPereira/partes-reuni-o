namespace PartesReuniao.Models;

public class ParteVidaCrista
{
    public int Id { get; set; }
    public int DesignacaoSemanaId { get; set; }
    public DesignacaoSemana DesignacaoSemana { get; set; } = null!;
    
    public int Ordem { get; set; }
    public int Minutos { get; set; } = 15;
    public string Titulo { get; set; } = "NECESSIDADES LOCAIS";
    
    public int? PessoaId { get; set; }
    public Pessoa? Pessoa { get; set; }
}