using RMS.Core.Domain.RequestBC;

namespace RMS.Persistence.Ef.RequestBC.Configurations
{
    internal class ConfigRequestStatus : EfEntityTypeConfiguration<RequestStatus>
    {
        public override void PostConfigure()
        {
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(300).IsRequired();
            Property(x => x.Description).HasMaxLength(500);

            HasIndex(x => x.Name).IsUnique();
        }
    }
}
