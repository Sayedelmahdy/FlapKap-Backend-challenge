using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class ChangeResultDto
    {
        public string Message { get; set; }
        public int Coins100 { get; set; }
        public int Coins50 { get; set; }
        public int Coins20 { get; set; }
        public int Coins10 { get; set; }
        public int Coins5 { get; set; }
    }
}
