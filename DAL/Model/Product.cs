using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Product
    {
        #region Properties
        [Key]
        public string ProductId { get; set; }=Guid.NewGuid().ToString();
        public string ProductName { get; set; }
        public int AmountAvailable { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }
        #endregion
        #region Navigations
        [ForeignKey(nameof(Seller))]
        public string SellerId { get; set; }
        public virtual User Seller { get; set; }
        #endregion
    }
}
