using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("LoaiDichVu")]
public partial class LoaiDichVuModel
{
    [Key]
    [Column("MaLDV")]
    public int MaLdv { get; set; }

    [Column("TenLDV")]
    [StringLength(50)]
    public string? TenLdv { get; set; }

    [StringLength(150)]
    public string? MoTa { get; set; }

    [InverseProperty("MaLdvNavigation")]
    public virtual ICollection<DichVuModel> DichVus { get; } = new List<DichVuModel>();
}
