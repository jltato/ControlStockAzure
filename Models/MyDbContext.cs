using System;
using System.Collections.Generic;
using ControlStock.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControlStock.Models;

public partial class MyDbContext : IdentityDbContext<MyUser>
{
   
    public MyDbContext(DbContextOptions<MyDbContext> options): base(options)  { }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<DepositoArticuloLote> DepositoArticuloLotes { get; set; }

    public virtual DbSet<Ingresos> Ingresos { get; set; }

    public virtual DbSet<DetalleIngreso> DetalleIngresos { get; set; }

    public virtual DbSet<Egresos> Egresos { get; set; }

    public virtual DbSet<DetalleEgreso> DetalleEgresos { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Rubro> Rubros { get; set; }

    public virtual DbSet<Scope> Scopes { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<SectionProveedor> SectionProveedores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyUser>()
           .HasIndex(u => u.Nombre)
           .IsUnique();

        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.IdArticulo);

            entity.ToTable("Articulo");

            entity.HasIndex(e => e.IdMarca, "IX_Articulo_IdMarca");

            entity.Property(e => e.Observaciones)
                .HasMaxLength(550)
                .HasDefaultValue("");

            entity.HasOne(d => d.Marca).WithMany(p => p.Articulos).HasForeignKey(d => d.IdMarca);
        });

        modelBuilder.Entity<DepositoArticuloLote>(entity =>
        {
            entity.HasIndex(e => e.ArticuloId, "IX_DepositoArticuloLotes_ArticuloId");

            entity.HasIndex(e => e.LoteId, "IX_DepositoArticuloLotes_LoteId");

            entity.HasIndex(e => e.ScopeId, "IX_DepositoArticuloLotes_ScopeId");

            entity.HasOne(d => d.Articulo).WithMany(p => p.DepositoArticuloLotes).HasForeignKey(d => d.ArticuloId);

            entity.HasOne(d => d.Lote).WithMany(p => p.DepositoArticuloLotes).HasForeignKey(d => d.LoteId);

            entity.HasOne(d => d.Scope).WithMany(p => p.DepositoArticuloLotes)
                .HasForeignKey(d => d.ScopeId)
                .HasConstraintName("FK_DepositoArticuloLotes_Scopes");
        });

        modelBuilder.Entity<Ingresos>(entity =>
        {
            entity.HasIndex(e => e.ProveedorId, "IX_Ingresos_ProveedorId");

            entity.Property(e => e.UserId).HasDefaultValue("");

            entity.Property(e => e.Observaciones)
                  .HasMaxLength(500)
                  .HasDefaultValue("");

            entity.HasOne(d => d.Proveedor)
                  .WithMany(p => p.Ingresos)
                  .HasForeignKey(d => d.ProveedorId);

            entity.HasOne(d => d.Scope)
                  .WithMany(p => p.Ingresos)
                  .HasForeignKey(d => d.ScopeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Ingreso_Scopes");
        });

        modelBuilder.Entity<DetalleIngreso>(entity =>
        {
            entity.HasKey(e => e.DetalleId);
            entity.ToTable("DetalleIngreso");
            entity.HasIndex(e => e.ArticuloId, "IX_DetalleIngreso_ArticuloId");
            entity.HasIndex(e => e.IngresoId, "IX_DetalleIngreso_IngresoId");
            entity.HasIndex(e => e.LoteId, "IX_DetalleIngreso_LoteId");   
            
            entity.HasOne(d => d.Articulo)
                  .WithMany(p => p.DetalleIngresos)
                  .HasForeignKey(d => d.ArticuloId);

            entity.HasOne(d => d.Ingresos)
                  .WithMany(p => p.DetalleIngresos)
                  .HasForeignKey(d => d.IngresoId);

            entity.HasOne(d => d.Lote)
                  .WithMany(p => p.DetalleIngresos)
                  .HasForeignKey(d => d.LoteId);            
        });

        modelBuilder.Entity<Egresos>(entity =>
        {
            entity.HasKey(e => e.EgresoId);
            entity.HasIndex(e => e.UserId)
                  .HasDatabaseName("IX_Egresos_UserId");

            entity.Property(e => e.Observaciones)
                  .HasMaxLength(500)
                  .HasDefaultValue("");

            entity.HasMany(e => e.DetalleEgresos)
                  .WithOne(d => d.Egreso)
                  .HasForeignKey(d => d.EgresoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Scope)
                 .WithMany(p => p.Egresos)
                 .HasForeignKey(d => d.ScopeId)
                 .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Section)
                 .WithMany(p => p.Egresos)
                 .HasForeignKey(d => d.SectionId)
                 .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Egresos>()
                   .HasOne(e => e.DestinoScope)
                   .WithMany()
                   .HasForeignKey(e => e.Destino)
                   .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DetalleEgreso>(entity =>
        {
            entity.HasKey(d => d.DetalleId);
            entity.HasIndex(e => e.ArticuloId, "IX_DetalleEgreso_ArticuloId");
            entity.HasIndex(e => e.EgresoId, "IX_DetalleEgreso_EgresoId");
            entity.HasIndex(e => e.LoteId, "IX_DetalleEgreso_LoteId");
            entity.HasOne(d => d.Lote)
                  .WithMany()
                  .HasForeignKey(d => d.LoteId)
                  .OnDelete(DeleteBehavior.SetNull); // Permite que LoteId sea null si el Lote relacionado se elimina
            entity.HasOne(d => d.Articulo)
                  .WithMany()
                  .HasForeignKey(d => d.ArticuloId)
                  .OnDelete(DeleteBehavior.Restrict); // Cambiar según sea necesario
        });

      

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.ToTable("Lote");

            entity.Property(e => e.NumeroLote).HasDefaultValue("");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca);

            entity.ToTable("Marca");

            entity.HasOne(d => d.Rubro).WithMany(p => p.Marcas)
                .HasForeignKey(d => d.IdRubro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Marca_Rubro");
        });

        modelBuilder.Entity<SectionProveedor>()
           .HasKey(sp => new { sp.ProveedorId, sp.SectionId });

        modelBuilder.Entity<SectionProveedor>()
            .HasOne(sp => sp.Proveedor)
            .WithMany(p => p.SectionProveedores)
            .HasForeignKey(sp => sp.ProveedorId);

        modelBuilder.Entity<SectionProveedor>()
            .HasOne(sp => sp.Section)
            .WithMany(s => s.SectionProveedores)
            .HasForeignKey(sp => sp.SectionId);


        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedor");
        });

        modelBuilder.Entity<Rubro>(entity =>
        {
            entity.HasKey(e => e.IdRubro);
            entity.ToTable("Rubro");

            entity.HasOne(d => d.Section)
                .WithMany(p => p.Rubros)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rubro_Sections");
        });

        modelBuilder.Entity<Scope>(entity =>
        {
            entity.Property(e => e.UserId).HasDefaultValue("");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SectionId, e.ScopeId });

            entity.HasIndex(e => e.ScopeId, "IX_UserPermissions_ScopeId");

            entity.HasIndex(e => e.SectionId, "IX_UserPermissions_SectionId");

            entity.HasOne(d => d.Scope)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.ScopeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Section)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermissions_Sections");

           
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
