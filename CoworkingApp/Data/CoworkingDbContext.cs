using System;
using System.Collections.Generic;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Data;

public partial class CoworkingDbContext : DbContext
{
    public CoworkingDbContext()
    {
    }

    public CoworkingDbContext(DbContextOptions<CoworkingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoworkingCenter> CoworkingCenters { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Workspace> Workspaces { get; set; }

    public virtual DbSet<WorkspaceHistory> WorkspaceHistories { get; set; }

    public virtual DbSet<WorkspacePricing> WorkspacePricings { get; set; }

    public virtual DbSet<WorkspaceStatus> WorkspaceStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoworkingCenter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("coworking_center_pkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reservation_pkey");

            entity.HasOne(d => d.Pricing).WithMany(p => p.Reservations).HasConstraintName("reservation_pricing_id_fkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_workspace_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_role_pkey");
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workspace_pkey");

            entity.HasOne(d => d.CoworkingCenter).WithMany(p => p.Workspaces)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_coworking_center_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Workspaces)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_status_id_fkey");
        });

        modelBuilder.Entity<WorkspaceHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workspace_history_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Status).WithMany(p => p.WorkspaceHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_history_status_id_fkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.WorkspaceHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_history_workspace_id_fkey");
        });

        modelBuilder.Entity<WorkspacePricing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workspace_pricing_pkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.WorkspacePricings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_pricing_workspace_id_fkey");
        });

        modelBuilder.Entity<WorkspaceStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workspace_status_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
