using System;
using System.Collections.Generic;
using System.Collections; 
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenSSL.X509;
using OpenSSL.Crypto;
using OpenSSL.Core;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Nini.Config;

using System.Net;


namespace NodeProxy
{
    public partial class frmMain : Form
    {
        // Get the Proxy Information from the registry
        private string OrigProxyEnable;
        private string OrigProxyServer;
        private const string ProxyEnable = "ProxyEnable";
        private const string ProxyServer = "ProxyServer";



        static System.Diagnostics.Process CMDprocess;
        static System.Diagnostics.ProcessStartInfo startInfo;
        private static bool processDone = false;
        string appPath;

        // Name of the ini file that will be installed in the app.exe location
        private static string NodeIniFile = "\\ProxyConfig.ini";
        private IConfigSource source = null;

        // Constants values for the ini 
        //
        // Prevents typos when referring the config name of keys in the inki
        private const string MainIniConfig = "NodeConfig";
        private const string NodeInstallKey = "NodeInstallDir";
        private const string NodeProxyAddrKey = "NodeProxyAddress";
        private const string AutoProxyKey = "AutoEnableProxy";
        private const string DomainCfgsKey = "DomainConfigs";

        private const string DomainKey = "domain";
        private const string ProjectDirKey = "projectdir";
        private const string DomainCertKey = "cert";

        private List<string> DomainKeys;
        private Hashtable DomainsHash;
        

        private string NodeInstallDir;
        private string NodeProxyAddress;

        private string ProjectDir;
        private string CertKeyFileName;
        private string CertFileName;

        private ContextMenuStrip mnuNodeProxy;

        private string NodeProxyLog;

        


        public frmMain()
        {
            InitializeComponent();
            Load += new EventHandler(frmMain_Load);
            Resize += new EventHandler(frmMain_Resize);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);
            Use_Notify(); // Setting up all Property of Notifyicon 

            LoadProxyIni();

            LoadDomains();
        }

        //
        // Uses a library called Nini from sourceforge 
        // http://nini.sourceforge.net/
        // http://sourceforge.net/projects/nini/?source=directory
        //
        // Intialize the default values of the program by opening and loading the ini 
        private void LoadProxyIni()
        {
            string AutoEnableStr;
            string DomainCfgs;

            // Load the configuration source file
            source = new IniConfigSource(appPath + NodeIniFile );
            // gets the values from each key
            //
            // NodeInstalDir - Installation directory of Node.exe
            // NodeProxyAddress - Proxy Address for testing using NodeProxy.js
            // AutoEnableStr - Setting for automatically turning on the proxy settings - if set 1 - then proxy will be turn on
            // DomainConfgs - comma seperate list of the domain headers inside the ini
            NodeInstallDir = source.Configs[MainIniConfig].Get(NodeInstallKey);
            NodeProxyAddress = source.Configs[MainIniConfig].Get(NodeProxyAddrKey);
            AutoEnableStr = source.Configs[MainIniConfig].Get(AutoProxyKey);
            DomainCfgs = source.Configs[MainIniConfig].Get(DomainCfgsKey);
 

            if (AutoEnableStr == "1")
            {
                ckAutoProxy.Checked = true;
            }

            // displays the values to the user 
            lblNodePath.Text = NodeInstallDir;
            lblProxyAddr.Text = NodeProxyAddress;
            lblProxyAddr.Text = "127.0.0.1:8081";

            // add the domains into list by splitting the string using comma as delimeter
            // used for loading the domains into the grid on the form and adding to menu in the system tray
            if (DomainCfgs != "")
            {
                DomainKeys = DomainCfgs.Split(',').ToList<string>();
            }
  
        }

