using System.ComponentModel.DataAnnotations;
using PartesReuniao.Models.Enums;

namespace PartesReuniao.Models;

public class PessoaParte
{
    [Key]
    public int Id { get; set; }
    
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
    
    public TipoParte TipoParte { get; set; }
    
    public bool Habilitado { get; set; } = true;
}