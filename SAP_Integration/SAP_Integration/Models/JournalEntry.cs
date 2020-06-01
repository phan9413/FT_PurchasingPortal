using SAP_Integration.SAPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP_Integration.Models
{
    public class JournalEntry : Journal
    {
        
        public List<JELines> Lines { get; set; }
        public IUserFields UserFields { get; set; }

        public JournalEntry()
        {
            UserFields = new JEUDF();
        }
    }

    public class JELines : JournalLines
    {

        public IUserFields UserFields { get; set; }

        public JELines()
        {
            UserFields = new JELinesUDF();
        }

    }

    public class JEUDF : IUserFields
    {

    }
    public class JELinesUDF : IUserFields
    {

    }
}