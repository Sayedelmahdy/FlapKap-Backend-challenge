using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UpdateProductDto
    {

        [Required]
        public string productId { get; set; }
        public string? ProductName { get; set; }
        public int? AmountAvailable { get; set; }
        public decimal? Cost { get; set; }
    }
}
