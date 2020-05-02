using RMS.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace RMS.Persistence.Ef.DataProviders
{
    /// <summary>
    /// Represents SQL Server data provider
    /// </summary>
    public class SqlServerDataProvider : IDataProvider
    {
        #region Utilities

        protected virtual string[] ParseCommands(IAppFileProvider fileProvider, string filePath, bool throwExceptionIfNonExists)
        {
            if (!fileProvider.FileExists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));

                return Array.Empty<string>();
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitializeDatabase()
        {
            var fileProvider = EngineContext.Current.Resolve<IAppFileProvider>();

            var connectionFactory = new SqlConnectionFactory();
            //TODO fix compilation warning (below)
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;

            var tablesToValidate = new[] { "Requests", "RequestStatuses" };

            var customCommands = new List<string>();
            customCommands.AddRange(ParseCommands(fileProvider, fileProvider.MapPath(SqlServerIndexesFilePath), false));
            customCommands.AddRange(ParseCommands(fileProvider, fileProvider.MapPath(SqlServerStoredProceduresFilePath), false));

            var initializer = new CreateTablesIfNotExist<EfObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        /// <summary>
        /// Get a support database parameter object (used by Stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Gets a path to the file that contains script to create SQL Server indexes
        /// </summary>
        protected virtual string SqlServerIndexesFilePath => "~/App_Data/Install/SqlServer.Indexes.sql";

        /// <summary>
        /// Gets a path to the file that contains script to create SQL Server Stored procedures
        /// </summary>
        protected virtual string SqlServerStoredProceduresFilePath => "~/App_Data/Install/SqlServer.StoredProcedures.sql";

        protected virtual string SqlServerScriptUpgradePath => "~/App_Data/Upgrade";
    }
}
