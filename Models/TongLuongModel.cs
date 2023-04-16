using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("TongLuong")]
public partial class TongLuongModel
{
    [Key]
    [Column("MaTL")]
    public int MaTl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayNhanLuong { get; set; }

    public int? SoNgayLam { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? TongTien { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? UngLuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayUngLuong { get; set; }

    [Column("MaNV")]
    public int MaNv { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("TongLuongs")]
    public virtual NhanVienModel MaNvNavigation { get; set; } = null!;
}
