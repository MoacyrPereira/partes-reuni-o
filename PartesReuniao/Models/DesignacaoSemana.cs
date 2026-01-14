namespace PartesReuniao.Models;

public class DesignacaoSemana
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public int NumeroSemana { get; set; }
    public string PeriodoTexto { get; set; } = string.Empty; // Ex: "01 A 07 DE SETEMBRO"
    
    // Seção 1
    public int CanticoInicial { get; set; }
    public int? OracaoInicialPessoaId { get; set; }
    public Pessoa? OracaoInicialPessoa { get; set; }
    public int MinutosPresidente { get; set; } = 1;
    public int? PresidentePessoaId { get; set; }
    public Pessoa? PresidentePessoa { get; set; }
    
    // Tesouros da Palavra
    public int MinutosTesourosPalavra { get; set; } = 10;
    public int? TesourosPalavraPessoaId { get; set; }
    public Pessoa? TesourosPalavraPessoa { get; set; }
    public string TesourosTema { get; set; } = string.Empty;
    
    public int MinutosJoiasEspirituais { get; set; } = 10;
    public int? JoiasEspirituaisPessoaId { get; set; }
    public Pessoa? JoiasEspirituaisPessoa { get; set; }
    public string JoiasTema { get; set; } = string.Empty;
    
    public int MinutosLeituraBiblia { get; set; } = 4;
    public int? LeituraBibliaPessoaId { get; set; }
    public Pessoa? LeituraBibliaPessoa { get; set; }
    public string LeituraTema { get; set; } = string.Empty;
    
    // Faça Seu Melhor no Ministério (dinâmico)
    public ICollection<ParteMinisterio> PartesMinisterio { get; set; } = new List<ParteMinisterio>();
    
    // Nossa Vida Cristã
    public int CanticoMeio { get; set; }
    public ICollection<ParteVidaCrista> PartesVidaCrista { get; set; } = new List<ParteVidaCrista>();
    
    // Estudo de Livro (fixo)
    public int MinutosEstudoLivro { get; set; } = 30;
    public int? EstudoLivroDirigentePessoaId { get; set; }
    public Pessoa? EstudoLivroDirigentePessoa { get; set; }
    public int? EstudoLivroLeitorPessoaId { get; set; }
    public Pessoa? EstudoLivroLeitorPessoa { get; set; }
    public string EstudoLivroTema { get; set; } = string.Empty;
    
    // Comentários Finais
    public int MinutosComentariosFinais { get; set; } = 3;
    public int? ComentariosFinaisPessoaId { get; set; }
    public Pessoa? ComentariosFinaisPessoa { get; set; }
    
    // Encerramento
    public int CanticoFinal { get; set; }
    public int? OracaoFinalPessoaId { get; set; }
    public Pessoa? OracaoFinalPessoa { get; set; }
}