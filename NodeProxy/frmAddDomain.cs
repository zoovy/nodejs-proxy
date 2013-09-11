using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NodeProxy
{
    public partial class frmAddDomain : Form
    {
        public frmAddDomain()
        {
            InitializeComponent();
            txtDomaiName.Text = "www.zoovy.com";
            txtProjectDir.Text = "C:\\users\\brian\\documents\\github\\www-zoovy-htdocs";
        }

        public string DomainName = "";
        public string ProjectDir = "";

        // btnAdd - Triggers DialogResult.OK
        //
        // textboxes declared as private variable are not visible outside this form
        // 
        // need to set DomainName and ProjectDir
        // for frmMain - so it can save the settings to ini and create the test certificate
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DomainName = txtDomaiName.Text;
            ProjectDir = txtProjectDir.Text;
 
        }
        // btnBrowse
        //
        // Displays folder dialog so user can select where the project directory is location
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog ProjectFolderDialog = new FolderBrowserDialog();
            // starts the folder dialog in the user's my documents folders
            // Can not use RootFolder - RootFolder only shows the folders and the sub folders of its folder
            // You can not browse to other folders above the RootFolder if set
            // ie: set MyDocuments - can only see MyDocuments can not browse to desktop or the c: drive
            //ProjectFolderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
            // instead use the SelectedPath to set the start folder in folder dialog

            folderPath = Environment.SpecialFolder.MyDocuments.ToString();
            ProjectFolderDialog.SelectedPath = folderPath;
        
            if (ProjectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = ProjectFolderDialog.SelectedPath.ToString();
                 
                // displays folder was chosen to the user
                txtProjectDir.Text = folderPath; 
            }


        }


    }
}
