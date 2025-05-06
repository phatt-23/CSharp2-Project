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
    public class CoworkingCenterService : ICoworkingCenterService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;
        public CoworkingCenterService(IHttpClientFactory httpFactory, IMapper mapper)
        {
            _http = httpFactory.CreateClient(App.API_HTTP_CLIENT);
            _mapper = mapper;
        }

        public async Task<CoworkingCenter?> CreateCoworkingCenter(CoworkingCenterCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/admin/coworking-center", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var center = await response.Content.ReadFromJsonAsync<CoworkingCenterDto>();
            return center == null ? null : _mapper.Map<CoworkingCenter>(center);
        }

        public async Task<CoworkingCenter?> DeleteCoworkingCenter(int id)
        {
            var response = await _http.DeleteAsync($"api/admin/coworking-center/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var center = await response.Content.ReadFromJsonAsync<CoworkingCenterDto>();
            return center == null ? null : _mapper.Map<CoworkingCenter>(center);
        }

        public async Task<CoworkingCenter?> GetCoworkingCenterById(int id)
        {
            var response = await _http.GetAsync($"api/admin/coworking-center/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var center = await response.Content.ReadFromJsonAsync<CoworkingCenterDto>();
            return center == null ? null : _mapper.Map<CoworkingCenter>(center);
        }

        public async Task<PagedResult<CoworkingCenter>> GetCoworkingCenters(int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/coworking-center?PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response)) 
            {
                var responseBody = await response.Content.ReadFromJsonAsync<CoworkingCenterResponseDto>();
                if (responseBody != null)
                {
                    return new PagedResult<CoworkingCenter>
                    {
                        TotalCount = responseBody.TotalCount,
                        Items = _mapper.Map<List<CoworkingCenter>>(responseBody.Centers)
                    };
                }
            }

            return PagedResult<CoworkingCenter>.EmptyResult;
        }

        public async Task<CoworkingCenter?> UpdateCoworkingCenter(CoworkingCenterUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/admin/coworking-center", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var center = await response.Content.ReadFromJsonAsync<CoworkingCenterDto>();
            return center == null ? null : _mapper.Map<CoworkingCenter>(center);
        }
    }
}
