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
    public class WorkspacesService : IWorkspacesService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;

        public WorkspacesService(IHttpClientFactory httpFactory, IMapper mapper)
        {
            _http = httpFactory.CreateClient("ApiClient");
            _mapper = mapper;
        }

        public async Task<Workspace?> CreateWorkspace(WorkspaceCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/admin/workspace", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var workspaceDto = await response.Content.ReadFromJsonAsync<WorkspaceDto>();
            if (workspaceDto == null) return null;

            return _mapper.Map<Workspace>(workspaceDto);
        }

        public async Task<Workspace?> DeleteWorkspace(int id)
        {
            var response = await _http.DeleteAsync($"api/admin/workspace/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var workspaceDto = await response.Content.ReadFromJsonAsync<WorkspaceDto>();
            if (workspaceDto == null) return null;

            return _mapper.Map<Workspace>(workspaceDto);
        }

        public async Task<PagedResult<WorkspaceStatusHistory>> GetStatusHistory(int id, int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/workspace/{id}/history?PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response))
            {
                var dto = await response.Content.ReadFromJsonAsync<WorkspaceStatusHistoryPageDto>();
                if (dto != null)
                {
                    return new PagedResult<WorkspaceStatusHistory>()
                    { 
                        Items = _mapper.Map<List<WorkspaceStatusHistory>>(dto.Histories),
                        TotalCount = dto.TotalCount,
                    };
                }
            }

            return PagedResult<WorkspaceStatusHistory>.EmptyResult;
        }

        public async Task<Workspace?> GetWorkspaceById(int id)
        {
            var response = await _http.GetAsync($"api/admin/workspace/{id}");
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var body = await response.Content.ReadFromJsonAsync<WorkspaceDto>();
            return (body == null) ? null : _mapper.Map<Workspace>(body);
        }

        public async Task<PagedResult<Reservation>> GetWorkspaceReservations(int id, int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/reservation?WorkspaceId={id}&PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response))
            {
                var body = await response.Content.ReadFromJsonAsync<ReservationPageDto>();
                if (body != null)
                {
                    return new PagedResult<Reservation>()
                    { 
                        Items = _mapper.Map<List<Reservation>>(body.Reservations),
                        TotalCount = body.TotalCount,
                    };
                }
            }
               
            return PagedResult<Reservation>.EmptyResult;
        }

        public async Task<PagedResult<Workspace>> GetWorkspaces(int page, int pageSize)
        {
            var response = await _http.GetAsync($"api/admin/workspace?PageNumber={page}&PageSize={pageSize}");
            if (await HttpResponseHelper.InfoOnBadStatusCode(response))
            {
                var body = await response.Content.ReadFromJsonAsync<WorkspacePageDto>();
                if (body != null)
                {
                    return new PagedResult<Workspace>
                    {
                        TotalCount = body.TotalCount,
                        Items = _mapper.Map<List<Workspace>>(body.Workspaces)
                    };
                }
            }

            return new PagedResult<Workspace>
            {
                TotalCount = 0,
                Items = []
            };
        }

        public async Task<Workspace?> UpdateWorkspace(WorkspaceUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/admin/workspace", dto);
            if (!await HttpResponseHelper.InfoOnBadStatusCode(response)) return null;

            var workspaceDto = await response.Content.ReadFromJsonAsync<WorkspaceDto>();
            if (workspaceDto == null) return null;

            return _mapper.Map<Workspace>(workspaceDto);
        }
    }
}
