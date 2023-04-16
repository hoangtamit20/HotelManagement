using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Table("Account")]
[Index("UserName", Name = "Account_UserName", IsUnique = true)]
[Index("Email", Name = "UQ__Account__A9D105341F5381BA", IsUnique = true)]
public partial class AccountModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(16)]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [DisplayName("Tên đăng nhập")]
    [Unicode(false)]
    public string? UserName { get; set; } = null!;

    [StringLength(255)]
    [DisplayName("Mật khẩu")]
    [Required(ErrorMessage = "{0} không được để trống!")]
    [Unicode(false)]
    public string? UserPassword { get; set; } = null!;

    public int? CountLoginFailed { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeLockOut { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(11)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateUpdate { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<NhanVienModel> NhanViens { get; } = new List<NhanVienModel>();

    public virtual RoleModel? RoleIdNavigation { get; set; } = null!;
}
