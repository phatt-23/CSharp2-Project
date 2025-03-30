using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;

namespace CoworkingApp;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<CoworkingCenter, CoworkingCenterDto>().ReverseMap();
        
        CreateMap<Workspace, WorkspaceDto>().ReverseMap();
        CreateMap<Workspace, WorkspaceDetailDto>().ReverseMap();
        
        CreateMap<WorkspaceStatus, WorkspaceStatusDto>().ReverseMap();
        
        CreateMap<Reservation, ReservationDto>().ReverseMap();
        CreateMap<ReservationCreateRequestDto, Reservation>().ReverseMap();

        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserRegisterRequestDto, User>().ReverseMap();

        CreateMap<UserRole, UserRoleDto>().ReverseMap();
    }
}
