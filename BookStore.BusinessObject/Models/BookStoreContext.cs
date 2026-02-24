using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class BookStoreContext : DbContext
{
    public BookStoreContext()
    {
    }

    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:bookstore-server-anpk.database.windows.net,1433;Initial Catalog=bookstore-anpk;Persist Security Info=False;User ID=PHUNGAN;Password=Phung@n02122003;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartID).HasName("PK__Carts__51BCD797C713154B");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Carts).HasConstraintName("FK__Carts__UserID__09A971A2");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemID).HasName("PK__CartItem__488B0B2A2A65FC69");

            entity.Property(e => e.AddedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__CartItems__CartI__07C12930");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasConstraintName("FK__CartItems__Produ__08B54D69");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID).HasName("PK__Categori__19093A2B2DFB5F2E");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID).HasName("PK__Orders__C3905BAF8DA890AA");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders).HasConstraintName("FK__Orders__StatusID__0C85DE4D");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__UserID__0D7A0286");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailID).HasName("PK__OrderDet__D3B9D30C93AE65B9");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Order__0A9D95DB");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Produ__0B91BA14");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusID).HasName("PK__OrderSta__C8EE20434C19026D");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID).HasName("PK__Products__B40CC6ED66E2087F");

            entity.Property(e => e.Stock).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Products__Catego__0E6E26BF");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleID).HasName("PK__Roles__8AFACE3A284F7C4B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK__Users__1788CCAC3F3464A7");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK__Users__RoleID__0F624AF8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
