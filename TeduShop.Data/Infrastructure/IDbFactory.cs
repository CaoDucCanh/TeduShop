using System;

namespace TeduShop.Data.Infrastructure
{
    //trước khi tạo IDbFactory thì tạo Class Disposable
    public interface IDbFactory : IDisposable
    {
        TeduShopDbContext Init();
        //phương thức này để init thằng DbContext
    }
}
