namespace RMS.Persistence.Ef.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestStatuses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        Description = c.String(maxLength: 500),
                        StatusId = c.Guid(nullable: false),
                        RaisedDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Attachments = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RequestStatuses", t => t.StatusId, cascadeDelete: true)
                .Index(t => new { t.Name, t.RaisedDate }, unique: true)
                .Index(t => t.StatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "StatusId", "dbo.RequestStatuses");
            DropIndex("dbo.Requests", new[] { "StatusId" });
            DropIndex("dbo.Requests", new[] { "Name", "RaisedDate" });
            DropIndex("dbo.RequestStatuses", new[] { "Name" });
            DropTable("dbo.Requests");
            DropTable("dbo.RequestStatuses");
        }
    }
}
