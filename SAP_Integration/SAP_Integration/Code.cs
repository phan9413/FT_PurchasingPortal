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

                IObjectSpace securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                IList<PurchaseRequest> prlist = securedObjectSpace.GetObjects<PurchaseRequest>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));
                IList<PurchaseOrder> polist = securedObjectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("DocStatus.IsSAPPosted=0 and DocStatus.CurrDocStatus=?", DocStatus.Posted));

                int cnt = 0;
                string key = "";
                foreach (PurchaseRequest obj in prlist)
                {
                    cnt++;
                    obj.JrnMemo = cnt.ToString();
                    Documents oDoc = new Documents();
                    if (sap.CreateDocuments(oDoc, ref key))
                    {
                        WriteLog("[Log]", "PR OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                        obj.DocStatus.IsSAPPosted = true;
                    }
                    else
                    {
                        WriteLog("[Log]", "PR OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                        obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                        obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                        obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                    }
                }
                foreach (PurchaseOrder obj in polist)
                {
                    cnt++;
                    obj.JrnMemo = cnt.ToString();
                    Documents oDoc = new Documents();
                    if (sap.CreateDocuments(oDoc, ref key))
                    {
                        WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                        obj.DocStatus.IsSAPPosted = true;
                    }
                    else
                    {
                        WriteLog("[Log]", "PO OID:[" + obj.Oid.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
                        obj.DocStatus.AddDocStatus(DocStatus.PostedCancel, sap.errMsg);
                        obj.DocStatus.CurrDocStatus = DocStatus.PostedCancel;
                        obj.DocStatus.SAPPostCancelRemarks = sap.errMsg;
                    }
                }

                securedObjectSpace.CommitChanges();
                /*
                DataSet ds = sap.getDataSet(LocalCurrency);
                DataTable dtDoc = ds.Tables["SalesInvoice"];
                DataTable dtDocLine = ds.Tables["SalesInvoiceDetails"];
                string temp = "";
                double doutemp = 0;
                double Quantity = 0, UnitPrice = 0, LineTotal = 0, Rounding = 0;
                int slittingChargeFreightCode = 1, transportChargeFreightCode = 2;
                double slittingCharge = 0, transportCharge = 0;
                int masterOID = 0;
                string sql = "";
                double linetotal = 0;
                double doctotal = 0;
                #region SalesInvoice
                foreach (DataRow docRow in dtDoc.Rows)
                {
                    if (docRow["DocObject"].ToString() == "0")
                        continue;

                    bool dtlfound = false;
                    
                    Documents oDoc = new Documents();
                    masterOID = int.Parse(docRow["OID"].ToString());

                    DataRow[] selectedRows = dtDocLine.Select("MasterBO = " + masterOID.ToString());

                    foreach (DataRow docLine in selectedRows)
                    {
                        if (!dtlfound)
                        {
                            if (docRow["DocObject"].ToString() == "1")
                            {
                                oDoc.DocObject = "oDrafts";
                                oDoc.DocObjectCode = "oInvoices";
                            }
                            else if (docRow["DocObject"].ToString() == "2")
                                oDoc.DocObject = "oInvoices";

                            if ((int)docRow["DocSeries"] > 0)
                                oDoc.Series = (int)docRow["DocSeries"];

                            oDoc.DocType = 0;
                            oDoc.CardCode = docRow["CardCode"].ToString();
                            oDoc.CardName = docRow["CardName"].ToString();
                            temp = docRow["MarketingExecutive"] == null ? "0" : docRow["MarketingExecutive"].ToString();
                            if (temp.Trim().Length == 0) temp = "0";
                            if (int.Parse(temp) > 0)
                                oDoc.SalesPersonCode = int.Parse(temp);
                            oDoc.DocDate = (DateTime)docRow["DocumentDate"];
                            oDoc.TaxDate = (DateTime)docRow["PostingDate"];
                            oDoc.NumAtCard = docRow["DocNo"].ToString();
                            if (LocalCurrency != docRow["Currency"].ToString())
                            {
                                oDoc.DocRate = double.Parse(docRow["CurrencyRate"].ToString());
                            }
                            temp = docRow["Project"] == null ? "" : docRow["Project"].ToString();
                            if (temp.Trim().Length > 0)
                                oDoc.Project = temp;

                            doutemp = double.Parse(docRow["Discount"].ToString());
                            if (doutemp != 0)
                            {
                                oDoc.Rounding = true;
                                oDoc.RoundingDiffAmount = doutemp * -1;
                            }
                            string ToDef = docRow["BillToDef"] == null ? "" : docRow["BillToDef"].ToString();
                            string ToAdress1 = docRow["BillToAdress1"] == null ? "" : docRow["BillToAdress1"].ToString();
                            string ToAdress2 = docRow["BillToAdress2"] == null ? "" : docRow["BillToAdress2"].ToString();
                            string ToAdress3 = docRow["BillToAdress3"] == null ? "" : docRow["BillToAdress3"].ToString();
                            string ToAdress4 = docRow["BillToAdress4"] == null ? "" : docRow["BillToAdress4"].ToString();
                            string ToCountry = docRow["BillToCountry"] == null ? "" : docRow["BillToCountry"].ToString();
                            oDoc.Address = getfulladdress(ToDef, ToAdress1, ToAdress2, ToAdress3, ToAdress4, ToCountry);

                            ToDef = docRow["ShipToDef"] == null ? "" : docRow["ShipToDef"].ToString();
                            ToAdress1 = docRow["ShipToAdress1"] == null ? "" : docRow["ShipToAdress1"].ToString();
                            ToAdress2 = docRow["ShipToAdress2"] == null ? "" : docRow["ShipToAdress2"].ToString();
                            ToAdress3 = docRow["ShipToAdress3"] == null ? "" : docRow["ShipToAdress3"].ToString();
                            ToAdress4 = docRow["ShipToAdress4"] == null ? "" : docRow["ShipToAdress4"].ToString();
                            ToCountry = docRow["ShipToCountry"] == null ? "" : docRow["ShipToCountry"].ToString();
                            oDoc.Address2 = getfulladdress(ToDef, ToAdress1, ToAdress2, ToAdress3, ToAdress4, ToCountry);

                            DocumentsUDF oDocUDF = new DocumentsUDF();
                            oDocUDF.U_P_ID = masterOID;
                            oDocUDF.U_P_MANUAL_DOCNUM = docRow["ManualDocumentNo"].ToString();
                            oDocUDF.U_P_PRNO = docRow["PRNo"].ToString();
                            oDocUDF.U_P_REFNUM = docRow["RefNo"].ToString();

                            oDoc.UserFields = oDocUDF;

                            oDoc.Lines = new List<DocumentLines>();
                        }
                        dtlfound = true;

                        DocumentLines oDocLine = new DocumentLines();
                        oDocLine.ItemCode = docLine["ItemCode"].ToString();
                        oDocLine.ItemDescription = docLine["ItemName"].ToString();
                        oDocLine.ItemDetails = docLine["DocumentDescription"].ToString();

                        Quantity = double.Parse(docLine["Quantity"].ToString());
                        UnitPrice = double.Parse(docLine["UnitPrice"].ToString());
                        LineTotal = double.Parse(docLine["LineTotal"].ToString());
                        oDocLine.Quantity = UnitPrice == 0 ? Quantity : Math.Round(LineTotal / UnitPrice, 3, MidpointRounding.AwayFromZero);
                        oDocLine.UnitPrice = UnitPrice;
                        oDocLine.LineTotal = LineTotal;

                        oDocLine.TaxCode = docLine["TaxCode"].ToString();

                        temp = docLine["WH"] == null ? "" : docLine["WH"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLine.WarehouseCode = temp;
                        temp = docLine["ProductionLocation"] == null ? "" : docLine["ProductionLocation"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLine.CostingCode = temp;
                        temp = docLine["U_ITEM_FAMILY_CODE"] == null ? "" : docLine["U_ITEM_FAMILY_CODE"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLine.CostingCode2 = temp;
                        temp = docLine["Project"] == null ? "" : docLine["Project"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLine.ProjectCode = temp;

                        DocumentLinesUDF oDocLineUDF = new DocumentLinesUDF();
                        oDocLineUDF.U_P_ID = docLine["OID"].ToString();
                        oDocLineUDF.U_P_QUANTITY = double.Parse(docLine["Quantity"].ToString());
                        oDocLineUDF.U_P_LENGTH = double.Parse(docLine["LenghtMM"].ToString());
                        oDocLineUDF.U_P_WIDTH = double.Parse(docLine["WidthMM"].ToString());
                        oDocLineUDF.U_PRICE_UOM = docLine["SalesUom"].ToString();

                        oDocLine.UserFields = oDocLineUDF;

                        oDoc.Lines.Add(oDocLine);
                        
                    }

                    doutemp = double.Parse(docRow["TripCost"].ToString());
                    if (doutemp != 0)
                    {
                        DocumentLines oDocLineTrip = new DocumentLines();
                        oDocLineTrip.ItemCode = docRow["TripItemCode"].ToString();
                        oDocLineTrip.TaxCode = docRow["TripTaxCode"].ToString();
                        oDocLineTrip.Quantity = 1;
                        oDocLineTrip.UnitPrice = doutemp;
                        temp = docRow["TripCostCenter"] == null ? "" : docRow["TripCostCenter"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLineTrip.CostingCode = temp;
                        temp = docRow["U_ITEM_FAMILY_CODE"] == null ? "" : docRow["U_ITEM_FAMILY_CODE"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLineTrip.CostingCode2 = temp;
                        temp = docRow["Project"] == null ? "" : docRow["Project"].ToString();
                        if (temp.Trim().Length > 0)
                            oDocLineTrip.ProjectCode = temp;

                        oDoc.Lines.Add(oDocLineTrip);
                    }

                    if (dtlfound)
                    {
                        sql = "";
                        if (sap.CreateDocuments(oDoc, ref key))
                        {
                            WriteLog("[Log]", "SalesInvoice  OID:[" + masterOID.ToString() + "] add Success:[" + key + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");

                            sql = "update SalesInvoice set SAPPosted = 1, PostCancelRemarks = '" + key + "' where OID = " + masterOID.ToString();
                        }
                        else
                        {
                            WriteLog("[Log]", "SalesInvoice OID:[" + masterOID.ToString() + "] add Failed:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");

                            sql = "update SalesInvoice set Status = 7, PostCancelRemarks = '" + sap.errMsg + "' where OID = " + masterOID.ToString();
                        }
                        if (sql != "")
                        {
                            SqlCommand mycmd = new SqlCommand();
                            mycmd.CommandText = sql;
                            mycmd.Connection = sap.sqlCon;
                            mycmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        WriteLog("[Log]", "SalesInvoice OID:[" + masterOID.ToString() + "] add Failed:[No details found] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");

                        sql = "update SalesInvoice set Status = 7, PostCancelRemarks = 'No details found' where OID = " + masterOID.ToString();
                        SqlCommand mycmd = new SqlCommand();
                        mycmd.CommandText = sql;
                        mycmd.Connection = sap.sqlCon;
                        mycmd.ExecuteNonQuery();
                    }
                }
                #endregion
                */

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

