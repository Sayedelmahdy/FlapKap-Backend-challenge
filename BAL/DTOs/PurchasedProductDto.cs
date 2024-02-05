using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
   public class PurchasedProductDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string ProductId {  get; set; }
        [Required]
        public int AmountOfProduct { get; set; }
    }
}
