using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IReservationsService
    {
        Task<PagedResult<Reservation>> GetReservations(int page, int pageSize);
        Task<Reservation?> GetReservationById(int id);
        Task<Reservation?> CreateReservation(ReservationCreateDto dto);
        Task<Reservation?> UpdateReservation(ReservationUpdateDto dto);
        Task<Reservation?> DeleteReservation(int id);
    }
}
