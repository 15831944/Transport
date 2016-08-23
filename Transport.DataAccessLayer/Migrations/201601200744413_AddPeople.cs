namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPeople : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Areas", "People", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Areas", "People");
        }
    }
}
