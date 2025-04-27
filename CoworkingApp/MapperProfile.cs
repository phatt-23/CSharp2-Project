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
        CreateMap<CoworkingCenter, CoworkingCenterCreateRequestDto>().ReverseMap();
        
        CreateMap<Workspace, WorkspaceDto>()
            .ForMember(dest => dest.LatestPricing, opt => opt.MapFrom(src => src.WorkspacePricings.FirstOrDefault()))
            .ReverseMap();
        CreateMap<Workspace, WorkspaceDetailDto>().ReverseMap();
        CreateMap<Workspace, AdminWorkspaceDto>().ReverseMap();
        CreateMap<Workspace, AdminWorkspaceDetailDto>().ReverseMap();
        
        CreateMap<WorkspaceStatus, WorkspaceStatusDto>().ReverseMap();
        
        CreateMap<Reservation, ReservationDto>().ReverseMap();
        CreateMap<ReservationCreateRequestDto, Reservation>().ReverseMap();
        CreateMap<Reservation, AdminReservationDto>().ReverseMap();
        
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserRegisterRequestDto, User>().ReverseMap();
        CreateMap<UserRole, UserRoleDto>().ReverseMap();
        
        CreateMap<WorkspacePricing, WorkspacePricingDto>().ReverseMap();
        CreateMap<WorkspacePricing, LatestWorkspacePricingDto>().ReverseMap();
        
        CreateMap<WorkspaceHistory, WorkspaceHistoryDto>().ReverseMap();

        CreateMap<CoworkingCenter, AdminCoworkingCenterDto>().ReverseMap();
        CreateMap<WorkspacePricing, AdminWorkspacePricingDto>().ReverseMap();
    }
}
