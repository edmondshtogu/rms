namespace RMS.Persistence.Ef.Migrations
{
    using RMS.Core.Domain.RequestBC;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RMS.Persistence.Ef.EfObjectContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RMS.Persistence.Ef.EfObjectContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            var requestStatuses = new List<RequestStatus>
            {
                RequestStatus.Create(Guid.NewGuid(), "Registered","Request Registered").Value,
                RequestStatus.Create(Guid.NewGuid(), "InProcess","Request In Process").Value,
                RequestStatus.Create(Guid.NewGuid(), "Completed","Request Completed").Value
            };
            requestStatuses.ForEach(s => context.Set<RequestStatus>().AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
        }
    }
}
