using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class STMTTRN
    {
        public string TRNTYPE { get; set; }
        public DateTime DTPOSTED { get; set; }
        public decimal TRNAMT { get; set; }
        public string MEMO { get; set; }
    }
}
