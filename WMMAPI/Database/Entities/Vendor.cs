using System.Collections.Generic;

namespace WMMAPI.Database.Entities
{
    public class Vendor : BaseVendCat
    {
        //Navigation Property
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual User User { get; set; }
    }
}
