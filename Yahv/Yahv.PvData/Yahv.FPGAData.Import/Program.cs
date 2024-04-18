using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.FPGAData.Import.Extends;

namespace Yahv.FPGAData.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataImport();
            RfqpartNumbersImport();

            Console.ReadKey();
        }

        static void RfqpartNumbersImport()
        {
            FileInfo fileInfo = new FileInfo("C:\\Users\\Administrator\\Desktop\\报价型号.txt");
            using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.Default))
            using (var conn = ConnManager.Current.PveStandard)
            {
                List<Models._temp_RFQ_PartNumbers> rfqPNList = new List<Models._temp_RFQ_PartNumbers>();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    rfqPNList.Add(new Models._temp_RFQ_PartNumbers()
                    {
                        PartNumber = line.Trim(),
                        Type = "报价"
                    });
                }
                conn.BulkInsert(rfqPNList);
            }
        }

        static void DataImport()
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "3A001 FPGA List w OPN_May 1st.txt"));
            using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.Default))
            using (var conn = ConnManager.Current.PvData)
            {
                List<Models.FPGA> fpgas = new List<Models.FPGA>();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    fpgas.Add(new Models.FPGA()
                    {
                        ID = Utils.GuidUtil.NewGuidUp(),
                        PartNumber = line.Trim(),
                        CreateDate = new DateTime(2020, 6, 18)
                    });
                }
                conn.BulkInsert(fpgas);
            }
        }

        static void TestPdfReader()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Length == 0)
                    continue;

                using (PdfReader pdfReader = new PdfReader(fileInfo.FullName))
                {
                    int numberOfPages = pdfReader.NumberOfPages;
                    /*
                    StringBuilder text = new StringBuilder();
                    for (int i = 1; i <= numberOfPages; ++i)
                    {
                        text.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, i));
                    }
                    */
                    string text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, 80);
                }
            }
        }
    }
}
