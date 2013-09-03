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

namespace NodeProxy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            Configuration cfg = new OpenSSL.X509.Configuration(@"C:\Users\Becky\Documents\zoovy\NodeProxy\openssl.cnf");
            
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
    }
}
