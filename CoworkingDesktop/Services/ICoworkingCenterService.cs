using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface ICoworkingCenterService
    {
        Task<PagedResult<CoworkingCenter>> GetCoworkingCenters(int page, int pageSize);
        Task<CoworkingCenter?> GetCoworkingCenterById(int id);
        Task<CoworkingCenter?> CreateCoworkingCenter(CoworkingCenterCreateDto dto);
        Task<CoworkingCenter?> UpdateCoworkingCenter(CoworkingCenterUpdateDto dto);
        Task<CoworkingCenter?> DeleteCoworkingCenter(int id);
    }
}
