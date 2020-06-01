using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP_Integration
{
    public class Map
    {
        public Dictionary<string, string> dict;

        public Dictionary<string, string> initMap()
        {
            dict = new Dictionary<string, string>();

            dict.Add("IF_NO", "U_IF_NO");
            dict.Add("SEQ", "U_SEQ");
            dict.Add("SEX", "U_SEX");
            dict.Add("DATA_LOAD_DT", "U_DATA_LOAD_DT");
            dict.Add("SSCR_DE", "U_SSCR_DE");
            dict.Add("FLAG", "U_FLAG");


            dict.Add("PO_DE", "DocDate");//Date of shipment
            dict.Add("WRH_CD", "WarehouseCode");//Warehouse code
            dict.Add("SUPLY_ETP_CD", "CardCode");//Supplier Code
            dict.Add("PO_NO", "DocNum");//Purchase order number
            dict.Add("PO_DTL_NO", "");//Purchase order seq
            dict.Add("ITEM_CD", "ItemCode");//Item code
            dict.Add("PO_QY", "Quantity");//Purchase Order quantity
            dict.Add("UNIT", "MeasureUnit");//unit
            dict.Add("PO_UPRC", "Price");//Purchase Order Unit Price
            dict.Add("SUPLY_PRC", "LineTotal");//a supply price
            dict.Add("STD_EXR", "DocCurrency");//standard exchange rate
            dict.Add("CRNCY_UNIT", "DocRate");//Currency unit
            dict.Add("PO_TY_CD", "TypeCode");//Purchase Order Type Code
            dict.Add("PO_TY_NM", "TypeName");//Purchase order type name

            return dict;
        }
        private string getValue(string key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return "";
        }        
    }
}
