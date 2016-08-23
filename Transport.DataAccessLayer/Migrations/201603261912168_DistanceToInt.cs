namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistanceToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AreaRoutes", "Distance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AreaRoutes", "Distance", c => c.Long(nullable: false));
        }
    }
}
