using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    //public enum DocumentStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    CLOSE = 3
    //}

    //public enum MaintenanceStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    WIP = 2,
    //    CLOSE = 3
    //}

    //public enum Region
    //{
    //    Local = 0,
    //    Oversea = 1
    //}
    public enum CopyToEnum
    {
        CopyZeroQty = 0,
        CopyAvailableQty = 1,
        CopyOnhandQty = 2
    }

    public enum TrxDocType
    {
        AP = 0,
        AR = 1
    }
    public enum ApprovalBy
    {
        User = 0,
        Position = 1,
        Appointed_User = 2
    }
    public enum PostToDocument
    {
        Draft = 0,
        Document = 1
    }
    public enum BudgetType
    {
        Period = 0,
        Document = 1
    }
    public enum DocStatus
    {
        Draft = 0, // new create document / doc user press save
        Cancelled = 1, // once doc user press cancel button
        Rejected = 2, // once approve rejected / or reject user press reject button
        Submited = 3, // once doc user press submit button (trigger approval base on approval condition)
        Accepted = 4, // once approve user approval completed / or no approval required after press submit button
        Closed = 5, // once close user press close button
        Posted = 6, // once post user press post button
        PostedCancel = 7, // once sap posting error

    }
    public enum LineStatusEnum
    {
        Open = 0,
        Close = 1,
        Cancel = 2,
        Delete = 3
    }


    public enum ApprovalStatus
    {
        Not_Applicable = 0,
        Approved = 1,
        Required_Approval = 2,
        Rejected = 3
    }

    public enum ApprovalAction
    {
        NA = 0,
        Yes = 1,
        No = 2
    }
    public enum ApprovalType
    {
        Budget = 0,
        Document = 1
    }
}
