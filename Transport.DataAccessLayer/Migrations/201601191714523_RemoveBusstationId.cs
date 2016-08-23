namespace Transport.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBusstationId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Busstops", name: "Area_AreaId", newName: "AreaId");
            RenameIndex(table: "dbo.Busstops", name: "IX_Area_AreaId", newName: "IX_AreaId");
            DropColumn("dbo.Busstops", "BusstationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Busstops", "BusstationId", c => c.Long());
            RenameIndex(table: "dbo.Busstops", name: "IX_AreaId", newName: "IX_Area_AreaId");
            RenameColumn(table: "dbo.Busstops", name: "AreaId", newName: "Area_AreaId");
        }
    }
}
