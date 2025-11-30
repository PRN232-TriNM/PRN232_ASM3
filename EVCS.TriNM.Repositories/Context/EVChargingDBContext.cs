#nullable disable
using Microsoft.EntityFrameworkCore;

namespace EVCS.TriNM.Repositories.Models;

public partial class EVChargingDBContext : DbContext
{
    public EVChargingDBContext()
    {
    }

    public EVChargingDBContext(DbContextOptions<EVChargingDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChargerTriNm> ChargerTriNMs { get; set; }

    public virtual DbSet<StationTriNm> StationTriNMs { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<ChargingTransaction> ChargingTransactions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChargerTriNm>(entity =>
        {
            entity.ToTable("ChargerTriNM");

            entity.Property(e => e.ChargerTriNmid).HasColumnName("ChargerTriNMID");
            entity.Property(e => e.ChargerTriNmtype)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ChargerTriNMType");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.StationTriNmid).HasColumnName("StationTriNMID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");

            entity.HasOne(d => d.StationTriNm).WithMany(p => p.ChargerTriNms)
                .HasForeignKey(d => d.StationTriNmid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChargerTriNM_StationTriNM");
        });

        modelBuilder.Entity<StationTriNm>(entity =>
        {
            entity.ToTable("StationTriNM");

            entity.Property(e => e.StationTriNmid).HasColumnName("StationTriNMID");
            entity.Property(e => e.StationTriNmcode)
                .HasMaxLength(50)
                .HasColumnName("StationTriNMCode");
            entity.Property(e => e.StationTriNmname)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("StationTriNMName");
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.Capacity).HasColumnType("int");
            entity.Property(e => e.CurrentAvailable).HasColumnType("int");
            entity.Property(e => e.Owner).HasMaxLength(150);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.ContactEmail).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("UserAccount");

            entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.GoogleId).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<ChargingTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.ToTable("ChargingTransaction");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.ChargerId).HasColumnName("ChargerTriNMID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.EnergyConsumed).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");

            entity.HasOne(d => d.ChargerTriNm).WithMany(p => p.ChargingTransactions)
                .HasForeignKey(d => d.ChargerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_ChargerTriNM");

            entity.HasOne(d => d.UserAccount).WithMany(p => p.ChargingTransactions)
                .HasForeignKey(d => d.UserAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_User");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PaidDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Transaction");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}