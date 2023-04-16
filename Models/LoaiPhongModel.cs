using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("LoaiPhong")]
public partial class LoaiPhongModel
{
    [Key]
    [Column("MaLP")]
    public int MaLp { get; set; }

    [Column("TenLP")]
    [StringLength(50)]
    public string TenLp { get; set; } = null!;

    [Column(TypeName = "decimal(19, 0)")]
    public decimal? DonGia { get; set; }

    [InverseProperty("MaLpNavigation")]
    public virtual ICollection<PhongModel> Phongs { get; } = new List<PhongModel>();
}