        private void LoadDomains()
        {

            string DomainKeyName;
            string DomainName;
            string ProjectDir;

            DomainKeyName = "";
            ProjectDir = "";

            // removes the items from the list
            this.listView1.Items.Clear();

            // clears the items in the menu , customizing the menu base domains in the list
            mnuNodeProxy.Items.Clear(); 

            DomainsHash  = new Hashtable();

            for (int i = 0; i < DomainKeys.Count; i++)
            {
                DomainKeyName = "";
                DomainName = "";
                ProjectDir = "";

                // gets the Domain Key to look up the fields that we want to display in the listview
                DomainKeyName = DomainKeys[i].ToString();
                //Console.WriteLine(DomainKeyName);
                IConfig DomainConfig = source.Configs[DomainKeyName];
                if (DomainConfig != null)
                {
                    DomainName = source.Configs[DomainKeyName].Get(DomainKey);
                    ProjectDir = source.Configs[DomainKeyName].Get(ProjectDirKey);
                    // Add the domain information to our listview
                    ListViewItem lvi = new ListViewItem(DomainName);
                    lvi.SubItems.Add(ProjectDir);
                    lvi.SubItems.Add("");
                    this.listView1.Items.Add(lvi);

                    ToolStripMenuItem mnuDomain = new ToolStripMenuItem(DomainName);
                    mnuDomain.Tag = DomainName;
                    mnuDomain.Click += new EventHandler(mnuDomain_Click);
                    mnuNodeProxy.Items.Add(mnuDomain);

                    // DomainsHash used for fast key lookup of the keys , since domain names are in the grid but not the keys
                    if (DomainsHash.ContainsKey(DomainName) == false)
                    {
                        DomainsHash.Add(DomainName, DomainKeyName);
                    }
                }
                
            }


            // Adds the standard menu in the system tray for the application

            if (DomainKeys.Count == 0)
            {
                // no domains - displays that there is no domains and when user clicks will bring up the configure screen
                ToolStripMenuItem mnuNone = new ToolStripMenuItem("No Domain Configured");
                mnuNone.Click += new EventHandler(mnuShowApp_Click);
                mnuNodeProxy.Items.Add(mnuNone); 
            }
            else
            {
                // domains exist, so it adds disconnect button
                ToolStripMenuItem mnuDisconnect = new ToolStripMenuItem("Disconnect");
                mnuDisconnect.Click += new EventHandler(mnuDisconnect_Click);
                mnuNodeProxy.Items.Add(mnuDisconnect);
            }

            // Adds a seperator to make easier to read in the right click
            ToolStripSeparator ToolStripSep1 = new ToolStripSeparator();
            mnuNodeProxy.Items.Add (ToolStripSep1);  

            // Add a Confuguration menu 
            ToolStripMenuItem mnuConfig = new ToolStripMenuItem("Configure");
            mnuConfig.Click += new EventHandler(mnuShowApp_Click);
            mnuNodeProxy.Items.Add(mnuConfig); 

            // Adds a close application in the system tray
            ToolStripMenuItem mnuClose = new ToolStripMenuItem("Close");
            mnuClose.Click += new EventHandler(mnuCloseApp_Click);
            mnuNodeProxy.Items.Add(mnuClose);
            

            
        }

        private void Use_Notify()
        {
            mnuNodeProxy = new ContextMenuStrip();


            //MyNotify.ContextMenuStrip = contextMenuStrip1;
            MyNotify.ContextMenuStrip = mnuNodeProxy;
            MyNotify.BalloonTipText = "This is A Sample Application";
            MyNotify.BalloonTipTitle = "Your Application Name";
            MyNotify.Text = "Node Proxy"; 
            MyNotify.ShowBalloonTip(1);
        }

        // Form Resize Event 
        private void frmMain_Resize(object sender, System.EventArgs e)
        {
            // Hide The Form when it's minimized
            if (FormWindowState.Minimized == this.WindowState)
            {
                MyNotify.Visible = true;
                MyNotify.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                MyNotify.Visible = false;
            }
        }
        private void MyNotify_DoubleClick(object sender, System.EventArgs e)
        {
            // Show the form when Dblclicked on Notifyicon
            Show();
            WindowState = FormWindowState.Normal;
        }
        private void mnuCloseApp_Click(object sender, EventArgs e)
        {
            // Will Close Your Application 
            MyNotify.Dispose();
            Application.Exit();
        }

