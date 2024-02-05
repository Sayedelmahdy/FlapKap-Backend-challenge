using BLL.DTOs;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMachineTransactions
    {
        Task<string> Deposit(DepositDto depositDto);
        Task<PurchaseResultDto> Buy (PurchasedProductDto purchasedProductDto);
        Task<string> Reset(string UserName);
    }
}
