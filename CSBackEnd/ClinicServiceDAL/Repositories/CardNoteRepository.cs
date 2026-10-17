using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class CardNoteRepository : RepositoryBase<CardNote>, ICardNoteRepository
    {
        public CardNoteRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddCardNote(CardNote cardNote)
        {
            return AddObject(cardNote);
        }

        public Task EditCardNote(CardNote cardNote)
        {
            return UpdateObject(cardNote);
        }

        public Task<CardNote[]> GetCardNotes(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteCardNote(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
