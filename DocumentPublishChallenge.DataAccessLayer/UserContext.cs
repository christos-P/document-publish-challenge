using System.Data;
using System.Threading.Tasks;
using Dapper;
using DocumentPublishChallenge.Domain;

namespace DocumentPublishChallenge.DataAccessLayer
{
    public class UserContext : BaseContext
    {
        public async Task<int> CreateUser(UserModel model)
        {
            await DbConnection.OpenAsync();
            const string sql = "insert_user";
            var sqlParams = new
            {
                email = model.Email,
                password = model.Password
            };
            return
                await
                    DbConnection.QueryFirstOrDefaultAsync<int>(sql, sqlParams, commandType: CommandType.StoredProcedure);
        }

        public async Task<string> GetUserStoredPassword(string email)
        {
            await DbConnection.OpenAsync();
            const string sql = "get_user_password_by_email";
            var sqlParams = new
            {
                email
            };
            var storedPassword =
                await
                    DbConnection.QueryFirstOrDefaultAsync<string>(sql, sqlParams,
                        commandType: CommandType.StoredProcedure);
            return storedPassword;
        }
    }
}