namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BusstationRenameToArea : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Busstops", "BusstationId", "dbo.Busstations");
            DropIndex("dbo.Busstops", new[] { "BusstationId" });
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        AreaId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.AreaId);
            
            AddColumn("dbo.Buildings", "AreaId", c => c.Long());
            AddColumn("dbo.Busstops", "Area_AreaId", c => c.Long());
            CreateIndex("dbo.Buildings", "AreaId");
            CreateIndex("dbo.Busstops", "Area_AreaId");
            AddForeignKey("dbo.Buildings", "AreaId", "dbo.Areas", "AreaId");
            AddForeignKey("dbo.Busstops", "Area_AreaId", "dbo.Areas", "AreaId");
            DropTable("dbo.Busstations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Busstations",
                c => new
                    {
                        BusstationId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.BusstationId);
            
            DropForeignKey("dbo.Busstops", "Area_AreaId", "dbo.Areas");
            DropForeignKey("dbo.Buildings", "AreaId", "dbo.Areas");
            DropIndex("dbo.Busstops", new[] { "Area_AreaId" });
            DropIndex("dbo.Buildings", new[] { "AreaId" });
            DropColumn("dbo.Busstops", "Area_AreaId");
            DropColumn("dbo.Buildings", "AreaId");
            DropTable("dbo.Areas");
            CreateIndex("dbo.Busstops", "BusstationId");
            AddForeignKey("dbo.Busstops", "BusstationId", "dbo.Busstations", "BusstationId");
        }
    }
}
