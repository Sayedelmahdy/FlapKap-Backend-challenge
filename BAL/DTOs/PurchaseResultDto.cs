using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
   public class PurchaseResultDto
    {
        public string Message { get; set; }
        public decimal TotalSpent { get; set; }
        public string ProductId { get; set; }
       
        public int AmountOfProduct { get; set; }
        public ChangeResultDto Change { get; set; }
    }
}
