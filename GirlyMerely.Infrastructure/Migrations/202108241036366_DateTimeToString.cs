namespace GirlyMerely.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateTimeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoices", "ShipingDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoices", "ShipingDate", c => c.DateTime(nullable: false));
        }
    }
}
