﻿using System;
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

                    if (tDtl.GetType() == typeof(PurchaseRequestDetail))
                    {
                        tClassD = ((PurchaseRequestDetail)tDtl).UDFs;
                    }
                    else if (tDtl.GetType() == typeof(PurchaseOrderDetail))
                    {
                        tClassD = ((PurchaseOrderDetail)tDtl).UDFs;
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
                    tDtl.UnitMsr = dtl.UnitMsr;
                    tDtl.UnitPrice = dtl.UnitPrice;

                    if (dtl.ObjType != null)
                        tDtl.BaseType = ios.GetObjectByKey<DocType>(dtl.ObjType.Oid);

                    tDtl.Baseline = dtl.Oid;
                    #region create object by type
                    if (tObject.GetType() == typeof(PurchaseRequest))
                    {
                        ((PurchaseRequest)tObject).PurchaseRequestDetail.Add((PurchaseRequestDetail)tDtl);
                    }
                    else if (tObject.GetType() == typeof(PurchaseOrder))
                    {
                        ((PurchaseOrder)tObject).PurchaseOrderDetail.Add((PurchaseOrderDetail)tDtl);
                    }
                    #endregion

                }

            }
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

                if (tObject.GetType() == typeof(PurchaseRequest))
                {
                    tClass = ((PurchaseRequest)tObject).UDFs;
                }
                else if (tObject.GetType() == typeof(PurchaseOrder))
                {
                    tClass = ((PurchaseOrder)tObject).UDFs;
                }
                AssignUDFHeader(ref sClass, ref tClass);
                #endregion
            }
            ListPropertyEditor listviewDetail = null;
            foreach (ViewItem item in sDetailView.Items)
            {
                if ((item is ListPropertyEditor))
                    if (item.Id == "PurchaseRequestDetail" || item.Id == "PurchaseOrderDetail")
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

                        if (tDtl.GetType() == typeof(PurchaseRequestDetail))
                        {
                            tClassD = ((PurchaseRequestDetail)tDtl).UDFs;
                        }
                        else if (tDtl.GetType() == typeof(PurchaseOrderDetail))
                        {
                            tClassD = ((PurchaseOrderDetail)tDtl).UDFs;
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
                        tDtl.UnitMsr = dtl.UnitMsr;
                        tDtl.UnitPrice = dtl.UnitPrice;

                        if (dtl.ObjType != null)
                            tDtl.BaseType = ios.GetObjectByKey<DocType>(dtl.ObjType.Oid);

                        tDtl.Baseline = dtl.Oid;
                        #region create object by type
                        if (tObject.GetType() == typeof(PurchaseRequest))
                        {
                            ((PurchaseRequest)tObject).PurchaseRequestDetail.Add((PurchaseRequestDetail)tDtl);
                        }
                        else if (tObject.GetType() == typeof(PurchaseOrder))
                        {
                            ((PurchaseOrder)tObject).PurchaseOrderDetail.Add((PurchaseOrderDetail)tDtl);
                        }
                        #endregion
                         
                    }

                }
            }
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

    }
}