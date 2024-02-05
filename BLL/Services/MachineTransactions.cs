using BLL.DTOs;
using BLL.Enums;
using BLL.Interfaces;
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
    public class MachineTransactions : IMachineTransactions
    {
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<Product> _product;

        public MachineTransactions(UserManager<User> userManager , IBaseRepository<Product> product)
        {
            _userManager = userManager;
            _product = product;
        }
        public async Task<PurchaseResultDto> Buy(PurchasedProductDto purchasedProductDto)
        {
            var buyer = await _userManager.FindByNameAsync(purchasedProductDto.Username);
            if (buyer == null)
            {
                return new PurchaseResultDto { Message = "The user not found / invalid Purchase" };
            }
            var product = await _product.GetByIdAsync(purchasedProductDto.ProductId);
            if (product == null)
            {
                return new PurchaseResultDto { Message = "The product not found / invalid Purchase" };
            }
            if (product.AmountAvailable<purchasedProductDto.AmountOfProduct)
            {
                return new PurchaseResultDto { Message = $"Insufficient stock for product '{product.ProductName}'. Available quantity: {product.AmountAvailable}. Requested quantity: {purchasedProductDto.AmountOfProduct}" };
            }
            decimal totalCost = product.Cost * purchasedProductDto.AmountOfProduct;
            if (buyer.Deposit<totalCost)
            {
                return new PurchaseResultDto { Message = "Insufficient funds" };
            }
            bool failed = false;
            do {
                using (var transaction = await _product.BeginTransactionAsync())
                {
                    try
                    {
                        var seller = await _userManager.FindByIdAsync(product.SellerId);
                        seller.Deposit += totalCost;
                        await _userManager.UpdateAsync(seller);
                        buyer.Deposit -= totalCost;
                        await _userManager.UpdateAsync(buyer);
                        product.AmountAvailable -= purchasedProductDto.AmountOfProduct;
                        await _product.UpdateAsync(product);
                    }
                    catch (Exception)
                    {
                        failed = true;
                        await transaction.RollbackAsync();
                        throw;
                    }
                    transaction.Commit();
                }
            }while (failed);
            decimal change = buyer.Deposit;

            return new PurchaseResultDto
            {
                Message= "Purchase successful",
                TotalSpent= totalCost,
                ProductId=product.ProductId,
                AmountOfProduct=purchasedProductDto.AmountOfProduct,
                Change=CalculateChange(change),
            };

        }

        public async Task<string> Reset(string UserName)
        {
            var user =await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return "The user not found / invalid reset";
            }
            if (user.Deposit==0)
            {
                return "Your deposit is already zero. There's nothing to reset";
            }

            user.Deposit = 0;
            await _userManager.UpdateAsync(user);
            return "Your deposit reset successfully now your deposit is = 0 ";

        }

       public async Task<string> Deposit(DepositDto depositDto)
        {
            if (!Enum.IsDefined(typeof(CoinDenomination), depositDto.coin))
            {
                return $"Invalid coin denomination: {depositDto.coin}";
            }
            var user = await _userManager.FindByNameAsync(depositDto.UserName);
            if (user == null)
            {
                return "The user not found";
            }
            user.Deposit += depositDto.coin;
            await _userManager.UpdateAsync(user);

            return $"Deposit successful. Your current deposit: {user.Deposit}";
        }



        private ChangeResultDto CalculateChange(decimal change)
        {
            var changeResult = new ChangeResultDto 
            {
               Message = "Here's your change:",
                Coins100 = (int)(change / 100),
                Coins50 = (int)((change % 100) / 50),
                Coins20 = (int)((change % 150) / 20),
                Coins10 = (int)((change % 170) / 10),
                Coins5 = (int)((change % 180)  / 5)
            };

            return changeResult;
        }
    }
}
