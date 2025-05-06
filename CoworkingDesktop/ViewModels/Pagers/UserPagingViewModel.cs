using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels.Pagers
{
    public class UserPagingViewModel : PaginatedViewModel<User>
    {
        private readonly IUsersService _usersService;
        public UserPagingViewModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        protected override async Task<PagedResult<User>> LoadPageAsync(int page, int pageSize)
        {
            return await _usersService.GetUsers(page, pageSize);
        }
    }
}
