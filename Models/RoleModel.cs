using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLKSMVC.Models;

[Index("RoleId", Name = "Roles_RoleId", IsUnique = true)]
public partial class RoleModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<AccountModel> Accounts { get; } = new List<AccountModel>();
}
