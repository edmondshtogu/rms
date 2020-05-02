using RMS.Core;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace RMS.Persistence.Ef.DataProviders
{
    public class SqlCeDataProvider : IDataProvider
    {

        public void InitializeDatabase()
        {
            var connectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            //TODO fix compilation warning (below)
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;

            var initializer = new CreateCeDatabaseIfNotExists<EfObjectContext>();
            Database.SetInitializer(initializer);
        }
        
        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }
    }
}
