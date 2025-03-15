using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResumeUploadService.Models;

public partial class ResumeAnalyzerDbContext : DbContext
{
    public ResumeAnalyzerDbContext()
    {
    }

    public ResumeAnalyzerDbContext(DbContextOptions<ResumeAnalyzerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<ResumeKeyword> ResumeKeywords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Keyword1, "Keyword").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Keyword1)
                .HasMaxLength(100)
                .HasColumnName("Keyword");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Resume");

            entity.HasIndex(e => e.UserId, "fk_userinfo");

            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ResumeKeyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.KeywordId, "KeywordId");

            entity.HasIndex(e => e.ResumeId, "ResumeId");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Keyword).WithMany(p => p.ResumeKeywords)
                .HasForeignKey(d => d.KeywordId)
                .HasConstraintName("resumekeywords_ibfk_2");

            entity.HasOne(d => d.Resume).WithMany(p => p.ResumeKeywords)
                .HasForeignKey(d => d.ResumeId)
                .HasConstraintName("resumekeywords_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
