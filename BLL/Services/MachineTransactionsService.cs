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
    public class MachineTransactionsService : IMachineTransactionsService
    {
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<Product> _product;

        public MachineTransactionsService(UserManager<User> userManager, IBaseRepository<Product> product)
        {
            _userManager = userManager;
            _product = product;
        }
        public async Task<PurchaseResultDto> Buy(PurchasedProductDto purchasedProductDto, string UserName)
        {
            var buyer = await _userManager.FindByNameAsync(UserName);
            if (buyer == null)
            {
                return new PurchaseResultDto { Message = "The user not found / invalid Purchase" };
            }
            var product = await _product.GetByIdAsync(purchasedProductDto.ProductId);
            if (product == null)
            {
                return new PurchaseResultDto { Message = "The product not found / invalid Purchase" };
            }
            if (product.AmountAvailable < purchasedProductDto.AmountOfProduct)
            {
                return new PurchaseResultDto { Message = $"Insufficient stock for product '{product.ProductName}'. Available quantity: {product.AmountAvailable}. Requested quantity: {purchasedProductDto.AmountOfProduct}" };
            }
            decimal totalCost = product.Cost * purchasedProductDto.AmountOfProduct;
            if (buyer.Deposit < totalCost)
            {//50 
                return new PurchaseResultDto { Message = "Insufficient funds" };
            }
            decimal change=0;
            bool failed = false;
            int maxRetries = 5;
            do {
                using (var transaction = await _product.BeginTransactionAsync())
                {
                    try
                    {
                        var seller = await _userManager.FindByIdAsync(product.SellerId);
                        seller.Deposit += totalCost;
                        await _userManager.UpdateAsync(seller);
                        buyer.Deposit -= totalCost;
                        change = buyer.Deposit;
                        buyer.Deposit = 0;
                        await _userManager.UpdateAsync(buyer);
                        product.AmountAvailable -= purchasedProductDto.AmountOfProduct;
                        await _product.UpdateAsync(product);
                        transaction.Commit();
                        failed = false;
                    }
                    catch (Exception)
                    {
                        failed = true;
                        await transaction.RollbackAsync();
                        maxRetries--;
                    }

                }
            } while (failed && maxRetries > 0);
            if (failed)
            {
                return new PurchaseResultDto
                {
                    Message = "Failed to complete the purchase",

                };
            }
            

            return new PurchaseResultDto
            {
                Message = "Purchase successful",
                TotalSpent = totalCost,
                ProductId = product.ProductId,
                AmountOfProduct = purchasedProductDto.AmountOfProduct,
                Change = CalculateChange(change),
            };

        }

        public async Task<string> Reset(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return "The user not found / invalid reset";
            }
            if (user.Deposit == 0)
            {
                return "Your deposit is already zero. There's nothing to reset";
            }

            user.Deposit = 0;
            await _userManager.UpdateAsync(user);
            return "Your deposit reset successfully now your deposit is = 0 ";

        }

        public async Task<string> Deposit(int coin, string UserName)
        {
            if (!Enum.IsDefined(typeof(CoinDenomination), coin))
            {
                return $"Invalid coin denomination: {coin}";
            }
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return "The user not found";
            }
            user.Deposit += coin;
            await _userManager.UpdateAsync(user);
           
            return $"Deposit successful. Your current deposit: {user.Deposit}";
        }

        public async Task<CoinResultDto> Withdraw(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return new CoinResultDto { Message= "The user not found" };
            }
            if (user.Deposit ==0)
            {
                return new CoinResultDto { Message = "No money to withdraw" };
            }
            var coinResult = CalculateChange(user.Deposit);
            user.Deposit = 0;
            await _userManager.UpdateAsync(user);
            return coinResult;

        }

        private CoinResultDto CalculateChange(decimal change)
        {
            var changeResult = new CoinResultDto 
            { 
               Message = "Here's your change:",
                Coins100 = (int)(change / 100),
                Coins50 = (int)((change % 100) / 50),
                Coins20 = (int)(((change % 100) % 50) / 20),
                Coins10 = (int)((((change % 100) % 50) % 20) / 10),
                Coins5 = (int)(((((change % 100) % 50) % 20) % 10) / 5)
            };

            return changeResult;
        }
    }
}
