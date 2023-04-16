using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("Phong")]
[Index("SoPhong", Name = "Phong_SoPhong", IsUnique = true)]
public partial class PhongModel
{
    [Key]
    public int MaP { get; set; }

    [DisplayName("Số phòng")]
    public int SoPhong { get; set; }

    [DisplayName("Hình ảnh")]
    [Column(TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [StringLength(150)]
    public string? MoTa { get; set; }

    [Column("MaLP")]
    public int MaLp { get; set; }

    [Column("MaTVP")]
    public int MaTvp { get; set; }

    [InverseProperty("MaPNavigation")]
    public virtual ICollection<DatPhongModel> DatPhongs { get; } = new List<DatPhongModel>();

    [ForeignKey("MaLp")]
    [InverseProperty("Phongs")]
    public virtual LoaiPhongModel? MaLpNavigation { get; set; } = null!;

    [ForeignKey("MaTvp")]
    [InverseProperty("Phongs")]
    public virtual TacVuPhongModel? MaTvpNavigation { get; set; } = null!;

    [NotMapped]
    [DisplayName("Upload Hình Ảnh Phòng")]
    public IFormFile? fileUpload {get;set;}
}
