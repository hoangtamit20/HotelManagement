using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("ChamCongNhanVien")]
public partial class ChamCongNhanVienModel
{
    [Key]
    [Column("MaCCNV")]
    public int MaCcnv { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGioDiLam { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGioRaVe { get; set; }

    public bool? DiLamTre { get; set; }

    public bool? RaVeSom { get; set; }

    public bool? XinNghiPhep { get; set; }

    [Column(TypeName = "ntext")]
    public string? LyDoNghi { get; set; }

    [Column("MaNV")]
    public int MaNv { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("ChamCongNhanViens")]
    public virtual NhanVienModel MaNvNavigation { get; set; } = null!;
}
