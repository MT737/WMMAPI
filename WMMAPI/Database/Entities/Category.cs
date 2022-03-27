using System.Collections.Generic;

namespace WMMAPI.Database.Entities
{
    public class Category : BaseVendCat
    {
        //Navigation Property
        public virtual User User { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
