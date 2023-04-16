using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("ChucVu")]
public partial class ChucVuModel
{
    [Key]
    [Column("MaCV")]
    public int MaCv { get; set; }

    [Column("TenCV")]
    [StringLength(50)]
    public string TenCv { get; set; } = null!;

    [Column(TypeName = "decimal(19, 0)")]
    public decimal LuongCanBan { get; set; }

    [StringLength(150)]
    public string? MoTa { get; set; }

    [InverseProperty("MaCvNavigation")]
    public virtual ICollection<NhanVienModel> NhanViens { get; } = new List<NhanVienModel>();
}
