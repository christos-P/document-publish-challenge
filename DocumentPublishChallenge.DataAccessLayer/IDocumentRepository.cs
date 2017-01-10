using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentPublishChallenge.Domain;

namespace DocumentPublishChallenge.DataAccessLayer
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentEntity>> GetUserDocuments(int userId);
    }
}