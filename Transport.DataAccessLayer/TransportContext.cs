using System.Data.Entity;
using Transport.DataAccessLayer.Migrations;
using Transport.Domain.Entities;

namespace Transport.DataAccessLayer
{
    public class TransportContext : DbContext
    {
        public TransportContext()
            : base("name=TransportContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TransportContext, Configuration>());
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Busstop> Busstops { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Org> Orgs { get; set; }
        public virtual DbSet<OrgFil> OrgFils { get; set; }
        public virtual DbSet<OrgRub> OrgRubs { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<AreaRoutes> AreaRoutes { get; set; }
    }
}