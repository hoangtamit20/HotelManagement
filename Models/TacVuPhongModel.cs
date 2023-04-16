using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("TacVuPhong")]
public partial class TacVuPhongModel
{
    [Key]
    [Column("MaTVP")]
    public int MaTvp { get; set; }

    [Column("TenTVP")]
    [StringLength(50)]
    public string TenTvp { get; set; } = null!;

    [StringLength(150)]
    public string? MoTa { get; set; }

    [InverseProperty("MaTvpNavigation")]
    public virtual ICollection<PhongModel> Phongs { get; } = new List<PhongModel>();
}
