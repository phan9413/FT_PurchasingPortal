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
                string draft = "";
                IObjectSpace securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                int cnt = 0;
                string key = "";

                draft = ConfigurationManager.AppSettings["PRDraft"].ToString().ToUpper();
                temp = ConfigurationManager.AppSettings["PRPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<PurchaseRequest> prlist = securedObjectSpace.GetObjects<PurchaseRequest>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                    foreach (PurchaseRequest obj in prlist)
                    {
                        cnt++;
                        obj.JrnMemo = cnt.ToString();
                        Documents oDoc = new Documents();
                        if (draft == "Y" || draft == "YES" || draft == "TRUE" || draft == "1")
                        {
                            oDoc.DocObject = "oDrafts";
                            oDoc.DocObjectCode = "oPurchaseRequest";
                        }
                        else
                            oDoc.DocObject = "oPurchaseRequest";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        oDoc.DocDate = obj.DocDate;
                        oDoc.DocDueDate = obj.DocDueDate;
                        oDoc.TaxDate = obj.TaxDate;
                        oDoc.RequriedDate = obj.ReqDate;

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

                        if (obj.ShipToCode != null)
                            oDoc.ShipToCode = obj.ShipToCode.Address;
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

                        foreach (PurchaseRequestDetail dtl in obj.PurchaseRequestDetail)
                        {
                            DocumentLines oDocLine = new DocumentLines();
                            oDocLine.ItemCode = dtl.ItemCode.ItemCode;
                            oDocLine.ItemDescription = dtl.Dscription;
                            oDocLine.ItemDetails = dtl.ItemDetails;

                            if (dtl.LineVendor != null)
                                oDocLine.LineVendor = dtl.LineVendor.CardCode;
                            else if (obj.CardCode != null)
                                oDocLine.LineVendor = obj.CardCode.CardCode;

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
                                oDocLine.Expenses.Add(new DocumentLinesExpenses()
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
                            WriteLog("[Log]", "PR OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            securedObjectSpace.CommitChanges();
                        }
                        else
                        {
                            WriteLog("[Log]", "PR OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                        }
                    }
                }

                draft = ConfigurationManager.AppSettings["PODraft"].ToString().ToUpper();
                temp = ConfigurationManager.AppSettings["POPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<PurchaseOrder> polist = securedObjectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                    foreach (PurchaseOrder obj in polist)
                    {
                        cnt++;
                        obj.JrnMemo = cnt.ToString();
                        Documents oDoc = new Documents();
                        if (draft == "Y" || draft == "YES" || draft == "TRUE" || draft == "1")
                        {
                            oDoc.DocObject = "oDrafts";
                            oDoc.DocObjectCode = "oPurchaseOrders";
                        }
                        else
                            oDoc.DocObject = "oPurchaseOrders";

                        #region posttosap
                        if (obj.DocTypeSeries.SAPSeries > 0)
                            oDoc.Series = obj.DocTypeSeries.SAPSeries;

                        oDoc.CardCode = obj.CardCode.CardCode;
                        oDoc.CardName = obj.CardName;

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

                        if (obj.ShipToCode != null)
                            oDoc.ShipToCode = obj.ShipToCode.Address;
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
                                oDocLine.Expenses.Add(new DocumentLinesExpenses()
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
                            securedObjectSpace.CommitChanges();
                        }
                        else
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                        }
                    }
                }

                draft = ConfigurationManager.AppSettings["STRDraft"].ToString().ToUpper();
                temp = ConfigurationManager.AppSettings["STRPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    IList<StockTransferRequest> polist = securedObjectSpace.GetObjects<StockTransferRequest>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                    foreach (StockTransferRequest obj in polist)
                    {
                        cnt++;
                        obj.JrnMemo = cnt.ToString();
                        StockTransfer oDoc = new StockTransfer();
                        if (draft == "Y" || draft == "YES" || draft == "TRUE" || draft == "1")
                            oDoc.DocObjectCode = "oStockTransferDraft";
                        else
                            oDoc.DocObjectCode = "oInventoryTransferRequest";



                        if (sap.CreateStockTransferDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.IsSAPPosted = true;
                            securedObjectSpace.CommitChanges();
                        }
                        else
                        {
                            WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                            obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                            obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                            obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                        }
                    }
                }

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

