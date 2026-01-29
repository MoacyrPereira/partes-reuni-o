namespace PartesReuniao.Models;

public class DesignacaoMecanica
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public int MesInicial { get; set; }
    public int AnoInicial { get; set; }
    
    // Indicadores (3 pessoas)
    public int? Indicador1Id { get; set; }
    public Pessoa? Indicador1 { get; set; }
    
    public int? Indicador2Id { get; set; }
    public Pessoa? Indicador2 { get; set; }
    
    public int? Indicador3Id { get; set; }
    public Pessoa? Indicador3 { get; set; }
    
    // Volante (2 pessoas)
    public int? Volante1Id { get; set; }
    public Pessoa? Volante1 { get; set; }
    
    public int? Volante2Id { get; set; }
    public Pessoa? Volante2 { get; set; }
    
    // Áudio e Vídeo (2 pessoas)
    public int? AudioVideo1Id { get; set; }
    public Pessoa? AudioVideo1 { get; set; }
    
    public int? AudioVideo2Id { get; set; }
    public Pessoa? AudioVideo2 { get; set; }
}