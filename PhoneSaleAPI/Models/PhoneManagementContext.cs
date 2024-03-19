using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PhoneSaleAPI.Models
{
    public partial class PhoneManagementContext : DbContext
    {
        public PhoneManagementContext()
        {
        }

        public PhoneManagementContext(DbContextOptions<PhoneManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<ShoppingCartDetail> ShoppingCartDetails { get; set; } = null!;
        public virtual DbSet<Storage> Storages { get; set; } = null!;
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DUNGDEPTRAI\\SQLEXPRESS;Initial Catalog=PhoneManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK__Account__536C85E5283A5209");

                entity.ToTable("Account");

                entity.Property(e => e.Username).HasMaxLength(30);

                entity.Property(e => e.Password).HasMaxLength(100);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.BillId)
                    .HasMaxLength(30)
                    .HasColumnName("BillID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.DateBill).HasColumnType("date");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(30)
                    .HasColumnName("EmployeeID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Bill_CustomerID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_Bill_EmployeeID");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => new { e.BillId, e.ProductId });

                entity.ToTable("BillDetail");

                entity.Property(e => e.BillId)
                    .HasMaxLength(30)
                    .HasColumnName("BillID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_BillID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_ProductID");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId)
                    .HasMaxLength(30)
                    .HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(30);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.ColorName)
                    .HasName("PK__Color__C71A5A7ABD3CA344");

                entity.ToTable("Color");

                entity.Property(e => e.ColorName).HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534CF7FF7B1")
                    .IsUnique();

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CustomerName).HasMaxLength(30);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(30)
                    .HasColumnName("EmployeeID");

                entity.Property(e => e.EmployeeName).HasMaxLength(30);

                entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.CategoryId)
                    .HasMaxLength(30)
                    .HasColumnName("CategoryID");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.Img).HasMaxLength(50);

                entity.Property(e => e.ProductName).HasMaxLength(30);

                entity.Property(e => e.StorageGb).HasColumnName("StorageGB");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(30)
                    .HasColumnName("VendorID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_CategoryID");

                entity.HasOne(d => d.ColorNameNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ColorName)
                    .HasConstraintName("FK_Product_ColorName");

                entity.HasOne(d => d.StorageGbNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StorageGb)
                    .HasConstraintName("FK_Product_StorageGB");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("FK_Product_VendorID");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("ShoppingCart");

                entity.HasIndex(e => e.CustomerId, "UQ__Shopping__A4AE64B9E0DBE575")
                    .IsUnique();

                entity.Property(e => e.ShoppingCartId)
                    .HasMaxLength(30)
                    .HasColumnName("ShoppingCartID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.ShoppingCart)
                    .HasForeignKey<ShoppingCart>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCart_CustomerID");
            });

            modelBuilder.Entity<ShoppingCartDetail>(entity =>
            {
                entity.HasKey(e => new { e.ShoppingCartId, e.ProductId })
                    .HasName("PK__Shopping__B13856EA8A89FBE8");

                entity.ToTable("ShoppingCartDetail");

                entity.Property(e => e.ShoppingCartId)
                    .HasMaxLength(30)
                    .HasColumnName("ShoppingCartID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCartDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCartDetail_ProductID");

                entity.HasOne(d => d.ShoppingCart)
                    .WithMany(p => p.ShoppingCartDetails)
                    .HasForeignKey(d => d.ShoppingCartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCartDetail_ShoppingCartID");
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.HasKey(e => e.StorageGb)
                    .HasName("PK__Storage__8A246E773FB80672");

                entity.ToTable("Storage");

                entity.Property(e => e.StorageGb)
                    .ValueGeneratedNever()
                    .HasColumnName("StorageGB");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("Vendor");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(30)
                    .HasColumnName("VendorID");

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.VendorName).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
