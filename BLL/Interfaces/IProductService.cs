using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        Task<GetProductDto> AddProduct(AddProductDto productDto,string SellerUserName);
        Task<string> DeleteProduct(string ProductId , string SellerUserName);
        Task<GetProductDto> UpdateProduct(UpdateProductDto productDto,string SellerUserName);
        IEnumerable< GetProductDto>? GetAllProduct();
        Task <GetProductDto> GetProduct(string ProductId);

    }
}
