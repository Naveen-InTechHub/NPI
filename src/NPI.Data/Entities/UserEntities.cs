using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPI.Data.Entities
{
    // User.cs
    public class User : BaseEntity
    { 
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    // Role.cs
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    // UserRole.cs
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }

}
