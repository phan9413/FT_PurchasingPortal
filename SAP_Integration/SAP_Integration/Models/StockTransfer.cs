using SAP_Integration.SAPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_Integration.Models
{
    public class StockTransfer : InventoryTransfer
    {
        public List<StockTransferLines> Lines { get; set; }
        public IUserFields UserFields { get; set; }

        public StockTransfer()
        {
            UserFields = new StockTransferUDF();
        }
    }

    public class StockTransferLines : InventoryTransferLines
    {
        
        public IUserFields UserFields { get; set; }

        public StockTransferLines()
        {
            UserFields = new StockTransferLinesUDF();
        }

    }
    public class StockTransferUDF : IUserFields
    {
    }
    public class StockTransferLinesUDF : IUserFields
    {
    }
}