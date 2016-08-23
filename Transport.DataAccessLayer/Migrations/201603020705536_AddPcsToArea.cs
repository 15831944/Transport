namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPcsToArea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Areas", "DeparturePct", c => c.Double(nullable: false));
            AddColumn("dbo.Areas", "ArrivalPct", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Areas", "ArrivalPct");
            DropColumn("dbo.Areas", "DeparturePct");
        }
    }
}
