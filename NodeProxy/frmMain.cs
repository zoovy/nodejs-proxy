using System;
using System.Collections.Generic;
using System.Collections; 
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenSSL;
using OpenSSL.X509;
using OpenSSL.Crypto;
using OpenSSL.Core;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Nini.Config;

using System.Net;

using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace NodeProxy
{
    public partial class frmMain : Form
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        bool settingsReturn, refreshReturn;

        // Get the Proxy Information from the registry
        private string OrigProxyEnable;
        private string OrigProxyServer;
        private const string ProxyEnable = "ProxyEnable";
        private const string ProxyServer = "ProxyServer";



        static System.Diagnostics.Process CMDprocess;
        static System.Diagnostics.ProcessStartInfo startInfo;
        string appPath;

        // Name of the ini file that will be installed in the app.exe location
        private string AppDataDir;
        private static string ProxyIniFileName = "\\ProxyConfig.ini";
        private IConfigSource ProxyIni = null;

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
        

        private string NodeInstallDir = "";
        private string NodeProxyAddress = "";

        private string ProjectDir;
        private string CertKeyFileName;
        private string PemFileName;

        private ContextMenuStrip mnuNodeProxy;

        private string NodeProxyLog;

        


        public frmMain()
        {
            InitializeComponent();
            Load += new EventHandler(frmMain_Load);
            Resize += new EventHandler(frmMain_Resize);
            Closed += new EventHandler(frmMain_Closed);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // running debugger
                Console.WriteLine("Detected Debugger - modifying AppPath\n");
                appPath += "\\..\\..\\..\\..";
            }

            //AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //AppDataDir += "\\CommerceRack";

            AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);   

            //AppDataDir = @"C:\Users\Becky\Documents";

            LoadSystemProxySetting();

            Use_Notify(); // Setting up all Property of Notifyicon 

            LoadProxyIni();

            LoadDomains();
        }

        private void frmMain_Closed(object sender, EventArgs e)
        {
            EnableDisableProxy(false);
        }

        //
        // Uses a library called Nini from sourceforge 
        // http://nini.sourceforge.net/
        // http://sourceforge.net/projects/nini/?source=directory
        //
        // Intialize the default values of the program by opening and loading the ini 
        private void LoadProxyIni()
        {
            string AutoEnableStr = "0";
            string DomainCfgs = "";

            // Load the configuration ProxyIni file
            if (! File.Exists(AppDataDir + ProxyIniFileName)) {
                // http://sourceforge.net/p/nini/bugs/32/
                // If you create an instance of the IniWriter class
                // through a class that does not have a BaseStream
                // property then Nini will throw an exception on the
                // Dispose call.

                var NL = Environment.NewLine;
                string toWrite = ";debug" + NL
                    + "[" + MainIniConfig + "]" + NL;
                System.IO.File.WriteAllText(AppDataDir + ProxyIniFileName, toWrite);
                
                txtNodeLog.Text += AppDataDir + ProxyIniFileName + "not found, creating one.\n";

                // next we'll reload ProxyIni because NINI module doesn't let us specify where 
                //  .save() method saves to unless it is initialized from a file.         
            }



            if (File.Exists(AppDataDir + ProxyIniFileName) == true)
            {
                ProxyIni = new IniConfigSource(AppDataDir + ProxyIniFileName);
                // gets the values from each key
                //
                // NodeInstalDir - Installation directory of Node.exe
                // NodeProxyAddress - Proxy Address for testing using NodeProxy.js
                // AutoEnableStr - Setting for automatically turning on the proxy settings - if set 1 - then proxy will be turn on
                // DomainConfgs - comma seperate list of the domain headers inside the ini
                NodeInstallDir = ProxyIni.Configs[MainIniConfig].Get(NodeInstallKey);
                NodeProxyAddress = ProxyIni.Configs[MainIniConfig].Get(NodeProxyAddrKey);
                AutoEnableStr = ProxyIni.Configs[MainIniConfig].Get(AutoProxyKey);
                DomainCfgs = ProxyIni.Configs[MainIniConfig].Get(DomainCfgsKey);
            }

            if ((NodeProxyAddress == null) || (NodeProxyAddress.Length == 0)) {
                // a reasonable guess for the proxy server
                NodeProxyAddress = "127.0.0.1:8080";
                txtNodeLog.Text += "Auto-guess Proxy server: " + NodeProxyAddress + "\n";
            }

            IPAddress test = IPAddress.Loopback;
            txtNodeLog.Text += "Loopback: " + test.ToString() + "\n";


            if ((NodeInstallDir == null) || (NodeInstallDir.Length == 0))
            {
                NodeInstallDir = FindNodeExePath();
                if (NodeInstallDir.Length > 0)
                {
                    txtNodeLog.Text += "Guessed Node.exe path: " + NodeInstallDir + "\n";
                    // maybe we should tell the user that we have node installed
                    // and that we auto-detected it. ??
                }
                else
                {
                    txtNodeLog.Text += "*** ERROR *** Node.exe not found -- please install nodejs\n";
                }
            }

            if (NodeInstallDir.Length == 0)
            {
                    // warn the user node isn't installed and they're going to have a bad time.
                txtNodeLog.Text += "No Node.js == You're going to have a bad time (please download and install it)\n";
            }

            // fakeRoot.Key is hardcoded.
            CertKeyFileName = appPath + "\\openssl\\FakeRoot.key";
            if (! File.Exists(CertKeyFileName)) 
            {
                Console.WriteLine(CertKeyFileName);
                txtNodeLog.Text += "Missing " + CertKeyFileName + " can't create SSL certificates\n";
            }

            if (AutoEnableStr == "1")
            {
                ckAutoProxy.Checked = true;
            }

            // displays the values to the user 
            lblNodePath.Text = NodeInstallDir;
            lblProxyAddr.Text = NodeProxyAddress;
            // for testing
            //lblProxyAddr.Text = "127.0.0.1:8080";

            // add the domains into list by splitting the string using comma as delimeter
            // used for loading the domains into the grid on the form and adding to menu in the system tray
            if (String.IsNullOrEmpty(DomainCfgs))
            {
                // make sure DomainCfgs is initialized to BLANK here else bad things happen later
                DomainCfgs = "";
            }
            DomainKeys = DomainCfgs.Split(',').ToList<string>();
            // SANITY: at this point DomainCfgs and DomainKeys are both initialized.
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

            if (DomainKeys != null)
            {
                
                for (int i = 0; i < DomainKeys.Count; i++)
                {
                    DomainKeyName = "";
                    DomainName = "";
                    ProjectDir = "";


                   
                    // gets the Domain Key to look up the fields that we want to display in the listview
                    DomainKeyName = DomainKeys[i].ToString();
                    //Console.WriteLine(DomainKeyName);

                    
                    IConfig DomainConfig = ProxyIni.Configs[DomainKeyName];

                    

                    if (DomainConfig != null)
                    {
                        DomainName = ProxyIni.Configs[DomainKeyName].Get(DomainKey);
                        ProjectDir = ProxyIni.Configs[DomainKeyName].Get(ProjectDirKey);
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
            }

            
            


           
            // Adds the standard menu in the system tray for the application

            if (DomainKeys != null)
            {
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
            }
            else
            {
                // no domains - displays that there is no domains and when user clicks will bring up the configure screen
                ToolStripMenuItem mnuNone = new ToolStripMenuItem("No Domain Configured");
                mnuNone.Click += new EventHandler(mnuShowApp_Click);
                mnuNodeProxy.Items.Add(mnuNone);
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
            MyNotify.BalloonTipText = "CommerceRack NodeProxy";
            MyNotify.BalloonTipTitle = "CommereRack NodeProxy";
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
                NodeProxyStart(DomainName);
                //  for testing 
                //NodeProxyCommand("www.domain.com");
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

        private string FindNodeExePath()
        {
            
            string fileNameToFind = "\\Node.exe";

          

            
            // some common sense places we *might* find Node.exe
            var dirsToSearchForNodeExec = new string[] { 
                @"C:\program files\nodejs",
                @"C:\program files\Node64",
                @"C:\program files\NodeJS64",
                @ProgramFilesx86()+ "\\nodejs",
                @ProgramFilesx86()+ "\\Node64",
                @ProgramFilesx86()+ "\\NodeJS64",
                };

            //string directoryToSearch = @"C:\\program files\nodejs";
            var NodeExecPath = "";
            foreach (string value in dirsToSearchForNodeExec)
            {
                if (NodeExecPath == "")
                {
                    //string[] files = Directory.GetFileSystemEntries(directoryToSearch, fileNameToFind);
                    //foreach (string f in files)
                    //{
                    //    NodeExecPath = f;
                    //    Console.WriteLine("File: " + f);
                    //}
                    if (File.Exists(value + fileNameToFind) == true)
                    {
                        NodeExecPath = value;
                    }
                }
            }

            Console.WriteLine("Found:" + NodeExecPath);

            return NodeExecPath;
        }

        private void NodeProxyStart(string DomainName)
        {
            bool StartNodeProxy = true;

            //string ProjectDir;
            //string DomainCertKey;
            //string DomainCertFile;

            ProjectDir = "./demo";
            PemFileName = "** PEM FILENAME NOT SET **";

            // node.exe installation directory can not be blank, checks before continuing         
            if (NodeInstallDir.Length == 0)
                {
                    MessageBox.Show("Can not find the Node.exe. Please check to see if Node.exe is installed",
                        "Can not find node.exe", MessageBoxButtons.OK);
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
                StartNodeProxy = LoadDomainFields(DomainName, false);  
            }

            if (StartNodeProxy == true)
            {

                string appPathLoc;
                // hard code appPath because several directories 
                // ie appPath - "C:\\Users\\Becky\\Documents\\zoovy\\NodeProxy\\NodeProxy\\bin\\x86\\Debug"
                // when we need this for testing

                // run shell commands from C#
                string strCmdText;
                strCmdText = " /A /K ";
                strCmdText += " \" "; // beginning "
                strCmdText += " cd " + NodeInstallDir;
                if (File.Exists(NodeInstallDir + "\\nodevars.bat"))
                {
                    strCmdText += " & nodevars.bat";
                }
                else if (File.Exists(NodeInstallDir + "\\nodejsvars.bat"))
                {
                    strCmdText += " & nodejsvars.bat";
                }
                else
                {
                    txtNodeLog.Text += "ERROR: Could not fine nodevars.bat or nodejsvars.bat\n";
                }
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

                //CertKeyFileName = @"C:\Users\Becky\Documents\zoovy\newdomain\FakeRoot.key";
                //PemFileName = @"C:\Users\Becky\Documents\zoovy\newdomain\www.domain.com.crt";
                // when we are using appPathLoc for testing
                // strCmdText += " & cd " + appPath;

                if (!Directory.Exists(ProjectDir))
                {
                    txtNodeLog.Text = "ERROR: Project " + ProjectDir + " is invalid\n";
                }

                if (!File.Exists(PemFileName))
                {
                    txtNodeLog.Text = "ERROR: PEM File " + PemFileName + " does not exist\n";
                }

                strCmdText += " & node.exe "+appPath+"\\javascript\\nodeproxy.js ";
                strCmdText += "--domain=" + DomainName;
                strCmdText += " --rootdir=" + ProjectDir;
                strCmdText += " --key=" + PemFileName ;
                strCmdText += " --cert=" + PemFileName ;
                // strCmdText += " --proxy=" + ProxyServer;
                strCmdText += " \" ";   // terminating "
                Console.WriteLine(strCmdText);
                txtNodeLog.Text += strCmdText + "\n";



                CMDprocess = new System.Diagnostics.Process();

                startInfo = new System.Diagnostics.ProcessStartInfo();
                // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                //startInfo.RedirectStandardInput = true;
                // startInfo.RedirectStandardOutput = true;
                // startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false; //required to redirect
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = strCmdText;
                CMDprocess.EnableRaisingEvents = true;
                CMDprocess.StartInfo = startInfo;


                // CMDprocess.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
                // CMDprocess.Exited += new EventHandler(OnCmdExited);

                CMDprocess.Start();
                // CMDprocess.BeginOutputReadLine();
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
            }


            
        }

       

        private void CreateDomainPEMFile(string domain)
        {
            bool CreateCert = true;
            if (String.IsNullOrEmpty(domain))
            {
                Console.WriteLine("Invalid call to CreateDomainPEMFile");
                CreateCert = false;
                var trace = new System.Diagnostics.StackTrace();
            }

            CreateCert = LoadDomainFields(domain, true);

            if (CreateCert == true)
            {

                var csrDetails = new OpenSSL.X509.X509Name();
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

                BIO bio = BIO.MemoryBuffer();
                certificate.Write(bio);
                string certString;
                certString = bio.ReadString();

                // opens the key file and appends to the crt to help create the pem file
                FileStream fsKey = new FileStream(CertKeyFileName , FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fsKey);
                certString += System.Text.Encoding.ASCII.GetString(br.ReadBytes(Convert.ToInt32(fsKey.Length))); 
                fsKey.Close();

                // Now you can save this certificate to your file system.
                FileStream fs = new FileStream(PemFileName, FileMode.Create, FileAccess.ReadWrite);
                //FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\" + domain + ".crt", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                //bw.Write(certString);
                // adding ascii coding to write the pem file correctly
                // must write as arrey of bytes 
                // writing to string adds characters
                bw.Write(System.Text.Encoding.ASCII.GetBytes(certString));
                bw.Close();
            }
         
          }

       



        //
        // this has a bug, it only references/checks the first domain
        //
        private bool LoadDomainFields(string DomainName, bool createpem)
        {
            ProjectDir = "";
            PemFileName = "";


            string DomainINIKey = "";
            var ErrorMsg = "";

            if (DomainsHash.ContainsKey(DomainName) == false)
            {
                ErrorMsg = "Domain Key was not found. Domain: " + DomainName;
            }
            else 
            {
                DomainINIKey = DomainsHash[DomainName].ToString();
                if (DomainINIKey.Length == 0) {
                   ErrorMsg = "Domain Key was blank. Can not load information for domain: " + DomainName;
                   }
            }


            IConfig DomainConfig;

            if (ErrorMsg != "") {
                // shit happened
            }
            else {
                // sanity: DomainINIKey *MUST BE* set and valid
                DomainConfig = ProxyIni.Configs[DomainINIKey];
                if (DomainConfig == null)
                {
                    ErrorMsg = "Alas, it appears "+DomainINIKey.ToString()+" is not defined in ProxyConfig.Ini\n";
                }
            }

            if (ErrorMsg == "") {
               if (File.Exists(CertKeyFileName) == false) {
                   ErrorMsg = "Missing "+CertKeyFileName;
               }
            }


            
            if (ErrorMsg == "") {
                // grabs the variables - projectdir, key, cert from ini file
                ProjectDir = ProxyIni.Configs[DomainINIKey].Get(ProjectDirKey);
                PemFileName = ProjectDir + @"\" + DomainName + ".pem";
                if (ProjectDir == "")
                {
                    ErrorMsg = "Please specify a valid Project Directory";
                }
                else if ((Directory.Exists(ProjectDir) == false))
                {
                   ErrorMsg = "ProjectDir "+ProjectDir+" does not exist";
                }
                else if (createpem) {
                    // we'll create a pem file so don't worry about it.
                }
                else if (! Directory.Exists(PemFileName)) {
                    // txtNodeLog += PemFileName+ " doesn't exist, creating.";
                    createpem = true;
                }
            }

            if (ErrorMsg != "")
            {
                // txtNodeLog += "ERROR: "+ErrorMsg+"\n";
                MessageBox.Show(ErrorMsg, "Project/Domain Init error", MessageBoxButtons.OK);
            }


            return (ErrorMsg == "");
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
                ProxyIni.Configs[MainIniConfig].Set(NodeInstallKey, NodeInstallDir);
                ProxyIni.Save();
                
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

            ProxyIni.Configs[MainIniConfig].Set(AutoProxyKey , AutoProxy);
            ProxyIni.Save();
        }

        private void LoadSystemProxySetting()
        {
            OrigProxyEnable = "";
            OrigProxyServer = "";
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            //registry.SetValue("ProxyEnable", 1);
            //registry.SetValue("ProxyServer", "127.0.0.1:8080");
            if (registry != null)
            {
                OrigProxyEnable = registry.GetValue(ProxyEnable).ToString();
                if (registry.GetValue(ProxyServer) != null)
                {
                    OrigProxyServer = registry.GetValue(ProxyServer).ToString();
                }
                registry.Close();
            }
        }


        // http://stackoverflow.com/questions/197725/programmatically-set-browser-proxy-settings-in-c-sharp
        private void EnableDisableProxy(bool EnableProxy)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            if (EnableProxy == true)
            {
                // enables the proxy based of NodeProxyAddress from the ini
                registry.SetValue("ProxyEnable", 1);
                registry.SetValue("ProxyServer", "127.0.0.1:8080");
            }
            else
            {
                // returns the proxy settings to original proxy settings in the computer
                registry.SetValue("ProxyEnable", OrigProxyEnable);
                registry.SetValue("ProxyServer", OrigProxyServer);
            }

            registry.Close(); 

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            
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
            string Domain = "";
            string PrjDir = "";
            string DomainINIKey;

            string DConfigs;
            string ErrorMsg = "";

            DConfigs = "";

            frmAddDomain AddDomainFrm = new frmAddDomain();
            if (AddDomainFrm.ShowDialog() != DialogResult.OK)
            {
                ErrorMsg = "NO_PROBLEMS"; // not really an error
            }
            else
            {
                // Validates the addDomainFrm
                Domain = AddDomainFrm.DomainName;
                if (Domain == "")
                {
                    ErrorMsg = "domain is required";
                }
                else if (Domain == "www.domain.com")
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                       // www.domain.com is valid in the debugger.
                    }
                    else
                    {
                        ErrorMsg = "Please specify a real domain (www.domain.com is invalid)";
                    }
                }
                PrjDir = AddDomainFrm.ProjectDir;
                if (PrjDir == "")
                {
                    ErrorMsg = "Project Directory is required";
                }
                else if (!Directory.Exists(PrjDir))
                {
                    ErrorMsg = "Project Directory is invalid";
                }
            }

            if (ErrorMsg == "") {
               // generates a new domain key for the ini file
                // finds the next available key to use in the ini file
                int DCnt;
                DomainINIKey = "";
                DCnt = 0;

                if ((DomainKeys == null) || (DomainKeys.Count==0)) 
                {
                    // first domain be added - so need to initalize the string list;
                    DomainINIKey = "d1";
                    DomainKeys = new List<string>();
                }
                else
                {
                    // creates at starting key as d + count
                    //
                    // future - possible feature - add a domain nickname for testing
                    // then it will use the domina nick name instead
                    DCnt = DomainKeys.Count;
                    DomainINIKey = "d" + DCnt.ToString();

                    while (DomainKeys.Contains(DomainINIKey))
                    {
                        // checks to see if the key exists, increments by one until a key is not found
                        DCnt += 1;
                        DomainINIKey = "d" + DCnt.ToString();
                    }
                }

                // add the new key to the string list
                DomainKeys.Add(DomainINIKey);

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
                ProxyIni.Configs[MainIniConfig].Set(DomainCfgsKey, DConfigs);

                // Adds the new key for domain to ini
                ProxyIni.AddConfig(DomainINIKey);

                // adds domain and the project directory to the ini using the new domain ket
                ProxyIni.Configs[DomainINIKey].Set(DomainKey , Domain );
                ProxyIni.Configs[DomainINIKey].Set(ProjectDirKey, PrjDir);

                // saves the ini file before we begin the next step
                ProxyIni.Save();

                // refreshes the list view and loads the domains
                LoadDomains();

                // create certificate Pem File in project dir.
                CreateDomainPEMFile(Domain);                
            }

            if (ErrorMsg == "")
            {
            }
            else if (ErrorMsg == "NO_PROBLEMS")
            {
                // a cheap short circuit.
            }
            else {
                // txtNodeLog += "ERROR: "+ErrorMsg+"\n";
                MessageBox.Show(ErrorMsg, "Project/Domain Init error", MessageBoxButtons.OK);
            }

        }


        // loads the form
        private void btnProxyAddr_Click(object sender, EventArgs e)
        {
            string ProxyAddr = "";

            frmProxySettings ProxySettingFrm = new frmProxySettings();
            if (ProxySettingFrm.ShowDialog () == DialogResult.OK)
            {
                //
                ProxyAddr = ProxySettingFrm.ProxyAddress;
                // ProxyAddr was set and the ok button was click 
                if (ProxyAddr.Length > 0)
                {
                    // saves the new address to variable and saves to ini file
                    NodeProxyAddress = ProxyAddr; 


                    ProxyIni.Configs[MainIniConfig].Set(NodeProxyAddrKey , NodeProxyAddress );
                    ProxyIni.Save();

                    // displays the changes to the user
                    lblProxyAddr.Text = NodeProxyAddress; 
                }
            }
        }


        private void RemoveDomain(string domain)
        {
            string DConfigs = "";


            if (DomainsHash.ContainsKey(domain) == true)
            {
                // removes the fields and config header from the ini file
                string DomainINIKey;
                DomainINIKey = DomainsHash[domain ].ToString();
                if (DomainINIKey.Length > 0)
                {
                    IConfig DomainConfig = ProxyIni.Configs[DomainINIKey];
                    if (DomainConfig != null)
                    {
                        DomainConfig.Remove(ProjectDirKey);
                        DomainConfig.Remove(DomainKey); 
                    }

                    ProxyIni.Configs.Remove(DomainConfig);
                }

                if (DomainKeys != null)
                {
                    // removes the key from the list
                    DomainKeys.Remove(DomainINIKey);

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
                    ProxyIni.Configs[MainIniConfig].Set(DomainCfgsKey, DConfigs);
                }
                
                ProxyIni.Save();

                // refreshes the list view
                LoadDomains();
            }

        }

        // lauches the browser and displays the help page
        private void mnuHelpIndex_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/zoovy/nodejs-proxy/wiki";

            System.Diagnostics.Process.Start(url);
        }

        // removes the domain name from the list
        private void btnRemove_Click(object sender, EventArgs e)
        {
            string domain;
            ListView.SelectedIndexCollection sel = listView1.SelectedIndices;
            if (sel.Count == 1)
            {
                ListViewItem selItem = listView1.Items[sel[0]];
                domain  = selItem.SubItems[0].Text;

                if (MessageBox.Show("Do you want to remove domain:" + domain , "Remove Domain", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // removes the domain for the ini and listview 
                    RemoveDomain(domain);
                }
               
            }
        }

        // displays the about form - information comes the file - AssemblyInfo.cs
        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAbout AboutFrm = new frmAbout();
            AboutFrm.ShowDialog(); 
        }

       

        
        

     

        



       

        

       

      
        

       
    }
}
