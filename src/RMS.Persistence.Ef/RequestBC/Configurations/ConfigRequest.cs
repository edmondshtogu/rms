using RMS.Core.Domain.RequestBC;

namespace RMS.Persistence.Ef.RequestBC.Configurations
{
    internal class ConfigRequest : EfEntityTypeConfiguration<Request>
    {
        public override void PostConfigure()
        {
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(300).IsRequired();
            Property(x => x.Description).HasMaxLength(500);
                        
            //relationship
            HasRequired(x => x.Status)
                .WithMany(s => s.Requests)
                .HasForeignKey(s => s.StatusId)
                .WillCascadeOnDelete(false);

            HasIndex(x => new { x.Name, x.RaisedDate }).IsUnique();
        }
    }
}
