using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class User : IdentityUser
    {
        #region Properties
        [Column(TypeName = "decimal(18,4)")]
        public decimal Deposit { get; set; } = 0;
        #endregion
        #region Navigations

        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
        #endregion
    }
}
