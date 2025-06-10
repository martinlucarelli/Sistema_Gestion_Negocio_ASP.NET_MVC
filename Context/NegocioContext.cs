using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Context
{
    public class NegocioContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Negocio>Negocios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas{ get; set; }
        public DbSet<DetalleVenta> DetalleVentas{ get; set; }
        public DbSet<Rubro> Rubros{ get; set; }
        public DbSet<FormaPago> FormasPago { get; set; }


        public NegocioContext(DbContextOptions<NegocioContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(usuario =>
            {
                usuario.ToTable("Usuario");
                //IdUsuario
                usuario.HasKey(u => u.IdUsuario);
                //Correo
                usuario.Property(u => u.Correo).IsRequired();
                //Clave
                usuario.Property(u=> u.Clave).IsRequired();
                //Nombre
                usuario.Property(u=> u.Nombre).IsRequired();
                //Confirmado
                usuario.Property(u => u.Confirmado).HasDefaultValue(false);
                //Token
                usuario.Property(u=> u.TokenConfirmacion).IsRequired(false);
                //Rol
                usuario.Property(u => u.Rol).IsRequired();
                usuario.Property(u => u.Rol).HasConversion<int>(); //Convierte el enum en entero para que lo entienda EF
                //NegocioID (FK)
                usuario.HasOne(u=> u.Negocio).WithMany(neg=> neg.Usuarios).HasForeignKey(u=> u.NegocioId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Negocio>(negocio =>
            {
                negocio.ToTable("Negocio");
                // idNegocio
                negocio.HasKey(n=> n.IdNegocio);
                //Nombre
                negocio.Property(n => n.Nombre).IsRequired();
                //Direccion
                negocio.Property(n=> n.Direccion).IsRequired(false);
                //Rubro (FK)
                negocio.HasOne(n=> n.Rubro).WithMany(rub=> rub.negocios).HasForeignKey(n=>n.RubroId);
            
            });

            modelBuilder.Entity<Producto>(producto => 
            {
                producto.ToTable("Producto");
                //idProducto
                producto.HasKey(p => p.IdProducto);
                //Nombre
                producto.Property(p=> p.Nombre).IsRequired().HasMaxLength(150);
                //Precio
                producto.Property(p => p.Precio).IsRequired();
                //Stock
                producto.Property(p => p.Stock).IsRequired();
                //negocioId (FK)
                producto.HasOne(p=> p.Negocio).WithMany(neg=>neg.Productos).HasForeignKey(p=>p.NegocioId).OnDelete(DeleteBehavior.Restrict);
            
            });

            modelBuilder.Entity<Venta>(venta =>
            {
                venta.ToTable("Venta");
                //IdVenta
                venta.HasKey(v => v.IdVenta);
                //Fecha
                venta.Property(v=> v.Fecha).IsRequired();
                //Total
                venta.Property(v => v.Total).IsRequired();
                //FormaPagoId (FK)
                venta.HasOne(v=> v.FormaPago).WithMany(formPag=> formPag.ventas).HasForeignKey(v=> v.FormaPagoId);
                //UsuarioId (FK)
                venta.HasOne(v => v.Usuario).WithMany(user => user.ventas).HasForeignKey(v => v.UsuarioId).OnDelete(DeleteBehavior.SetNull); //DeleteBehavior asegura que si se ellimina
                //NegocioId (FK)                                                                                                            //un usuario, las ventas de ese usuario seguiran con usuarioId = null.
                venta.HasOne(v => v.Negocio).WithMany(neg => neg.ventas).HasForeignKey(v => v.NegocioId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<DetalleVenta>(detVent =>
            {
                detVent.ToTable("Detalle_Ventas");
                //IdDetalleVenta
                detVent.HasKey(detVent=> detVent.IdDetalleVenta);
                //CantidadProductos
                detVent.Property(detVent => detVent.CantidadProductos).IsRequired();
                //Subtotal
                detVent.Property(detVent => detVent.Subtotal).IsRequired();
                //VentaId (FK)
                detVent.HasOne(detVent=>detVent.venta).WithMany(vent=>vent.detalleVentas).HasForeignKey(detVent=> detVent.VentaId);
                //ProductoId (FK)                                                                                               DeleteBeHavior permite que si se elimina un producto, la venta siga registrada con idPorducto = null
                detVent.HasOne(detVent => detVent.producto).WithMany(prod => prod.detalleVentas).HasForeignKey(detVent => detVent.ProductoId).OnDelete(DeleteBehavior.SetNull);

            });

            modelBuilder.Entity<Rubro>(r =>
            {
                r.ToTable("Rubro");
                //IdRubro
                r.HasKey(r=> r.IdRubro);
                r.Property(r=>r.IdRubro).UseIdentityColumn(seed:1000,increment: 1); //Arranca en 1000 y se autoincrementa de a uno
                //Nombre
                r.Property(r=> r.Nombre).IsRequired();
            });

            modelBuilder.Entity<FormaPago>(fp =>
            {
                fp.ToTable("Forma_Pago");
                //IdFormaPago
                fp.HasKey(fp => fp.IdFormaPago);
                fp.Property(fp => fp.IdFormaPago).UseIdentityColumn(seed: 100, increment: 1); //Arranca en 1000 y se autoincrementa de a uno
                //TipoFormaPago
                fp.Property(fp => fp.TipoFormaPago).IsRequired();
            });




        }





    }
}
