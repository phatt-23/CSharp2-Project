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
    public class PricingService : IPricingService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;

        public PricingService(IHttpClientFactory httpFactory, IMapper mapper)
        {
            _http = httpFactory.CreateClient(App.API_HTTP_CLIENT);
            _mapper = mapper;
        }

        public async Task<Pricing?> GetPricingById(int id)
        {
            var response = await _http.GetAsync($"api/admin/pricing/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var dto = await response.Content.ReadFromJsonAsync<PricingDto>();
            if (dto == null) return null;

            return _mapper.Map<Pricing>(dto);
        }

        public async Task<List<Pricing>> GetPricingsOfWorkspace(int workspaceId)
        {
            var response = await _http.GetAsync($"api/admin/pricing?Workspace={workspaceId}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return [];

            var dtos = await response.Content.ReadFromJsonAsync<List<PricingDto>>();
            if (dtos == null) return [];

            return _mapper.Map<List<Pricing>>(dtos);
        }

        public async Task<Pricing?> AddPricing(PricingCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync($"api/admin/pricing", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var pricingDto = await response.Content.ReadFromJsonAsync<PricingDto>();
            if (pricingDto == null) return null;

            return _mapper.Map<Pricing>(pricingDto);
        }
    }

}
