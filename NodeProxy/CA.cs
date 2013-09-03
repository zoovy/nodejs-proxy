using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSSL;
using OpenSSL.Core;
using OpenSSL.Crypto; 
using OpenSSL.X509; 

namespace NodeProxy
{
    class CA
    {
        public static CryptoKey CreateNewRSAKey(int numberOfBits)
        {
            using (var rsa = new RSA())
            {
                BigNumber exponent = 0x10001; // this needs to be a prime number
                rsa.GenerateKeys(numberOfBits, exponent, null, null);

                return new CryptoKey(rsa);
            }
        }
        
        public X509Request CreateCertificateSigningRequest()
        {
            using (var requestDetails = GetCertificateSigningRequestSubject())
            using (var key = CreateNewRSAKey(4096))
            {
                int version = 2; // Version 2 is X.509 Version 3
                return new X509Request(version, requestDetails, key);
            }
        }

        private static X509Name GetCertificateSigningRequestSubject()
        {
            var requestDetails = new X509Name();

            requestDetails.Common = "http://testserver.example.com/";
            requestDetails.Country = "UK";
            requestDetails.StateOrProvince = "Hampshire";
            requestDetails.Organization = "Test Co";
            requestDetails.OrganizationUnit = "Development";

            return requestDetails;
        }
         
    }
}