        private void mnuShowApp_Click(object sender, EventArgs e)
        {
            //Will Restore Your Application 
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void mnuDisconnect_Click(object sender, EventArgs e)
        {
            NodeProxyCmdClose();
        }

        private void mnuDomain_Click(object sender, EventArgs e)
        {
            string DomainName;

            //checks to see if sender is menu item
            ToolStripItem item = sender as ToolStripItem;
            if (item != null)
            {
                // DomainName is set in the tag 
                //
                // tag - incase we decide in the future to display the menu diferently to the user
                DomainName = item.Tag.ToString();
                // starts the node proxy from the domain selected
                //NodeProxyCommand(DomainName);
                NodeProxyCommand("www.domain.com");
            }


        }

       
        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void NodeProxyCommand(string DomainName)
        {
            bool StartNodeProxy = true;

            //string ProjectDir;
            //string DomainCertKey;
            //string DomainCertFile;

            ProjectDir = "./demo";
            CertKeyFileName = "./openssl/FakeRoot.key";
            CertFileName = "./www.domain.com.crt";

            // node.exe installation directory can not be blank, checks before continuing         
            if (NodeInstallDir.Length == 0)
            {
                MessageBox.Show("Node Installation Directory is blank. Please browse for the directory by hitting the select.", 
                    "Directory is blank", MessageBoxButtons.OK);
                StartNodeProxy = false;
            }

            // checks see if the node.exe exists
            if (StartNodeProxy == true)
            {
                string NodeExeFileName;
                NodeExeFileName = NodeInstallDir + @"\node.exe";
                if (File.Exists(NodeExeFileName) == false)
                {
                    MessageBox.Show("Node.exe does not exist at this location: " + NodeInstallDir + ". " + 
                        "Please browse for the directory by hitting the select.",
                        "Node.exe does not exist", MessageBoxButtons.OK);
                    StartNodeProxy = false;
                }
            }

            if (StartNodeProxy == true)
            {
                // loads the fields  - projectdir, certkey, certfile 
                // returns false - if one of the fields fail in loading
                StartNodeProxy = LoadDomainFields(DomainName);  
            }

            if (StartNodeProxy == true)
            {

                string NodeBat;
                NodeBat = "nodevars.bat";




                string appPathLoc;
                // hard code appPath because several directories 
                // ie appPath - "C:\\Users\\Becky\\Documents\\zoovy\\NodeProxy\\NodeProxy\\bin\\x86\\Debug"
                // when we need this for testing
                appPathLoc = @"C:\Users\Becky\Documents\zoovy\NodeProxy";


                // run shell commands from C#
                string strCmdText;
                strCmdText = " /A /K ";
                strCmdText += " echo Hello";
                strCmdText += " & cd " + NodeInstallDir;
                strCmdText += " & nodevars.bat";
                // strCmdText += "rem Ensure this Node.js and NPM are first in the PATH
                //strCmdText += "set PATH=%APPDATA%\npm;%~dp0;%PATH%";

                // strCmdText += "rem Figure out node version and architecture and print it.
                // strCmdText += "setlocal";
                // strCmdText += "pushd "%~dp0"";
                //strCmdText += "& set print_version=.\node.exe -p -e \"process.versions.node + ' (' + process.arch + ')'\"";
                //strCmdText += "& for /F \"usebackq delims=\" %%v in (`%print_version%`) do set version=%%v";
                // echo Your environment has been set up for using Node.js %version% and NPM
                // strCmdText += "popd";
                // strCmdText += "endlocal";

                // rem If we're in the node.js directory, change to the user's home dir.
                // strCmdText += "& if \"%CD%\\\\"==\"%~dp0\" cd /d \"%HOMEDRIVE%%HOMEPATH%\"";


                // if we do not change the directory, then node starts in C:\Users\username
                // ie - C:\Users\Beckystr

                //strCmdText += " & cd " + appPath;
                // hard code appPath because several directories 
                // ie appPath - "C:\\Users\\Becky\\Documents\\zoovy\\NodeProxy\\NodeProxy\\bin\\x86\\Debug"
                // when we are using appPathLoc for testing
                strCmdText += " & cd " + appPathLoc;
                strCmdText += " & node.exe javascript/nodeproxy.js ";
                strCmdText += "--domain=" + DomainName;
                strCmdText += " --rootdir=" + ProjectDir;
                strCmdText += " --key=" + CertKeyFileName ;
                strCmdText += " --cert=" + CertFileName ;
                Console.WriteLine(strCmdText);



                CMDprocess = new System.Diagnostics.Process();

                startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                //startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false; //required to redirect
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = strCmdText;
                CMDprocess.EnableRaisingEvents = true;
                CMDprocess.StartInfo = startInfo;


                CMDprocess.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
                CMDprocess.Exited += new EventHandler(OnCmdExited);

                CMDprocess.Start();
                CMDprocess.BeginOutputReadLine();
                CMDprocess.WaitForExit();

                //while (!processDone)
                //    Thread.Sleep(100);
                //System.IO.StreamReader SR = CMDprocess.StandardOutput;

                //System.IO.StreamWriter SW = CMDprocess.StandardInput;
                //SW.WriteLine("@echo on");
                //SW.WriteLine("cd\\"); //the command you wish to run.....


                //CMDprocess.CloseMainWindow();

                

                //while (!processDone)
                //    Thread.Sleep(100);
                //System.IO.StreamReader SR = CMDprocess.StandardOutput;

                //System.IO.StreamWriter SW = CMDprocess.StandardInput;
                //SW.WriteLine("@echo on");
                //SW.WriteLine("cd\\"); //the command you wish to run.....


                //CMDprocess.CloseMainWindow();
            }

            
            

            
        }


        private  void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Output: " + e.Data);
            //txtNodeLog.Text += txtNodeLog.Text + e.Data.ToString() ;
            //SetText(e.Data.ToString());
            if (e.Data != null)
            {
                NodeProxyLog += e.Data.ToString() + "\n";
            }
            
        }


        


       
        //
        //
        // NodeProxyLog - is placeholder to hold all the log information of the proxy
        // try setting txtNodeLog.text  in OnOutputDataReceived Event 
        //
        // but threw following error
        //Cross-thread operation not valid: Control 'txtNodeLog' accessed from a thread other than the thread it was created on.
        //
        // Possible solution:
        // http://stackoverflow.com/questions/10775367/cross-thread-operation-not-valid-control-textbox1-accessed-from-a-thread-othe
        //
        //
        // When added one of the solutions , the proxy froze after outputing Hello
        //
        // Will try to readress for the later time
        //
        // For now added to button for the showing the logs
        //
        //delegate void SetTextCallback(string text);

