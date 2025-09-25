using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;
public class UserEntity : IdentityUser
{
  [ProtectedPersonalData]
  [Required]
  [Column(TypeName = "nvarchar(100)")]
  public string FirstName { get; set; } = null!;
  [ProtectedPersonalData]
  [Required]
  [Column(TypeName = "nvarchar(100)")]
  public string LastName { get; set; } = null!;
}
