using Microsoft.EntityFrameworkCore;
using PartesReuniao.Models;

namespace PartesReuniao.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<PessoaParte> PessoasPartes { get; set; }
    public DbSet<HistoricoDesignacao> HistoricoDesignacoes { get; set; }
    public DbSet<DesignacaoSemana> DesignacoesSemanas { get; set; }
    public DbSet<ParteMinisterio> PartesMinisterio { get; set; }
    public DbSet<ParteVidaCrista> PartesVidaCrista { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações de Pessoa
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nome);
        });
        
        // Configurações de PessoaParte
        modelBuilder.Entity<PessoaParte>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Pessoa)
                  .WithMany(p => p.Partes)
                  .HasForeignKey(e => e.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.PessoaId, e.TipoParte }).IsUnique();
        });
        
        // Configurações de HistoricoDesignacao
        modelBuilder.Entity<HistoricoDesignacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Pessoa)
                  .WithMany(p => p.Historicos)
                  .HasForeignKey(e => e.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.PessoaId, e.TipoParte, e.Data });
        });
        
        // Configurações de DesignacaoSemana
        modelBuilder.Entity<DesignacaoSemana>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.OracaoInicialPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.OracaoInicialPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.PresidentePessoa)
                  .WithMany()
                  .HasForeignKey(e => e.PresidentePessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.TesourosPalavraPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.TesourosPalavraPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.JoiasEspirituaisPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.JoiasEspirituaisPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.LeituraBibliaPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.LeituraBibliaPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.EstudoLivroDirigentePessoa)
                  .WithMany()
                  .HasForeignKey(e => e.EstudoLivroDirigentePessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.EstudoLivroLeitorPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.EstudoLivroLeitorPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ComentariosFinaisPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.ComentariosFinaisPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.OracaoFinalPessoa)
                  .WithMany()
                  .HasForeignKey(e => e.OracaoFinalPessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configurações de ParteMinisterio
        modelBuilder.Entity<ParteMinisterio>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.DesignacaoSemana)
                  .WithMany(d => d.PartesMinisterio)
                  .HasForeignKey(e => e.DesignacaoSemanaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Pessoa1)
                  .WithMany()
                  .HasForeignKey(e => e.Pessoa1Id)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Pessoa2)
                  .WithMany()
                  .HasForeignKey(e => e.Pessoa2Id)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configurações de ParteVidaCrista
        modelBuilder.Entity<ParteVidaCrista>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.DesignacaoSemana)
                  .WithMany(d => d.PartesVidaCrista)
                  .HasForeignKey(e => e.DesignacaoSemanaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Pessoa)
                  .WithMany()
                  .HasForeignKey(e => e.PessoaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}