        //private void SetText(string text)
        //{
        //    // InvokeRequired required compares the thread ID of the
        //    // calling thread to the thread ID of the creating thread.
        //    // If these threads are different, it returns true.
        //    if (this.txtNodeLog.InvokeRequired)
        //    {
        //        SetTextCallback d = new SetTextCallback(SetText);
        //        this.Invoke(d, new object[] { text });
        //    }
        //    else
        //    {
        //        this.txtNodeLog.Text = text;
        //    }
        //}

        //  buttons that displays and clearing the logging info
        //
        // NodeProxyLog - stores the logs while the proxy is turn on
        private void btnLoadLog_Click(object sender, EventArgs e)
        {
            txtNodeLog.Text = NodeProxyLog;
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            NodeProxyLog = "";
            txtNodeLog.Text = NodeProxyLog;
        }
        


        // close the command window that is running nodeproxy
        private void NodeProxyCmdClose()
        {
            if (CMDprocess != null)
            {
                EventArgs e = new EventArgs();
                //CMDprocess.CloseMainWindow();
                OnCmdExited(CMDprocess, e);
            }
            
        }

        static void OnCmdExited(object sender, EventArgs e)
        {
            if (CMDprocess != null)
            {
                Console.WriteLine("Process has finished executing.");
                //CMDprocess.OutputDataReceived -= new DataReceivedEventHandler(xOnOutputDataReceived);
                //CMDprocess.ErrorDataReceived -= new DataReceivedEventHandler(OnErrorDataReceived);
                CMDprocess.Exited -= new EventHandler(OnCmdExited);
                processDone = true;
            }


            
        }

       

