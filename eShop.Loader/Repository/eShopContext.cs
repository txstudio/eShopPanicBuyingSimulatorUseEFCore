using Microsoft.EntityFrameworkCore;

namespace eShop.Loader
{
    public class eShopContext : DbContext
    {
        //請自行變更資料庫連線字串
        private readonly string _connectionString = "server=192.168.0.80;database=eShopInMySQL;user=root;password=password";

        public DbSet<ProductMain> ProductMains { get; set; }
        public DbSet<ProductStorage> ProductStorages { get; set; }

        public DbSet<OrderMain> OrderMains { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Products

            //Products.ProductMains
            modelBuilder.Entity<ProductMain>(entity => {
                entity.ToTable("ProductMains");
                entity.HasKey(x => x.No);
                entity.HasOne(x => x.ProductStorage)
                        .WithOne(x => x.ProductMain)
                        .HasForeignKey<ProductStorage>(x => x.ProductNo);

                entity.Property(x => x.No).HasColumnName("No").HasColumnType("int");
                entity.Property(x => x.Schema).HasColumnName("Schema").HasColumnType("varchar(15)");
                entity.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar(50)");
                entity.Property(x => x.SellPrice).HasColumnName("SellPrice").HasColumnType("decimal");
            });

            //Products.ProductStorages
            modelBuilder.Entity<ProductStorage>(entity => {
                entity.ToTable("ProductStorages");
                entity.HasKey(x => x.ProductNo);

                entity.Property(x => x.ProductNo).HasColumnName("ProductNo").HasColumnType("int");

                //entity.Property(x => x.Storage).HasColumnName("Storage").HasColumnType("smallint");

                //加入 IsConcurrencyToken 確保資料的一致性 (不會超賣)
                entity.Property(x => x.Storage).HasColumnName("Storage").HasColumnType("smallint").IsConcurrencyToken();
            });

            #endregion

            #region Orders

            //Orders.OrderMains
            modelBuilder.Entity<OrderMain>(entity =>
            {
                entity.ToTable("OrderMains");
                entity.HasKey(x => x.No);
                entity.HasIndex(x => x.Schema).IsUnique();

                entity.Property(x => x.No).HasColumnName("No").HasColumnType("int").ValueGeneratedOnAdd();
                entity.Property(x => x.Schema).HasColumnName("Schema").HasColumnType("char(36)");
                entity.Property(x => x.OrderDate).HasColumnName("OrderDate").HasColumnType("timestamp");
                entity.Property(x => x.MemberGUID).HasColumnName("MemberGUID").HasColumnType("char(36)");
                entity.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit");
            });

            //Orders.OrderDetails
            modelBuilder.Entity<OrderDetail>(entity => {
                entity.ToTable("OrderDetails");
                entity.HasKey(x => new { x.OrderNo, x.ProductNo });
                entity.HasOne(x => x.OrderMain)
                        .WithMany(x => x.OrderDetails)
                        .HasForeignKey(x => x.OrderNo);
                entity.HasOne(x => x.ProductMain)
                        .WithMany(x => x.OrderDetails)
                        .HasForeignKey(x => x.ProductNo);

                entity.Property(x => x.OrderNo).HasColumnName("OrderNo").HasColumnType("int");
                entity.Property(x => x.ProductNo).HasColumnName("ProductNo").HasColumnType("int");
                entity.Property(x => x.SellPrice).HasColumnName("SellPrice").HasColumnType("decimal");
                entity.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("smallint");
            });

            #endregion

            #region Events

            //Events.EventLogs
            modelBuilder.Entity<EventLog>(entity => {
                entity.ToTable("EventLogs");
                entity.HasKey(x => x.No);

                entity.Property(x => x.No).HasColumnName("No").HasColumnType("int").ValueGeneratedOnAdd();
                entity.Property(x => x.EventDateTime).HasColumnName("EventDateTime").HasColumnType("timestamp");

                entity.Property(x => x.MemberGUID).HasColumnName("MemberGUID").HasColumnType("char(36)");
                entity.Property(x => x.ProductSchema).HasColumnName("ProductSchema").HasColumnType("varchar(15)");
                entity.Property(x => x.ProductName).HasColumnName("ProductName").HasColumnType("varchar(50)");
                entity.Property(x => x.OrginalStorage).HasColumnName("OrginalStorage").HasColumnType("smallint");
                entity.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("smallint");

                entity.Property(x => x.Elapsed).HasColumnName("Elapsed").HasColumnType("int");
                entity.Property(x => x.IsSuccess).HasColumnName("IsSuccess").HasColumnType("bit");

                entity.Property(x => x.Exception).HasColumnName("Exception").HasColumnType("varchar(500)");
                entity.Property(x => x.Retry).HasColumnName("Retry").HasColumnType("int");
            });

            #endregion
        }
    }

}