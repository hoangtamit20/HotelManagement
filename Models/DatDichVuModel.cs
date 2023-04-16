using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("DatDichVu")]
public partial class DatDichVuModel
{
    [Key]
    [Column("MaDDV")]
    public int MaDdv { get; set; }

    public int? SoLuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDatDichVu { get; set; }

    [Column("MaDV")]
    public int MaDv { get; set; }

    [Column("MaKH")]
    public int MaKh { get; set; }

    [Column("MaDP")]
    public int MaDp { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    [DisplayName("Tổng tiền dịch vụ")]
    public decimal? TongTien { get; set; }

    [ForeignKey("MaDp")]
    [InverseProperty("DatDichVus")]
    public virtual DatPhongModel MaDpNavigation { get; set; } = null!;

    [ForeignKey("MaDv")]
    [InverseProperty("DatDichVus")]
    public virtual DichVuModel MaDvNavigation { get; set; } = null!;

    [ForeignKey("MaKh")]
    [InverseProperty("DatDichVus")]
    public virtual KhachHangModel MaKhNavigation { get; set; } = null!;
}
