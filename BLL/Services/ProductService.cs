using BLL.DTOs;
using BLL.Interfaces;
using DAL.Context;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductService:IProductService
    {
        private readonly IBaseRepository<Product> _Product;
        private readonly UserManager<User> _userManager;
        public ProductService(IBaseRepository<Product> Product,UserManager<User> UserManager)
        {
            _userManager = UserManager;
            _Product = Product;
        }

        public async Task<GetProductDto> AddProduct(AddProductDto productDto, string SellerUserName)
        {
            var user = await  _userManager.FindByNameAsync(SellerUserName);
           
            if (user == null)
            {
                return new GetProductDto { Message = "the username not found" };
            }
            var Product = new Product
            {
                ProductName = productDto.ProductName,
                AmountAvailable = productDto.AmountAvailable,
                Cost = productDto.Cost,
                SellerId = user.Id,
            };
            await _Product.AddAsync(Product);
            var ProductDetail = new GetProductDto
            {
                Message = "Product added successfully ",
                ProductId = Product.ProductId,
                AmountAvailable = Product.AmountAvailable,
                Cost = Product.Cost,
                ProductName = Product.ProductName,

            };
            return ProductDetail;
        }

        public async Task<string> DeleteProduct(string ProductId, string SellerUserName)
        {
            var user = await _userManager.FindByNameAsync(SellerUserName);

            if (user == null)
            {
                return "the username not found";
            }

            var product = await _Product.FindAsync(p=>p.ProductId == ProductId);
          
            if (product == null)
            {
                return "The Product not found";
            }
            if (product.SellerId != user.Id) {
                return "You aren't allowed for deleting this product";
            }
            await _Product.DeleteAsync(product);
            return "Product Deleted successfully ";
        }

        public IEnumerable< GetProductDto>? GetAllProduct()
        {
          var products= _Product.GetAll()?.Select(p =>new GetProductDto
          {
              
              AmountAvailable=p.AmountAvailable,
              Cost=p.Cost,
              ProductId=p.ProductId,
              ProductName=p.ProductName,    

          }).ToList();
            if(products==null)
            {
                return Enumerable.Empty< GetProductDto>();
            }
            return products;
        }
        
        public async Task< GetProductDto> GetProduct(string productId)
        {

            var product = await _Product.GetByIdAsync(productId);
            if (product == null)
            {
                return new GetProductDto { Message = "Product not found" };
            }
            return new GetProductDto
            {
                AmountAvailable = product.AmountAvailable,
                Cost = product.Cost,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Message="product found"
            };
        }

        public async Task<GetProductDto> UpdateProduct(UpdateProductDto productDto, string SellerUserName)
        {
            var user = await _userManager.FindByNameAsync(SellerUserName);

            if (user == null)
            {
                return new GetProductDto { Message = "the username not found" };
            }
            var product = await _Product.FindAsync(p => p.ProductId == productDto.productId);
            if (product == null)
            {
                return new GetProductDto { Message = "The Product not found" };

            }
            if (product.SellerId != user.Id)
            {
                return new GetProductDto {Message = "You aren't allowed for deleting this product"};
            }
            product.ProductName= productDto.ProductName??product.ProductName;
            product.AmountAvailable=productDto.AmountAvailable??product.AmountAvailable;
            product.Cost = productDto.Cost??product.Cost;
          await  _Product.UpdateAsync(product);

            return new GetProductDto
            {
                Message = "product updated successfully ",
                AmountAvailable = product.AmountAvailable,
                Cost = product.Cost,
                ProductName = product.ProductName,
                ProductId = product.ProductId,
            };

        }
    }
}
