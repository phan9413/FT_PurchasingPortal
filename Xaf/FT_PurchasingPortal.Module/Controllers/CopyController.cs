using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CopyController : ViewController
    {
        public CopyController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        public bool CopyFromDocument(ClassDocument tObject, ListView sListView, IObjectSpace ios)
        {
            if (tObject == null) return false;

            bool added = false;
            int minvalue = 0;
            int comparevalue = 0;
            int maxvisorder = 0;
            int comparevisorder = 0;
            if (tObject.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest masterobject = (PurchaseRequest)tObject;
                if (masterobject.PurchaseRequestDetail.Count > 0)
                {
                    comparevalue = masterobject.PurchaseRequestDetail.Min(pp => pp.Oid);
                    comparevisorder = masterobject.PurchaseRequestDetail.Max(pp => pp.VisOrder);
                }
                if (comparevalue < minvalue) minvalue = comparevalue - 1;
                if (comparevisorder > maxvisorder) maxvisorder = comparevisorder + 1;
            }
            else if (tObject.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder masterobject = (PurchaseOrder)tObject;
                if (masterobject.PurchaseOrderDetail.Count > 0)
                {
                    comparevalue = masterobject.PurchaseOrderDetail.Min(pp => pp.Oid);
                    comparevisorder = masterobject.PurchaseOrderDetail.Max(pp => pp.VisOrder);
                }
                if (comparevalue < minvalue) minvalue = comparevalue - 1;
                if (comparevisorder > maxvisorder) maxvisorder = comparevisorder + 1;
            }
            else if (tObject.GetType() == typeof(PurchaseDelivery))
            {
                PurchaseDelivery masterobject = (PurchaseDelivery)tObject;
                if (masterobject.PurchaseDeliveryDetail.Count > 0)
                {
                    comparevalue = masterobject.PurchaseDeliveryDetail.Min(pp => pp.Oid);
                    comparevisorder = masterobject.PurchaseDeliveryDetail.Max(pp => pp.VisOrder);
                }
                if (comparevalue < minvalue) minvalue = comparevalue - 1;
                if (comparevisorder > maxvisorder) maxvisorder = comparevisorder + 1;
            }
            else if (tObject.GetType() == typeof(PurchaseReturn))
            {
                PurchaseReturn masterobject = (PurchaseReturn)tObject;
                if (masterobject.PurchaseReturnDetail.Count > 0)
                {
                    comparevalue = masterobject.PurchaseReturnDetail.Min(pp => pp.Oid);
                    comparevisorder = masterobject.PurchaseReturnDetail.Max(pp => pp.VisOrder);
                }
                if (comparevalue < minvalue) minvalue = comparevalue - 1;
                if (comparevisorder > maxvisorder) maxvisorder = comparevisorder + 1;
            }
            else if (tObject.GetType() == typeof(PurchaseQuotation))
            {
                PurchaseQuotation masterobject = (PurchaseQuotation)tObject;
                if (masterobject.PurchaseQuotationDetail.Count > 0)
                {
                    comparevalue = masterobject.PurchaseQuotationDetail.Min(pp => pp.Oid);
                    comparevisorder = masterobject.PurchaseQuotationDetail.Max(pp => pp.VisOrder);
                }
                if (comparevalue < minvalue) minvalue = comparevalue - 1;
                if (comparevisorder > maxvisorder) maxvisorder = comparevisorder + 1;
            }

            foreach (ClassDocumentDetail dtl in sListView.SelectedObjects)
            {
                if (dtl.OpenQty > 0 && dtl.LineStatus == LineStatusEnum.Open)
                {
                    minvalue--;
                    maxvisorder++;
                    added = true;

                    #region create object by type
                    ClassDocumentDetail tDtl = null;
                    if (tObject.GetType() == typeof(PurchaseRequest))
                    {
                        tDtl = ios.CreateObject<PurchaseRequestDetail>();
                    }
                    else if (tObject.GetType() == typeof(PurchaseOrder))
                    {
                        tDtl = ios.CreateObject<PurchaseOrderDetail>();
                    }
                    else if (tObject.GetType() == typeof(PurchaseDelivery))
                    {
                        tDtl = ios.CreateObject<PurchaseDeliveryDetail>();
                    }
                    else if (tObject.GetType() == typeof(PurchaseReturn))
                    {
                        tDtl = ios.CreateObject<PurchaseReturnDetail>();
                    }
                    else if (tObject.GetType() == typeof(PurchaseQuotation))
                    {
                        tDtl = ios.CreateObject<PurchaseQuotationDetail>();
                    }
                    #endregion

                    #region assign udf detail
                    ClassUDFDetail sClassD = null;
                    ClassUDFDetail tClassD = null;
                    if (dtl.GetType() == typeof(PurchaseRequestDetail))
                    {
                        sClassD = ((PurchaseRequestDetail)dtl).UDFs;
                    }
                    else if (dtl.GetType() == typeof(PurchaseOrderDetail))
                    {
                        sClassD = ((PurchaseOrderDetail)dtl).UDFs;
                    }
                    else if (dtl.GetType() == typeof(PurchaseDeliveryDetail))
                    {
                        sClassD = ((PurchaseDeliveryDetail)dtl).UDFs;
                    }
                    else if (dtl.GetType() == typeof(PurchaseReturnDetail))
                    {
                        sClassD = ((PurchaseReturnDetail)dtl).UDFs;
                    }
                    else if (dtl.GetType() == typeof(PurchaseQuotationDetail))
                    {
                        sClassD = ((PurchaseQuotationDetail)dtl).UDFs;
                    }

                    if (tDtl.GetType() == typeof(PurchaseRequestDetail))
                    {
                        tClassD = ((PurchaseRequestDetail)tDtl).UDFs;
                    }
                    else if (tDtl.GetType() == typeof(PurchaseOrderDetail))
                    {
                        tClassD = ((PurchaseOrderDetail)tDtl).UDFs;
                    }
                    else if (tDtl.GetType() == typeof(PurchaseDeliveryDetail))
                    {
                        tClassD = ((PurchaseDeliveryDetail)tDtl).UDFs;
                    }
                    else if (tDtl.GetType() == typeof(PurchaseReturnDetail))
                    {
                        tClassD = ((PurchaseReturnDetail)tDtl).UDFs;
                    }
                    else if (tDtl.GetType() == typeof(PurchaseQuotationDetail))
                    {
                        tClassD = ((PurchaseQuotationDetail)tDtl).UDFs;
                    }
                    AssignUDFDetail(ref sClassD, ref tClassD);
                    #endregion

                    tDtl.Oid = minvalue;
                    tDtl.VisOrder = maxvisorder;
                    tDtl.Quantity = dtl.OpenQty;
                    if (dtl.DocCur != null)
                        tDtl.DocCur = ios.FindObject<vwCurrency>(CriteriaOperator.Parse("BoKey=?", dtl.DocCur.BoKey));
                    if (dtl.ItemCode != null)
                        tDtl.ItemCode = ios.FindObject<vwItemMasters>(CriteriaOperator.Parse("BoKey=?", dtl.ItemCode.BoKey));
                    tDtl.Dscription = dtl.Dscription;
                    if (dtl.WhsCode != null)
                        tDtl.WhsCode = ios.FindObject<vwWarehouses>(CriteriaOperator.Parse("BoKey=?", dtl.WhsCode.BoKey));
                    if (dtl.BinCode != null)
                        tDtl.BinCode = ios.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", dtl.BinCode.BoKey));
                    if (dtl.OcrCode != null)
                        tDtl.OcrCode = ios.FindObject<vwDimension1>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode.BoKey));
                    if (dtl.OcrCode2 != null)
                        tDtl.OcrCode2 = ios.FindObject<vwDimension2>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode2.BoKey));
                    if (dtl.OcrCode3 != null)
                        tDtl.OcrCode3 = ios.FindObject<vwDimension3>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode3.BoKey));
                    if (dtl.OcrCode4 != null)
                        tDtl.OcrCode4 = ios.FindObject<vwDimension4>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode4.BoKey));
                    if (dtl.OcrCode5 != null)
                        tDtl.OcrCode5 = ios.FindObject<vwDimension5>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode5.BoKey));
                    if (dtl.PrjCode != null)
                        tDtl.PrjCode = ios.FindObject<vwProjects>(CriteriaOperator.Parse("BoKey=?", dtl.PrjCode.BoKey));
                    if (dtl.AcctCode != null)
                        tDtl.AcctCode = ios.FindObject<vwAccounts>(CriteriaOperator.Parse("BoKey=?", dtl.AcctCode.BoKey));
                    if (dtl.TaxCode != null)
                        tDtl.TaxCode = ios.FindObject<vwTaxes>(CriteriaOperator.Parse("BoKey=?", dtl.TaxCode.BoKey));
                    if (dtl.FreightCharge != null)
                        tDtl.FreightCharge = ios.FindObject<vwExpenses>(CriteriaOperator.Parse("BoKey=?", dtl.FreightCharge.BoKey));

                    tDtl.BatchNumber = dtl.BatchNumber;
                    tDtl.UnitMsr = dtl.UnitMsr;
                    tDtl.UnitPrice = dtl.UnitPrice;
                    tDtl.TaxPerc = dtl.TaxPerc;
                    tDtl.TaxAmt = dtl.TaxAmt;
                    tDtl.DiscountAmt = dtl.DiscountAmt;
                    tDtl.FreightAmt = dtl.FreightAmt;

                    if (dtl.ObjType != null)
                        tDtl.BaseType = ios.GetObjectByKey<DocType>(dtl.ObjType.Oid);

                    tDtl.Baseline = dtl.Oid;

                    tDtl.SAPBaseType = dtl.SAPObjType;
                    tDtl.SAPBaseEntry = dtl.SAPDocEntry;
                    tDtl.SAPBaseLine = dtl.SAPLineNum;

                    #region create object by type
                    if (tObject.GetType() == typeof(PurchaseRequest))
                    {
                        ((PurchaseRequest)tObject).PurchaseRequestDetail.Add((PurchaseRequestDetail)tDtl);
                    }
                    else if (tObject.GetType() == typeof(PurchaseOrder))
                    {
                        ((PurchaseOrder)tObject).PurchaseOrderDetail.Add((PurchaseOrderDetail)tDtl);
                    }
                    else if (tObject.GetType() == typeof(PurchaseDelivery))
                    {
                        ((PurchaseDelivery)tObject).PurchaseDeliveryDetail.Add((PurchaseDeliveryDetail)tDtl);
                    }
                    else if (tObject.GetType() == typeof(PurchaseReturn))
                    {
                        ((PurchaseReturn)tObject).PurchaseReturnDetail.Add((PurchaseReturnDetail)tDtl);
                    }
                    #endregion

                }

            }
            #region assign total
            if (tObject.GetType() == typeof(PurchaseRequest))
            {
                ((PurchaseRequest)tObject).DocB4Total = ((PurchaseRequest)tObject).PurchaseRequestDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseOrder))
            {
                ((PurchaseOrder)tObject).DocB4Total = ((PurchaseOrder)tObject).PurchaseOrderDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseDelivery))
            {
                ((PurchaseDelivery)tObject).DocB4Total = ((PurchaseDelivery)tObject).PurchaseDeliveryDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseReturn))
            {
                ((PurchaseReturn)tObject).DocB4Total = ((PurchaseReturn)tObject).PurchaseReturnDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseQuotation))
            {
                ((PurchaseQuotation)tObject).DocB4Total = ((PurchaseQuotation)tObject).PurchaseQuotationDetail.Sum(pp => pp.LineTotal);
            }
            #endregion
            return added;
        }

        public bool CopyToDocument(ClassDocument sObject, ClassDocument tObject, IObjectSpace ios, DetailView sDetailView)
        {
            if (sObject == null) return false;
            if (tObject == null) return false;

            bool added = false;
            int minvalue = 0;
            int maxvisorder = 0;

            if (sDetailView != null)
            {
                if (sObject.CardCode != null)
                    tObject.CardCode = ios.FindObject<vwBusinessPartners>(CriteriaOperator.Parse("BoKey=?", sObject.CardCode.BoKey));
                tObject.CardName = sObject.CardName;
                if (sObject.CncttCode != null)
                    tObject.CncttCode = ios.FindObject<vwContactPersons>(CriteriaOperator.Parse("BoKey=?", sObject.CncttCode.BoKey));
                if (sObject.DocCur != null)
                    tObject.DocCur = ios.FindObject<vwCurrency>(CriteriaOperator.Parse("BoKey=?", sObject.DocCur.BoKey));
                if (sObject.ShipToCode != null)
                    tObject.ShipToCode = ios.FindObject<vwShipToAddress>(CriteriaOperator.Parse("BoKey=?", sObject.ShipToCode.BoKey));
                tObject.Address2 = sObject.Address2;
                if (sObject.BillToCode != null)
                    tObject.BillToCode = ios.FindObject<vwBillToAddress>(CriteriaOperator.Parse("BoKey=?", sObject.BillToCode.BoKey));
                tObject.Address = sObject.Address;
                if (sObject.SlpCode != null)
                    tObject.SlpCode = ios.FindObject<vwSalesPersons>(CriteriaOperator.Parse("BoKey=?", sObject.SlpCode.BoKey));
                tObject.JrnMemo = sObject.JrnMemo;
                tObject.Comments = sObject.Comments;

                #region assign udf header
                ClassUDFHeader sClass = null;
                ClassUDFHeader tClass = null;
                if (sObject.GetType() == typeof(PurchaseRequest))
                {
                    sClass = ((PurchaseRequest)sObject).UDFs;
                }
                else if (sObject.GetType() == typeof(PurchaseOrder))
                {
                    sClass = ((PurchaseOrder)sObject).UDFs;
                }
                else if (sObject.GetType() == typeof(PurchaseDelivery))
                {
                    sClass = ((PurchaseDelivery)sObject).UDFs;
                }
                else if (sObject.GetType() == typeof(PurchaseReturn))
                {
                    sClass = ((PurchaseReturn)sObject).UDFs;
                }
                else if (sObject.GetType() == typeof(PurchaseQuotation))
                {
                    sClass = ((PurchaseQuotation)sObject).UDFs;
                }

                if (tObject.GetType() == typeof(PurchaseRequest))
                {
                    tClass = ((PurchaseRequest)tObject).UDFs;
                }
                else if (tObject.GetType() == typeof(PurchaseOrder))
                {
                    tClass = ((PurchaseOrder)tObject).UDFs;
                }
                else if (tObject.GetType() == typeof(PurchaseDelivery))
                {
                    tClass = ((PurchaseDelivery)tObject).UDFs;
                }
                else if (tObject.GetType() == typeof(PurchaseReturn))
                {
                    tClass = ((PurchaseReturn)tObject).UDFs;
                }
                else if (tObject.GetType() == typeof(PurchaseQuotation))
                {
                    tClass = ((PurchaseQuotation)tObject).UDFs;
                }
                AssignUDFHeader(ref sClass, ref tClass);
                #endregion
            }
            ListPropertyEditor listviewDetail = null;
            foreach (ViewItem item in sDetailView.Items)
            {
                if ((item is ListPropertyEditor))
                    if (item.Id == "PurchaseRequestDetail" || item.Id == "PurchaseOrderDetail" || item.Id == "PurchaseDeliveryDetail")
                        listviewDetail = item as ListPropertyEditor;
            }
            if (listviewDetail != null && listviewDetail.ListView != null)
            {
                foreach (ClassDocumentDetail dtl in listviewDetail.ListView.CollectionSource.List)
                {
                    if (dtl.OpenQty > dtl.CopyQty && dtl.LineStatus == LineStatusEnum.Open)
                    {
                        minvalue--;
                        maxvisorder++;
                        added = true;

                        #region create object by type
                        ClassDocumentDetail tDtl = null;
                        if (tObject.GetType() == typeof(PurchaseRequest))
                        {
                            tDtl = ios.CreateObject<PurchaseRequestDetail>();
                        }
                        else if (tObject.GetType() == typeof(PurchaseOrder))
                        {
                            tDtl = ios.CreateObject<PurchaseOrderDetail>();
                        }
                        else if (tObject.GetType() == typeof(PurchaseDelivery))
                        {
                            tDtl = ios.CreateObject<PurchaseDeliveryDetail>();
                        }
                        else if (tObject.GetType() == typeof(PurchaseReturn))
                        {
                            tDtl = ios.CreateObject<PurchaseReturnDetail>();
                        }
                        else if (tObject.GetType() == typeof(PurchaseQuotation))
                        {
                            tDtl = ios.CreateObject<PurchaseQuotationDetail>();
                        }
                        #endregion

                        #region assign udf detail
                        ClassUDFDetail sClassD = null;
                        ClassUDFDetail tClassD = null;
                        if (dtl.GetType() == typeof(PurchaseRequestDetail))
                        {
                            sClassD = ((PurchaseRequestDetail)dtl).UDFs;
                        }
                        else if (dtl.GetType() == typeof(PurchaseOrderDetail))
                        {
                            sClassD = ((PurchaseOrderDetail)dtl).UDFs;
                        }
                        else if (dtl.GetType() == typeof(PurchaseDelivery))
                        {
                            sClassD = ((PurchaseDeliveryDetail)dtl).UDFs;
                        }
                        else if (dtl.GetType() == typeof(PurchaseReturn))
                        {
                            sClassD = ((PurchaseReturnDetail)dtl).UDFs;
                        }
                        else if (dtl.GetType() == typeof(PurchaseQuotation))
                        {
                            sClassD = ((PurchaseQuotationDetail)dtl).UDFs;
                        }

                        if (tDtl.GetType() == typeof(PurchaseRequestDetail))
                        {
                            tClassD = ((PurchaseRequestDetail)tDtl).UDFs;
                        }
                        else if (tDtl.GetType() == typeof(PurchaseOrderDetail))
                        {
                            tClassD = ((PurchaseOrderDetail)tDtl).UDFs;
                        }
                        else if (tDtl.GetType() == typeof(PurchaseDeliveryDetail))
                        {
                            tClassD = ((PurchaseDeliveryDetail)tDtl).UDFs;
                        }
                        else if (tDtl.GetType() == typeof(PurchaseReturnDetail))
                        {
                            tClassD = ((PurchaseReturnDetail)tDtl).UDFs;
                        }
                        else if (tDtl.GetType() == typeof(PurchaseQuotationDetail))
                        {
                            tClassD = ((PurchaseQuotationDetail)tDtl).UDFs;
                        }
                        AssignUDFDetail(ref sClassD, ref tClassD);
                        #endregion

                        tDtl.Oid = minvalue;
                        tDtl.VisOrder = maxvisorder;
                        tDtl.Quantity = dtl.OpenQty;
                        if (dtl.DocCur != null)
                            tDtl.DocCur = ios.FindObject<vwCurrency>(CriteriaOperator.Parse("BoKey=?", dtl.DocCur.BoKey));
                        if (dtl.ItemCode != null)
                            tDtl.ItemCode = ios.FindObject<vwItemMasters>(CriteriaOperator.Parse("BoKey=?", dtl.ItemCode.BoKey));
                        tDtl.Dscription = dtl.Dscription;
                        if (dtl.WhsCode != null)
                            tDtl.WhsCode = ios.FindObject<vwWarehouses>(CriteriaOperator.Parse("BoKey=?", dtl.WhsCode.BoKey));
                        if (dtl.BinCode != null)
                            tDtl.BinCode = ios.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", dtl.BinCode.BoKey));
                        if (dtl.OcrCode != null)
                            tDtl.OcrCode = ios.FindObject<vwDimension1>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode.BoKey));
                        if (dtl.OcrCode2 != null)
                            tDtl.OcrCode2 = ios.FindObject<vwDimension2>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode2.BoKey));
                        if (dtl.OcrCode3 != null)
                            tDtl.OcrCode3 = ios.FindObject<vwDimension3>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode3.BoKey));
                        if (dtl.OcrCode4 != null)
                            tDtl.OcrCode4 = ios.FindObject<vwDimension4>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode4.BoKey));
                        if (dtl.OcrCode5 != null)
                            tDtl.OcrCode5 = ios.FindObject<vwDimension5>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode5.BoKey));
                        if (dtl.PrjCode != null)
                            tDtl.PrjCode = ios.FindObject<vwProjects>(CriteriaOperator.Parse("BoKey=?", dtl.PrjCode.BoKey));
                        if (dtl.AcctCode != null)
                            tDtl.AcctCode = ios.FindObject<vwAccounts>(CriteriaOperator.Parse("BoKey=?", dtl.AcctCode.BoKey));
                        if (dtl.TaxCode != null)
                            tDtl.TaxCode = ios.FindObject<vwTaxes>(CriteriaOperator.Parse("BoKey=?", dtl.TaxCode.BoKey));
                        if (dtl.FreightCharge != null)
                            tDtl.FreightCharge = ios.FindObject<vwExpenses>(CriteriaOperator.Parse("BoKey=?", dtl.FreightCharge.BoKey));

                        tDtl.BatchNumber = dtl.BatchNumber;
                        tDtl.UnitMsr = dtl.UnitMsr;
                        tDtl.UnitPrice = dtl.UnitPrice;
                        tDtl.TaxPerc = dtl.TaxPerc;
                        tDtl.TaxAmt = dtl.TaxAmt;
                        tDtl.DiscountAmt = dtl.DiscountAmt;
                        tDtl.FreightAmt = dtl.FreightAmt;

                        if (dtl.ObjType != null)
                            tDtl.BaseType = ios.GetObjectByKey<DocType>(dtl.ObjType.Oid);

                        tDtl.Baseline = dtl.Oid;

                        tDtl.SAPBaseType = dtl.SAPObjType;
                        tDtl.SAPBaseEntry = dtl.SAPDocEntry;
                        tDtl.SAPBaseLine = dtl.SAPLineNum;

                        #region create object by type
                        if (tObject.GetType() == typeof(PurchaseRequest))
                        {
                            ((PurchaseRequest)tObject).PurchaseRequestDetail.Add((PurchaseRequestDetail)tDtl);
                        }
                        else if (tObject.GetType() == typeof(PurchaseOrder))
                        {
                            ((PurchaseOrder)tObject).PurchaseOrderDetail.Add((PurchaseOrderDetail)tDtl);
                        }
                        else if (tObject.GetType() == typeof(PurchaseDelivery))
                        {
                            ((PurchaseDelivery)tObject).PurchaseDeliveryDetail.Add((PurchaseDeliveryDetail)tDtl);
                        }
                        else if (tObject.GetType() == typeof(PurchaseReturn))
                        {
                            ((PurchaseReturn)tObject).PurchaseReturnDetail.Add((PurchaseReturnDetail)tDtl);
                        }
                        else if (tObject.GetType() == typeof(PurchaseQuotation))
                        {
                            ((PurchaseQuotation)tObject).PurchaseQuotationDetail.Add((PurchaseQuotationDetail)tDtl);
                        }
                        #endregion

                    }

                }

            }
            #region assign total
            if (tObject.GetType() == typeof(PurchaseRequest))
            {
                ((PurchaseRequest)tObject).DocB4Total = ((PurchaseRequest)tObject).PurchaseRequestDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseOrder))
            {
                ((PurchaseOrder)tObject).DocB4Total = ((PurchaseOrder)tObject).PurchaseOrderDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseDelivery))
            {
                ((PurchaseDelivery)tObject).DocB4Total = ((PurchaseDelivery)tObject).PurchaseDeliveryDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseReturn))
            {
                ((PurchaseReturn)tObject).DocB4Total = ((PurchaseReturn)tObject).PurchaseReturnDetail.Sum(pp => pp.LineTotal);
            }
            else if (tObject.GetType() == typeof(PurchaseQuotation))
            {
                ((PurchaseQuotation)tObject).DocB4Total = ((PurchaseQuotation)tObject).PurchaseQuotationDetail.Sum(pp => pp.LineTotal);
            }
            #endregion
            return added;
        }

        public void AssignUDFHeader(ref ClassUDFHeader sClass, ref ClassUDFHeader tClass)
        {
            if (sClass is null || tClass is null) return;
            System.Reflection.PropertyInfo[] properties = typeof(ClassUDFHeader).GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                //property.GetValue(sClass);
                if (property.Name.Contains("U_"))
                    property.SetValue(tClass, property.GetValue(sClass, null), null);
            }
        }
        public void AssignUDFDetail(ref ClassUDFDetail sClass, ref ClassUDFDetail tClass)
        {
            if (sClass is null || tClass is null) return;
            System.Reflection.PropertyInfo[] properties = typeof(ClassUDFDetail).GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                //property.GetValue(sClass);
                if (property.Name.Contains("U_"))
                    property.SetValue(tClass, property.GetValue(sClass,null), null);
            }
        }

        public void copyClassDocumentDetail(ClassDocumentDetail dtl, ClassDocumentDetail tDtl, IObjectSpace ios)
        {
            #region assign udf detail
            ClassUDFDetail sClassD = null;
            ClassUDFDetail tClassD = null;
            if (dtl.GetType() == typeof(PurchaseRequestDetail))
            {
                sClassD = ((PurchaseRequestDetail)dtl).UDFs;
            }
            else if (dtl.GetType() == typeof(PurchaseOrderDetail))
            {
                sClassD = ((PurchaseOrderDetail)dtl).UDFs;
            }
            else if (dtl.GetType() == typeof(PurchaseDeliveryDetail))
            {
                sClassD = ((PurchaseDeliveryDetail)dtl).UDFs;
            }
            else if (dtl.GetType() == typeof(PurchaseReturnDetail))
            {
                sClassD = ((PurchaseReturnDetail)dtl).UDFs;
            }
            else if (dtl.GetType() == typeof(PurchaseQuotationDetail))
            {
                sClassD = ((PurchaseQuotationDetail)dtl).UDFs;
            }

            if (tDtl.GetType() == typeof(PurchaseRequestDetail))
            {
                tClassD = ((PurchaseRequestDetail)tDtl).UDFs;
            }
            else if (tDtl.GetType() == typeof(PurchaseOrderDetail))
            {
                tClassD = ((PurchaseOrderDetail)tDtl).UDFs;
            }
            else if (tDtl.GetType() == typeof(PurchaseDeliveryDetail))
            {
                tClassD = ((PurchaseDeliveryDetail)tDtl).UDFs;
            }
            else if (tDtl.GetType() == typeof(PurchaseReturnDetail))
            {
                tClassD = ((PurchaseReturnDetail)tDtl).UDFs;
            }
            else if (tDtl.GetType() == typeof(PurchaseQuotationDetail))
            {
                tClassD = ((PurchaseQuotationDetail)tDtl).UDFs;
            }
            AssignUDFDetail(ref sClassD, ref tClassD);
            #endregion

            tDtl.Quantity = dtl.Quantity;
            if (dtl.DocCur != null)
                tDtl.DocCur = ios.FindObject<vwCurrency>(CriteriaOperator.Parse("BoKey=?", dtl.DocCur.BoKey));
            if (dtl.ItemCode != null)
                tDtl.ItemCode = ios.FindObject<vwItemMasters>(CriteriaOperator.Parse("BoKey=?", dtl.ItemCode.BoKey));
            tDtl.Dscription = dtl.Dscription;
            if (dtl.WhsCode != null)
                tDtl.WhsCode = ios.FindObject<vwWarehouses>(CriteriaOperator.Parse("BoKey=?", dtl.WhsCode.BoKey));
            if (dtl.BinCode != null)
                tDtl.BinCode = ios.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", dtl.BinCode.BoKey));
            if (dtl.OcrCode != null)
                tDtl.OcrCode = ios.FindObject<vwDimension1>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode.BoKey));
            if (dtl.OcrCode2 != null)
                tDtl.OcrCode2 = ios.FindObject<vwDimension2>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode2.BoKey));
            if (dtl.OcrCode3 != null)
                tDtl.OcrCode3 = ios.FindObject<vwDimension3>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode3.BoKey));
            if (dtl.OcrCode4 != null)
                tDtl.OcrCode4 = ios.FindObject<vwDimension4>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode4.BoKey));
            if (dtl.OcrCode5 != null)
                tDtl.OcrCode5 = ios.FindObject<vwDimension5>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode5.BoKey));
            if (dtl.PrjCode != null)
                tDtl.PrjCode = ios.FindObject<vwProjects>(CriteriaOperator.Parse("BoKey=?", dtl.PrjCode.BoKey));
            if (dtl.AcctCode != null)
                tDtl.AcctCode = ios.FindObject<vwAccounts>(CriteriaOperator.Parse("BoKey=?", dtl.AcctCode.BoKey));
            if (dtl.TaxCode != null)
                tDtl.TaxCode = ios.FindObject<vwTaxes>(CriteriaOperator.Parse("BoKey=?", dtl.TaxCode.BoKey));
            if (dtl.FreightCharge != null)
                tDtl.FreightCharge = ios.FindObject<vwExpenses>(CriteriaOperator.Parse("BoKey=?", dtl.FreightCharge.BoKey));

            tDtl.BatchNumber = dtl.BatchNumber;
            tDtl.UnitMsr = dtl.UnitMsr;
            tDtl.UnitPrice = dtl.UnitPrice;
            tDtl.TaxPerc = dtl.TaxPerc;
            tDtl.TaxAmt = dtl.TaxAmt;
            tDtl.DiscountAmt = dtl.DiscountAmt;
            tDtl.FreightAmt = dtl.FreightAmt;

            if (dtl.BaseType != null)
                tDtl.BaseType = ios.GetObjectByKey<DocType>(dtl.BaseType.Oid);

            tDtl.Baseline = dtl.Baseline;

            tDtl.SAPBaseType = dtl.SAPBaseType;
            tDtl.SAPBaseEntry = dtl.SAPBaseEntry;
            tDtl.SAPBaseLine = dtl.SAPBaseLine;


            tDtl.IsDuplicated = true;
        }

        public void copyClassStockTransferDocumentDetail(ClassStockTransferDocumentDetail dtl, ClassStockTransferDocumentDetail tDtl, IObjectSpace ios)
        {
            #region assign udf detail
            ClassUDFDetail sClassD = null;
            ClassUDFDetail tClassD = null;
            if (dtl.GetType() == typeof(StockTransferRequestDetail))
            {
                sClassD = ((StockTransferRequestDetail)dtl).UDFs;
            }
            AssignUDFDetail(ref sClassD, ref tClassD);
            #endregion

            tDtl.Quantity = dtl.Quantity;
            if (dtl.ItemCode != null)
                tDtl.ItemCode = ios.FindObject<vwItemMasters>(CriteriaOperator.Parse("BoKey=?", dtl.ItemCode.BoKey));
            tDtl.Dscription = dtl.Dscription;
            if (dtl.WhsCode != null)
                tDtl.WhsCode = ios.FindObject<vwWarehouses>(CriteriaOperator.Parse("BoKey=?", dtl.WhsCode.BoKey));
            if (dtl.OcrCode != null)
                tDtl.OcrCode = ios.FindObject<vwDimension1>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode.BoKey));
            if (dtl.OcrCode2 != null)
                tDtl.OcrCode2 = ios.FindObject<vwDimension2>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode2.BoKey));
            if (dtl.OcrCode3 != null)
                tDtl.OcrCode3 = ios.FindObject<vwDimension3>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode3.BoKey));
            if (dtl.OcrCode4 != null)
                tDtl.OcrCode4 = ios.FindObject<vwDimension4>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode4.BoKey));
            if (dtl.OcrCode5 != null)
                tDtl.OcrCode5 = ios.FindObject<vwDimension5>(CriteriaOperator.Parse("BoKey=?", dtl.OcrCode5.BoKey));
            if (dtl.PrjCode != null)
                tDtl.PrjCode = ios.FindObject<vwProjects>(CriteriaOperator.Parse("BoKey=?", dtl.PrjCode.BoKey));
            tDtl.UnitMsr = dtl.UnitMsr;

            tDtl.IsDuplicated = true;
        }
    }
}
