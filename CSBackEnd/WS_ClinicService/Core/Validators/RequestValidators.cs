using FluentValidation;
using WS_ClinicService.Contracts.Requests;

namespace WS_ClinicService.Core.Validators
{
    public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
    {
        public CreatePatientRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.BirthDate).NotEqual(default(DateTimeOffset)).LessThan(DateTimeOffset.UtcNow);
            RuleFor(x => x.PhoneNumber).MaximumLength(50);
            RuleFor(x => x.BloodGroup).MaximumLength(50);
        }
    }

    public class CreateMedicalCardRequestValidator : AbstractValidator<CreateMedicalCardRequest>
    {
        public CreateMedicalCardRequestValidator()
        {
            RuleFor(x => x.Patient).NotEqual(Guid.Empty);
            RuleFor(x => x.Policy).NotEqual(Guid.Empty);
        }
    }

    public class CreatePolicyRequestValidator : AbstractValidator<CreatePolicyRequest>
    {
        public CreatePolicyRequestValidator()
        {
            RuleFor(x => x.MedicalPolicyNumber).NotEmpty();
            RuleFor(x => x.InsuranceProvider).NotEqual(Guid.Empty);
        }
    }

    public class CreateInsuranceProviderRequestValidator : AbstractValidator<CreateInsuranceProviderRequest>
    {
        public CreateInsuranceProviderRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LicenseNumber).NotEmpty();
        }
    }

    public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
    {
        public CreateDoctorRequestValidator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty);
            RuleFor(x => x.Specialisations).NotNull().NotEmpty();
            RuleFor(x => x.DoctorWorkStatus).IsInEnum();
        }
    }

    public class CreateSpecialisationRequestValidator : AbstractValidator<CreateSpecialisationRequest>
    {
        public CreateSpecialisationRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class CreateDiagnosisRequestValidator : AbstractValidator<CreateDiagnosisRequest>
    {
        public CreateDiagnosisRequestValidator()
        {
            RuleFor(x => x.IcdCode).NotEmpty();
            RuleFor(x => x.Doctor).NotEqual(Guid.Empty);
            RuleFor(x => x.MedicalCard).NotEqual(Guid.Empty);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        }
    }

    public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
    {
        public CreateAppointmentRequestValidator()
        {
            RuleFor(x => x.Patient).NotEqual(Guid.Empty);
            RuleFor(x => x.MedicalCard).NotEqual(Guid.Empty);
            RuleFor(x => x.Doctor).NotEqual(Guid.Empty);
            RuleFor(x => x.CreatedBy).NotEqual(Guid.Empty);
            RuleFor(x => x.AppointmentDateTime).NotEqual(default(DateTimeOffset));
        }
    }

    public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
    {
        public UpdateAppointmentRequestValidator()
        {
            RuleFor(x => x.EditedBy).NotEqual(Guid.Empty);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        }
    }

    public class CreateScheduleRequestValidator : AbstractValidator<CreateScheduleRequest>
    {
        public CreateScheduleRequestValidator()
        {
            RuleFor(x => x.Doctor).NotEqual(Guid.Empty);
        }
    }

    public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
    {
        public CreatePersonRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Login).NotEmpty();
        }
    }

    public class AddAppointmentSlipRequestValidator : AbstractValidator<AddAppointmentSlipRequest>
    {
        public AddAppointmentSlipRequestValidator()
        {
            RuleFor(x => x.ScheduleId).NotEqual(Guid.Empty);
            RuleFor(x => x.AppointmentDateTime).NotEqual(default(DateTimeOffset));
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}