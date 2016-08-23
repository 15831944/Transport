namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaRoutes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AreaRoutes",
                c => new
                    {
                        OriginId = c.Long(nullable: false),
                        DestinationId = c.Long(nullable: false),
                        Distance = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.OriginId, t.DestinationId })
                .ForeignKey("dbo.Areas", t => t.DestinationId, cascadeDelete: false)
                .ForeignKey("dbo.Areas", t => t.OriginId, cascadeDelete: false)
                .Index(t => t.OriginId)
                .Index(t => t.DestinationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaRoutes", "OriginId", "dbo.Areas");
            DropForeignKey("dbo.AreaRoutes", "DestinationId", "dbo.Areas");
            DropIndex("dbo.AreaRoutes", new[] { "DestinationId" });
            DropIndex("dbo.AreaRoutes", new[] { "OriginId" });
            DropTable("dbo.AreaRoutes");
        }
    }
}
