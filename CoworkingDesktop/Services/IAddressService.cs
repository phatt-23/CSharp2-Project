using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IAddressService
    {
        Task<Address?> GetAddressById(int id);
        Task<Address?> GetAddressByCoords(double latitude, double longitude);
    }
}
