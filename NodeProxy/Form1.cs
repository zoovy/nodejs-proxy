using System;
using System.Collections.Generic;
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

namespace NodeProxy
{
    public partial class Form1 : Form
    {
        static System.Diagnostics.Process CMDprocess;
        static System.Diagnostics.ProcessStartInfo startInfo;
        private static bool processDone = false;


        public Form1()
        {
            InitializeComponent();
            Load += new EventHandler(Form1_Load);
            Resize += new EventHandler(Form1_Resize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Use_Notify(); // Setting up all Property of Notifyicon 
        }

        private void Use_Notify()
        {
            MyNotify.ContextMenuStrip = contextMenuStrip1;
            MyNotify.BalloonTipText = "This is A Sample Application";
            MyNotify.BalloonTipTitle = "Your Application Name";
            MyNotify.Text = "Node Proxy"; 
            MyNotify.ShowBalloonTip(1);
        }


                private void Form1_Resize(object sender, System.EventArgs e)
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


        private void button1_Click(object sender, EventArgs e)
        {
            NodeProxyCommand();
        }

        private void mnuDomain_Click(object sender, EventArgs e)
        {
            NodeProxyCommand();  

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

        private void NodeProxyCommand()
        {
            string ProgramFiles;
            ProgramFiles = ProgramFilesx86();

            string NodeInstallDir;
            NodeInstallDir = ProgramFiles + "\\nodejs";

            string NodeBat;
            NodeBat = "nodevars.bat";

            

            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            // hard code appPath because several directories 
            // ie appPath - "C:\\Users\\Becky\\Documents\\zoovy\\NodeProxy\\NodeProxy\\bin\\x86\\Debug"
            // when we need this for testing
            appPath = @"C:\Users\Becky\Documents\zoovy\NodeProxy"; 


            // run shell commands from C#
            string strCmdText;
            strCmdText = " /A /K cd " + NodeInstallDir + " & " +  NodeBat ;
            // if we do not change the directory, then node starts in C:\Users\username
            // ie - C:\Users\Becky
            strCmdText += " & cd " + appPath;
            strCmdText += " & node javascript/nodeproxy.js --domain=www.domain.com";
            strCmdText += " --rootdir=./demo --key=./openssl/FakeRoot.key";
            strCmdText += " --cert=./www.domain.com.crt";

            
            
            
            CMDprocess = new System.Diagnostics.Process();
            
            startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; 
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            //startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false; //required to redirect
            startInfo.FileName = "CMD.exe";
            startInfo.Arguments = strCmdText;
            CMDprocess.EnableRaisingEvents = true;
            CMDprocess.StartInfo = startInfo;


            CMDprocess.OutputDataReceived += new DataReceivedEventHandler(xOnOutputDataReceived);
            //CMDprocess.Exited += new EventHandler(OnCmdExited);

            CMDprocess.Start();
            CMDprocess.BeginOutputReadLine();
            CMDprocess.WaitForExit(); 

            while (!processDone)
                Thread.Sleep(100);
            //System.IO.StreamReader SR = CMDprocess.StandardOutput;
            
            //System.IO.StreamWriter SW = CMDprocess.StandardInput;
            //SW.WriteLine("@echo on");
            //SW.WriteLine("cd\\"); //the command you wish to run.....


            //CMDprocess.CloseMainWindow();

            //CMDprocess.WaitForExit();
        }


        static void xOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Output: " + e.Data);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NodeProxyCmdClose();
        }

        private void NodeProxyCmdClose()
        {
            CMDprocess.CloseMainWindow();
        }

        static void OnCmdExited(object sender, EventArgs e)
        {
            Console.WriteLine("Process has finished executing.");
            CMDprocess.OutputDataReceived -= new DataReceivedEventHandler(xOnOutputDataReceived);
            //CMDprocess.ErrorDataReceived -= new DataReceivedEventHandler(OnErrorDataReceived);
            CMDprocess.Exited -= new EventHandler(OnCmdExited);
            processDone = true;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Settings 1
            MessageBox.Show("Your Application Settings 1");
        }

        private void settings2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Settings 2
            MessageBox.Show("Your Application Settings 2");
        }

        private void button2_Click(object sender, EventArgs e)
        {


            var domain = "www.domain.com";
            var csrDetails = new X509Name();
            csrDetails.Common = domain;         // this MUST be the server fully qualified hostname+domain
            csrDetails.Country = "US";
            csrDetails.StateOrProvince = "Test";
            csrDetails.Organization = "Company "+domain;
            csrDetails.OrganizationUnit = "** TESTING **";

            var signedCert = new X509Request();

            var rootKey = CA.CreateNewRSAKey(4096);
            var certSignRequest = new X509Request(2, csrDetails, rootKey);            // Version 2 is X.509 Version 3
 
            // var myKey = new X509Request();
            //var myKey = new CryptoKey();
            //var myPrivateKey = certSignRequest.PEM;
            //var myCryptoKey = new CryptoKey();
            //myCryptoKey = CA.CreateNewRSAKey(4096);

            // http://stackoverflow.com/questions/5763313/openssl-net-create-certeficate-509x
            // ###################################################
            // Create a configuration object using openssl.cnf file.
            Configuration cfg = new OpenSSL.X509.Configuration(@"C:\Users\Becky\Documents\zoovy\NodeProxy\openssl\openssl.cnf");
            
            // Create a root certificate authority which will have a self signed certificate.
            X509CertificateAuthority RootCA = OpenSSL.X509.X509CertificateAuthority.SelfSigned(cfg, new SimpleSerialNumber(),"NodeProxyRoot CA", DateTime.Now, TimeSpan.FromDays(365));
                
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
            FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\"+domain+".crt", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            BIO bio = BIO.MemoryBuffer();
            certificate.Write(bio);
            string certString = bio.ReadString();
            bw.Write(certString);
            bw.Close();




           




            //var ca = new X509CertificateAuthority(pkcs12.Certificate, pkcs12.PrivateKey, new SimpleSerialNumber(42), null);
            
            //var signedCert = ca.ProcessRequest(myCryptoKey, DateTime.UtcNow,
            //  DateTime.UtcNow.AddYears(1), MessageDigest.SHA512);

            // var myCipher = Cipher.Null;

            //// Now you can save this certificate to your file system.
            //FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\new_cert.cer", FileMode.Create, FileAccess.ReadWrite);
            
            //BinaryWriter bw = new BinaryWriter(fs);
            //OpenSSL.Core.BIO bio = OpenSSL.Core.BIO.MemoryBuffer();
            //// signedCert.Write(bio);
            //myCryptoKey.WritePrivateKey(bio, Cipher.DES_CBC, "");
            //string certString = bio.ReadString();
            //bw.Write(certString);

            //bw.Close();
            //bio.Dispose();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // use the output
            //string output = outputBuilder.ToString();
            //Console.WriteLine(output); 
        }

       

      
        

       
    }
}
