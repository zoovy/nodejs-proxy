using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenSSL.X509;
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
            var signedCert = new X509Request();

            var Test = new CA();
            signedCert = Test.CreateCertificateSigningRequest();

            // Now you can save this certificate to your file system.
            FileStream fs = new FileStream(@"C:\Users\Becky\Documents\zoovy\NodeProxy\new_cert.cer", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            OpenSSL.Core.BIO bio = OpenSSL.Core.BIO.MemoryBuffer();
            signedCert.Write(bio);
            string certString = bio.ReadString();
            bw.Write(certString);

            bw.Close();
            bio.Dispose();

            //using (var bio = OpenSSL.Core.BIO.File(@"C:\new cert.cer", "w"))
            //{
            //    signedCert.Write(bio);
            //}

            

            //var signedCert = CA.ProcessRequest(x509Request, DateTime.UtcNow,
            //DateTime.UtcNow.AddYears(1), MessageDigest.SHA512);
        }
    }
}
