using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public UserRoleType Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public UserRoleType Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class UserCreateDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; } 
        public required UserRoleType Role { get; set; }
    }

    public class UserRoleChangeDto
    {
        public required int UserId { get; set; }
        public required UserRoleType Role { get; set; }
    }

    public class UserRole
    {
        public int UserRoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }

    public class UserRoleDto
    {
        public int UserRoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }

    public class UserPageDto : PaginationResponseDto
    {
        public List<UserDto> Users { get; set; } = null!;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRoleType { Admin, Customer }

}
