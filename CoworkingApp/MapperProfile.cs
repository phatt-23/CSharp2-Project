using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using Microsoft.EntityFrameworkCore;

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
            ;

        /*
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
        */

        CreateMap<CoworkingCenterCreateRequestDto, CoworkingCenter>();



        CreateMap<Workspace, WorkspaceDto>()
            .ForMember(dest => dest.PricePerHour, opt => opt.MapFrom(src =>
                src.WorkspacePricings
                    .OrderByDescending(wp => wp.ValidFrom)
                    .Select(wp => wp.PricePerHour)
                    .FirstOrDefault(1000)
            ))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                Enum.Parse<WorkspaceStatusType>(src.WorkspaceHistories
                    .OrderByDescending(wh => wh.ChangeAt)
                    .First().Status.Name)
            ))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.GetCurrentStatus().Type))
            .ForMember(d => d.CoworkingCenterDisplayName, o => o.MapFrom(s => s.CoworkingCenter.Name));

        CreateMap<WorkspaceDto, Workspace>()
            .ForMember(w => w.WorkspacePricings, o => o.Ignore());

        CreateMap<Workspace, WorkspaceDetailDto>().ReverseMap();

        CreateMap<Workspace, AdminWorkspaceDto>()
            .ForMember(d => d.PricePerHour, o => o.MapFrom(s => s.GetCurrentPricePerHour()))
            .ForMember(d => d.CoworkingCenterDisplayName, o => o.MapFrom(s => s.CoworkingCenter.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.GetCurrentStatus().Type))
            .ReverseMap();

        CreateMap<Workspace, AdminWorkspaceDetailDto>().ReverseMap();



        CreateMap<WorkspaceStatus, WorkspaceStatusDto>().ReverseMap();


        CreateMap<Reservation, ReservationDto>()
            .ForMember(d => d.WorkspaceDisplayName, o => o.MapFrom(s => s.Workspace.Name))
            .ForMember(d => d.PricingPerHour, o => o.MapFrom(s => s.Pricing.PricePerHour))
            .ReverseMap();

        CreateMap<Reservation, AnonymousReservationDto>().ReverseMap();

        CreateMap<Reservation, AdminReservationDto>()
            .ForMember(d => d.WorkspaceDisplayName, o => o.MapFrom(s => s.Workspace.Name))
            .ForMember(d => d.PricingPerHour, o => o.MapFrom(s => s.Pricing.PricePerHour))
            .ForMember(d => d.CustomerEmail, o => o.MapFrom(s => s.Customer!.Email))
            .ReverseMap();
        
        CreateMap<ReservationCreateRequestDto, Reservation>().ReverseMap();

        CreateMap<ReservationUpdateRequestDto, Reservation>().ReverseMap();

        CreateMap<AdminReservationUpdateRequestDto, Reservation>().ReverseMap();


        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.Type))
            .ReverseMap();

        CreateMap<UserRegisterRequestDto, User>().ReverseMap();

        CreateMap<UserRole, UserRoleDto>()
            .ReverseMap();

        CreateMap<AdminUserCreateDto, User>().ReverseMap();

        CreateMap<User, AdminUserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.Type))
            .ReverseMap();



        CreateMap<WorkspacePricing, WorkspacePricingDto>().ReverseMap();

        CreateMap<WorkspacePricing, AdminWorkspacePricingDto>().ReverseMap();

        CreateMap<WorkspacePricingCreateRequestDto, WorkspacePricing>().ReverseMap();


        CreateMap<WorkspaceHistory, WorkspaceHistoryDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Type))
            .ReverseMap();


        CreateMap<Address, AddressDto>()
            .ForMember(d => d.City, o => o.MapFrom(s => s.City.Name))
            .ForMember(d => d.Country, o => o.MapFrom(s => s.City.Country.Name))
            ;
    }
}
