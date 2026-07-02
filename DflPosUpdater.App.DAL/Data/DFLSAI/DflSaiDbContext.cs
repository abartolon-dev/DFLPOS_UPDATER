using System;
using System.Collections.Generic;
using DflPosUpdater.App.Entities.DFLSAI;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.DAL.DFLSAI;

public partial class DflSaiDbContext : DbContext
{
    public DflSaiDbContext(DbContextOptions<DflSaiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MaCode> MaCodes { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaCode>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Vkey }).HasName("PK_ma_code");

            entity.Property(e => e.Used).IsFixedLength();
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.Property(e => e.DeleteFlag).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
