namespace RMS.Persistence.Ef.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelationshipUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requests", "StatusId", "dbo.RequestStatuses");
            AddForeignKey("dbo.Requests", "StatusId", "dbo.RequestStatuses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "StatusId", "dbo.RequestStatuses");
            AddForeignKey("dbo.Requests", "StatusId", "dbo.RequestStatuses", "Id", cascadeDelete: true);
        }
    }
}
