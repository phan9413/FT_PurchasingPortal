using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace SAP_Integration
{
    public partial class MainForm : Form
    {
        public string defuserid { get; set; }
        public string defpassword { get; set; }
        public bool autopostafterlogin { get; set; }
        public bool autologin { get; set; }

        private IObjectSpace securedObjectSpace;
        public SecurityStrategyComplex Security { get; set; }
        public IObjectSpaceProvider ObjectSpaceProvider { get; set; }
        public MainForm(SecurityStrategyComplex security, IObjectSpaceProvider objectSpaceProvider)
        {
            InitializeComponent();
            Security = security;
            ObjectSpaceProvider = objectSpaceProvider;

        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            txtUserID.Text = defuserid;
            txtPassword.Text = defpassword;
            Post.Enabled = false;
            if (autologin)
            {
                Login.PerformClick();
            }
        }

        private void Login_Click(object sender, EventArgs e)
        {
            string userName = txtUserID.Text;
            string password = txtPassword.Text;

            if (!Security.IsAuthenticated)
            {
                securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                Security.Authentication.SetLogonParameters(new AuthenticationStandardLogonParameters(userName, password));
                try
                {
                    Security.Logon(securedObjectSpace);
                    //DialogResult = DialogResult.OK;
                    //Close();
                    txtUserID.Enabled = false;
                    txtPassword.Enabled = false;
                    FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
                    FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = Security.UserName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Login.Enabled = false;
            Post.Enabled = true;
            if (autopostafterlogin)
            {
                Post.PerformClick();
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Security.IsAuthenticated)
                Security.Logoff();
        }

        private void Posting()
        {
            IObjectSpace ios = ObjectSpaceProvider.CreateObjectSpace();
            IList<PurchaseRequest> prlist = ios.GetObjects<PurchaseRequest>().ToList();
            IList<PurchaseOrder> polist = ios.GetObjects<PurchaseOrder>().ToList();
            Code obj = new Code(Security, ObjectSpaceProvider);

            txtUserID.Enabled = true;
            txtPassword.Enabled = true;
        }

        private void Post_Click(object sender, EventArgs e)
        {
            Posting();

            this.Close();

            Application.Exit();
            GC.Collect();
        }
    }
}
