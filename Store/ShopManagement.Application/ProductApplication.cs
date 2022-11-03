using System;
using System.Collections.Generic;
using System.Text;
using _0_Framework.Application;
using ShopManagement.Application.Contracts.Product;
using ShopManagement.Domain.ProductAgg;
using ShopManagement.Domain.ProductCategoryAgg;

namespace ShopManagement.Application
{
    public class ProductApplication : IProductApplication
    {
        private IProductCategoryRepository _categoryRepository;
        private IFileUploader _fileUploader;
        private readonly IProductRepository _productRepository;

        public ProductApplication(IProductRepository productRepository, IFileUploader fileUploader, IProductCategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _fileUploader = fileUploader;
            _categoryRepository = categoryRepository;
        }

        public OperationResult Create(CreateProduct command)
        {
            var operationResult=new OperationResult();
            if (_productRepository.Exists(x => x.Name == command.Name))
                return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
            var slug = command.Slug.Slugify();
            var categorySlug = _categoryRepository.GetSlugById(command.CategoryId);
            var path = $"{categorySlug}//{slug}";
            var productPicture = _fileUploader.Upload(command.Picture, path);
            var product=new Product(command.Name,command.Code,command.ShortDescription,command.Description,productPicture,command.PictureAlt,command.PictureTitle,command.CategoryId,slug,command.Keywords,command.MetaDescription);
            _productRepository.Create(product);
            _productRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public OperationResult Edit(EditProduct command)
        {
            var operationResult=new OperationResult();
            var product = _productRepository.GetProductWithCategory(command.Id);
            if (product==null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);
            if (_productRepository.Exists(x => x.Name == command.Name && x.Id!=command.Id))
                return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
            var slug = command.Slug.Slugify();
            
            var path = $"{product.Category.Slug}//{command.Slug}";
            var productPicture = _fileUploader.Upload(command.Picture, path);
            product.Edit(command.Name,command.Code,command.ShortDescription,command.Description,productPicture,command.PictureAlt,command.PictureTitle,command.CategoryId,slug,command.Keywords,command.MetaDescription);
            _productRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public OperationResult Remove(EditProduct command)
        {
            var operationResult = new OperationResult();
            var product = _productRepository.Get(command.Id);
            if (product == null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);

            //_productRepository.Remove(product);
            _productRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public List<ProductViewModel> Search(ProductSearchModel searchModel)
        {
            return _productRepository.Search(searchModel);
        }

        public EditProduct GetDetails(long id)
        {
            return _productRepository.GetDetails(id);
        }

        
        

        public List<ProductViewModel> GetProducts()
        {
            return _productRepository.GetProducts();
        }
    }
}
