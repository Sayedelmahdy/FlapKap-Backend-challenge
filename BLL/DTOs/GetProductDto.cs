using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class GetProductDto
    {
        public string Message { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int AmountAvailable { get; set; }
        public decimal Cost { get; set; }
    }
}
