using ClinicServiceContext.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IMedicalCardSnapshotRepository : IClinicServiceRepository<MedicalCardSnapshot>
    {
        Task AddMedicalCard(MedicalCardSnapshot medicalCard);

        Task EditMedicalCard(MedicalCardSnapshot medicalCard);

        Task<MedicalCardSnapshot[]> GetMedicalCards(Guid[] Ids, CancellationToken cancellationToken);

        Task DeleteMedicalCard(Guid Id, CancellationToken cancellationToken);
    }
}
