namespace SistemaPagos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClienteDetalles",
                c => new
                    {
                        detalleClienteID = c.Int(nullable: false, identity: true),
                        ClienteID = c.Int(nullable: false),
                        Nombre = c.String(),
                        Parentezco = c.String(),
                        OtrosDatos = c.String(),
                    })
                .PrimaryKey(t => t.detalleClienteID)
                .ForeignKey("dbo.Clientes", t => t.ClienteID, cascadeDelete: true)
                .Index(t => t.ClienteID);
            
            CreateTable(
                "dbo.Clientes",
                c => new
                    {
                        ClienteId = c.Int(nullable: false, identity: true),
                        ClienteCodigo = c.String(),
                        SucursalId = c.Int(nullable: false),
                        PlanId = c.Int(nullable: false),
                        Nombre = c.String(),
                        Cedula = c.String(),
                        Direccion = c.String(),
                        Telefono = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        ProductFactura_productoId = c.Int(),
                    })
                .PrimaryKey(t => t.ClienteId)
                .ForeignKey("dbo.Products", t => t.ProductFactura_productoId)
                .ForeignKey("dbo.Plans", t => t.PlanId, cascadeDelete: true)
                .ForeignKey("dbo.Sucursals", t => t.SucursalId, cascadeDelete: true)
                .Index(t => t.SucursalId)
                .Index(t => t.PlanId)
                .Index(t => t.ProductFactura_productoId);
            
            CreateTable(
                "dbo.Facturas",
                c => new
                    {
                        FacturaId = c.Int(nullable: false, identity: true),
                        FechaFactura = c.DateTime(nullable: false),
                        ClienteId = c.Int(nullable: false),
                        FacEstatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FacturaId)
                .ForeignKey("dbo.Clientes", t => t.ClienteId, cascadeDelete: true)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "dbo.DetalleFacturas",
                c => new
                    {
                        DetalleFacturaId = c.Int(nullable: false, identity: true),
                        FacturaId = c.Int(nullable: false),
                        productoId = c.Int(nullable: false),
                        descripcion = c.String(),
                        precio = c.Single(nullable: false),
                        cantidad = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.DetalleFacturaId)
                .ForeignKey("dbo.Facturas", t => t.FacturaId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.productoId, cascadeDelete: true)
                .Index(t => t.FacturaId)
                .Index(t => t.productoId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        productoId = c.Int(nullable: false, identity: true),
                        descripcion = c.String(),
                        precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fecha = c.DateTime(nullable: false),
                        stock = c.Single(nullable: false),
                        cantidad = c.Single(),
                        ClienteId = c.Int(),
                        PlanId = c.Int(),
                        precioo = c.Decimal(precision: 18, scale: 2),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.productoId);
            
            CreateTable(
                "dbo.DetallePagoes",
                c => new
                    {
                        DetallePagoId = c.Int(nullable: false, identity: true),
                        PagoId = c.Int(nullable: false),
                        productoId = c.Int(nullable: false),
                        facturaId = c.Int(nullable: false),
                        DetalleFacturaId = c.Int(nullable: false),
                        descripcion = c.String(),
                        cantidad = c.Single(nullable: false),
                        precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DetallePagoId)
                .ForeignKey("dbo.Pagoes", t => t.PagoId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.productoId, cascadeDelete: true)
                .Index(t => t.PagoId)
                .Index(t => t.productoId);
            
            CreateTable(
                "dbo.Pagoes",
                c => new
                    {
                        PagoId = c.Int(nullable: false, identity: true),
                        FacturaId = c.Int(nullable: false),
                        ClienteId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Comentario = c.String(),
                        Tipo_TipoId = c.Int(),
                    })
                .PrimaryKey(t => t.PagoId)
                .ForeignKey("dbo.Clientes", t => t.ClienteId, cascadeDelete: true)
                .ForeignKey("dbo.Tipoes", t => t.Tipo_TipoId)
                .Index(t => t.ClienteId)
                .Index(t => t.Tipo_TipoId);
            
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        PlanId = c.Int(nullable: false, identity: true),
                        descripcion = c.String(),
                        precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PlanId);
            
            CreateTable(
                "dbo.Sucursals",
                c => new
                    {
                        SucursalId = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Direccion = c.String(),
                        Telefono = c.String(),
                    })
                .PrimaryKey(t => t.SucursalId);
            
            CreateTable(
                "dbo.Tipoes",
                c => new
                    {
                        TipoId = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.TipoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pagoes", "Tipo_TipoId", "dbo.Tipoes");
            DropForeignKey("dbo.ClienteDetalles", "ClienteID", "dbo.Clientes");
            DropForeignKey("dbo.Clientes", "SucursalId", "dbo.Sucursals");
            DropForeignKey("dbo.Clientes", "PlanId", "dbo.Plans");
            DropForeignKey("dbo.Clientes", "ProductFactura_productoId", "dbo.Products");
            DropForeignKey("dbo.DetallePagoes", "productoId", "dbo.Products");
            DropForeignKey("dbo.DetallePagoes", "PagoId", "dbo.Pagoes");
            DropForeignKey("dbo.Pagoes", "ClienteId", "dbo.Clientes");
            DropForeignKey("dbo.DetalleFacturas", "productoId", "dbo.Products");
            DropForeignKey("dbo.DetalleFacturas", "FacturaId", "dbo.Facturas");
            DropForeignKey("dbo.Facturas", "ClienteId", "dbo.Clientes");
            DropIndex("dbo.Pagoes", new[] { "Tipo_TipoId" });
            DropIndex("dbo.Pagoes", new[] { "ClienteId" });
            DropIndex("dbo.DetallePagoes", new[] { "productoId" });
            DropIndex("dbo.DetallePagoes", new[] { "PagoId" });
            DropIndex("dbo.DetalleFacturas", new[] { "productoId" });
            DropIndex("dbo.DetalleFacturas", new[] { "FacturaId" });
            DropIndex("dbo.Facturas", new[] { "ClienteId" });
            DropIndex("dbo.Clientes", new[] { "ProductFactura_productoId" });
            DropIndex("dbo.Clientes", new[] { "PlanId" });
            DropIndex("dbo.Clientes", new[] { "SucursalId" });
            DropIndex("dbo.ClienteDetalles", new[] { "ClienteID" });
            DropTable("dbo.Tipoes");
            DropTable("dbo.Sucursals");
            DropTable("dbo.Plans");
            DropTable("dbo.Pagoes");
            DropTable("dbo.DetallePagoes");
            DropTable("dbo.Products");
            DropTable("dbo.DetalleFacturas");
            DropTable("dbo.Facturas");
            DropTable("dbo.Clientes");
            DropTable("dbo.ClienteDetalles");
        }
    }
}
