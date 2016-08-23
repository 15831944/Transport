namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDbGenForAreaId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Areas", "AreaId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Areas", "AreaId", c => c.Long(nullable: false, identity: true));
        }
    }
}
