using System.ComponentModel.DataAnnotations;

namespace PartesReuniao.Models;

public class Pessoa
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    
    
    public ICollection<PessoaParte> Partes { get; set; } = new List<PessoaParte>();
    public ICollection<HistoricoDesignacao> Historicos { get; set; } = new List<HistoricoDesignacao>();
}