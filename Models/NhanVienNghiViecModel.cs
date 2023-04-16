using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("NhanVienNghiViec")]
public partial class NhanVienNghiViecModel
{
    [Key]
    [Column("MaNVNV")]
    public int MaNvnv { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayNghiViec { get; set; }

    [Column(TypeName = "ntext")]
    public string? LyDo { get; set; }

    [Column("MaNV")]
    public int MaNv { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("NhanVienNghiViecs")]
    public virtual NhanVienModel? MaNvNavigation { get; set; } = null!;
}