        private void CreateDomainCrt(string domain)
        {
            bool CreateCert = true;

            CreateCert = LoadDomainFields(domain);

            if (CreateCert == true)
            {
                var csrDetails = new X509Name();
                csrDetails.Common = domain;         // this MUST be the server fully qualified hostname+domain
                csrDetails.Country = "US";
                csrDetails.StateOrProvince = "Test";
                csrDetails.Organization = "Company " + domain;
                csrDetails.OrganizationUnit = "** TESTING **";

                var signedCert = new X509Request();

                // creates new RSA Key
                var rsa = new RSA();
                BigNumber exponent = 0x10001; // this needs to be a prime number
                rsa.GenerateKeys(4096, exponent, null, null);
                var rootKey = new CryptoKey(rsa);

                var certSignRequest = new X509Request(2, csrDetails, rootKey);            // Version 2 is X.509 Version 3

                // var myKey = new X509Request();
                //var myKey = new CryptoKey();
                //var myPrivateKey = certSignRequest.PEM;
                //var myCryptoKey = new CryptoKey();
                //myCryptoKey = CA.CreateNewRSAKey(4096);

                // http://stackoverflow.com/questions/5763313/openssl-net-create-certeficate-509x
                // ###################################################
                // Create a configuration object using openssl.cnf file.
                //Configuration cfg = new OpenSSL.X509.Configuration(@"C:\Users\Becky\Documents\zoovy\NodeProxy\openssl\openssl.cnf");
                Configuration cfg = new OpenSSL.X509.Configuration(appPath + @"\openssl\openssl.cnf");

                // Create a root certificate authority which will have a self signed certificate.
                X509CertificateAuthority RootCA = OpenSSL.X509.X509CertificateAuthority.SelfSigned(cfg, new SimpleSerialNumber(), "NodeProxyRoot CA", DateTime.Now, TimeSpan.FromDays(365));

                // If you want the certificate of your root CA which you just create, you can get the certificate like this.
                X509Certificate RootCertificate = RootCA.Certificate;


                // Here you need CSR(Certificate Signing Request) which you might have already got it from your web server e.g. IIS7.0.
                // string strCSR = System.IO.File.ReadAllText(@"Path to your CSR file");
                //FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\new_cert.cer", FileMode.Create, FileAccess.ReadWrite);
                //BinaryWriter bw = new BinaryWriter(fs);
                OpenSSL.Core.BIO csrbio = OpenSSL.Core.BIO.MemoryBuffer();
                certSignRequest.Write(csrbio);
                string strCSR = csrbio.ReadString();
                csrbio.Dispose();
                //bw.Write(certString);
                //bw.Close();

                // You need to write the CSR string to a BIO object as shown below.
                OpenSSL.Core.BIO ReqBIO = OpenSSL.Core.BIO.MemoryBuffer();
                ReqBIO.Write(strCSR);

                // Now you can create X509Rquest object using the ReqBIO.
                OpenSSL.X509.X509Request Request = new OpenSSL.X509.X509Request(ReqBIO);

                // Once you have a request object, you can create a SSL Certificate which is signed by the self signed RootCA.
                OpenSSL.X509.X509Certificate certificate = RootCA.ProcessRequest(Request, DateTime.Now, DateTime.Now + TimeSpan.FromDays(365 * 10));

                // Now you can save this certificate to your file system.
                FileStream fs = new FileStream(CertFileName, FileMode.Create, FileAccess.ReadWrite);
                //FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\" + domain + ".crt", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                BIO bio = BIO.MemoryBuffer();
                certificate.Write(bio);
                string certString = bio.ReadString();
                bw.Write(certString);
                bw.Close();


            }
         
          }

