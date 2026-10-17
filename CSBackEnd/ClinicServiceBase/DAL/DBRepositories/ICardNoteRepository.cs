using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface ICardNoteRepository : IClinicServiceRepository<CardNote>
    {
        Task AddCardNote(CardNote cardNote);

        Task EditCardNote(CardNote cardNote);

        Task<CardNote[]> GetCardNotes(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteCardNote(Guid id, CancellationToken cancellationToken);
    }
}
