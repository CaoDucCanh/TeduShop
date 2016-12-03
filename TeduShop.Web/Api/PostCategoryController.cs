using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeduShop.Service;
using TeduShop.Web.Infrastructure.Core;

namespace TeduShop.Web.Api
{
    public class PostCategoryController : ApiControllerBase
    {
        IPostCategoryService _postCategoryService;
        public PostCategoryController(IErrorSevice errorSevice, IPostCategoryService postCategoryService)
            : base(errorSevice)
        {
            this._postCategoryService = postCategoryService;
        }
    }
}