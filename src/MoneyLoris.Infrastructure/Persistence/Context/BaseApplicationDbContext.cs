using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoneyLoris.Application.Domain.Entities;

namespace MoneyLoris.Infrastructure.Persistence.Context;
public class BaseApplicationDbContext : DbContext
{
    public IConfiguration _configuration { get; } = null!;
    public BaseApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BaseApplicationDbContext(DbContextOptions<BaseApplicationDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Categoria> Categorias { get; set; } = null!;
    public virtual DbSet<Lancamento> Lancamentos { get; set; } = null!;
    public virtual DbSet<MeioPagamento> MeiosPagamento { get; set; } = null!;
    public virtual DbSet<Subcategoria> Subcategorias { get; set; } = null!;
    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categoria");

            entity.HasIndex(e => e.IdUsuario, "IX_Categoria_Usuario");

            entity.HasIndex(e => new { e.IdUsuario, e.Nome }, "UI_Categoria_Nome")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");

            entity.Property(e => e.Nome).HasMaxLength(20);

            entity.Property(e => e.Ordem).HasColumnType("tinyint(4)");

            entity.Property(e => e.Tipo).HasColumnType("tinyint(4)");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Categorias)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Usuario");
        });

        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.ToTable("lancamento");

            entity.HasIndex(e => e.IdCategoria, "IX_Lancamento_Categoria");

            entity.HasIndex(e => e.IdLancamentoTransferencia, "IX_Lancamento_LancamentoTransferencia");

            entity.HasIndex(e => e.IdMeioPagamento, "IX_Lancamento_MeioPagamento");

            entity.HasIndex(e => e.IdSubcategoria, "IX_Lancamento_Subcategoria");

            entity.HasIndex(e => e.IdUsuario, "IX_Lancamento_Usuario");

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.Data).HasColumnType("datetime");

            entity.Property(e => e.Descricao).HasMaxLength(50);

            entity.Property(e => e.IdCategoria).HasColumnType("int(11)");

            entity.Property(e => e.IdLancamentoTransferencia).HasColumnType("int(11)");

            entity.Property(e => e.IdMeioPagamento).HasColumnType("int(11)");

            entity.Property(e => e.IdSubcategoria).HasColumnType("int(11)");

            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");

            entity.Property(e => e.Operacao).HasColumnType("tinyint(4)");

            entity.Property(e => e.Realizado).HasDefaultValueSql("'1'");

            entity.Property(e => e.Tipo).HasColumnType("tinyint(4)");

            entity.Property(e => e.TipoTransferencia).HasColumnType("tinyint(4)");

            entity.Property(e => e.Valor).HasPrecision(8, 2);

            entity.Property(e => e.ParcelaAtual).HasColumnType("smallint(7)");

            entity.Property(e => e.ParcelaTotal).HasColumnType("smallint(7)");

            entity.HasOne(d => d.Categoria)
                .WithMany(p => p.Lancamentos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Lancamento_Categoria");

            entity.HasOne(d => d.LancamentoTransferencia)
                .WithMany(p => p.LancamentoTransferenciaRelacionado)
                .HasForeignKey(d => d.IdLancamentoTransferencia)
                .HasConstraintName("FK_Lancamento_LancamentoTransferencia");

            entity.HasOne(d => d.MeioPagamento)
                .WithMany(p => p.Lancamentos)
                .HasForeignKey(d => d.IdMeioPagamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lancamento_MeioPagamento");

            entity.HasOne(d => d.Subcategoria)
                .WithMany(p => p.Lancamentos)
                .HasForeignKey(d => d.IdSubcategoria)
                .HasConstraintName("FK_Lancamento_Subcategoria");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Lancamentos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lancamento_Usuario");
        });

        modelBuilder.Entity<MeioPagamento>(entity =>
        {
            entity.ToTable("meiopagamento");

            entity.HasIndex(e => e.IdUsuario, "IX_MeioPagamento_Usuario");

            entity.HasIndex(e => new { e.IdUsuario, e.Nome }, "UI_MeioPagamento_Nome")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.Cor).HasMaxLength(6);

            entity.Property(e => e.DiaFechamento).HasColumnType("tinyint(4)");

            entity.Property(e => e.DiaVencimento).HasColumnType("tinyint(4)");

            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");

            entity.Property(e => e.Limite).HasPrecision(8, 2);

            entity.Property(e => e.Nome).HasMaxLength(20);

            entity.Property(e => e.Ordem).HasColumnType("tinyint(4)");

            entity.Property(e => e.Saldo).HasPrecision(8, 2);

            entity.Property(e => e.Tipo).HasColumnType("tinyint(4)");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.MeiosPagamento)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeioPagamento_Usuario");
        });

        modelBuilder.Entity<Subcategoria>(entity =>
        {
            entity.ToTable("subcategoria");

            entity.HasIndex(e => e.IdCategoria, "IX_Subcategoria_Categoria");

            entity.HasIndex(e => new { e.IdCategoria, e.Nome }, "UI_Subcategoria_Nome")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.IdCategoria).HasColumnType("int(11)");

            entity.Property(e => e.Nome).HasMaxLength(20);

            entity.Property(e => e.Ordem).HasColumnType("tinyint(4)");

            entity.HasOne(d => d.Categoria)
                .WithMany(p => p.Subcategorias)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subcategoria_Categoria");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");

            entity.HasIndex(e => e.Login, "UI_Usuario_Login")
                    .IsUnique();

            entity.HasIndex(e => e.Nome, "UI_Usuario_Nome")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.DataCriacao)
                .HasColumnType("timestamp");

            entity.Property(e => e.DataInativacao).HasColumnType("timestamp");

            entity.Property(e => e.IdPerfil).HasColumnType("tinyint(4)");

            entity.Property(e => e.Login).HasMaxLength(20);

            entity.Property(e => e.Nome).HasMaxLength(100);

            entity.Property(e => e.Senha).HasMaxLength(64);

            entity.Property(e => e.UltimoLogin).HasColumnType("timestamp");
        });

    }
}
