using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Models;

namespace ShoeStoreLibrary.Contexts;

public partial class ShoeStoreDbContext : DbContext
{
    public ShoeStoreDbContext()
    {
    }

    public ShoeStoreDbContext(DbContextOptions<ShoeStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-34CIBTD\\SQLEXPRESS1; Database=Project ;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("Manufacturer");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(CONVERT([date],getdate()))");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK_ShoeOrder");

            entity.ToTable("OrderProduct");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOrder_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOrder_Product1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Shoe");

            entity.ToTable("Product");

            entity.Property(e => e.Article)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("шт.");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Manufacturer");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Supplier");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Login, "UQ_User_Login").IsUnique();

            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName).HasMaxLength(30);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.SecondName).HasMaxLength(30);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
