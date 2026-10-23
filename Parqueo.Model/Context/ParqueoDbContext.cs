using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Parqueo.Model.Entities;

namespace Parqueo.Model.Context;

public partial class ParqueoDbContext : DbContext
{
    public ParqueoDbContext(DbContextOptions<ParqueoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PrqAutomovil> PrqAutomovils { get; set; }

    public virtual DbSet<PrqIngresoAutomovil> PrqIngresoAutomovils { get; set; }

    public virtual DbSet<PrqParqueo> PrqParqueos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("user_08", "match_kind", new[] { "competition", "friendly" })
            .HasPostgresEnum("user_08", "match_status", new[] { "scheduled", "live", "finished" })
            .HasPostgresEnum("user_08", "sport_type", new[] { "soccer", "basketball", "baseball", "volleyball", "rugby", "hockey" })
            .HasPostgresEnum("user_25", "match_kind", new[] { "friendly", "official" })
            .HasPostgresEnum("user_25", "match_status", new[] { "scheduled", "live", "finished" })
            .HasPostgresEnum("user_25", "sport", new[] { "soccer", "basketball", "baseball", "volleyball", "rugby", "hockey" });

        modelBuilder.Entity<PrqAutomovil>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prq_automovil_pkey");

            entity.ToTable("prq_automovil", "user_12");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.Fabricante)
                .HasMaxLength(100)
                .HasColumnName("fabricante");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<PrqIngresoAutomovil>(entity =>
        {
            entity.HasKey(e => e.Consecutivo).HasName("prq_ingreso_automovil_pkey");

            entity.ToTable("prq_ingreso_automovil", "user_12");

            entity.HasIndex(e => e.IdAutomovil, "idx_prq_ingreso_automovil");

            entity.HasIndex(e => e.FechaHoraEntrada, "idx_prq_ingreso_fecha");

            entity.HasIndex(e => e.IdParqueo, "idx_prq_ingreso_parqueo");

            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.FechaHoraEntrada)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora_entrada");
            entity.Property(e => e.FechaHoraSalida)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora_salida");
            entity.Property(e => e.IdAutomovil).HasColumnName("id_automovil");
            entity.Property(e => e.IdParqueo).HasColumnName("id_parqueo");

            entity.HasOne(d => d.IdAutomovilNavigation).WithMany(p => p.PrqIngresoAutomovils)
                .HasForeignKey(d => d.IdAutomovil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_prq_ingreso_automovil");

            entity.HasOne(d => d.IdParqueoNavigation).WithMany(p => p.PrqIngresoAutomovils)
                .HasForeignKey(d => d.IdParqueo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_prq_ingreso_parqueo");
        });

        modelBuilder.Entity<PrqParqueo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prq_parqueo_pkey");

            entity.ToTable("prq_parqueo", "user_12");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreProvincia)
                .HasMaxLength(100)
                .HasColumnName("nombre_provincia");
            entity.Property(e => e.PrecioPorHora)
                .HasPrecision(10, 2)
                .HasColumnName("precio_por_hora");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
