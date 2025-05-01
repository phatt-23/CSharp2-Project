using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.Exceptions;

public class ReservationStartTimeInPastException(string message) : FormValidationException(message, nameof(ReservationCreateRequestDto.StartTime));
public class ReservationEndTimeInPastException(string message) : FormValidationException(message, nameof(ReservationCreateRequestDto.EndTime));
public class WorkspaceNotFoundException(string message) : FormValidationException(message, nameof(ReservationCreateRequestDto.WorkspaceId));
public class NoPricingForWorkspaceException(string message) : Exception(message);
public class ClashingReservationTimeException(string message) : Exception(message);
public class ReservationAlreadyTakingPlaceException(string m) : Exception(m);
