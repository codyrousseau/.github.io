namespace crousseau2_College_Strike.DAL.StrikeMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitalCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignment",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        AssignmentName = c.String(nullable: false, maxLength: 500),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.AssignmentName, unique: true, name: "IX_Unique_Assignment");
            
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Phone = c.Long(nullable: false),
                        eMail = c.String(nullable: false, maxLength: 255),
                        AssignmentID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Assignment", t => t.AssignmentID)
                .Index(t => t.eMail, unique: true, name: "IX_Unique_Member_email")
                .Index(t => t.AssignmentID);
            
            CreateTable(
                "dbo.Shift",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ShiftDate = c.DateTime(nullable: false),
                        MemberID = c.Int(nullable: false),
                        AssignmentID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Assignment", t => t.AssignmentID)
                .ForeignKey("dbo.Member", t => t.MemberID)
                .Index(t => new { t.MemberID, t.ShiftDate }, unique: true, name: "IX_Unique_Shift")
                .Index(t => t.AssignmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shift", "MemberID", "dbo.Member");
            DropForeignKey("dbo.Shift", "AssignmentID", "dbo.Assignment");
            DropForeignKey("dbo.Member", "AssignmentID", "dbo.Assignment");
            DropIndex("dbo.Shift", new[] { "AssignmentID" });
            DropIndex("dbo.Shift", "IX_Unique_Shift");
            DropIndex("dbo.Member", new[] { "AssignmentID" });
            DropIndex("dbo.Member", "IX_Unique_Member_email");
            DropIndex("dbo.Assignment", "IX_Unique_Assignment");
            DropTable("dbo.Shift");
            DropTable("dbo.Member");
            DropTable("dbo.Assignment");
        }
    }
}