        private bool LoadDomainFields(string DomainName)
        {
            bool DomainFieldsSucess = true;

            ProjectDir = "";
            CertKeyFileName = "";
            CertFileName = "";

            if (DomainsHash.ContainsKey(DomainName) == true)
            {
                string DKey;
                DKey = DomainsHash[DomainName].ToString();
                if (DKey.Length > 0)
                {
                    IConfig DomainConfig = source.Configs[DKey];
                    if (DomainConfig == null)
                    {
                        // key is not in the ini file
                        DomainFieldsSucess = false;
                    }
                    // grabs the variables - projectdir, key, cert from ini file
                    ProjectDir = source.Configs[DKey].Get(ProjectDirKey);
                    if ((Directory.Exists(ProjectDir) == true) && (DomainFieldsSucess == true))
                    {
                        CertKeyFileName = ProjectDir + @"\FakeRoot.key";
                        if (File.Exists(CertKeyFileName) == true)
                        {
                            CertFileName = ProjectDir + @"\" + DomainName + ".crt";
                            if (File.Exists(CertKeyFileName) == false)
                            {
                                // certificate does not exist - so can not load the necessary info to start the proxy
                                MessageBox.Show("Certificate does not exist: " + CertFileName,
                                   "Certificate does not exist", MessageBoxButtons.OK);
                                DomainFieldsSucess = false;
                            }
                        }
                        else
                        {
                            // did not the cert key - so can not load the necessary info to start the proxy
                            MessageBox.Show("Certificate key does not exist: " + CertKeyFileName,
                               "Certificate key does not exist", MessageBoxButtons.OK);
                            DomainFieldsSucess = false;
                        }

                    }
                    else
                    {
                        // did not found project directory - so can not load the necessary info to start the proxy
                        MessageBox.Show("Project directory does not exist: " + ProjectDir,
                           "project directory does not exist", MessageBoxButtons.OK);
                        DomainFieldsSucess = false;
                    }
                }
                else
                {
                    // domain key was blank - so can not load the necessary info to start the proxy
                    MessageBox.Show("Domain Key was blank. Can not load information for domain: " + DomainName,
                       "Domain Key was blank", MessageBoxButtons.OK);
                    DomainFieldsSucess = false;

                }
            }
            else
            {
                // did not found domain key - so can not load the necessary info to start the proxy
                MessageBox.Show("Domain Key was not found. Domain: " + DomainName,
                   "Domain Key Not Found", MessageBoxButtons.OK);
                DomainFieldsSucess = false;
            }

            return DomainFieldsSucess;
        }

