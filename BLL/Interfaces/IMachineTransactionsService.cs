using BLL.DTOs;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMachineTransactionsService
    {
        Task<string> Deposit(int coin,string UserName);
        Task<CoinResultDto> Withdraw( string UserName);
        Task<PurchaseResultDto> Buy (PurchasedProductDto purchasedProductDto,string UserName);
        Task<string> Reset(string UserName);
    }
}
