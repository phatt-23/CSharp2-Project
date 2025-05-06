using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CoworkingDesktop.Services
{
    public interface IUsersService
    {
        Task<PagedResult<User>> GetUsers(int page, int pageSize);
        Task<User?> GetUserById(int id);
        Task<User?> CreateUser(UserCreateDto dto);
        Task<User?> ChangeRole(UserRoleChangeDto dto);
        Task<User?> DeleteUser(int id);
        Task<List<UserRole>> GetRoles();
        Task<UserRole?> GetRole(UserRoleType type);
        Task<PagedResult<Reservation>> GetReservationOfUser(int id, int page, int pageSize);
    }
}
