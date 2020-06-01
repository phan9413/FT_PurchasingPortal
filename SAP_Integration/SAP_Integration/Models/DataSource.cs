using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP_Integration.Models
{
    public class DataSource
    {
        public String IF_NO { get; set; }
        public int SEQ { get; set; }
        public String IMPORT_FLAG { get; set; }
        public DateTime IMPORT_DT { get; set; }
        public String IMPORT_LOG { get; set; }
        public String SAP_TB { get; set; }
        public String SAP_KEY { get; set; }
        public String IF_TB { get; set; }
    }
}
