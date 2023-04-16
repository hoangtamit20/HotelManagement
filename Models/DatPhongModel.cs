using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("DatPhong")]
public partial class DatPhongModel
{
    [Key]
    [Column("MaDP")]
    public int MaDp { get; set; }
    
    [Required(ErrorMessage = "{0} không được để trống!")]
    [DisplayName("Số người")]
    [Range(1,10, ErrorMessage = "{0} phải có ít nhất là {1} người và tối đa là {2} người")]
    public int? SoNguoi { get; set; }

    [Column(TypeName = "datetime")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [DisplayName("Ngày đặt phòng")]
    public DateTime? NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [DisplayName("Ngày trả phòng")]
    public DateTime? NgayKetThuc { get; set; }

    [Column(TypeName = "decimal(19, 0)")]
    [DisplayName("Tiền đặt cọc")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    public decimal? TienDatCoc { get; set; }

    [DisplayName("Phòng")]
    public int MaP { get; set; }

    [Column("MaKH")]
    [DisplayName("Khách hàng")]
    public int MaKh { get; set; }

    [InverseProperty("MaDpNavigation")]
    public virtual ICollection<DatDichVuModel> DatDichVus { get; } = new List<DatDichVuModel>();

    [InverseProperty("MaDpNavigation")]
    public virtual ICollection<HoaDonModel> HoaDons { get; } = new List<HoaDonModel>();

    [ForeignKey("MaKh")]
    [InverseProperty("DatPhongs")]
    public virtual KhachHangModel? MaKhNavigation { get; set; } = null!;

    [ForeignKey("MaP")]
    [InverseProperty("DatPhongs")]
    public virtual PhongModel? MaPNavigation { get; set; } = null!;
}
