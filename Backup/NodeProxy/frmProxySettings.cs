using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace NodeProxy
{
    public partial class frmProxySettings : Form
    {
        public frmProxySettings()
        {
            InitializeComponent();

            //
            txtAddress.Enabled = false;
            txtPort.Enabled = false;

            txtPort.KeyPress +=
                new KeyPressEventHandler(txtPort_KeyPress);
            txtAddress.KeyPress +=
               new KeyPressEventHandler(txtAddress_KeyPress);
        }

        public string ProxyAddress;

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            // only user to type 0-9 in Port text box
            // backspace is also an exception to the user undo changes
            if ((e.KeyChar >= 48 && e.KeyChar <= 57) || (e.KeyChar == 8))
            {
                // do nothing
            }
            else
            {
                // do not output the character pressed by the keyboard
                e.Handled = true;
            }
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            // only user to type 0-9  or "." in Address text box
            // backspace is also an exception to the user undo changes
            if ((e.KeyChar >= 48 && e.KeyChar <= 57) || (e.KeyChar == 8) || (e.KeyChar == 46))
            {
                //do nothing
            }
            else
            {
                // do not output the character pressed by the keyboard
                e.Handled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ProxyAddress = "";

            string ip;
            string port;

            string ProxyUrl = "www.google.com";

            ip = "";
            port = "";


            if (rbtnIESettings.Checked == true)
            {
            
                //set the proxy address based of the IE settings


                //http://stackoverflow.com/questions/4254351/get-the-uri-from-the-default-web-proxy
                var proxy = System.Net.HttpWebRequest.GetSystemWebProxy();

                //gets the proxy uri, will only work if the request needs to go via the proxy 
                //(i.e. the requested url isn't in the bypass list, etc)
                Uri proxyUri = proxy.GetProxy(new Uri("http://" + ProxyUrl ));

                ip = proxyUri.Host.ToString();
                port = proxyUri.Port.ToString();

                //  if proxy is enabled in IE 
                // ip will return the proxy information instead www.google.com
                // check to see if proxy is enable by comparing the ip against the proxyurl
                if (ProxyUrl != ip)
                {
                    ProxyAddress = ip + ":" + port;
                }
            }



            if (rbtnManualConfig.Checked == true)
            {
                // sets the proxy address based off what was enter in the text boxes
                if ((txtAddress.Text.Length > 0) && (txtPort.Text.Length > 0))
                {
                    ProxyAddress = txtAddress.Text + ":" + txtPort.Text;
                }
            }

            

              
        }

        private void rbtnIESettings_CheckedChanged(object sender, EventArgs e)
        {
            // disables the ablility to type anything in the text boxes if IE is used
            if (rbtnIESettings.Checked == true)
            {
                txtAddress.Enabled = false; 
                txtPort.Enabled = false;

            }
        }

        private void rbtnManualConfig_CheckedChanged(object sender, EventArgs e)
        {
            // enables the ablility to type information in the text boxes
            if (rbtnManualConfig.Checked == true)
            {

                txtAddress.Enabled = true;
                txtPort.Enabled = true ;
            }
        }
    }
}
