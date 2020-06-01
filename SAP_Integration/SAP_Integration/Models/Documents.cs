using SAP_Integration.SAPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_Integration.Models
{
    public class Documents : MarketingDocument
    {
        public List<DocumentLines> Lines { get; set; }
        public List<DocumentDownPayments> DownPaymentToDraw { get; set; }

        public IUserFields UserFields { get; set; }

        public Documents()
        {
            UserFields = new DocumentsUDF();
        }
    }

    public class DocumentLines : MarketingDocumentLines
    {

        public List<DocumentSerials> Serials { get; set; }
        public List<DocumentBatchs> Batches { get; set; }
        public List<DocumentLinesExpenses> Expenses { get; set; }

        public IUserFields UserFields { get; set; }

        public DocumentLines()
        {
            UserFields = new DocumentLinesUDF();
        }
    }

    public class DocumentSerials : MarketingDocumentLinesSerials
    {
        
        public IUserFields UserFields { get; set; }
        public DocumentSerials()
        {
            UserFields = new DocumentSerialsUDF();
        }
    }

    public class DocumentBatchs : MarketingDocumentLinesBatch
    {
        

        public IUserFields UserFields { get; set; }
        public DocumentBatchs()
        {
            UserFields = new DocumentBatchsUDF();
        }
      
    }

    public class DocumentDownPayments : MarketingDocumentDownPayment
    {
        

    }
    public class DocumentLinesExpenses : MarketingDocumentLinesExpenses
    {

    }

    public class DocumentsUDF : IUserFields
    {
        //public string TypeCode { get; set; }
        //public string TypeName { get; set; }
        public int U_P_ID { get; set; }
        public string U_P_MANUAL_DOCNUM { get; set; }
        public string U_P_PRNO { get; set; }
        public string U_P_REFNUM { get; set; }
    }
    public class DocumentSerialsUDF : IUserFields
    {
    }
    public class DocumentLinesUDF : IUserFields
    {
        public String U_P_ID { get; set; }
        public double U_P_QUANTITY { get; set; }
        public double U_P_LENGTH { get; set; }
        public double U_P_WIDTH { get; set; }
        public String U_PRICE_UOM { get; set; }
    }
    public class DocumentBatchsUDF : IUserFields
    {
    }
}