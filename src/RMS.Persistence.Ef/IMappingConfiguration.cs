using System.Data.Entity;

namespace RMS.Persistence.Ef
{
    /// <summary>
    /// Represents database context model mapping configuration
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Add this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        void AddConfiguration(DbModelBuilder modelBuilder);
    }
}
