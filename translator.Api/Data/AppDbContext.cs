using Microsoft.EntityFrameworkCore;
using translator.Api.Models;

namespace translator.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TranslationRecord> TranslationRecords => Set<TranslationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TranslationRecord>(entity =>
        {
            entity.ToTable("translation_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Id).HasColumnName("id");
            entity.Property(record => record.SourceLanguageCode).HasColumnName("source_language_code").HasMaxLength(16);
            entity.Property(record => record.TargetLanguageCode).HasColumnName("target_language_code").HasMaxLength(16);
            entity.Property(record => record.SourceText).HasColumnName("source_text");
            entity.Property(record => record.TranslatedText).HasColumnName("translated_text");
            entity.Property(record => record.CreatedAtUtc).HasColumnName("created_at_utc");
            entity.HasIndex(record => record.CreatedAtUtc);
        });
    }
}
