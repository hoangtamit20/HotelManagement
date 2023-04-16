using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("DichVu")]
public partial class DichVuModel
{
    [Key]
    [Column("MaDV")]
    public int MaDv { get; set; }

    [Column("TenDV")]
    [StringLength(50)]
    public string TenDv { get; set; } = null!;

    public int? SoLuong { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? DonGia { get; set; }

    [Column(TypeName = "ntext")]
    public string? MoTa { get; set; }

    [Column(TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [Column("MaLDV")]
    public int MaLdv { get; set; }

    [InverseProperty("MaDvNavigation")]
    public virtual ICollection<DatDichVuModel> DatDichVus { get; } = new List<DatDichVuModel>();

    [ForeignKey("MaLdv")]
    [InverseProperty("DichVus")]
    public virtual LoaiDichVuModel MaLdvNavigation { get; set; } = null!;
}
