using RMS.Core.Domain;
using Inflector;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity;

namespace RMS.Persistence.Ef
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class EfEntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity>, IMappingConfiguration
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Initializes the <see cref="EfEntityTypeConfiguration{TEntity}"/> class.
        /// </summary>
        protected EfEntityTypeConfiguration()
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => new CultureInfo("en-us");

            string tblName = typeof(TEntity).Name;
            var customTableAttribute = typeof(TEntity).GetCustomAttributes(false);
            if (customTableAttribute.Length > 0)
                tblName = ((TableAttribute)customTableAttribute[0]).Name;

            ToTable(tblName.Pluralize());
            PostConfigure();
        }

        /// <summary>
        /// Developers should override this method in custom classes in order to apply their custom configuration code
        /// </summary>
        public abstract void PostConfigure();

        /// <summary>
        /// Add this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public void AddConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(this);
        }
    }
}
