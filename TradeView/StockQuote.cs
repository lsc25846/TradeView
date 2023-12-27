using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeView
{  
    public class StockQuote
    {        
        public decimal TransactionPrice { get; set; }
        public decimal Change { get; set; }
        public int Volume { get; set; }
    }
}
