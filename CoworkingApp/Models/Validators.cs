using FluentValidation;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;

namespace CoworkingApp.Models;

public class UserRegisterRequestDtoValidator : AbstractValidator<UserRegisterRequestDto>
{
    public UserRegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your password.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}

public class ReservationCreateRequestDtoValidator : AbstractValidator<ReservationCreateRequestDto>
{
    public ReservationCreateRequestDtoValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.Now)
            .WithSeverity(Severity.Error)
            .WithMessage("Start time must start in the future");
        
        RuleFor(x => x.EndTime)
            .GreaterThan(DateTime.Now)
            .WithSeverity(Severity.Error)
            .WithMessage("End time must start in the future");

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithSeverity(Severity.Error)
            .WithMessage("End time must be after start time");
    }
}

public class CoworkingCenterValidator : AbstractValidator<CoworkingCenterCreateRequestDto>
{
    public CoworkingCenterValidator(IGeocodingService geo)
    {
        // all your normal NotEmpty() + length checks…
        //RuleFor(x => x)
        //    .MustAsync(async (dto, ct) =>
        //        (await geo.GeocodeAsync(
        //            dto.StreetAddress,
        //            dto.District,
        //            dto.City,
        //            dto.PostalCode,
        //            dto.Country)
        //        ) is not null
        //    )
        //    .WithMessage("Address could not be validated.");
    }
}