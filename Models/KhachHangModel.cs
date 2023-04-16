using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("KhachHang")]
[Index("Cccd", Name = "UQ__KhachHan__A955A0AA250B6CAA", IsUnique = true)]
public partial class KhachHangModel
{
    [Key]
    [Column("MaKH")]
    public int MaKh { get; set; }

    [DisplayName("Họ và tên")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "{0} phải có ít nhất 2 ký tự và tối đa {1} ký tự.")]
    [Required(ErrorMessage = "{0} Không được để trống!")]
    public string? HoTen { get; set; } = null!;

    [Column("CCCD")]
    [DisplayName("CCCD/CMND")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "{0} phải bao gồm 12 chữ số.")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [RegularExpression("([1-9][0-9]*)", ErrorMessage = "{0} phải là các chữ số 0 đến 9.")] 
    [Unicode(false)]
    public string? Cccd { get; set; }

    [Column("SoDT")]
    [DisplayName("Số điện thoại")]
    [StringLength(11, MinimumLength = 10, ErrorMessage = "{0} phải có ít nhất {2} chữ số và tối đa {1} chữ số.")]
    [RegularExpression("([0-9]+)", ErrorMessage = "{0} phải là các chữ số 0 đến 9.")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [Unicode(false)]
    public string? SoDt { get; set; }

    [Column(TypeName = "datetime")]
    [DisplayName("Ngày sinh")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    public DateTime? NgaySinh { get; set; }

    [DisplayName("Giới tính")]
    public bool? GioiTinh { get; set; }

    [StringLength(150)]
    [DisplayName("Địa chỉ")]
    public string? DiaChi { get; set; }

    [Column(TypeName = "ntext")]
    [DisplayName("Hình ảnh")]
    public string? HinhAnh { get; set; }

    [InverseProperty("MaKhNavigation")]
    public virtual ICollection<DatDichVuModel> DatDichVus { get; } = new List<DatDichVuModel>();

    [InverseProperty("MaKhNavigation")]
    public virtual ICollection<DatPhongModel> DatPhongs { get; } = new List<DatPhongModel>();
}
