
namespace CoworkingApp.Services.Repositories;


[Serializable]
internal class ReservationTimeInPastException(string? message) : Exception(message)
{
    public string PropertyName { get; set; } = string.Empty;
}