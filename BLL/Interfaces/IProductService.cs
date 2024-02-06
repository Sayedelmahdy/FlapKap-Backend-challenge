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
        Task<GetProductDto> AddProductAsync(AddProductDto productDto,string SellerUserName);
        Task<string> DeleteProductAsync(string ProductId , string SellerUserName);
        Task<GetProductDto> UpdateProductAsync(UpdateProductDto productDto,string SellerUserName);
        IEnumerable< GetProductDto>? GetAllProduct();
        Task <GetProductDto> GetProductAsync(string ProductId);

    }
}
