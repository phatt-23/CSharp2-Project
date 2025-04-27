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

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

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
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("address_pkey");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.City).WithMany(p => p.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("address_city_id_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("city_pkey");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_country_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("country_pkey");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<CoworkingCenter>(entity =>
        {
            entity.HasKey(e => e.CoworkingCenterId).HasName("coworking_center_pkey");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Address).WithMany(p => p.CoworkingCenters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("coworking_center_address_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CoworkingCenters).HasConstraintName("coworking_center_updated_by_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("reservation_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsCancelled).HasDefaultValue(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Reservations).HasConstraintName("reservation_customer_id_fkey");

            entity.HasOne(d => d.Pricing).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_pricing_id_fkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_workspace_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("user_role_pkey");
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.WorkspaceId).HasName("workspace_pkey");

            entity.Property(e => e.IsRemoved).HasDefaultValue(false);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CoworkingCenter).WithMany(p => p.Workspaces)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_coworking_center_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.Workspaces).HasConstraintName("workspace_updated_by_fkey");
        });

        modelBuilder.Entity<WorkspaceHistory>(entity =>
        {
            entity.HasKey(e => e.WorkspaceHistoryId).HasName("workspace_history_pkey");

            entity.Property(e => e.ChangeAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Status).WithMany(p => p.WorkspaceHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_history_status_id_fkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.WorkspaceHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_history_workspace_id_fkey");
        });

        modelBuilder.Entity<WorkspacePricing>(entity =>
        {
            entity.HasKey(e => e.WorkspacePricingId).HasName("workspace_pricing_pkey");

            entity.Property(e => e.CreatedBy).HasComment("if null then it was created by the database admin who has direct access to the database");
            entity.Property(e => e.PricePerHour).HasComment("dollar\n");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WorkspacePricings).HasConstraintName("workspace_pricing_created_by_fkey");

            entity.HasOne(d => d.Workspace).WithMany(p => p.WorkspacePricings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("workspace_pricing_workspace_id_fkey");
        });

        modelBuilder.Entity<WorkspaceStatus>(entity =>
        {
            entity.HasKey(e => e.WorkspaceStatusId).HasName("workspace_status_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
