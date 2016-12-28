using System;
using Npgsql;
using static System.Configuration.ConfigurationManager;

namespace DocumentPublishChallenge.DataAccessLayer
{
    /// <summary>
    /// Base class used from context classes used to access the Database.
    /// </summary>
    public class BaseContext : IDisposable
    {
        private static readonly string ConnectionString = ConnectionStrings["eTravelDB"].ConnectionString;

        protected NpgsqlConnection DbConnection = new NpgsqlConnection(ConnectionString);

        public void Dispose() => DbConnection?.Dispose();
    }
}