using AutoMapper;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using WS_ClinicService.Contracts.Requests;

namespace WS_ClinicService.Mapping
{
    public class ClinicMappingProfile : Profile
    {
        public ClinicMappingProfile()
        {
            CreateMap<CreatePatientRequest, PatientSnapshot>();
            CreateMap<PatientSnapshot, PatientSnapshotDto>();

            CreateMap<CreateMedicalCardRequest, MedicalCardSnapshot>()
                .ForMember(d => d.RecordNumber, o => o.MapFrom(_ => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
            CreateMap<MedicalCardSnapshot, MedicalCardSnapshotDto>();

            CreateMap<CreatePolicyRequest, PolicySnapshot>()
                .ForMember(d => d.MedicalPolicyType, o => o.MapFrom(s => s.MedicalPolicyType ?? PolicyType.EHIC));
            CreateMap<PolicySnapshot, PolicySnapshotDto>();

            CreateMap<CreateInsuranceProviderRequest, InsuranceProviderSnapshot>();
            CreateMap<InsuranceProviderSnapshot, InsuranceProviderSnapshotDto>();

            CreateMap<CreateDoctorRequest, Doctor>();
            CreateMap<Doctor, DoctorsDto>();
            CreateMap<Doctor, PersonSnapshotDto>();
            CreateMap<Administrator, PersonSnapshotDto>();
            CreateMap<Administrator, AdministratorsDto>();

            CreateMap<CreateSpecialisationRequest, SpecialisationSnapshot>();
            CreateMap<SpecialisationSnapshot, SpecialisationSnapshotDto>();

            CreateMap<CreateDiagnosisRequest, DiagnosisSnapshot>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status ?? DiagnosisStatus.Actual));
            CreateMap<DiagnosisSnapshot, DiagnosisSnapshotDto>();

            CreateMap<CreateAppointmentRequest, AppointmentSnapshot>()
                .ForMember(d => d.PremilinaryReason, o => o.MapFrom(s => s.PreliminaryReason))
                .ForMember(d => d.Status, o => o.MapFrom(_ => AppointmentStatus.Pending));
            CreateMap<AppointmentSnapshot, AppointmentSnapshotDto>()
                .ForMember(d => d.PreliminaryReason, o => o.MapFrom(s => s.PremilinaryReason));

            CreateMap<CreateScheduleRequest, Schedule>()
                .ForMember(d => d.Appointments, o => o.MapFrom(s => s.Appointments ?? new List<Guid>()));
            CreateMap<Schedule, ScheduleDto>();

            CreateMap<CreatePersonRequest, PersonSnapshot>();
            CreateMap<PersonSnapshot, PersonSnapshotDto>();

            CreateMap<UpdatePatientRequest, PatientSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateMedicalCardRequest, MedicalCardSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdatePolicyRequest, PolicySnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateInsuranceProviderRequest, InsuranceProviderSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDoctorRequest, Doctor>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateSpecialisationRequest, SpecialisationSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDiagnosisRequest, DiagnosisSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateAppointmentRequest, AppointmentSnapshot>()
                .ForMember(d => d.PremilinaryReason, o => o.MapFrom(s => s.PreliminaryReason))
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateScheduleRequest, Schedule>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdatePersonRequest, PersonSnapshot>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
