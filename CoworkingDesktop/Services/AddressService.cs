using AutoMapper;
using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public class AddressService : IAddressService
    {
        public AddressService(IHttpClientFactory httpFactory, IMapper mapper)
        {
            _http = httpFactory.CreateClient(App.API_HTTP_CLIENT);
            _mapper = mapper;
        }

        public async Task<Address?> GetAddressById(int id)
        {
            var response = await _http.GetAsync($"api/admin/address/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var dto = await response.Content.ReadFromJsonAsync<AddressDto>();
            if (dto == null) return null;

            return _mapper.Map<Address>(dto);
        }

        public async Task<Address?> GetAddressByCoords(double latitude, double longitude)
        {
            var url = $"api/admin/address/coords?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";
            var response = await _http.GetAsync(url);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var dto = await response.Content.ReadFromJsonAsync<AddressDto>();
            if (dto == null) return null;

            return _mapper.Map<Address>(dto);
        }

        private readonly HttpClient _http;
        private readonly IMapper _mapper;
    }
}
