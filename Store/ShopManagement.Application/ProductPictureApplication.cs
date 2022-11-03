using System;
using System.Collections.Generic;
using System.Text;
using _0_Framework.Application;
using ShopManagement.Application.Contracts.ProductPicture;
using ShopManagement.Domain.ProductAgg;
using ShopManagement.Domain.ProductPictureAgg;

namespace ShopManagement.Application
{
    public class ProductPictureApplication:IProductPictureApplication
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileUploader _fileUploader;

        private readonly IProductPictureRepository _productPictureRepository;

        public ProductPictureApplication(IProductPictureRepository productPictureRepository, IFileUploader fileUploader, IProductRepository productRepository)
        {
            _productPictureRepository = productPictureRepository;
            _fileUploader = fileUploader;
            _productRepository = productRepository;
        }

        public OperationResult Create(CreateProductPicture command)
        {
           var operationResult=new OperationResult();
           var produc = _productRepository.GetProductWithCategory(command.ProductId);
           var path = $"{produc.Category.Slug}//{produc.Slug}";
           var picture = _fileUploader.Upload(command.Picture, path);
           //if (_productPictureRepository.Exist(x => x.Picture == command.Picture && x.ProductId == command.ProductId))
           //    return operationResult.Failed(ApplicationMessages.DublicatedRecord);

           var productPicture=new ProductPicture(command.ProductId,picture,command.PictureAlt,command.PictureTitle);
           _productPictureRepository.Create(productPicture);
           _productPictureRepository.SaveChanges();
           return operationResult.Succedded();
        }

        public OperationResult Edit(EditProductPicture command)
        {
            var operationResult = new OperationResult();
            var productPicture = _productPictureRepository.Get(command.Id);
            if (productPicture == null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);
            //if (_productPictureRepository.Exist(x => x.Picture == command.Picture && x.Id != command.Id && x.ProductId==command.ProductId))
            //    return operationResult.Failed(ApplicationMessages.DublicatedRecord);
            var produc = _productRepository.GetProductWithCategory(command.ProductId);
            var path = $"{produc.Category.Slug}//{produc.Slug}";
            var picture = _fileUploader.Upload(command.Picture, path);
            productPicture.Edit(command.ProductId, picture, command.PictureAlt, command.PictureTitle);
            _productPictureRepository.SaveChanges();
            return operationResult.Succedded();




        }

        public OperationResult Remove(long id)
        {
            var operationResult = new OperationResult();
            var productPicture = _productPictureRepository.Get(id);
            if (productPicture == null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);
          
            productPicture.Remove();
            _productPictureRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operationResult = new OperationResult();
            var productPicture = _productPictureRepository.Get(id);
            if (productPicture == null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);

            productPicture.Restore();
            _productPictureRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public OperationResult Delete(EditProductPicture picture)
        {
            var operationResult = new OperationResult();
            var productPicture = _productPictureRepository.Get(picture.Id);
            if (productPicture == null)
                return operationResult.Failed(ApplicationMessages.RecordNotFound);

            //_productPictureRepository.Remove(productPicture);
            
            _productPictureRepository.SaveChanges();
            return operationResult.Succedded();
        }

        public List<ProductPictureViewModel> Search(ProductPictureSearchModel searchModel)
        {
            return _productPictureRepository.Search(searchModel);
        }

        public EditProductPicture GetDetails(long id)
        {
            return _productPictureRepository.GetDetails(id);
        }
    }
}
