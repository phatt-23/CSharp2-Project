using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp;

public class MapperProfile : Profile
{
    private string AddressDisplayNameFromCoworkingCenter(CoworkingCenter s) => 
        $"{s.Address.StreetAddress}"
        + (string.IsNullOrEmpty(s.Address.District)? "" : $", {s.Address.District}")
        + $", {s.Address.PostalCode}"
        // assuming Address.City nav prop is loaded
        + (s.Address.City != null ? $", {s.Address.City.Name}" : "");


    public MapperProfile()
    {
        CreateMap<CoworkingCenter, CoworkingCenterDto>()
            // simple properties
            .ForMember(d => d.CoworkingCenterId, opt => opt.MapFrom(s => s.CoworkingCenterId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))   
            // assuming Address is loaded
            .ForMember(d => d.Latitude, opt => opt.MapFrom(s => s.Address.Latitude))
            .ForMember(d => d.Longitude, opt => opt.MapFrom(s => s.Address.Longitude))
            .ForMember(d => d.AddressDisplayName, opt => opt.MapFrom(s => AddressDisplayNameFromCoworkingCenter(s)));

        CreateMap<CoworkingCenter, AdminCoworkingCenterDto>()
            // simple properties
            .ForMember(d => d.CoworkingCenterId, opt => opt.MapFrom(s => s.CoworkingCenterId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.AddressId, opt => opt.MapFrom(s => s.AddressId))
            .ForMember(d => d.LastUpdated, opt => opt.MapFrom(s => s.LastUpdated))
            .ForMember(d => d.UpdatedBy, opt => opt.MapFrom(s => s.UpdatedBy))
            // flatten Address into a single display string
            .ForMember(d => d.AddressDisplayName, opt => opt.MapFrom(s => AddressDisplayNameFromCoworkingCenter(s)))
            // count related Workspaces
            .ForMember(d => d.WorkspaceCount, opt => opt.MapFrom(s => s.Workspaces.Count));

        CreateMap<CoworkingCenterCreateRequestDto, CoworkingCenter>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // the Address navigation is constructed from the same DTO
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src))
            // AddressId and UpdatedBy are handled later (EF will pick up Address and set AddressId)
            .ForMember(dest => dest.AddressId, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<CoworkingCenterCreateRequestDto, Address>()
            // simple, can be done now
            .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.StreetAddress))
            .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // setup later in code
            .ForMember(dest => dest.CityId, opt => opt.Ignore())
            .ForMember(dest => dest.Latitude, opt => opt.Ignore())
            .ForMember(dest => dest.Longitude, opt => opt.Ignore());

        

        CreateMap<CoworkingCenter, AdminCoworkingCenterDto>().ReverseMap();




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

        CreateMap<ReservationUpdateRequestDto, Reservation>().ReverseMap();


        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<UserRegisterRequestDto, User>().ReverseMap();

        CreateMap<UserRole, UserRoleDto>().ReverseMap();



        
        CreateMap<WorkspacePricing, WorkspacePricingDto>().ReverseMap();

        CreateMap<WorkspacePricing, LatestWorkspacePricingDto>().ReverseMap();

        CreateMap<WorkspacePricing, AdminWorkspacePricingDto>().ReverseMap();


        
        CreateMap<WorkspaceHistory, WorkspaceHistoryDto>().ReverseMap();





    }
}
