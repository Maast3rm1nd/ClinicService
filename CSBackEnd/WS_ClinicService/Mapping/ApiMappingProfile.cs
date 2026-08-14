using AutoMapper;
using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using WS_ClinicService.Contracts.Requests;

namespace WS_ClinicService.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<CreatePatientRequest, PatientSnapshot>();
            CreateMap<PatientSnapshot, Contracts.Responses.PatientSnapshot>();
            CreateMap<Contracts.Responses.PatientSnapshot, PatientSnapshot>();

            CreateMap<CreateMedicalCardRequest, MedicalCardSnapshot>()
                .ForMember(d => d.RecordNumber, o => o.MapFrom(_ => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
            CreateMap<MedicalCardSnapshot, Contracts.Responses.MedicalCardSnapshot>()
                .ForMember(d => d.Policy, o => o.MapFrom(s => s.Policy ?? Guid.Empty));
            CreateMap<Contracts.Responses.MedicalCardSnapshot, MedicalCardSnapshot>();

            CreateMap<CreatePolicyRequest, PolicySnapshot>()
                .ForMember(d => d.MedicalPolicyType, o => o.MapFrom(s => s.MedicalPolicyType ?? PolicyType.EHIC));
            CreateMap<PolicySnapshot, Contracts.Responses.PolicySnapshot>();
            CreateMap<Contracts.Responses.PolicySnapshot, PolicySnapshot>();

            CreateMap<CreateInsuranceProviderRequest, InsuranceProviderSnapshot>();
            CreateMap<InsuranceProviderSnapshot, Contracts.Responses.InsuranceProviderSnapshot>();
            CreateMap<Contracts.Responses.InsuranceProviderSnapshot, InsuranceProviderSnapshot>();

            CreateMap<CreateDoctorRequest, Doctor>();
            CreateMap<Doctor, Contracts.Responses.Doctors>();
            CreateMap<Contracts.Responses.Doctors, Doctor>();

            CreateMap<CreateSpecialisationRequest, SpecialisationSnapshot>();
            CreateMap<SpecialisationSnapshot, Contracts.Responses.SpecialisationSnapshot>();
            CreateMap<Contracts.Responses.SpecialisationSnapshot, SpecialisationSnapshot>();

            CreateMap<CreateDiagnosisRequest, DiagnosisSnapshot>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status ?? DiagnosisStatus.Actual));
            CreateMap<DiagnosisSnapshot, Contracts.Responses.DiagnosisSnapshot>();
            CreateMap<Contracts.Responses.DiagnosisSnapshot, DiagnosisSnapshot>();

            CreateMap<CreateAppointmentRequest, AppointmentSnapshot>()
                .ForMember(d => d.PremilinaryReason, o => o.MapFrom(s => s.PreliminaryReason));
            CreateMap<AppointmentSnapshot, Contracts.Responses.AppointmentSnapshot>()
                .ForMember(d => d.PreliminaryReason, o => o.MapFrom(s => s.PremilinaryReason));
            CreateMap<Contracts.Responses.AppointmentSnapshot, AppointmentSnapshot>()
                .ForMember(d => d.PremilinaryReason, o => o.MapFrom(s => s.PreliminaryReason));

            CreateMap<CreateScheduleRequest, Schedule>();
            CreateMap<Schedule, Contracts.Responses.Schedule>();
            CreateMap<Contracts.Responses.Schedule, Schedule>();

            CreateMap<CreatePersonRequest, PersonSnapshot>();
            CreateMap<PersonSnapshot, Contracts.Responses.PersonSnapshot>();
            CreateMap<Contracts.Responses.PersonSnapshot, PersonSnapshot>();
        }
    }
}