using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Models;

namespace QLKSMVC.Data;

public partial class QuanLyKhachSanDbContext : DbContext
{
    public QuanLyKhachSanDbContext()
    {
    }

    public QuanLyKhachSanDbContext(DbContextOptions<QuanLyKhachSanDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountModel>? Accounts { get; set; }

    public virtual DbSet<ChamCongNhanVienModel>? ChamCongNhanViens { get; set; }

    public virtual DbSet<ChucVuModel>? ChucVus { get; set; }

    public virtual DbSet<DatDichVuModel>? DatDichVus { get; set; }

    public virtual DbSet<DatPhongModel>? DatPhongs { get; set; }

    public virtual DbSet<DichVuModel>? DichVus { get; set; }

    public virtual DbSet<HoaDonModel>? HoaDons { get; set; }

    public virtual DbSet<KhachHangModel>? KhachHangs { get; set; }

    public virtual DbSet<LoaiDichVuModel>? LoaiDichVus { get; set; }

    public virtual DbSet<LoaiPhongModel>? LoaiPhongs { get; set; }

    public virtual DbSet<NhanVienModel>? NhanViens { get; set; }

    public virtual DbSet<NhanVienNghiViecModel>? NhanVienNghiViecs { get; set; }

    public virtual DbSet<PhongModel>? Phongs { get; set; }

    public virtual DbSet<RoleModel>? Roles { get; set; }

    public virtual DbSet<TacVuPhongModel>? TacVuPhongs { get; set; }

    public virtual DbSet<TongLuongModel>? TongLuongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:strCon");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3213E83F49FFA303");

            entity.HasOne(d => d.RoleIdNavigation).WithMany(p => p.Accounts)
                .HasPrincipalKey(p => p.RoleId)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAccount346982");
        });

        modelBuilder.Entity<ChamCongNhanVienModel>(entity =>
        {
            entity.HasKey(e => e.MaCcnv).HasName("PK__ChamCong__02519E0235F8BBE6");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.ChamCongNhanViens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKChamCongNh436421");
        });

        modelBuilder.Entity<ChucVuModel>(entity =>
        {
            entity.HasKey(e => e.MaCv).HasName("PK__ChucVu__27258E76AF005EA8");
        });

        modelBuilder.Entity<DatDichVuModel>(entity =>
        {
            entity.HasKey(e => e.MaDdv).HasName("PK__DatDichV__3D88C80A52146C32");

            entity.HasOne(d => d.MaDpNavigation).WithMany(p => p.DatDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatDichVu514375");

            entity.HasOne(d => d.MaDvNavigation).WithMany(p => p.DatDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatDichVu311419");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DatDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatDichVu480688");
        });

        modelBuilder.Entity<DatPhongModel>(entity =>
        {
            entity.HasKey(e => e.MaDp).HasName("PK__DatPhong__272586698E8C502E");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatPhong953051");

            entity.HasOne(d => d.MaPNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatPhong847484");
        });

        modelBuilder.Entity<DichVuModel>(entity =>
        {
            entity.HasKey(e => e.MaDv).HasName("PK__DichVu__27258657B1854889");

            entity.HasOne(d => d.MaLdvNavigation).WithMany(p => p.DichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDichVu830778");
        });

        modelBuilder.Entity<HoaDonModel>(entity =>
        {
            entity.HasKey(e => e.MaHddp).HasName("PK__HoaDon__1419AAD338EB58A0");

            entity.HasOne(d => d.MaDpNavigation).WithMany(p => p.HoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKHoaDon819354");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKHoaDon185048");
        });

        modelBuilder.Entity<KhachHangModel>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E1680A17D");
        });

        modelBuilder.Entity<LoaiDichVuModel>(entity =>
        {
            entity.HasKey(e => e.MaLdv).HasName("PK__LoaiDich__3B98A90D376B258C");
        });

        modelBuilder.Entity<LoaiPhongModel>(entity =>
        {
            entity.HasKey(e => e.MaLp).HasName("PK__LoaiPhon__2725C7772F4ED2FD");
        });

        modelBuilder.Entity<NhanVienModel>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70AB423B220");

            entity.HasOne(d => d.MaCvNavigation).WithMany(p => p.NhanViens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNhanVien198658");

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.NhanViens)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.UserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNhanVien75480");
        });

        modelBuilder.Entity<NhanVienNghiViecModel>(entity =>
        {
            entity.HasKey(e => e.MaNvnv).HasName("PK__NhanVien__27603DAE3C1BFEBA");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.NhanVienNghiViecs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNhanVienNg843903");
        });

        modelBuilder.Entity<PhongModel>(entity =>
        {
            entity.HasKey(e => e.MaP).HasName("PK__Phong__C7977BA88811C0A4");

            entity.HasOne(d => d.MaLpNavigation).WithMany(p => p.Phongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPhong603469");

            entity.HasOne(d => d.MaTvpNavigation).WithMany(p => p.Phongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPhong740362");
        });

        modelBuilder.Entity<RoleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F2BD18C84");
        });

        modelBuilder.Entity<TacVuPhongModel>(entity =>
        {
            entity.HasKey(e => e.MaTvp).HasName("PK__TacVuPho__31481FD5927C78B6");
        });

        modelBuilder.Entity<TongLuongModel>(entity =>
        {
            entity.HasKey(e => e.MaTl).HasName("PK__TongLuon__27250071124994A9");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.TongLuongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTongLuong742592");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
