namespace SistemaPagos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "UserName");
        }
    }
}
