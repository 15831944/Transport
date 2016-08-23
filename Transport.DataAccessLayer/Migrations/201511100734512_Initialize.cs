namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Long(nullable: false, identity: true),
                        Street = c.String(),
                        Number = c.String(),
                        BuildingId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Buildings", t => t.BuildingId, cascadeDelete: true)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "dbo.Buildings",
                c => new
                    {
                        BuildingId = c.Long(nullable: false, identity: true),
                        Levels = c.Int(nullable: false),
                        Location = c.Geography(),
                        Purpose = c.String(),
                        PostIndex = c.String(),
                        Square = c.Double(nullable: false),
                        Polygon = c.Geography(),
                    })
                .PrimaryKey(t => t.BuildingId);
            
            CreateTable(
                "dbo.OrgFils",
                c => new
                    {
                        OrgFilId = c.Long(nullable: false, identity: true),
                        OrgId = c.Long(nullable: false),
                        BuildingId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.OrgFilId)
                .ForeignKey("dbo.Buildings", t => t.BuildingId, cascadeDelete: true)
                .ForeignKey("dbo.Orgs", t => t.OrgId, cascadeDelete: true)
                .Index(t => t.OrgId)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "dbo.Orgs",
                c => new
                    {
                        OrgId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.OrgId);
            
            CreateTable(
                "dbo.OrgRubs",
                c => new
                    {
                        OrgRubId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ParentOrgRubId = c.Long(),
                    })
                .PrimaryKey(t => t.OrgRubId)
                .ForeignKey("dbo.OrgRubs", t => t.ParentOrgRubId)
                .Index(t => t.ParentOrgRubId);
            
            CreateTable(
                "dbo.Busstations",
                c => new
                    {
                        BusstationId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.BusstationId);
            
            CreateTable(
                "dbo.Busstops",
                c => new
                    {
                        BusstopId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.Geography(),
                        BusstationId = c.Long(),
                    })
                .PrimaryKey(t => t.BusstopId)
                .ForeignKey("dbo.Busstations", t => t.BusstationId)
                .Index(t => t.BusstationId);
            
            CreateTable(
                "dbo.OrgRubOrgs",
                c => new
                    {
                        OrgRub_OrgRubId = c.Long(nullable: false),
                        Org_OrgId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrgRub_OrgRubId, t.Org_OrgId })
                .ForeignKey("dbo.OrgRubs", t => t.OrgRub_OrgRubId, cascadeDelete: true)
                .ForeignKey("dbo.Orgs", t => t.Org_OrgId, cascadeDelete: true)
                .Index(t => t.OrgRub_OrgRubId)
                .Index(t => t.Org_OrgId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Busstops", "BusstationId", "dbo.Busstations");
            DropForeignKey("dbo.OrgRubs", "ParentOrgRubId", "dbo.OrgRubs");
            DropForeignKey("dbo.OrgRubOrgs", "Org_OrgId", "dbo.Orgs");
            DropForeignKey("dbo.OrgRubOrgs", "OrgRub_OrgRubId", "dbo.OrgRubs");
            DropForeignKey("dbo.OrgFils", "OrgId", "dbo.Orgs");
            DropForeignKey("dbo.OrgFils", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.Addresses", "BuildingId", "dbo.Buildings");
            DropIndex("dbo.OrgRubOrgs", new[] { "Org_OrgId" });
            DropIndex("dbo.OrgRubOrgs", new[] { "OrgRub_OrgRubId" });
            DropIndex("dbo.Busstops", new[] { "BusstationId" });
            DropIndex("dbo.OrgRubs", new[] { "ParentOrgRubId" });
            DropIndex("dbo.OrgFils", new[] { "BuildingId" });
            DropIndex("dbo.OrgFils", new[] { "OrgId" });
            DropIndex("dbo.Addresses", new[] { "BuildingId" });
            DropTable("dbo.OrgRubOrgs");
            DropTable("dbo.Busstops");
            DropTable("dbo.Busstations");
            DropTable("dbo.OrgRubs");
            DropTable("dbo.Orgs");
            DropTable("dbo.OrgFils");
            DropTable("dbo.Buildings");
            DropTable("dbo.Addresses");
        }
    }
}
