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
    public class StatsService : IStatsService
    {
        public StatsService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient(App.API_HTTP_CLIENT);
        }

        public async Task<CoworkingCenterRevenuesResponseDto?> GetCoworkingCenterRevenues(TimeBack timeback)
        {
            var response = await _http.GetAsync($"api/admin/stats/coworking-center/revenue?timeBack={timeback}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var body = await response.Content.ReadFromJsonAsync<CoworkingCenterRevenuesResponseDto>();
            if (body == null) return null;

            // map the response to the model, fuck it though
            return body;
        }

        public async Task<CoworkingCenterRevenueResponseDto?> GetCoworkingCenterRevenue(int coworkingCenterId)
        {
            var response = await _http.GetAsync($"api/admin/stats/coworking-center/{coworkingCenterId}/revenue");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var body = await response.Content.ReadFromJsonAsync<CoworkingCenterRevenueResponseDto>();
            if (body == null) return null;

            // map the response to the model, fuck it though
            return body;
        }

        private readonly HttpClient _http;
    }
}
