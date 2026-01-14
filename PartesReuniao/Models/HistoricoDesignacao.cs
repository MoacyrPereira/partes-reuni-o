using System.ComponentModel.DataAnnotations;
using PartesReuniao.Models.Enums;

namespace PartesReuniao.Models;

public class HistoricoDesignacao
{
    [Key]
    public int Id { get; set; }
    
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
    
    public TipoParte TipoParte { get; set; }
    
    public DateTime Data { get; set; }
    
    public int Semana { get; set; }
    
    public int Mes { get; set; }
    
    public int Ano { get; set; }
}