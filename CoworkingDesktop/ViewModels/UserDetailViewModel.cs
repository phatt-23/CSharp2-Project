using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Pagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels
{
    public class UserDetailViewModel : ViewModelBase
    {
        public User User { get => _user; private set => Set(ref _user, value); }
        public UserReservationPagerViewModel UserReservationsPager { get; }
        public int TotalFinishedReservation { get => _totalFinishedReservation; private set => Set(ref _totalFinishedReservation, value); }

        public UserDetailViewModel(User user, IUsersService usersService)
        {
            User = user;
            _usersService = usersService;
            UserReservationsPager = new(user, usersService);

            _ = Init();
        }

        public async Task Init()
        {
            UserReservationsPager.LoadPageCommand.Execute(null);

            var result = await _usersService.GetReservationOfUser(User.UserId, 1, int.MaxValue);
            TotalFinishedReservation = result.Items.Count(x => !x.IsCancelled && x.EndTime <= DateTime.UtcNow);
        }

        public string Title => "User Detail";
        private User _user;
        private int _totalFinishedReservation;
        private IUsersService _usersService;
    }
}
