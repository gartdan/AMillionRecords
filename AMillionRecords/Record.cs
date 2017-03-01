using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMillionRecords
{
    public class Record
    {
        public Guid TransactionId { get; set; }
        public Guid AccountId { get; set; }
        public string CustomerName { get; set; }
        public DateTime TransactionDate { get; set; }
        public double TransactionAmount { get; set; }
        public string Location { get; set; }
    }
}
