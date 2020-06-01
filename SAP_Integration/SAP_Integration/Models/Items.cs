using SAP_Integration.SAPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_Integration.Models
{
    public class Items : ItemMaster
    {
        //public List<ItemsPrices> PriceList { get; set; }
        public List<ItemsWhsInfo> WhsInfo { get; set; }
        public List<ItemsVendor> PreferedVendor { get; set; }
        public IUserFields UserFields { get; set; }

        public Items()
       {
           UserFields = new ItemsUDF();
       }
    }

    public class ItemsPrices : ItemMasterPriceList
    {

        public IUserFields UserFields { get; set; }

        public ItemsPrices()
        {
            UserFields = new ItemsPricesUDF();
        }
    }
    public class ItemsWhsInfo : ItemMasterWarehouse
    {
        

        public IUserFields UserFields { get; set; }

        public ItemsWhsInfo()
        {
            UserFields = new ItemsWhsInfoUDF();
        }
    }

    public class ItemsVendor : ItemMasterVendors
    {
        
    }

    public class ItemsUDF : IUserFields
    {
        public String U_IF_NO { get; set; }
        public int U_SEQ { get; set; }
        public DateTime U_DATA_LOAD_DT { get; set; }
        public String U_FLAG { get; set; }
        public String U_ItemType { get; set; }
        public String U_ItemAbrv { get; set; }
        
    }
    public class ItemsPricesUDF : IUserFields
    {
    }
    public class ItemsWhsInfoUDF : IUserFields
    {
    }
}