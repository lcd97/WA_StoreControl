using ModelosDB.General;
using ModelosDB.Inventario;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WA_StoreControl.Models
{
    public class DBStore : DbContext
    {
        public DBStore() : base("DBStore")
        {
            Database.SetInitializer<DBStore>(new CreateDatabaseIfNotExists<DBStore>());
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        //MODULO INVENTARIO
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<SubCategoria> SubCategorias { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Entrada> Entradas { get; set; }
        public virtual DbSet<DetalleEntrada> DetalleEntrada { get; set; }

        //GENERAL
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }
        public virtual DbSet<CompaniaTelefonica> CompaniasTelefonica { get; set; }
        public virtual DbSet<DetalleTelefono> DetallesTelefono { get; set; }
        public virtual DbSet<Identidad> Identidades { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}