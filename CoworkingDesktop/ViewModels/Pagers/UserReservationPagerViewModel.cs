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
    public class UserReservationPagerViewModel : PaginatedViewModel<Reservation>
    {
        public UserReservationPagerViewModel(User user, IUsersService usersService)
        {
            _user = user;
            _usersService = usersService;
        }

        protected override async Task<PagedResult<Reservation>> LoadPageAsync(int page, int pageSize)
        {
            return await _usersService.GetReservationOfUser(_user.UserId, page, pageSize);
        }

        private readonly IUsersService _usersService;
        private readonly User _user;
    }
}
