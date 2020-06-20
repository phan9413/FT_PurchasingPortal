using FT_PurchasingPortal.Module.BusinessObjects;
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
        public string SAPObjType { get; set; }
        public int SAPDocEntry { get; set; }
        public int SAPLineNum { get; set; }
        public List<DocumentSerials> Serials { get; set; }
        public List<DocumentBatchs> Batches { get; set; }
        public IUserFields UserFields { get; set; }

        public StockTransferLines()
        {
            UserFields = new StockTransferLinesUDF();
        }

    }
    public class StockTransferUDF : IUserFields
    {
        public int U_P_ID { get; set; }
        public string U_P_DOCNO { get; set; }
    }
    public class StockTransferLinesUDF : IUserFields
    {
    }
}