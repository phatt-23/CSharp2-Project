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
using System.Windows;
using System.Windows.Markup;
using static System.Net.WebRequestMethods;

namespace CoworkingDesktop.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;
        private readonly IWorkspacesService _workspacesService;
        private readonly IUsersService _usersService;
        private readonly IPricingService _pricingService;

        public ReservationsService(IHttpClientFactory httpFactory, IMapper mapper, IWorkspacesService workspacesService, IUsersService usersService, IPricingService pricingService)
        {
            _http = httpFactory.CreateClient("ApiClient");
            _mapper = mapper;
            _workspacesService = workspacesService;
            _usersService = usersService;
            _pricingService = pricingService;
        }

        public async Task<Reservation?> CreateReservation(ReservationCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/admin/reservation", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var resDto = await response.Content.ReadFromJsonAsync<ReservationDto>();
            if (resDto == null) return null;

            var res = _mapper.Map<Reservation>(resDto);

            return res;
        }

        public async Task<Reservation?> DeleteReservation(int id)
        {
            var response = await _http.DeleteAsync($"api/admin/reservation/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var resDto = await response.Content.ReadFromJsonAsync<ReservationDto>();
            if (resDto == null) return null;

            var res = _mapper.Map<Reservation>(resDto);

            return res;
        }

        public async Task<Reservation?> GetReservationById(int id)
        {
            var response = await _http.GetAsync($"api/reservation/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var dto = await response.Content.ReadFromJsonAsync<ReservationDto>();
            if (dto == null) return null;

            var res = _mapper.Map<Reservation>(dto);
            return res;
        }

        public async Task<PagedResult<Reservation>> GetReservations(int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/reservation?PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response)) 
            {
                var responseBody = await response.Content.ReadFromJsonAsync<ReservationPageDto>();
                if (responseBody != null)
                {
                    return new PagedResult<Reservation>()
                    {
                        Items = _mapper.Map<List<Reservation>>(responseBody.Reservations),
                        TotalCount = responseBody.TotalCount,
                    };
                }
            }

            return PagedResult<Reservation>.EmptyResult;
        }

        public async Task<Reservation?> UpdateReservation(ReservationUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/admin/reservation", dto);
            if (!response.IsSuccessStatusCode) return null;

            var resDto = await response.Content.ReadFromJsonAsync<ReservationDto>();

            if (resDto == null) return null;

            var res = _mapper.Map<Reservation>(resDto);

            return res;
        }
    }
}
