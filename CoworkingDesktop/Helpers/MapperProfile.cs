using CoworkingDesktop.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Helpers
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<ReservationDto, Reservation>();
            CreateMap<Reservation, ReservationUpdateDto>();
            CreateMap<Reservation, ReservationCreateDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserCreateDto>();
            CreateMap<UserRoleDto, UserRole>();
            CreateMap<WorkspaceDto, Workspace>();
            CreateMap<Workspace, WorkspaceCreateDto>();
            CreateMap<Workspace, WorkspaceUpdateDto>();
            CreateMap<CoworkingCenterDto, CoworkingCenter>();
            CreateMap<CoworkingCenter, CoworkingCenterCreateDto>();
            CreateMap<CoworkingCenter, CoworkingCenterUpdateDto>();
            CreateMap<PricingDto, Pricing>();
            CreateMap<Pricing, PricingCreateDto>();
            CreateMap<WorkspaceStatusHistoryDto, WorkspaceStatusHistory>();
            CreateMap<AddressDto, Address>();
        }
    }
}
