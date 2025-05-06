using AutoMapper;
using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;

        public UsersService(IHttpClientFactory httpFactory, IMapper mapper)
        {
            _http = httpFactory.CreateClient(App.API_HTTP_CLIENT);
            _mapper = mapper;
        }

        public async Task<User?> ChangeRole(UserRoleChangeDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/admin/user/role", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            if (userDto == null) return null;

            return (userDto == null) ? null : _mapper.Map<User>(userDto);
        }

        public async Task<User?> CreateUser(UserCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/admin/user", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            return (userDto == null) ? null : _mapper.Map<User>(userDto);
        }

        public async Task<User?> DeleteUser(int id)
        {
            var response = await _http.DeleteAsync($"api/admin/user/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            return (userDto == null) ? null : _mapper.Map<User>(userDto);
        }

        public async Task<PagedResult<Reservation>> GetReservationOfUser(int id, int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/reservation?CustomerId={id}&PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response))
            {
                var responseBody = await response.Content.ReadFromJsonAsync<ReservationPageDto>();
                if (responseBody != null)
                {
                    return new PagedResult<Reservation>
                    {
                        TotalCount = responseBody.TotalCount,
                        Items = _mapper.Map<List<Reservation>>(responseBody.Reservations)
                    };
                }
            }

            return PagedResult<Reservation>.EmptyResult;
        }

        public async Task<UserRole?> GetRole(UserRoleType type)
        {
            var response = await _http.GetAsync($"api/user/role?type={type}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var dtos = await response.Content.ReadFromJsonAsync<List<UserRoleDto>>();
            return (dtos == null) ? null : _mapper.Map<List<UserRole>>(dtos).FirstOrDefault();
        }

        public async Task<List<UserRole>> GetRoles()
        {
            var response = await _http.GetAsync("api/user/role");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return [];

            var dtos = await response.Content.ReadFromJsonAsync<List<UserRoleDto>>();
            return (dtos == null) ? [] : _mapper.Map<List<UserRole>>(dtos);
        }

        public async Task<User?> GetUserById(int id)
        {
            var response = await _http.GetAsync($"api/admin/user/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            return (userDto == null) ? null : _mapper.Map<User>(userDto);
        }

        public async Task<PagedResult<User>> GetUsers(int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/user?PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response))
            {
                var responseBody = await response.Content.ReadFromJsonAsync<UserPageDto>();
                if (responseBody != null)
                {
                    return new PagedResult<User>
                    {
                        TotalCount = responseBody.TotalCount,
                        Items = _mapper.Map<List<User>>(responseBody.Users)
                    };
                }
            }

            return PagedResult<User>.EmptyResult; 
        }
    }
}
