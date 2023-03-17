namespace GirlyMerely.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HolidyDate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HolidyDates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HolidyDates = c.String(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HolidyDates");
        }
    }
}
