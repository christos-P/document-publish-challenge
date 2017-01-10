using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DocumentPublishChallenge.Domain;

namespace DocumentPublishChallenge.DataAccessLayer
{
    public class DocumentContext : BaseContext, IDocumentRepository
    {
        public async Task<int> InsertDocument(DocumentEntity document)
        {
            await DbConnection.OpenAsync();
            const string sql = "insert_document";
            var sqlParams = new
            {
                id = document.Id.ToString("D"),
                link = document.Link,
                user_id = document.UserId,
                name = document.Name
            };
            return
                await
                    DbConnection.QueryFirstOrDefaultAsync<int>(sql, sqlParams,
                        commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<DocumentEntity>> GetUserDocuments(int userId)
        {
            await DbConnection.OpenAsync();
            const string sql = "get_user_documents";
            var sqlParams = new
            {
                user_id = userId
            };
            return
                await
                    DbConnection.QueryAsync<DocumentEntity>(sql, sqlParams, commandType: CommandType.StoredProcedure);
        }
    }
}