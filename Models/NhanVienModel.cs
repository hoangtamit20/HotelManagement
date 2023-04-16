using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("NhanVien")]
[Index("Cccd", Name = "NhanVien_CCCD", IsUnique = true)]
public partial class NhanVienModel
{
    [Key]
    [Column("MaNV")]
    public int MaNv { get; set; }

    [StringLength(50)]
    [DisplayName("Họ tên")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    public string? HoTen { get; set; } = null!;

    [DisplayName("CMND/CCCD")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [Column("CCCD")]
    [StringLength(12)]
    [Unicode(false)]
    public string? Cccd { get; set; } = null!;

    [DisplayName("Số điện thoại")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [Column("SoDT")]
    [StringLength(11)]
    [Unicode(false)]
    public string? SoDt { get; set; } = null!;

    [DisplayName("Ngày sinh")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [Column(TypeName = "datetime")]
    public DateTime? NgaySinh { get; set; }

    [DisplayName("Giới tính")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    public bool? GioiTinh { get; set; }

    [DisplayName("Địa chỉ")]
    [StringLength(150)]
    public string? DiaChi { get; set; }

    [DisplayName("Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [DisplayName("Hình Ảnh")]
    [Column(TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [DisplayName("Ngày vào làm")]
    [Column(TypeName = "datetime")]
    public DateTime? NgayVaoLam { get; set; }

    [DisplayName("Chức vụ")]
    [Column("MaCV")]
    public int MaCv { get; set; }

    [DisplayName("Tên đăng nhập")]
    [StringLength(16)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<ChamCongNhanVienModel> ChamCongNhanViens { get; } = new List<ChamCongNhanVienModel>();

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<HoaDonModel> HoaDons { get; } = new List<HoaDonModel>();

    [ForeignKey("MaCv")]
    [InverseProperty("NhanViens")]
    public virtual ChucVuModel? MaCvNavigation { get; set; } = null!;

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<NhanVienNghiViecModel> NhanVienNghiViecs { get; } = new List<NhanVienNghiViecModel>();

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<TongLuongModel> TongLuongs { get; } = new List<TongLuongModel>();

    public virtual AccountModel? UserNameNavigation { get; set; } = null!;

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? FileUpload {get;set;}
}
