using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using SAP_Integration.Models;
using System.Reflection;
using System.Globalization;
using DevExpress.ExpressApp;
using FT_PurchasingPortal.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace SAP_Integration
{
    class Code
    {

        private SortedDictionary<string, List<string>> logs = new SortedDictionary<string, List<string>>();
        private Dictionary<string, string> dict;

        public Code(SecurityStrategyComplex security, IObjectSpaceProvider ObjectSpaceProvider)
        {
            logs.Clear();
            WriteLog("[Log]", "--------------------------------------------------------------------------------");
            WriteLog("[Log]", "Integration Begin:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
            
            #region Connect to SAP  
            SAPCompany sap = new SAPCompany();
            if (sap.connectSAP())
            {
                WriteLog("[Log]", "Connected to SAP:[" + sap.oCom.CompanyName + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
            }
            else
            {
                WriteLog("[Error]", "SAP Connection:[" + sap.oCom.CompanyDB + "] Message:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh: mm:ss tt") + "]");
                sap.oCom = null;
                goto EndApplication;
            }
            #endregion

            //#region Connect to Interface Table
            //if (sap.connectDataSource())
            //{
            //    WriteLog("[Log]", "Connected to Interface Table:[" + sap.sqlCon.Database + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
            //}
            //else
            //{
            //    WriteLog("[Error]", "Interface Table Connection :[" + sap.sqlCon.Database + "] Message:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh: mm:ss tt") + "]");
            //    goto EndApplication;
            //}
            //#endregion

            // Begin Integration ======================================================================================
            // Your code here
            //DataSet ds = sap.getDataSet();
            //Map map = new Map();
            //dict = map.initMap();

            try
            {
                string LocalCurrency = ConfigurationManager.AppSettings["LocalCurrency"].ToString();
                string temp = "";
                int tempint = 0;
                IObjectSpace ListObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                IObjectSpace securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                string key = "";
                string docstatus = "";

                #region New PR
                //temp = ConfigurationManager.AppSettings["PRPost"].ToString().ToUpper();
                //if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                //{
                //    IList<PurchaseRequest> prlist = ListObjectSpace.GetObjects<PurchaseRequest>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                //    foreach (PurchaseRequest objfromlist in prlist)
                //    {
                //        PurchaseRequest obj = securedObjectSpace.GetObjectByKey<PurchaseRequest>(objfromlist.Oid);
                //        Documents oDoc = new Documents();
                //        if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                //        {
                //            oDoc.DocObject = "oDrafts";
                //            oDoc.DocObjectCode = "oPurchaseRequest";
                //        }
                //        else
                //            oDoc.DocObject = "oPurchaseRequest";

                //        #region posttosap
                //        if (obj.DocTypeSeries.SAPSeries > 0)
                //            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                //        oDoc.DocDate = obj.DocDate;
                //        oDoc.DocDueDate = obj.DocDueDate;
                //        oDoc.TaxDate = obj.TaxDate;
                //        oDoc.RequriedDate = obj.ReqDate;

                //        if (!string.IsNullOrEmpty(obj.NumAtCard))
                //            oDoc.NumAtCard = obj.NumAtCard;

                //        if (obj.CncttCode != null)
                //            oDoc.ContactPersonCode = int.Parse(obj.CncttCode.CntctCode);
                //        if (obj.SlpCode != null)
                //            oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                //        if (obj.ShipToCode != null)
                //            oDoc.ShipToCode = obj.ShipToCode.Address;
                //        if (!string.IsNullOrEmpty(obj.Address2))
                //            oDoc.Address2 = obj.Address2;

                //        if (obj.BillToCode != null)
                //            oDoc.PayToCode = obj.BillToCode.Address;
                //        if (!string.IsNullOrEmpty(obj.Address))
                //            oDoc.Address = obj.Address;

                //        if (!string.IsNullOrEmpty(obj.JrnMemo))
                //            oDoc.JournalMemo = obj.JrnMemo;
                //        if (!string.IsNullOrEmpty(obj.Comments))
                //            oDoc.Comments = obj.Comments;

                //        if (obj.Rounding != 0)
                //        {
                //            oDoc.Rounding = true;
                //            oDoc.RoundingDiffAmount = Convert.ToDouble(obj.Rounding);
                //        }
                //        oDoc.DocTotal = Convert.ToDouble(obj.DocTotal);


                //        #region assignhdrudf
                //        DocumentsUDF oDocUDF = new DocumentsUDF();
                //        System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                //        foreach (System.Reflection.PropertyInfo property in properties)
                //        {
                //            if (property.Name == "U_P_ID")
                //                property.SetValue(oDocUDF, obj.Oid, null);
                //            else if (property.Name == "U_P_DOCNO")
                //                property.SetValue(oDocUDF, obj.DocNo, null);
                //            else if (property.Name.Contains("U_"))
                //            {
                //                System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                //                foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                //                {
                //                    if (property.Name == sproperty.Name)
                //                        property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                //                }
                //            }
                //        }
                //        oDoc.UserFields = oDocUDF;
                //        oDoc.Lines = new List<DocumentLines>();
                //        #endregion

                //        foreach (PurchaseRequestDetail dtl in obj.PurchaseRequestDetail)
                //        {
                //            DocumentLines oDocLine = new DocumentLines();
                //            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                //            oDocLine.ItemDescription = dtl.Dscription;
                //            oDocLine.ItemDetails = dtl.ItemDetails;

                //            if (dtl.LineVendor != null)
                //                oDocLine.LineVendor = dtl.LineVendor.CardCode;
                //            else if (obj.CardCode != null)
                //                oDocLine.LineVendor = obj.CardCode.CardCode;

                //            if (dtl.WhsCode != null)
                //                oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                //            if (dtl.OcrCode != null)
                //                oDocLine.CostingCode = dtl.OcrCode.PrcCode;
                //            if (dtl.OcrCode2 != null)
                //                oDocLine.CostingCode2 = dtl.OcrCode2.PrcCode;
                //            if (dtl.OcrCode3 != null)
                //                oDocLine.CostingCode3 = dtl.OcrCode3.PrcCode;
                //            if (dtl.OcrCode4 != null)
                //                oDocLine.CostingCode4 = dtl.OcrCode4.PrcCode;
                //            if (dtl.OcrCode5 != null)
                //                oDocLine.CostingCode5 = dtl.OcrCode5.PrcCode;
                //            if (dtl.PrjCode != null)
                //                oDocLine.ProjectCode = dtl.PrjCode.PrjCode;
                //            if (dtl.AcctCode != null)
                //                oDocLine.AccountCode = dtl.AcctCode.AcctCode;

                //            oDocLine.Quantity = dtl.Quantity;
                //            oDocLine.UnitPrice = Convert.ToDouble(dtl.UnitPrice);

                //            if (dtl.TaxCode != null)
                //                oDocLine.TaxCode = dtl.TaxCode.Code;

                //            if (dtl.TaxAmt != 0)
                //                oDocLine.NetTaxAmount = Convert.ToDouble(dtl.TaxAmt);

                //            if (dtl.FreightCharge != null)
                //            {
                //                oDocLine.Expenses.Add(new DocumentLinesExpenses()
                //                {
                //                    ExpenseCode = int.Parse(dtl.FreightCharge.ExpnsCode),
                //                    LineTotal = Convert.ToDouble(dtl.FreightAmt)
                //                });
                //            }

                //            oDocLine.LineTotal = Convert.ToDouble(dtl.LineTotal);

                //            #region assigndtludf
                //            DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                //            System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                //            foreach (System.Reflection.PropertyInfo property in propertiesl)
                //            {
                //                if (property.Name == "U_P_ID")
                //                    property.SetValue(oDocLineUDF, dtl.Oid, null);
                //                else if (property.Name.Contains("U_"))
                //                {
                //                    System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                //                    foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                //                    {
                //                        if (property.Name == sproperty.Name)
                //                            property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                //                    }
                //                }
                //            }
                //            oDocLine.UserFields = oDocLineUDF;
                //            oDoc.Lines.Add(oDocLine);
                //            #endregion
                //        }

                //        #endregion

                //        if (sap.CreateDocuments(oDoc, ref key))
                //        {
                //            WriteLog("[Log]", (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft ? "Draft " : "") + "PR OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                //            obj.DocStatus.IsSAPPosted = true;
                //            securedObjectSpace.CommitChanges();
                //            if (obj.DocTypeSeries.PostToDocument != PostToDocument.Draft)
                //            {
                //                for (int i = 0; i < oDoc.Lines.Count; i++)
                //                {
                //                    obj.PurchaseRequestDetail[i].SAPObjType = oDoc.Lines[i].SAPObjType;
                //                    obj.PurchaseRequestDetail[i].SAPDocEntry = oDoc.Lines[i].SAPDocEntry;
                //                    obj.PurchaseRequestDetail[i].SAPLineNum = oDoc.Lines[i].SAPLineNum;
                //                }
                //                securedObjectSpace.CommitChanges();
                //            }
                //        }
                //        else
                //        {
                //            WriteLog("[Log]", "PR OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                //            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                //            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                //            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                //            securedObjectSpace.CommitChanges();
                //        }
                //        oDoc = null;
                //        obj = null;
                //    }
                //}
                #endregion

                #region new PO
                temp = ConfigurationManager.AppSettings["POPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<PurchaseOrder> polist = ListObjectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Accepted));
                    foreach (PurchaseOrder objfromlist in polist)
                    {
                        PurchaseOrder obj = securedObjectSpace.GetObjectByKey<PurchaseOrder>(objfromlist.Oid);
                        Documents oDoc = new Documents();
                        //if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                        //{
                        //    oDoc.DocObject = "oDrafts";
                        //    oDoc.DocObjectCode = "oPurchaseOrders";
                        //}
                        //else
                            oDoc.DocObject = "oPurchaseOrders";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        if (obj.CardCode != null)
                        {
                            oDoc.CardCode = obj.CardCode.CardCode;
                            oDoc.CardName = obj.CardName;
                        }
                        oDoc.DocDate = obj.DocDate;
                        oDoc.DocDueDate = obj.DocDueDate;
                        oDoc.TaxDate = obj.TaxDate;

                        if (!string.IsNullOrEmpty(obj.NumAtCard))
                            oDoc.NumAtCard = obj.NumAtCard;

                        if (obj.CncttCode != null)
                            oDoc.ContactPersonCode = int.Parse(obj.CncttCode.CntctCode);
                        if (obj.SlpCode != null)
                            oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                        if (obj.ShipToCode != null)
                            oDoc.ShipToCode = obj.ShipToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address2))
                            oDoc.Address2 = obj.Address2;

                        if (obj.BillToCode != null)
                            oDoc.PayToCode = obj.BillToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address))
                            oDoc.Address = obj.Address;

                        if (!string.IsNullOrEmpty(obj.JrnMemo))
                            oDoc.JournalMemo = obj.JrnMemo;
                        if (!string.IsNullOrEmpty(obj.Comments))
                            oDoc.Comments = obj.Comments;

                        if (obj.Rounding != 0)
                        {
                            oDoc.Rounding = true;
                            oDoc.RoundingDiffAmount = Convert.ToDouble(obj.Rounding);
                        }
                        oDoc.DocTotal = Convert.ToDouble(obj.DocTotal);


                        #region assignhdrudf
                        DocumentsUDF oDocUDF = new DocumentsUDF();
                        System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                        foreach (System.Reflection.PropertyInfo property in properties)
                        {
                            if (property.Name == "U_P_ID")
                                property.SetValue(oDocUDF, obj.Oid, null);
                            else if (property.Name == "U_P_DOCNO")
                                property.SetValue(oDocUDF, obj.DocNo, null);
                            else if (property.Name.Contains("U_"))
                            {
                                System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                                foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                                {
                                    if (property.Name == sproperty.Name)
                                        property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                                }
                            }
                        }
                        oDoc.UserFields = oDocUDF;
                        oDoc.Lines = new List<DocumentLines>();
                        #endregion

                        foreach (PurchaseOrderDetail dtl in obj.PurchaseOrderDetail)
                        {
                            DocumentLines oDocLine = new DocumentLines();
                            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                            oDocLine.ItemDescription = dtl.Dscription;
                            oDocLine.ItemDetails = dtl.ItemDetails;

                            if (dtl.WhsCode != null)
                                oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                            if (dtl.OcrCode != null)
                                oDocLine.CostingCode = dtl.OcrCode.PrcCode;
                            if (dtl.OcrCode2 != null)
                                oDocLine.CostingCode2 = dtl.OcrCode2.PrcCode;
                            if (dtl.OcrCode3 != null)
                                oDocLine.CostingCode3 = dtl.OcrCode3.PrcCode;
                            if (dtl.OcrCode4 != null)
                                oDocLine.CostingCode4 = dtl.OcrCode4.PrcCode;
                            if (dtl.OcrCode5 != null)
                                oDocLine.CostingCode5 = dtl.OcrCode5.PrcCode;
                            if (dtl.PrjCode != null)
                                oDocLine.ProjectCode = dtl.PrjCode.PrjCode;
                            if (dtl.AcctCode != null)
                                oDocLine.AccountCode = dtl.AcctCode.AcctCode;

                            oDocLine.Quantity = dtl.Quantity;
                            oDocLine.UnitPrice = Convert.ToDouble(dtl.UnitPrice);

                            if (dtl.TaxCode != null)
                                oDocLine.TaxCode = dtl.TaxCode.Code;

                            if (dtl.TaxAmt != 0)
                                oDocLine.NetTaxAmount = Convert.ToDouble(dtl.TaxAmt);

                            if (dtl.FreightCharge != null)
                            {
                                if (oDocLine.Expenses == null) oDocLine.Expenses = new List<DocumentExpenses>();
                                oDocLine.Expenses.Add(new DocumentExpenses()
                                {
                                    ExpenseCode = int.Parse(dtl.FreightCharge.ExpnsCode),
                                    LineTotal = Convert.ToDouble(dtl.FreightAmt)
                                });
                            }

                            oDocLine.LineTotal = Convert.ToDouble(dtl.LineTotal);

                            #region assigndtludf
                            DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                            System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                            foreach (System.Reflection.PropertyInfo property in propertiesl)
                            {
                                if (property.Name == "U_P_ID")
                                    property.SetValue(oDocLineUDF, dtl.Oid, null);
                                else if (property.Name.Contains("U_"))
                                {
                                    System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                                    foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                                    {
                                        if (property.Name == sproperty.Name)
                                            property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                                    }
                                }
                            }
                            oDocLine.UserFields = oDocLineUDF;
                            oDoc.Lines.Add(oDocLine);
                            #endregion
                        }

                        #endregion

                        if (sap.CreateDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            obj.PostVerNo = obj.VerNo;// + 1;
                            obj.SAPDocEntry = oDoc.SAPDocEntry;
                            //if (obj.DocTypeSeries.PostToDocument != PostToDocument.Draft)
                            //{
                                for (int i = 0; i < oDoc.Lines.Count; i++)
                                {
                                    obj.PurchaseOrderDetail[i].SAPObjType = oDoc.Lines[i].SAPObjType;
                                    obj.PurchaseOrderDetail[i].SAPDocEntry = oDoc.Lines[i].SAPDocEntry;
                                    obj.PurchaseOrderDetail[i].SAPLineNum = oDoc.Lines[i].SAPLineNum;
                                    obj.PurchaseOrderDetail[i].PostVerNo = obj.PurchaseOrderDetail[i].VerNo + 1; // PurchaseOrderDetail will auto +1 when save changes
                            }
                                securedObjectSpace.CommitChanges();
                            //}
                        }
                        else
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                            securedObjectSpace.CommitChanges();
                        }
                        oDoc = null;
                        obj = null;
                    }
                }
                #endregion

                #region new STR
                temp = ConfigurationManager.AppSettings["STRPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<StockTransferRequest> polist = ListObjectSpace.GetObjects<StockTransferRequest>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                    foreach (StockTransferRequest objfromlist in polist)
                    {
                        StockTransferRequest obj = securedObjectSpace.GetObjectByKey<StockTransferRequest>(objfromlist.Oid);
                        StockTransfer oDoc = new StockTransfer();
                        //if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                        //{
                        //    oDoc.DocObject = "oStockTransferDraft";
                        //    oDoc.DocObjectCode = "oStockTransfer";
                        //}
                        //else
                            oDoc.DocObject = "oInventoryTransferRequest";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        if (obj.CardCode != null)
                        {
                            oDoc.CardCode = obj.CardCode.CardCode;
                            oDoc.CardName = obj.CardName;
                        }

                        oDoc.DocDate = obj.DocDate;
                        oDoc.DueDate = obj.DocDueDate;
                        oDoc.TaxDate = obj.TaxDate;

                        oDoc.FromWarehouse = obj.Filler.WhsCode;
                        oDoc.ToWarehouse = obj.ToWhsCode.WhsCode;

                        if (obj.SlpCode != null)
                            oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                        //if (obj.ShipToCode != null)
                        //    oDoc.ShipToCode = obj.ShipToCode.Address;
                        //if (!string.IsNullOrEmpty(obj.Address2))
                        //    oDoc.Address2 = obj.Address2;

                        //if (obj.ShipToCode != null)
                        //    oDoc.PayToCode = obj.ShipToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address))
                            oDoc.Address = obj.Address;

                        if (!string.IsNullOrEmpty(obj.JrnMemo))
                            oDoc.JournalMemo = obj.JrnMemo;
                        if (!string.IsNullOrEmpty(obj.Comments))
                            oDoc.Comments = obj.Comments;


                        #region assignhdrudf
                        DocumentsUDF oDocUDF = new DocumentsUDF();
                        System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                        foreach (System.Reflection.PropertyInfo property in properties)
                        {
                            if (property.Name == "U_P_ID")
                                property.SetValue(oDocUDF, obj.Oid, null);
                            else if (property.Name == "U_P_DOCNO")
                                property.SetValue(oDocUDF, obj.DocNo, null);
                            else if (property.Name.Contains("U_"))
                            {
                                System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                                foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                                {
                                    if (property.Name == sproperty.Name)
                                        property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                                }
                            }
                        }
                        oDoc.UserFields = oDocUDF;
                        oDoc.Lines = new List<StockTransferLines>();
                        #endregion

                        foreach (StockTransferRequestDetail dtl in obj.StockTransferRequestDetail)
                        {
                            StockTransferLines oDocLine = new StockTransferLines();
                            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                            oDocLine.ItemDescription = dtl.Dscription;

                            if (dtl.FromWhsCod != null)
                                oDocLine.FromWarehouseCode = dtl.FromWhsCod.WhsCode;
                            else
                                oDocLine.FromWarehouseCode = obj.Filler.WhsCode;

                            if (dtl.WhsCode != null)
                                oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                            else
                                oDocLine.WarehouseCode = obj.ToWhsCode.WhsCode;

                            if (dtl.PrjCode != null)
                                oDocLine.ProjectCode = dtl.PrjCode.PrjCode;

                            oDocLine.Quantity = dtl.Quantity;

                            #region assigndtludf
                            DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                            System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                            foreach (System.Reflection.PropertyInfo property in propertiesl)
                            {
                                if (property.Name == "U_P_ID")
                                    property.SetValue(oDocLineUDF, dtl.Oid, null);
                                else if (property.Name.Contains("U_"))
                                {
                                    System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                                    foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                                    {
                                        if (property.Name == sproperty.Name)
                                            property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                                    }
                                }
                            }
                            oDocLine.UserFields = oDocLineUDF;
                            oDoc.Lines.Add(oDocLine);
                            #endregion
                        }

                        #endregion

                        if (sap.CreateStockTransferDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "STR OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            securedObjectSpace.CommitChanges();
                        }
                        else
                        {
                            WriteLog("[Log]", "STR OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                            securedObjectSpace.CommitChanges();
                        }
                        oDoc = null;
                        obj = null;
                    }
                }
                #endregion

                #region old PO
                temp = ConfigurationManager.AppSettings["POPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    //IList<PurchaseOrder> polist = ListObjectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=1 and VerNo > PostVerNo and DocStatus.CurrDocStatus=?", DocStatus.Accepted));
                    IList<PurchaseOrder> polist = ListObjectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("[DocStatus.IsSAPPosted]=1 and ([VerNo] > [PostVerNo] or [PurchaseOrderDetail][[VerNo] > [PostVerNo]])"));
                    foreach (PurchaseOrder objfromlist in polist)
                    {
                        PurchaseOrder obj = securedObjectSpace.GetObjectByKey<PurchaseOrder>(objfromlist.Oid);
                        docstatus = "";
                        switch (obj.DocStatus.CurrDocStatus)
                        {
                            case DocStatus.Accepted:
                                docstatus = "OPEN";
                                break;
                            case DocStatus.Closed:
                                docstatus = "CLOSE";
                                break;
                            case DocStatus.Cancelled:
                                docstatus = "CANCEL";
                                break;
                        }
                        if (string.IsNullOrEmpty(docstatus)) continue;

                        key = obj.SAPDocEntry.ToString();

                        Documents oDoc = new Documents();

                        //if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                        //{
                        //    oDoc.DocObject = "oDrafts";
                        //    oDoc.DocObjectCode = "oPurchaseOrders";
                        //}
                        //else
                            oDoc.DocObject = "oPurchaseOrders";

                        if (obj.VerNo > obj.PostVerNo)
                        {
                            #region posttosap
                            oDoc.DocDate = obj.DocDate;
                            oDoc.DocDueDate = obj.DocDueDate;
                            oDoc.TaxDate = obj.TaxDate;

                            if (!string.IsNullOrEmpty(obj.NumAtCard))
                                oDoc.NumAtCard = obj.NumAtCard;

                            if (obj.CncttCode != null)
                                oDoc.ContactPersonCode = int.Parse(obj.CncttCode.CntctCode);
                            if (obj.SlpCode != null)
                                oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                            if (obj.ShipToCode != null)
                                oDoc.ShipToCode = obj.ShipToCode.Address;
                            if (!string.IsNullOrEmpty(obj.Address2))
                                oDoc.Address2 = obj.Address2;

                            if (obj.BillToCode != null)
                                oDoc.PayToCode = obj.BillToCode.Address;
                            if (!string.IsNullOrEmpty(obj.Address))
                                oDoc.Address = obj.Address;

                            if (!string.IsNullOrEmpty(obj.JrnMemo))
                                oDoc.JournalMemo = obj.JrnMemo;
                            if (!string.IsNullOrEmpty(obj.Comments))
                                oDoc.Comments = obj.Comments;

                            if (obj.Rounding != 0)
                            {
                                oDoc.Rounding = true;
                                oDoc.RoundingDiffAmount = Convert.ToDouble(obj.Rounding);
                            }
                            oDoc.DocTotal = Convert.ToDouble(obj.DocTotal);

                            #region assignhdrudf
                            DocumentsUDF oDocUDF = new DocumentsUDF();
                            System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                            foreach (System.Reflection.PropertyInfo property in properties)
                            {
                                if (property.Name == "U_P_ID")
                                    property.SetValue(oDocUDF, obj.Oid, null);
                                else if (property.Name == "U_P_DOCNO")
                                    property.SetValue(oDocUDF, obj.DocNo, null);
                                else if (property.Name.Contains("U_"))
                                {
                                    System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                                    foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                                    {
                                        if (property.Name == sproperty.Name)
                                            property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                                    }
                                }
                            }
                            oDoc.UserFields = oDocUDF;
                        }

                        #endregion
                        oDoc.Lines = new List<DocumentLines>();

                        foreach (PurchaseOrderDetail dtl in obj.PurchaseOrderDetail)
                        {
                            DocumentLines oDocLine = new DocumentLines();
                            if (dtl.VerNo > dtl.PostVerNo)
                            {
                                oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                                oDocLine.ItemDescription = dtl.Dscription;
                                oDocLine.ItemDetails = dtl.ItemDetails;

                                if (dtl.WhsCode != null)
                                    oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                                if (dtl.OcrCode != null)
                                    oDocLine.CostingCode = dtl.OcrCode.PrcCode;
                                if (dtl.OcrCode2 != null)
                                    oDocLine.CostingCode2 = dtl.OcrCode2.PrcCode;
                                if (dtl.OcrCode3 != null)
                                    oDocLine.CostingCode3 = dtl.OcrCode3.PrcCode;
                                if (dtl.OcrCode4 != null)
                                    oDocLine.CostingCode4 = dtl.OcrCode4.PrcCode;
                                if (dtl.OcrCode5 != null)
                                    oDocLine.CostingCode5 = dtl.OcrCode5.PrcCode;
                                if (dtl.PrjCode != null)
                                    oDocLine.ProjectCode = dtl.PrjCode.PrjCode;
                                if (dtl.AcctCode != null)
                                    oDocLine.AccountCode = dtl.AcctCode.AcctCode;

                                oDocLine.Quantity = dtl.Quantity;
                                oDocLine.UnitPrice = Convert.ToDouble(dtl.UnitPrice);

                                if (dtl.TaxCode != null)
                                    oDocLine.TaxCode = dtl.TaxCode.Code;

                                if (dtl.TaxAmt != 0)
                                    oDocLine.NetTaxAmount = Convert.ToDouble(dtl.TaxAmt);

                                if (dtl.FreightCharge != null)
                                {
                                    if (oDocLine.Expenses == null) oDocLine.Expenses = new List<DocumentExpenses>();
                                    oDocLine.Expenses.Add(new DocumentExpenses()
                                    {
                                        ExpenseCode = int.Parse(dtl.FreightCharge.ExpnsCode),
                                        LineTotal = Convert.ToDouble(dtl.FreightAmt)
                                    });
                                }

                                oDocLine.LineTotal = Convert.ToDouble(dtl.LineTotal);
                                #region assigndtludf
                                DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                                System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                                foreach (System.Reflection.PropertyInfo property in propertiesl)
                                {
                                    if (property.Name == "U_P_ID")
                                        property.SetValue(oDocLineUDF, dtl.Oid, null);
                                    else if (property.Name.Contains("U_"))
                                    {
                                        System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                                        foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                                        {
                                            if (property.Name == sproperty.Name)
                                                property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                                        }
                                    }
                                }
                                oDocLine.UserFields = oDocLineUDF;
                            }
                            oDocLine.SAPObjType = dtl.SAPObjType;
                            oDocLine.SAPDocEntry = dtl.SAPDocEntry;
                            oDocLine.SAPLineNum = dtl.SAPLineNum;
                            oDoc.Lines.Add(oDocLine);
                            #endregion
                        }

                        #endregion

                        if (sap.UpdateDocuments(oDoc, key, docstatus))
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] update Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            obj.PostVerNo = obj.VerNo;// + 1;
                            securedObjectSpace.CommitChanges();
                            //if (obj.DocTypeSeries.PostToDocument != PostToDocument.Draft)
                            //{
                                for (int i = 0; i < oDoc.Lines.Count; i++)
                                {
                                    obj.PurchaseOrderDetail[i].SAPObjType = oDoc.Lines[i].SAPObjType;
                                    obj.PurchaseOrderDetail[i].SAPDocEntry = oDoc.Lines[i].SAPDocEntry;
                                    obj.PurchaseOrderDetail[i].SAPLineNum = oDoc.Lines[i].SAPLineNum;
                                    obj.PurchaseOrderDetail[i].PostVerNo = obj.PurchaseOrderDetail[i].VerNo + 1; // PurchaseOrderDetail will auto +1 when save changes
                            }
                                securedObjectSpace.CommitChanges();
                            //}
                        }
                        else
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] update Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                            securedObjectSpace.CommitChanges();
                        }
                        oDoc = null;
                        obj = null;
                    }
                }
                #endregion


                #region GRN
                temp = ConfigurationManager.AppSettings["GRNPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<PurchaseDelivery> polist = ListObjectSpace.GetObjects<PurchaseDelivery>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Accepted));
                    foreach (PurchaseDelivery objfromlist in polist)
                    {
                        PurchaseDelivery obj = securedObjectSpace.GetObjectByKey<PurchaseDelivery>(objfromlist.Oid);
                        Documents oDoc = new Documents();
                        //if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                        //{
                        //    oDoc.DocObject = "oDrafts";
                        //    oDoc.DocObjectCode = "oPurchaseOrders";
                        //}
                        //else
                        oDoc.DocObject = "oPurchaseDeliveryNotes";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        if (obj.CardCode != null)
                        {
                            oDoc.CardCode = obj.CardCode.CardCode;
                            oDoc.CardName = obj.CardName;
                        }
                        oDoc.DocDate = obj.DocDate;
                        oDoc.DocDueDate = obj.DocDueDate;
                        oDoc.TaxDate = obj.TaxDate;

                        if (!string.IsNullOrEmpty(obj.NumAtCard))
                            oDoc.NumAtCard = obj.NumAtCard;

                        if (obj.CncttCode != null)
                            oDoc.ContactPersonCode = int.Parse(obj.CncttCode.CntctCode);
                        if (obj.SlpCode != null)
                            oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                        if (obj.ShipToCode != null)
                            oDoc.ShipToCode = obj.ShipToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address2))
                            oDoc.Address2 = obj.Address2;

                        if (obj.BillToCode != null)
                            oDoc.PayToCode = obj.BillToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address))
                            oDoc.Address = obj.Address;

                        if (!string.IsNullOrEmpty(obj.JrnMemo))
                            oDoc.JournalMemo = obj.JrnMemo;
                        if (!string.IsNullOrEmpty(obj.Comments))
                            oDoc.Comments = obj.Comments;

                        if (obj.Rounding != 0)
                        {
                            oDoc.Rounding = true;
                            oDoc.RoundingDiffAmount = Convert.ToDouble(obj.Rounding);
                        }
                        oDoc.DocTotal = Convert.ToDouble(obj.DocTotal);


                        #region assignhdrudf
                        DocumentsUDF oDocUDF = new DocumentsUDF();
                        System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                        foreach (System.Reflection.PropertyInfo property in properties)
                        {
                            if (property.Name == "U_P_ID")
                                property.SetValue(oDocUDF, obj.Oid, null);
                            else if (property.Name == "U_P_DOCNO")
                                property.SetValue(oDocUDF, obj.DocNo, null);
                            else if (property.Name.Contains("U_"))
                            {
                                System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                                foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                                {
                                    if (property.Name == sproperty.Name)
                                        property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                                }
                            }
                        }
                        oDoc.UserFields = oDocUDF;
                        oDoc.Lines = new List<DocumentLines>();
                        #endregion

                        foreach (PurchaseDeliveryDetail dtl in obj.PurchaseDeliveryDetail)
                        {
                            DocumentLines oDocLine = new DocumentLines();
                            tempint = 0;
                            if (int.TryParse(dtl.SAPBaseType, out tempint))
                            {
                                if (tempint != 0)
                                {
                                    oDocLine.BaseType = tempint;
                                    oDocLine.BaseEntry = dtl.SAPBaseEntry;
                                    oDocLine.BaseLine = dtl.SAPBaseLine;
                                }
                            }
                            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                            oDocLine.ItemDescription = dtl.Dscription;
                            oDocLine.ItemDetails = dtl.ItemDetails;

                            if (dtl.WhsCode != null)
                                oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                            if (dtl.OcrCode != null)
                                oDocLine.CostingCode = dtl.OcrCode.PrcCode;
                            if (dtl.OcrCode2 != null)
                                oDocLine.CostingCode2 = dtl.OcrCode2.PrcCode;
                            if (dtl.OcrCode3 != null)
                                oDocLine.CostingCode3 = dtl.OcrCode3.PrcCode;
                            if (dtl.OcrCode4 != null)
                                oDocLine.CostingCode4 = dtl.OcrCode4.PrcCode;
                            if (dtl.OcrCode5 != null)
                                oDocLine.CostingCode5 = dtl.OcrCode5.PrcCode;
                            if (dtl.PrjCode != null)
                                oDocLine.ProjectCode = dtl.PrjCode.PrjCode;
                            if (dtl.AcctCode != null)
                                oDocLine.AccountCode = dtl.AcctCode.AcctCode;

                            oDocLine.Quantity = dtl.Quantity;
                            oDocLine.UnitPrice = Convert.ToDouble(dtl.UnitPrice);
                            
                            if (dtl.TaxCode != null)
                                oDocLine.TaxCode = dtl.TaxCode.Code;

                            if (dtl.TaxAmt != 0)
                                oDocLine.NetTaxAmount = Convert.ToDouble(dtl.TaxAmt);

                            if (dtl.FreightCharge != null)
                            {
                                if (oDocLine.Expenses == null) oDocLine.Expenses = new List<DocumentExpenses>();
                                oDocLine.Expenses.Add(new DocumentExpenses()
                                {
                                    ExpenseCode = int.Parse(dtl.FreightCharge.ExpnsCode),
                                    LineTotal = Convert.ToDouble(dtl.FreightAmt)
                                });
                            }
                            #region batch serial bin
                            if (dtl.ItemCode.ManBtchNum)
                            {
                                if (!string.IsNullOrWhiteSpace(dtl.BatchNumber))
                                {
                                    if (oDocLine.Batches == null) oDocLine.Batches = new List<DocumentBatchs>();
                                    oDocLine.Batches.Add(new DocumentBatchs()
                                    {
                                        BatchNumber = dtl.BatchNumber,
                                        Quantity = dtl.Quantity
                                    });

                                }
                            }
                            else if (dtl.ItemCode.ManSerNum)
                            {
                                if (!string.IsNullOrWhiteSpace(dtl.BatchNumber))
                                {
                                    if (oDocLine.Serials == null) oDocLine.Serials = new List<DocumentSerials>();
                                    oDocLine.Serials.Add(new DocumentSerials()
                                    {
                                        //InternalSerialNumber = dtl.BatchNumber,
                                        ManufacturerSerialNumber = dtl.BatchNumber,
                                        Quantity = dtl.Quantity
                                    });

                                }
                            }
                            if (dtl.BinCode != null && dtl.BinCode.BinAbsEntry != 0)
                            {
                                if (oDocLine.BinAllocations == null) oDocLine.BinAllocations = new List<DocumentBinAllocations>();
                                DocumentBinAllocations bin = new DocumentBinAllocations();
                                bin.BinAbsEntry = dtl.BinCode.BinAbsEntry;
                                bin.Quantity = dtl.Quantity;
                                if (dtl.ItemCode.ManBtchNum || dtl.ItemCode.ManSerNum)
                                    bin.SerialAndBatchNumbersBaseLine = 0;
                                oDocLine.BinAllocations.Add(bin);

                            }
                            #endregion
                            oDocLine.LineTotal = Convert.ToDouble(dtl.LineTotal);

                            #region assigndtludf
                            DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                            System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                            foreach (System.Reflection.PropertyInfo property in propertiesl)
                            {
                                if (property.Name == "U_P_ID")
                                    property.SetValue(oDocLineUDF, dtl.Oid, null);
                                else if (property.Name.Contains("U_"))
                                {
                                    System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                                    foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                                    {
                                        if (property.Name == sproperty.Name)
                                            property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                                    }
                                }
                            }
                            oDocLine.UserFields = oDocLineUDF;
                            oDoc.Lines.Add(oDocLine);
                            #endregion
                        }

                        #endregion

                        if (sap.CreateDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "GRN OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            obj.DocStatus.AddDocStatus(DocStatus.Posted, "SAP Interface done");
                            obj.DocStatus.CurrDocStatus = DocStatus.Posted;
                            obj.PostVerNo = obj.VerNo;// + 1;
                            obj.SAPDocEntry = oDoc.SAPDocEntry;
                            //if (obj.DocTypeSeries.PostToDocument != PostToDocument.Draft)
                            //{
                            for (int i = 0; i < oDoc.Lines.Count; i++)
                            {
                                obj.PurchaseDeliveryDetail[i].SAPObjType = oDoc.Lines[i].SAPObjType;
                                obj.PurchaseDeliveryDetail[i].SAPDocEntry = oDoc.Lines[i].SAPDocEntry;
                                obj.PurchaseDeliveryDetail[i].SAPLineNum = oDoc.Lines[i].SAPLineNum;
                                obj.PurchaseDeliveryDetail[i].PostVerNo = obj.PurchaseDeliveryDetail[i].VerNo + 1; // PurchaseDeliveryDetail will auto +1 when save changes
                            }
                            securedObjectSpace.CommitChanges();
                            //}
                        }
                        else
                        {
                            WriteLog("[Log]", "GRN OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                            securedObjectSpace.CommitChanges();
                        }
                        oDoc = null;
                        obj = null;
                    }
                }
                #endregion

                #region PUR
                temp = ConfigurationManager.AppSettings["PURPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<PurchaseReturn> polist = ListObjectSpace.GetObjects<PurchaseReturn>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Accepted));
                    foreach (PurchaseReturn objfromlist in polist)
                    {
                        PurchaseReturn obj = securedObjectSpace.GetObjectByKey<PurchaseReturn>(objfromlist.Oid);
                        Documents oDoc = new Documents();
                        //if (obj.DocTypeSeries.PostToDocument == PostToDocument.Draft)
                        //{
                        //    oDoc.DocObject = "oDrafts";
                        //    oDoc.DocObjectCode = "oPurchaseOrders";
                        //}
                        //else
                        oDoc.DocObject = "oPurchaseReturns";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        if (obj.CardCode != null)
                        {
                            oDoc.CardCode = obj.CardCode.CardCode;
                            oDoc.CardName = obj.CardName;
                        }
                        oDoc.DocDate = obj.DocDate;
                        oDoc.DocDueDate = obj.DocDueDate;
                        oDoc.TaxDate = obj.TaxDate;

                        if (!string.IsNullOrEmpty(obj.NumAtCard))
                            oDoc.NumAtCard = obj.NumAtCard;

                        if (obj.CncttCode != null)
                            oDoc.ContactPersonCode = int.Parse(obj.CncttCode.CntctCode);
                        if (obj.SlpCode != null)
                            oDoc.SalesPersonCode = int.Parse(obj.SlpCode.SlpCode);

                        if (obj.ShipToCode != null)
                            oDoc.ShipToCode = obj.ShipToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address2))
                            oDoc.Address2 = obj.Address2;

                        if (obj.BillToCode != null)
                            oDoc.PayToCode = obj.BillToCode.Address;
                        if (!string.IsNullOrEmpty(obj.Address))
                            oDoc.Address = obj.Address;

                        if (!string.IsNullOrEmpty(obj.JrnMemo))
                            oDoc.JournalMemo = obj.JrnMemo;
                        if (!string.IsNullOrEmpty(obj.Comments))
                            oDoc.Comments = obj.Comments;

                        if (obj.Rounding != 0)
                        {
                            oDoc.Rounding = true;
                            oDoc.RoundingDiffAmount = Convert.ToDouble(obj.Rounding);
                        }
                        oDoc.DocTotal = Convert.ToDouble(obj.DocTotal);


                        #region assignhdrudf
                        DocumentsUDF oDocUDF = new DocumentsUDF();
                        System.Reflection.PropertyInfo[] properties = typeof(DocumentsUDF).GetProperties();
                        foreach (System.Reflection.PropertyInfo property in properties)
                        {
                            if (property.Name == "U_P_ID")
                                property.SetValue(oDocUDF, obj.Oid, null);
                            else if (property.Name == "U_P_DOCNO")
                                property.SetValue(oDocUDF, obj.DocNo, null);
                            else if (property.Name.Contains("U_"))
                            {
                                System.Reflection.PropertyInfo[] sproperties = typeof(ClassUDFHeader).GetProperties();
                                foreach (System.Reflection.PropertyInfo sproperty in sproperties)
                                {
                                    if (property.Name == sproperty.Name)
                                        property.SetValue(oDocUDF, sproperty.GetValue(obj.UDFs, null), null);
                                }
                            }
                        }
                        oDoc.UserFields = oDocUDF;
                        oDoc.Lines = new List<DocumentLines>();
                        #endregion

                        foreach (PurchaseReturnDetail dtl in obj.PurchaseReturnDetail)
                        {
                            DocumentLines oDocLine = new DocumentLines();
                            tempint = 0;
                            if (int.TryParse(dtl.SAPBaseType, out tempint))
                            {
                                if (tempint != 0)
                                {
                                    oDocLine.BaseType = tempint;
                                    oDocLine.BaseEntry = dtl.SAPBaseEntry;
                                    oDocLine.BaseLine = dtl.SAPBaseLine;
                                }
                            }
                            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                            oDocLine.ItemDescription = dtl.Dscription;
                            oDocLine.ItemDetails = dtl.ItemDetails;

                            if (dtl.WhsCode != null)
                                oDocLine.WarehouseCode = dtl.WhsCode.WhsCode;
                            if (dtl.OcrCode != null)
                                oDocLine.CostingCode = dtl.OcrCode.PrcCode;
                            if (dtl.OcrCode2 != null)
                                oDocLine.CostingCode2 = dtl.OcrCode2.PrcCode;
                            if (dtl.OcrCode3 != null)
                                oDocLine.CostingCode3 = dtl.OcrCode3.PrcCode;
                            if (dtl.OcrCode4 != null)
                                oDocLine.CostingCode4 = dtl.OcrCode4.PrcCode;
                            if (dtl.OcrCode5 != null)
                                oDocLine.CostingCode5 = dtl.OcrCode5.PrcCode;
                            if (dtl.PrjCode != null)
                                oDocLine.ProjectCode = dtl.PrjCode.PrjCode;
                            if (dtl.AcctCode != null)
                                oDocLine.AccountCode = dtl.AcctCode.AcctCode;

                            oDocLine.Quantity = dtl.Quantity;
                            oDocLine.UnitPrice = Convert.ToDouble(dtl.UnitPrice);

                            if (dtl.TaxCode != null)
                                oDocLine.TaxCode = dtl.TaxCode.Code;

                            if (dtl.TaxAmt != 0)
                                oDocLine.NetTaxAmount = Convert.ToDouble(dtl.TaxAmt);

                            if (dtl.FreightCharge != null)
                            {
                                if (oDocLine.Expenses == null) oDocLine.Expenses = new List<DocumentExpenses>();
                                oDocLine.Expenses.Add(new DocumentExpenses()
                                {
                                    ExpenseCode = int.Parse(dtl.FreightCharge.ExpnsCode),
                                    LineTotal = Convert.ToDouble(dtl.FreightAmt)
                                });
                            }
                            #region batch serial bin
                            if (dtl.ItemCode.ManBtchNum)
                            {
                                if (!string.IsNullOrWhiteSpace(dtl.BatchNumber))
                                {
                                    if (oDocLine.Batches == null) oDocLine.Batches = new List<DocumentBatchs>();
                                    oDocLine.Batches.Add(new DocumentBatchs()
                                    {
                                        BatchNumber = dtl.BatchNumber,
                                        Quantity = dtl.Quantity
                                    });

                                }
                            }
                            else if (dtl.ItemCode.ManSerNum)
                            {
                                if (!string.IsNullOrWhiteSpace(dtl.BatchNumber))
                                {
                                    int systemserialnumber = sap.GetSysSerialNumberFromDocLine(dtl);
                                    if (oDocLine.Serials == null) oDocLine.Serials = new List<DocumentSerials>();
                                    oDocLine.Serials.Add(new DocumentSerials()
                                    {
                                        SystemSerialNumber = systemserialnumber,
                                        //InternalSerialNumber = dtl.BatchNumber,
                                        //ManufacturerSerialNumber = dtl.BatchNumber,
                                        Quantity = dtl.Quantity
                                    });

                                }
                            }
                            if (dtl.BinCode != null && dtl.BinCode.BinAbsEntry != 0)
                            {
                                if (oDocLine.BinAllocations == null) oDocLine.BinAllocations = new List<DocumentBinAllocations>();
                                DocumentBinAllocations bin = new DocumentBinAllocations();
                                bin.BinAbsEntry = dtl.BinCode.BinAbsEntry;
                                bin.Quantity = dtl.Quantity;
                                if (dtl.ItemCode.ManBtchNum || dtl.ItemCode.ManSerNum)
                                    bin.SerialAndBatchNumbersBaseLine = 0;
                                oDocLine.BinAllocations.Add(bin);

                            }
                            #endregion

                            oDocLine.LineTotal = Convert.ToDouble(dtl.LineTotal);

                            #region assigndtludf
                            DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                            System.Reflection.PropertyInfo[] propertiesl = typeof(DocumentLinesUDF).GetProperties();
                            foreach (System.Reflection.PropertyInfo property in propertiesl)
                            {
                                if (property.Name == "U_P_ID")
                                    property.SetValue(oDocLineUDF, dtl.Oid, null);
                                else if (property.Name.Contains("U_"))
                                {
                                    System.Reflection.PropertyInfo[] spropertiesl = typeof(ClassUDFDetail).GetProperties();
                                    foreach (System.Reflection.PropertyInfo sproperty in spropertiesl)
                                    {
                                        if (property.Name == sproperty.Name)
                                            property.SetValue(oDocLineUDF, sproperty.GetValue(dtl.UDFs, null), null);
                                    }
                                }
                            }
                            oDocLine.UserFields = oDocLineUDF;
                            oDoc.Lines.Add(oDocLine);
                            #endregion
                        }

                        #endregion

                        if (sap.CreateDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "GRN OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            obj.DocStatus.AddDocStatus(DocStatus.Posted, "SAP Interface done");
                            obj.DocStatus.CurrDocStatus = DocStatus.Posted;
                            obj.PostVerNo = obj.VerNo;// + 1;
                            obj.SAPDocEntry = oDoc.SAPDocEntry;
                            //if (obj.DocTypeSeries.PostToDocument != PostToDocument.Draft)
                            //{
                            for (int i = 0; i < oDoc.Lines.Count; i++)
                            {
                                obj.PurchaseReturnDetail[i].SAPObjType = oDoc.Lines[i].SAPObjType;
                                obj.PurchaseReturnDetail[i].SAPDocEntry = oDoc.Lines[i].SAPDocEntry;
                                obj.PurchaseReturnDetail[i].SAPLineNum = oDoc.Lines[i].SAPLineNum;
                                obj.PurchaseReturnDetail[i].PostVerNo = obj.PurchaseReturnDetail[i].VerNo + 1; // PurchaseReturnDetail will auto +1 when save changes
                            }
                            securedObjectSpace.CommitChanges();
                            //}
                        }
                        else
                        {
                            WriteLog("[Log]", "GRN OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                            securedObjectSpace.CommitChanges();
                        }
                        oDoc = null;
                        obj = null;
                    }
                }
                #endregion

                GC.Collect();

                //int totalRows = dtPO.Rows.Count;
                //int totalColumns = dtPO.Columns.Count;
                //foreach (DataRow row in (dtPO == null) ? new DataTable().Rows : dtPO.Rows)
                //{
                //    DocumentLines docPOLine = GetItem<DocumentLines>(row);
                //    DocumentsUDF docPOUDF = GetItem<DocumentsUDF>(row);
                //    Documents docPO = GetItem<Documents>(row);
                //    docPO.Lines.Add(docPOLine);
                //    docPO.UserFields = docPOUDF;
                //    docPO.Series = 10;
                //    string key = null;
                //    if (sap.CreateDocuments(docPO, ref key))
                //    {
                //        WriteLog("[Log]", "PO add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                //    }
                //}


            }
            catch (Exception ex)
            {
                WriteLog("[Error]", "Message:" + ex.Message);
            }
        // End Integration ======================================================================================
        EndApplication:
            return;

            bool sendEmail = !Boolean.Parse(ConfigurationManager.AppSettings.Get("emailErrorOnly").ToString());
            foreach (KeyValuePair<string, List<string>> kvp in logs)
            {
                string status = kvp.Key;
                List<string> msgs = kvp.Value;
                if (status == "[Error]") sendEmail = true;
            }
            if (sendEmail)
            {
                WriteLog("[Log]", " ➜ Send E-Mail");
                SendEmail();
            };
            WriteLog("", "Integration End:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
        }

        private List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
           

                    if (pro.Name == translate(column.ColumnName))
                    {
                        object colObj = dr[column.ColumnName];
                        if (pro.PropertyType.Name == "DateTime")
                        {
                            colObj = DateTime.ParseExact(colObj.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                        if (pro.PropertyType.Name == "Double")
                        {
                            colObj = double.Parse(colObj.ToString());
                        }
                        if (pro.PropertyType.Name == "Int32")
                        {
                            colObj = int.Parse(colObj.ToString());
                        }
                        Console.WriteLine(pro.Name + ":" + pro.PropertyType.Name);
                        pro.SetValue(obj, colObj, null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
        private string translate(string key)
        {          
            string str = "";
            if (dict.ContainsKey(key))
            {
                str = dict[key];
            }
            return str;
        }

        private void WriteLog(string lvl, string str)
        {
            FileStream fileStream = null;

            string filePath = "C:\\SAP_Integration_Log\\";
            filePath = filePath + "[" + "Integration" + "] Log_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
            if (!dirInfo.Exists) dirInfo.Create();

            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }

            StreamWriter log = new StreamWriter(fileStream);
            string status = lvl.ToString().Replace("[Log]", "");

            //For SAP_Integration_Log
            log.WriteLine("{0}{1}", status, str.ToString());

            //For E-Mail
            createEmailLog(status + str.ToString());
            setEmailContet(lvl.ToString(), str.ToString());

            log.Close();

        }
        private void SendEmail()
        {
            // Outgoing Mail Settings
            string smtp = ConfigurationManager.AppSettings.Get("emailhost");
            string port = ConfigurationManager.AppSettings.Get("emailport");
            string user = ConfigurationManager.AppSettings.Get("emailuser");
            string pass = ConfigurationManager.AppSettings.Get("emailpass");

            string fr = ConfigurationManager.AppSettings.Get("emailfr");
            string fn = ConfigurationManager.AppSettings.Get("emailfn");
            string to = ConfigurationManager.AppSettings.Get("emailto");
            string cc = ConfigurationManager.AppSettings.Get("emailcc");

            Boolean ssl = Boolean.Parse(ConfigurationManager.AppSettings["enableSSL"].ToString());

            string pattern = "[;,| ]";

            MailMessage oMail = new MailMessage();
            oMail.From = new MailAddress(fr, fn);
            foreach (string t in Regex.Split(to, pattern) ?? Enumerable.Empty<string>())
            {
                if (t.Equals("")) { continue; }
                oMail.To.Add(new MailAddress(t));
            }
            foreach (string c in Regex.Split(cc, pattern) ?? Enumerable.Empty<string>())
            {
                if (c.Equals("")) { continue; }
                oMail.CC.Add(new MailAddress(c));
            }

            string body = string.Empty;
            string desc = string.Empty;
            foreach (KeyValuePair<string, List<string>> kvp in logs)
            {
                string status = kvp.Key;
                List<string> msgs = kvp.Value;

                if (status == "[Log]") continue;
                desc += "<br/><b class='btn-'" + status + " btn'>" + status.Replace("[", "").Replace("]", "") + ":-</b>";
                desc += "<ul style = 'list-style-type:none'>";
                foreach (string msg in msgs)
                {
                    desc += "<li>" + msg + "</li>";
                }
                desc += "</ul>";
            }
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string file = dir + @"\temp.html";
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", "User");
            body = body.Replace("{Title}", "SAP Integration");
            body = body.Replace("{Url}", "http://www.company.com.my");
            body = body.Replace("{website}", "Contact US");
            body = body.Replace("{Description}", desc);

            Attachment oAttach = new Attachment(createEmailLog(""));
            oMail.Attachments.Add(oAttach);

            oMail.Subject = "SAP Integration";
            oMail.Body = body;
            oMail.IsBodyHtml = true;
            oMail.BodyEncoding = Encoding.UTF8;
            oMail.SubjectEncoding = Encoding.UTF8;

            SmtpClient oSTMP = new SmtpClient();
            oSTMP.Host = smtp;
            oSTMP.Port = int.Parse(port);
            oSTMP.EnableSsl = ssl;
            oSTMP.DeliveryMethod = SmtpDeliveryMethod.Network;
            oSTMP.Credentials = new NetworkCredential(user, pass);
            oSTMP.Send(oMail);
            oSTMP.Dispose();
            oMail.Attachments.Dispose();
        }
        private string createEmailLog(object strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo dirInfo = null;
            FileInfo fileInfo;

            string filePath = Path.GetTempPath() + "\\";

            filePath = filePath + "LOG_" + System.DateTime.Today.ToString("yyyyMMddhhmmtt") + "." + "txt";
            fileInfo = new FileInfo(filePath);
            dirInfo = new DirectoryInfo(fileInfo.DirectoryName);

            if (!dirInfo.Exists) dirInfo.Create();

            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine("{0}", strLog.ToString());
            log.Close();
            return filePath;
        }
        private void setEmailContet(string lvl, string str)
        {
            if (!logs.ContainsKey(lvl))
            {
                logs.Add(lvl, new List<string>());
            }
            logs[lvl].Add(str.TrimEnd());
        }


        private string getfulladdress(string ToDef, string ToAdress1, string ToAdress2, string ToAdress3, string ToAdress4, string ToCountry)
        {
            string rtn = "";
            rtn += ToDef != null && ToDef.Trim().Length > 0 ? rtn.Length > 0 ? System.Environment.NewLine + ToDef : ToDef : "";
            rtn += ToAdress1 != null && ToAdress1.Trim().Length > 0 ? rtn.Length > 0 ? System.Environment.NewLine + ToAdress1 : ToAdress1 : "";
            rtn += ToAdress2 != null && ToAdress2.Trim().Length > 0 ? rtn.Length > 0 ? System.Environment.NewLine + ToAdress2 : ToAdress2 : "";
            rtn += ToAdress3 != null && ToAdress3.Trim().Length > 0 ? rtn.Length > 0 ? System.Environment.NewLine + ToAdress3 : ToAdress3 : "";
            rtn += ToAdress4 != null && ToAdress4.Trim().Length > 0 ? rtn.Length > 0 ? System.Environment.NewLine + ToAdress4 : ToAdress4 : "";
            rtn += ToCountry != null && ToCountry.Trim().Length > 0 ? rtn.Length > 0 ? " " + ToCountry : ToCountry : "";

            return rtn;

        }
    }



}

