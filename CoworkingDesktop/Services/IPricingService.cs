using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IPricingService
    {
        Task<Pricing?> GetPricingById(int id);
        Task<List<Pricing>> GetPricingsOfWorkspace(int workspaceId);
        Task<Pricing?> AddPricing(PricingCreateDto dto);
    }
}
