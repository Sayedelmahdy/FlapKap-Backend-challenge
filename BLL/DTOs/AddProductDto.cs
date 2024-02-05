using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class AddProductDto
    {
        
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int AmountAvailable { get; set; }
        [Required]
        public decimal Cost { get; set; }
    }
}
