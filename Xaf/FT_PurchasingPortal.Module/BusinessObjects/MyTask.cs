using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using System.ComponentModel;

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Task")]
    [DomainComponent]
    public class MyTask
    {
        public MyTask()
        {
            MyNotifications = new List<MyNotification>();
        }
        [Browsable(false)]
        public int Oid { get; private set; }
        public string Subject { get; set; }
        [Aggregated]
        public virtual IList<MyNotification> MyNotifications { get; set; }
    }
}