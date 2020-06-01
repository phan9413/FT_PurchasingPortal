using SAP_Integration.SAPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_Integration.Models
{
    public class BPMaster : BusinessPartnerMaster
    {
        public List<BPAddresses> Addresses { get; set; }
        public List<BPContact> Contacts { get; set; }

        public IUserFields UserFields { get; set; }

        public BPMaster()
        {
            UserFields = new BPMasterUDF();
        }

    }

    public class BPAddresses : BusinessPartnerMasterAddress
    {

        public IUserFields UserFields { get; set; }
        public BPAddresses()
        {
            UserFields = new BPAddressesUDF();
        }
    }

    public class BPContact : BusinesspartnerMasterContact
    {

        public IUserFields UserFields { get; set; }

        public BPContact()
        {
            UserFields = new BPContactUDF();
        }
    }
    public class BPMasterUDF : IUserFields
    {
        public String U_IF_NO { get; set; }
        public int U_SEQ { get; set; }
        public DateTime U_DATA_LOAD_DT { get; set; }
        public String U_FLAG { get; set; }
        public String U_SEX { get; set; }
        public String U_SSCR_DE { get; set; }
    }
    public class BPAddressesUDF : IUserFields
    {
    }
    public class BPContactUDF : IUserFields
    {
    }
}