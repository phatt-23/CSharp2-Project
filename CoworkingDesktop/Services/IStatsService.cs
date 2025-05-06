using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IStatsService
    {
        Task<CoworkingCenterRevenuesResponseDto?> GetCoworkingCenterRevenues(TimeBack timeback);
        Task<CoworkingCenterRevenueResponseDto?> GetCoworkingCenterRevenue(int coworkingCenterId);
    }
}
