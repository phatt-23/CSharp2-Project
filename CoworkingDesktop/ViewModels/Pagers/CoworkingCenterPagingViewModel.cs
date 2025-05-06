using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels.Pagers
{
    public class CoworkingCenterPagingViewModel : PaginatedViewModel<CoworkingCenter>
    {
        private readonly ICoworkingCenterService _centersService;
        public CoworkingCenterPagingViewModel(ICoworkingCenterService centersService)
        {
            _centersService = centersService;
        }

        protected override async Task<PagedResult<CoworkingCenter>> LoadPageAsync(int page, int pageSize)
        {
            return await _centersService.GetCoworkingCenters(page, pageSize);
        }
    }
}
