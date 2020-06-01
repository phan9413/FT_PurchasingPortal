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

namespace SAP_Integration
{
    class Backup
    {
        private static string conString = ConfigurationManager.ConnectionStrings["DataSourceConnectionString"].ToString();
        string smtp = ConfigurationManager.AppSettings.Get("emailhost");
        string port = ConfigurationManager.AppSettings.Get("emailport");
        string user = ConfigurationManager.AppSettings.Get("emailuser");
        string pass = ConfigurationManager.AppSettings.Get("emailpass");
        string fr = ConfigurationManager.AppSettings.Get("emailfr");
        string fn = ConfigurationManager.AppSettings.Get("emailfn");
        Boolean ssl = Boolean.Parse(ConfigurationManager.AppSettings["EnableSSL"].ToString());
        int errCode = 0;
        string errMsg = "";

        private static SortedDictionary<string, List<string>> logs = new SortedDictionary<string, List<string>>();

        public Backup()
        {
            logs.Clear();
            WriteLog("[Log]", "--------------------------------------------------------------------------------");
            WriteLog("[Log]", "Integration Begin:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");


            #region Connect to SAP            
            SAPbobsCOM.Company oCom = new SAPbobsCOM.Company();
            string dbServerType = ConfigurationManager.AppSettings.Get("dbServerType");
            if (dbServerType == "MSSQL2005")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005;
            }
            else if (dbServerType == "MSSQL2008")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
            }
            else if (dbServerType == "MSSQL2012")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            }
            else if (dbServerType == "MSSQL2014")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
            }
            else if (dbServerType == "HANADB")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            }
            else if (dbServerType == "DB_2")
            {
                oCom.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_DB_2;
            }
            oCom.Server = ConfigurationManager.AppSettings.Get("Server");
            oCom.DbUserName = ConfigurationManager.AppSettings.Get("dbuser");
            oCom.DbPassword = ConfigurationManager.AppSettings.Get("dbpass");
            oCom.LicenseServer = ConfigurationManager.AppSettings.Get("LicenseServer");
            oCom.CompanyDB = ConfigurationManager.AppSettings.Get("CompanyDB");
            oCom.UserName = ConfigurationManager.AppSettings.Get("UserName");
            oCom.Password = ConfigurationManager.AppSettings.Get("Password");
            oCom.language = SAPbobsCOM.BoSuppLangs.ln_English;
            if (oCom.Connect() != 0)
            {
                WriteLog("[Error]", "Connection:[" + oCom.CompanyDB + "] Message:[" + oCom.GetLastErrorDescription() + "] Time:[" + DateTime.Now.ToString("hh: mm:ss tt") + "]");
                goto EndApplication;
            }

            WriteLog("[Log]", "Connected to:[" + oCom.CompanyName + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
            #endregion


            #region Connect to Data Source
            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand("", conn);
            conn.Close();
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                WriteLog("[Error]", "Data Source Connection : " + DateTime.Now.ToString() + " : " + ex.Message);
                goto EndApplication;
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(conString);
            string server = builder.DataSource;
            string database = builder.InitialCatalog;
            WriteLog("[Log]", "Connected to Data Source:[" + database + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");


            cmd.CommandText = "SELECT ROW_NUMBER() over (Partition by BaseEntry Order by BaseEntry )-1 as [Line], * " +
                                "FROM [dbo].[ProductionReceipt] T0 Where IsNull(T0.Posted,'N') <> 'Y'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            int nGR = dt.Rows.Count;
            if (nGR == 0)
            {
                WriteLog("[Log]", "No production receipt to import!");
                conn.Close();
                da.Dispose();
                goto EndApplication;
            }

            List<int> idList = new List<int>();
            Dictionary<int, DataTable> doc = new Dictionary<int, DataTable>();

            //Parsing data into header and details
            foreach (DataRow row in dt.Rows)
            {
                int baseEntry = int.Parse(row["BaseEntry"].ToString());
                if (doc.ContainsKey(baseEntry))
                {
                    DataTable dtDoc = doc[baseEntry];
                    dtDoc.ImportRow(row);
                    doc[baseEntry] = dtDoc;
                }
                else
                {
                    DataTable dtDoc = dt.Clone();
                    dtDoc.ImportRow(row);
                    doc.Add(baseEntry, dtDoc);
                }
            }

            WriteLog("[Log]", "Record found:[" + doc.Keys.Count + "] Total Rows:[" + dt.Rows.Count + "]");


            oCom.StartTransaction();

            int n = 0;
            foreach (var item in doc)
            {
                try
                {
                    SAPbobsCOM.Documents oDocReceipt = (SAPbobsCOM.Documents)oCom.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                    SAPbobsCOM.ProductionOrders oDocProductionOrders = (SAPbobsCOM.ProductionOrders)oCom.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);
                    SAPbobsCOM.Items oItem = oCom.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);

                    DataTable dtDoc = doc[item.Key];
                    DataRow hRow = dtDoc.Rows[0];

                    int retcode = 0;
                    idList = new List<int>();

                    string itemCode = hRow["ItemCode"].ToString();
                    oItem.GetByKey(itemCode);
                    SAPbobsCOM.BoYesNoEnum isManageBatchNumbers = oItem.ManageBatchNumbers;


                    WriteLog("[Log]", "No:[" + (++n) + "] H:Production Order[" + hRow["DocNum"].ToString() + "] Base:[" + item.Key + "] TotalRows:[" + dtDoc.Rows.Count + "]" + " PlannedQty:" + hRow["Quantity"].ToString());
                    // Add Header -----------------------------------
                    string proNo = hRow["DocNum"].ToString();
                    int baseEntry = int.Parse(hRow["BaseEntry"].ToString());
                    oDocReceipt.Series = 53;
                    oDocReceipt.DocDate = DateTime.Parse(hRow["DocDate"].ToString());
                    oDocReceipt.DocDueDate = DateTime.Parse(hRow["DocDueDate"].ToString());
                    oDocReceipt.Comments = hRow["Comments"].ToString();
                    oDocReceipt.JournalMemo = hRow["JournalMemo"].ToString();
                    // Add Details start ----------------------------
                    for (int i = 0; i < dtDoc.Rows.Count; i++)
                    {
                        DataRow row = dtDoc.Rows[i];
                        int id = int.Parse(row["Id"].ToString());

                        WriteLog("[Log]", "Id:[" + row["Id"].ToString() + "] R:[" + i + "] ItemCode:" + itemCode + "] BatchNo:" + row["DistNumber"].ToString() + " " + row["status"].ToString() + " Qty:" + row["BatchQuantity"].ToString());

                        idList.Add(id);
                        oDocReceipt.Lines.Add();
                        oDocReceipt.Lines.SetCurrentLine(i);
                        oDocReceipt.Lines.BaseType = (int)SAPbobsCOM.BoObjectTypes.oProductionOrders;
                        oDocReceipt.Lines.BaseEntry = int.Parse(row["BaseEntry"].ToString());
                        //oDocReceipt.Lines.BaseLine = 0;
                        oDocReceipt.Lines.Quantity = Double.Parse(row["BatchQuantity"].ToString()); //Batch Quantity = Receipt Quantity
                        oDocReceipt.Lines.WarehouseCode = "KH010";
                        oDocReceipt.Lines.FreeText = row["Notes"].ToString();

                        //oDocReceipt.Lines.WarehouseCode = row["WarehouseCode"].ToString();
                        string status = row["Status"].ToString().Trim();
                        oDocReceipt.Lines.TransactionType = (status == "AC") ? SAPbobsCOM.BoTransactionTypeEnum.botrntComplete : SAPbobsCOM.BoTransactionTypeEnum.botrntReject;

                        if (isManageBatchNumbers.Equals("tYES"))
                        {
                            oDocReceipt.Lines.BatchNumbers.BatchNumber = row["DistNumber"].ToString();
                            oDocReceipt.Lines.BatchNumbers.Quantity = Double.Parse(row["BatchQuantity"].ToString());
                            if (!String.IsNullOrEmpty(row["ExpiryDate"].ToString())) oDocReceipt.Lines.BatchNumbers.ExpiryDate = DateTime.Parse(row["ExpiryDate"].ToString());
                            if (!String.IsNullOrEmpty(row["MnfDate"].ToString())) oDocReceipt.Lines.BatchNumbers.ManufacturingDate = DateTime.Parse(row["MnfDate"].ToString());
                            if (!String.IsNullOrEmpty(row["InDate"].ToString())) oDocReceipt.Lines.BatchNumbers.AddmisionDate = DateTime.Parse(row["InDate"].ToString());
                            oDocReceipt.Lines.BatchNumbers.Notes = row["Notes"].ToString();
                            oDocReceipt.Lines.BatchNumbers.Add();
                        }

                        // add line end --------------------
                    }

                    retcode = oDocReceipt.Add();
                    if (retcode == 0)
                    {
                        // Success
                        string docEntry = "";
                        string docNum = "";

                        oCom.GetNewObjectCode(out docEntry);
                        if (docEntry == "")
                        {
                            errMsg = "Unknown Error! Please try again!";
                            if (oCom.InTransaction) oCom.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            WriteLog("[Error]", "Production Receipt:" + errMsg);
                            continue;
                        }
                        SAPbobsCOM.Documents sapDoc = (SAPbobsCOM.Documents)oCom.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                        if (sapDoc.GetByKey(int.Parse(docEntry)))
                        {
                            docNum = sapDoc.DocNum.ToString();
                        }

                        foreach (int id in idList)
                        {
                            string str = "exec sp_FT_UpdateProductionOrder '" + id + "','Y','" + "" + "','" + docNum + "'";
                            SqlCommand cmdExec = new SqlCommand(str, conn);
                            cmdExec.ExecuteNonQuery();
                            WriteLog("[Success]", "Production Receipt:[" + docNum + "] has been added!");
                        }

                        if (oCom.InTransaction) oCom.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                        cmd.CommandText = "SELECT CASE WHEN Sum(isNull(BatchQuantity,0.00)) >= MAX(T0.Quantity) Then 1 else 0 END " +
                        "FROM[dbo].[ProductionReceipt] T0 " +
                        "Where ISNULL(T0.Posted,'N') = 'Y' AND T0.BaseEntry=" + baseEntry + " GROUP BY T0.BaseEntry";

                        Int32 totalReceived = (Int32)cmd.ExecuteScalar();

                        // Closed Production
                        if (oDocProductionOrders.GetByKey(baseEntry) && totalReceived == 1)
                        {
                            string msg = "Production Order:[" + oDocProductionOrders.DocumentNumber.ToString() + "] has been closed!";
                            string rmk = oDocProductionOrders.Remarks;
                            oDocProductionOrders.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposClosed;
                            oDocProductionOrders.Remarks = rmk + " :closed by integration";
                            //oDocProductionOrders.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposCancelled;
                            oDocProductionOrders.Update();
                            WriteLog("[Success]", msg);
                            foreach (int id in idList)
                            {
                                string str = "exec sp_FT_UpdateProductionOrder '" + id + "','Y','" + msg + "','" + docNum + "'";
                                SqlCommand cmdExec = new SqlCommand(str, conn);
                                cmdExec.ExecuteNonQuery();
                            }

                        }
                    }
                    else
                    {  // Error

                        oCom.GetLastError(out errCode, out errMsg);
                        foreach (int id in idList)
                        {
                            string str = "exec sp_FT_UpdateProductionOrder '" + id + "','N','" + errMsg.Replace("'", "") + "',''";
                            SqlCommand cmdExec = new SqlCommand(str, conn);
                            cmdExec.ExecuteNonQuery();
                        }
                        WriteLog("[Error]", "Production Order:[" + proNo + "] Error Msg:" + oCom.GetLastErrorDescription());
                        if (oCom.InTransaction) oCom.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }

                }
                catch (Exception ex)
                {
                    oCom.GetLastError(out errCode, out errMsg);
                    string msg = (ex.Message + " SAP msg:" + errMsg).Replace("'", "");
                    foreach (int id in idList)
                    {
                        string str = "exec sp_FT_UpdateProductionOrder '" + id + "','N','" + msg + "','" + "" + "'";
                        SqlCommand cmdExec = new SqlCommand(str, conn);
                        cmdExec.ExecuteNonQuery();
                    }
                    WriteLog("[Error]", "Production Receipt:" + msg);
                }
            }
        #endregion

        EndApplication:

            bool sendEmail = false;
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
            WriteLog("", "Integration End");
            //Thread.Sleep(3000);

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
    }



}

