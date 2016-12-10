using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeduShop.Common;
using TeduShop.Data.Infrastructure;
using TeduShop.Data.Repositories;
using TeduShop.Model.Models;

namespace TeduShop.Service
{
    public interface IProductService
    {
        Product Add(Product product);

        void Update(Product product);

        void Delete(int id);

        IEnumerable<Product> GetAll();

        IEnumerable<Product> GetAll(string keyword);

        // get list product thuộc top
        IEnumerable<Product> GetHotProduct(int top);

        IEnumerable<Product> GetListProductByCategoryPaging(int category, int page, int pageSize, string sort, out int totalRow);

        //tìm kiếm phân trang
        IEnumerable<Product> Search(string keyword, int page, int pageSize, string sort, out int totalRow);

        //get các sản phẩm cùng loại
        IEnumerable<Product> GetReatedProducts(int id, int top);

        //get list produc theo Chuỗi Name truyền vào 
        IEnumerable<string> GetListProductByName(string name);

        // get product theo ID truyên vào
        Product GetById(int id);

        // get list tag của 1 sản phẩm
        IEnumerable<Tag> GetListTagByProductId(int id);

        // get tag theo tagId truyền vào
        Tag GetTag(string tagId);

        //tính lượng view của từng sản phẩm
        void IncreaseView(int id);

        // get list product theo tagId truyền vào
        IEnumerable<Product> GetListProductByTag(string tagId, int page, int pagesize, out int totalRow);

        void Save();
    }
    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;
        private IProductTagRepository _productTagRepository;
        private ITagRepository _tagRepository;
        private IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IProductTagRepository productTagRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _productTagRepository = productTagRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public Product Add(Product product)
        {
            //add product vào bảng Product
            var newproduct = _productRepository.Add(product);
            _unitOfWork.Commit();

            if (!string.IsNullOrEmpty(product.Tags))
            {
                //cắt chuỗi tags truyền vào
                string[] tags = product.Tags.Split(',');

                //vòng lặp kiểm tra từng tags sau khi đã cắt
                for (var i = 0; i < tags.Length; i++)
                {
                    //sử dụng StringHelper để cắt tags làm tagId
                    var tagId = StringHelper.ToUnSignString(tags[i]);

                    //kiểm tra xem tagId đã tồn tại chưa nếu chưa tồn tại thì Add thêm
                    if (_tagRepository.Count(x => x.ID == tagId) == 0)
                    {
                        Tag tag = new Tag();
                        tag.ID = tagId;
                        tag.Name = tags[i];
                        tag.Type = CommonConstants.ProductTag;
                        _tagRepository.Add(tag);
                    }

                    //add dữ liệu vào bảng ProductTag 
                    ProductTag productTag = new ProductTag();
                    productTag.ProductID = product.ID;
                    productTag.TagID = tagId;
                    _productTagRepository.Add(productTag);
                }
            }
            //trả về giá trị product mới được thêm vào
            return newproduct;
        }

        public void Delete(int id)
        {
            _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public IEnumerable<Product> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                return _productRepository.GetMulti(x => x.Name.Contains(keyword) || x.Description.Contains(keyword));
            }
            else
            {
                return _productRepository.GetAll();
            }
        }

        public Product GetById(int id)
        {
            return _productRepository.GetSingleById(id);
        }

        public IEnumerable<Product> GetHotProduct(int top)
        {
            return
                _productRepository.GetMulti(x => x.Status && x.HotFlag == true)
                    .OrderByDescending(x => x.CreatDate)
                    .Take(top);
        }

        public IEnumerable<Product> GetListProductByCategoryPaging(int category, int page, int pageSize, string sort, out int totalRow)
        {
            var query = _productRepository.GetMulti(x => x.Status && x.CategoryID == category);
            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;
                case "discount":
                    query = query.OrderByDescending(x => x.PromotionPrice.HasValue);
                    break;
                case "price":
                    query = query.OrderByDescending(x => x.Price);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatDate);
                    break;
            }
            totalRow = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize);

        }

        public IEnumerable<string> GetListProductByName(string name)
        {
            return _productRepository.GetMulti(x => x.Name.Contains(name)).Select(y => y.Name);
        }

        public IEnumerable<Product> GetListProductByTag(string tagId, int page, int pagesize, out int totalRow)
        {
            var model = _productRepository.GetListProductByTag(tagId, page, pagesize, out totalRow);
            return model;
        }

        public IEnumerable<Tag> GetListTagByProductId(int id)
        {
            return _productTagRepository.GetMulti(x => x.ProductID == id, new string[] { "Tag" }).Select(y => y.Tag);
        }

        public IEnumerable<Product> GetReatedProducts(int id, int top)
        {
            var product = _productRepository.GetSingleById(id);
            return
                _productRepository.GetMulti(x => x.Status && x.ID != id && x.CategoryID == product.CategoryID)
                    .OrderByDescending(x => x.CreatDate)
                    .Take(top);
        }

        public Tag GetTag(string tagId)
        {
            return _tagRepository.GetSingleByCondition(x => x.ID == tagId);
        }

        public void IncreaseView(int id)
        {
            var product = _productRepository.GetSingleById(id);
            if (product.ViewCount.HasValue)
            {
                product.ViewCount += 1;
            }
            else
            {
                product.ViewCount = 1;
            }
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Product> Search(string keyword, int page, int pageSize, string sort, out int totalRow)
        {
            var query = _productRepository.GetMulti(x => x.Status && x.Name.Contains(keyword));
            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;
                case "discount":
                    query = query.OrderByDescending(x => x.PromotionPrice.HasValue);
                    break;
                case "price":
                    query = query.OrderByDescending(x => x.Price);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatDate);
                    break;
            }
            totalRow = query.Count();

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(Product product)
        {
            //Update lại dữ liêu product vào bảng Product
            _productRepository.Update(product);
            _unitOfWork.Commit();

            if (!string.IsNullOrEmpty(product.Tags))
            {
                //cắt chuỗi tags truyền vào
                string[] tags = product.Tags.Split(',');

                //vòng lặp kiểm tra từng tags sau khi đã cắt
                for (var i = 0; i < tags.Length; i++)
                {
                    //sử dụng StringHelper để cắt tags làm tagId
                    var tagId = StringHelper.ToUnSignString(tags[i]);

                    //kiểm tra xem tagId đã tồn tại chưa nếu chưa tồn tại thì Add thêm
                    if (_tagRepository.Count(x => x.ID == tagId) == 0)
                    {
                        Tag tag = new Tag();
                        tag.ID = tagId;
                        tag.Name = tags[i];
                        tag.Type = CommonConstants.ProductTag;
                        _tagRepository.Add(tag);
                    }

                    //add dữ liệu vào bảng ProductTag 
                    ProductTag productTag = new ProductTag();
                    productTag.ProductID = product.ID;
                    productTag.TagID = tagId;
                    _productTagRepository.Add(productTag);
                }
            }
        }
    }
}
