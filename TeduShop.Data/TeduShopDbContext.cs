using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TeduShop.Model.Models;

namespace TeduShop.Data
{
    public class TeduShopDbContext : IdentityDbContext<ApplicationUser> //kế thừa từ DbContext
    {
        public TeduShopDbContext()
            : base("TeduShopConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        //trong model có những bảng nào thì liệt kê hết
        public DbSet<Footer> Footers { set; get; }

        public DbSet<Menu> Menus { set; get; }
        public DbSet<MenuGroup> MenuGroups { set; get; }
        public DbSet<Order> Orders { set; get; }
        public DbSet<OrderDetail> OrderDetails { set; get; }
        public DbSet<Page> Pages { set; get; }
        public DbSet<Post> Posts { set; get; }
        public DbSet<PostCategory> PostCategories { set; get; }
        public DbSet<PostTag> PostTags { set; get; }
        public DbSet<Product> Products { set; get; }
        public DbSet<ProductCategory> ProductCategories { set; get; }
        public DbSet<ProductTag> ProductTags { set; get; }
        public DbSet<Slide> Slides { set; get; }
        public DbSet<SupportOnline> SupportOnlines { set; get; }
        public DbSet<SystemConfig> SystemConfigs { set; get; }
        public DbSet<Tag> Tags { set; get; }
        public DbSet<VisitorStatistic> VisitorStatistics { set; get; }
        public DbSet<Error> Errosrs { set; get; }

        public static TeduShopDbContext Create()
        {
            return  new TeduShopDbContext();
        }

        //trong qúa trình làm ta phải ghi đè 1 phương thức của DBContext, nó sẽ chạy khi mà chúng ta khởi tạo entity framework
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<IdentityUserRole>().HasKey(i => new {i.UserId, i.RoleId});
            builder.Entity<IdentityUserLogin>().HasKey(i => i.UserId);

        }
    }
}