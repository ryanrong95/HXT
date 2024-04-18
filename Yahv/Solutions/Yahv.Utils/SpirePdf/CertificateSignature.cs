using Spire.Pdf.Graphics;
using Spire.Pdf.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.SpirePdf
{
    public class CertificateSignature
    {
        private PdfDocument pdf;

        private CertificateSignature()
        {
        }

        internal CertificateSignature(PdfDocument Doc)
        {
            this.pdf = Doc;
        }

        #region 数字签名证书

        public void GenerateSignsture(string pfxPath, string password, RectangleF rectangleF)
        {

            //load the certificate
            PdfCertificate cert = new PdfCertificate(pfxPath, password);

            //initialize a PdfSignature instance
            PdfSignature signature = new PdfSignature(pdf, pdf.Pages[0], cert, "Signature1");

            //set the signature location
            signature.Bounds = rectangleF;

            //set the image of signature
            //   signature.SignImageSource = PdfImage.FromFile(@"C:\Users\Administrator\Desktop\E-iceblueLogo.png");

            //set the content of signature
            //     signature.GraphicsMode = GraphicMode.SignImageAndSignDetail;
            //signature.NameLabel = "Signer:";
            //signature.Name = "Gary";
            signature.ContactInfoLabel = "ContactInfo:";
            signature.ContactInfo = signature.Certificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, true);
            //   signature.DistinguishedNameLabel = "DN: ";
            //signature.DistinguishedName = signature.Certificate.IssuerName.Name;
            signature.LocationInfoLabel = "Location:";
            signature.LocationInfo = "suzhou";
            signature.ReasonLabel = "Reason: ";
            signature.Reason = "Test for custom";
            signature.DateLabel = "Date:";
            signature.Date = DateTime.Now;
            signature.DocumentPermissions = PdfCertificationFlags.AllowFormFill | PdfCertificationFlags.ForbidChanges;
            signature.Certificated = true;

            //set fonts
            //signature.SignDetailsFont = new PdfFont(PdfFontFamily.TimesRoman, 10f);
            //signature.SignNameFont = new PdfFont(PdfFontFamily.Courier, 15);

            //set the sign image layout mode
            //  signature.SignImageLayout = SignImageLayout.None;
        }



        #endregion
    }
}
