using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("HoaDon")]
public partial class HoaDonModel
{
    [Key]
    [Column("MaHDDP")]
    public int MaHddp { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayHoaDon { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? TongTienPhong { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? TongTienDichVu { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? TongTien { get; set; }

    [Column("MaDP")]
    public int MaDp { get; set; }

    [Column("MaNV")]
    public int MaNv { get; set; }

    [ForeignKey("MaDp")]
    [InverseProperty("HoaDons")]
    public virtual DatPhongModel? MaDpNavigation { get; set; } = null!;

    [ForeignKey("MaNv")]
    [InverseProperty("HoaDons")]
    public virtual NhanVienModel? MaNvNavigation { get; set; } = null!;
}
