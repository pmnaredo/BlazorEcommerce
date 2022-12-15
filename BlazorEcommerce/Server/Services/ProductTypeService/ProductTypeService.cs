using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private DataContext _context;

        public ProductTypeService(DataContext context)
        {
            _context = context;
        }

        public Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<ProductType>>> DeleteProductType(int productTypeId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<ProductType>>> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return new ServiceResponse<List<ProductType>> { Data = productTypes };
        }

        public Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType)
        {
            throw new NotImplementedException();
        }
    }
}
