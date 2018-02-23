namespace crousseau2_College_Strike.DAL.StrikeMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManyToMany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Position",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        PositionTitle = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PositionMember",
                c => new
                    {
                        Position_id = c.Int(nullable: false),
                        Member_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Position_id, t.Member_id })
                .ForeignKey("dbo.Position", t => t.Position_id, cascadeDelete: true)
                .ForeignKey("dbo.Member", t => t.Member_id, cascadeDelete: true)
                .Index(t => t.Position_id)
                .Index(t => t.Member_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PositionMember", "Member_id", "dbo.Member");
            DropForeignKey("dbo.PositionMember", "Position_id", "dbo.Position");
            DropIndex("dbo.PositionMember", new[] { "Member_id" });
            DropIndex("dbo.PositionMember", new[] { "Position_id" });
            DropTable("dbo.PositionMember");
            DropTable("dbo.Position");
        }
    }
}