        // Brings up an open dialog for finding the location of node.exe
        private void btnNodeBrowse_Click(object sender, EventArgs e)
        {
            // IntDir - starts the diretory in where the dialog expects node.exe.
            string IntDir;
            IntDir = ProgramFilesx86() + "\\nodejs";

            string FileName;
            string FilePath;

            // creates open file dialog to find the node exe.
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open Node EXE";
            fDialog.FileName = "node.exe";
            fDialog.Filter = "Node EXE|*.exe";
            fDialog.InitialDirectory = IntDir ;

            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                FileName = fDialog.FileName.ToString();
                FilePath = Path.GetDirectoryName(FileName);

                //MessageBox.Show(FilePath );

                // displays the node.exe path on the main screen
                NodeInstallDir = FilePath;
                lblNodePath.Text = NodeInstallDir;

                // saves the node.exe installation directory to ini
                source.Configs[MainIniConfig].Set(NodeInstallKey, NodeInstallDir);
                source.Save();
                
            }
        }

        // ckAutoProxy Event - triggers when user checks or unchecks the box
        private void ckAutoProxy_CheckedChanged(object sender, EventArgs e)
        {
            string AutoProxy;

            AutoProxy = "";

            // if user checks, then saves the value to 1 to ini file
            if (ckAutoProxy.Checked == true) 
            {
                AutoProxy = "1";


            }

            EnableDisableProxy(true);

            source.Configs[MainIniConfig].Set(AutoProxyKey , AutoProxy);
            source.Save();
        }

        // http://stackoverflow.com/questions/12050415/set-default-proxy-programmatically-instead-of-using-app-config
        private void EnableDisableProxy(bool EnableProxy)
        {
            string ip;
            string  port;

             ip = "127.0.0.1";
            port = "";
            //WebProxy proxy = (WebProxy)WebRequest.GetSystemWebProxy();
            //test = proxy.Address.AbsolutePath.ToString();

            //http://stackoverflow.com/questions/4254351/get-the-uri-from-the-default-web-proxy
            var proxy = System.Net.HttpWebRequest.GetSystemWebProxy();

            //gets the proxy uri, will only work if the request needs to go via the proxy 
            //(i.e. the requested url isn't in the bypass list, etc)
            Uri proxyUri = proxy.GetProxy(new Uri("http://www.google.com"));
            
            ip = proxyUri.Host.ToString();
            port = proxyUri.Port.ToString();  
            //http://stackoverflow.com/questions/14887679/whats-the-difference-between-webrequest-defaultwebproxy-and-webrequest-getsys
            //System.Net.GlobalProxySelection.Select = new System.Net.WebProxy(ip, 8081);

            // think this might be it for proxy
            //http://www.dreamincode.net/forums/topic/160555-working-with-proxy-servers/
            
        }

        // to Add test domain for proxy application
        //
        //  1. Display AddDomain for adding a new domain
        //  2. If user clicks Add
        //      a. save the information from the form
        //      b. create the ssl certificate for testing
        //      c. reloads the listview control 
        //          SANITY: have to reload list before creating the certicate
        //              reloads function - recreates the hashtable lookup for domain
        //              if you do not this first, then throws domain key not found error
        //      d. saves the ssl certificate and key to project directory
        //      e. saves the certificate and key name to ini file (dont think we need to do this since the cert and key will be in the project directoy
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            string Domain;
            string PrjDir;
            string Dkey;

            string DConfigs;

            DConfigs = "";

            frmAddDomain AddDomainFrm = new frmAddDomain();
            if (AddDomainFrm.ShowDialog() == DialogResult.OK)
            {
                // gets the information that set in AddDomainFrm
                Domain = AddDomainFrm.DomainName;
                PrjDir = AddDomainFrm.ProjectDir;

                // generates a new domain key for the ini file
                Dkey = NewDomainKey();

                // updates the DomainConfigs list 
                // the list is saved comma seperate
                for (int i = 0; i < DomainKeys.Count; i++)
                {
                    
                    if (i == 0)
                    {
                        DConfigs = DomainKeys[i].ToString();
                    }
                    else
                    {
                        DConfigs += "," + DomainKeys[i].ToString();

                    }
                }
                
                // saves the DomainConfigs list
                source.Configs[MainIniConfig].Set(DomainCfgsKey , DConfigs );

                // Adds the new key for domain to ini
                source.AddConfig(Dkey);

                // adds domain and the project directory to the ini using the new domain ket
                source.Configs[Dkey].Set(DomainKey , Domain );
                source.Configs[Dkey].Set(ProjectDirKey, PrjDir);

                // saves the ini file before we begin the next step
                source.Save();

                // refreshes the list view and loads the domains
                LoadDomains();

                // create certificate 
                CreateDomainCrt(Domain); 

               
                
            }
        }

        // NewDomainKey - finds the next available key to use in the ini file
        private string NewDomainKey()
        {
            string DKey;
            int DCnt;
            DKey = "";
            DCnt = 0;

            if (DomainKeys == null)
            {
                // first domain be added - so need to initalize the string list;
                DKey = "d1";
                DomainKeys = new List<string>();
                
            }
            else
            {
                // creates at starting key as d + count
                //
                // future - possible feature - add a domain nickname for testing
                // then it will use the domina nick name instead
                DCnt = DomainKeys.Count; 
                DKey = "d" + DCnt.ToString();

                while (DomainKeys.Contains(DKey))
                {
                    // checks to see if the key exists, increments by one until a key is not found
                    DCnt += 1;
                    DKey = "d" + DCnt.ToString();
                }
            }

            // add the new key to the string list
            DomainKeys.Add(DKey); 

            return DKey;
        }

        
        

     

        



       

        

       

      
        

       
    }
}
