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
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductDetail> ProductDetails { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public virtual DbSet<ReviewImage> ReviewImages { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<ShoppingCartDetail> ShoppingCartDetails { get; set; } = null!;
        public virtual DbSet<Storage> Storages { get; set; } = null!;
        public virtual DbSet<SystemNotification> SystemNotifications { get; set; } = null!;
        public virtual DbSet<SystemNotificationRead> SystemNotificationReads { get; set; } = null!;
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-SVVHT2F\\SQLEXPRESS;Database=PhoneManagement;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Username, "UQ__Account__536C85E48DE8D5F2")
                    .IsUnique();

                entity.Property(e => e.AccountId)
                    .HasMaxLength(30)
                    .HasColumnName("AccountID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Username).HasMaxLength(30);
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

                entity.Property(e => e.CustomerName).HasMaxLength(50);

                entity.Property(e => e.CustomerPhone).HasMaxLength(20);

                entity.Property(e => e.DateBill)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Bill_CustomerID");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => new { e.BillId, e.ProductId, e.ColorName, e.StorageGb });

                entity.ToTable("BillDetail");

                entity.Property(e => e.BillId)
                    .HasMaxLength(30)
                    .HasColumnName("BillID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.StorageGb).HasColumnName("StorageGB");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_BillID");

                entity.HasOne(d => d.ColorNameNavigation)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.ColorName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_ColorName");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_ProductID");

                entity.HasOne(d => d.StorageGbNavigation)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.StorageGb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_StorageGB");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId)
                    .HasMaxLength(30)
                    .HasColumnName("CategoryID");

                entity.Property(e => e.CategoryImage).HasMaxLength(100);

                entity.Property(e => e.CategoryName).HasMaxLength(30);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__ChatMess__C87C037C35A860B2");

                entity.ToTable("ChatMessage");

                entity.Property(e => e.MessageId)
                    .HasMaxLength(30)
                    .HasColumnName("MessageID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(30)
                    .HasColumnName("AccountID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.SentAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SessionId)
                    .HasMaxLength(30)
                    .HasColumnName("SessionID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ChatMessage_AccountID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ChatMessage_CustomerID");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("FK_ChatMessages_SessionID");
            });

            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__ChatSess__C9F49270EE7DB725");

                entity.ToTable("ChatSession");

                entity.Property(e => e.SessionId)
                    .HasMaxLength(30)
                    .HasColumnName("SessionID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(30)
                    .HasColumnName("AccountID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastMessageAt).HasColumnType("datetime");

                entity.Property(e => e.SentAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SessionName).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ChatSessions)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ChatSession_AccountID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ChatSessions)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ChatSession_CustomerID");
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.ColorName)
                    .HasName("PK__Color__C71A5A7AA3D36897");

                entity.ToTable("Color");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.ColorImage).HasMaxLength(100);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534A925B925")
                    .IsUnique();

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerName).HasMaxLength(30);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
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

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProductName).HasMaxLength(30);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(30)
                    .HasColumnName("VendorID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_CategoryID");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("FK_Product_VendorID");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ColorName, e.StorageGb });

                entity.ToTable("ProductDetail");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.StorageGb).HasColumnName("StorageGB");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ColorNameNavigation)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ColorName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductDetail_ColorName");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductDetail_ProductID");

                entity.HasOne(d => d.StorageGbNavigation)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.StorageGb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductDetail_StorageGB");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");

                entity.Property(e => e.ProductImageId)
                    .HasMaxLength(100)
                    .HasColumnName("ProductImageID");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImagePath).HasMaxLength(100);

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ColorNameNavigation)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ColorName)
                    .HasConstraintName("FK_ProductImage_ColorName");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductImage_ProductID");
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.ToTable("ProductReview");

                entity.Property(e => e.ProductReviewId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductReviewID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ProductReview_CustomerID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductReview_ProductID");
            });

            modelBuilder.Entity<ReviewImage>(entity =>
            {
                entity.ToTable("ReviewImage");

                entity.Property(e => e.ReviewImageId)
                    .HasMaxLength(30)
                    .HasColumnName("ReviewImageID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImagePath).HasMaxLength(255);

                entity.Property(e => e.IsPrimary).HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductReviewId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductReviewID");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ProductReview)
                    .WithMany(p => p.ReviewImages)
                    .HasForeignKey(d => d.ProductReviewId)
                    .HasConstraintName("FK_ReviewImage_ProductReviewID");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("ShoppingCart");

                entity.HasIndex(e => e.CustomerId, "UQ__Shopping__A4AE64B971324A2C")
                    .IsUnique();

                entity.Property(e => e.ShoppingCartId)
                    .HasMaxLength(30)
                    .HasColumnName("ShoppingCartID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.ShoppingCart)
                    .HasForeignKey<ShoppingCart>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCart_CustomerID");
            });

            modelBuilder.Entity<ShoppingCartDetail>(entity =>
            {
                entity.HasKey(e => new { e.ShoppingCartId, e.ProductId, e.ColorName, e.StorageGb });

                entity.ToTable("ShoppingCartDetail");

                entity.Property(e => e.ShoppingCartId)
                    .HasMaxLength(30)
                    .HasColumnName("ShoppingCartID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(30)
                    .HasColumnName("ProductID");

                entity.Property(e => e.ColorName).HasMaxLength(50);

                entity.Property(e => e.StorageGb).HasColumnName("StorageGB");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ColorNameNavigation)
                    .WithMany(p => p.ShoppingCartDetails)
                    .HasForeignKey(d => d.ColorName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCartDetail_ColorName");

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

                entity.HasOne(d => d.StorageGbNavigation)
                    .WithMany(p => p.ShoppingCartDetails)
                    .HasForeignKey(d => d.StorageGb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCartDetail_StorageGB");
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.HasKey(e => e.StorageGb)
                    .HasName("PK__Storage__8A246E77BE58073A");

                entity.ToTable("Storage");

                entity.Property(e => e.StorageGb)
                    .ValueGeneratedNever()
                    .HasColumnName("StorageGB");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SystemNotification>(entity =>
            {
                entity.HasKey(e => e.NotificationId)
                    .HasName("PK__SystemNo__20CF2E32156D7A45");

                entity.ToTable("SystemNotification");

                entity.Property(e => e.NotificationId)
                    .ValueGeneratedNever()
                    .HasColumnName("NotificationID");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.NotificationType).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<SystemNotificationRead>(entity =>
            {
                entity.HasKey(e => e.ReadId)
                    .HasName("PK__SystemNo__1FABC84C6A998BCD");

                entity.ToTable("SystemNotificationRead");

                entity.Property(e => e.ReadId)
                    .ValueGeneratedNever()
                    .HasColumnName("ReadID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(30)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.ReadAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.SystemNotificationReads)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_SystemNotificationRead_CustomerID");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.SystemNotificationReads)
                    .HasForeignKey(d => d.NotificationId)
                    .HasConstraintName("FK_SystemNotificationRead_NotificationID");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("Vendor");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(30)
                    .HasColumnName("VendorID");

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VendorName).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
