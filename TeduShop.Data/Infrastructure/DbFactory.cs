namespace TeduShop.Data.Infrastructure
{
    //nếu kế thừa từ 1 Abtract class và 1 interface thì được, nhưng Abtract class thì không được
    public class DbFactory : Disposable, IDbFactory
    {
        TeduShopDbContext dbContext;
        public TeduShopDbContext Init()
        {
            return dbContext ?? (dbContext = new TeduShopDbContext());
            //nếu dbContext null thì khởi tạo new 1 cái
        }

        //Dispose nếu khác null thì ta sẽ Dispose nó
        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
