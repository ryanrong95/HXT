using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvData.SolrService.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvData.WebApp.Test
{
    public partial class LinqTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch outWatch = new System.Diagnostics.Stopwatch();
            outWatch.Start();

            Test4();

            outWatch.Stop();
            long span = outWatch.ElapsedMilliseconds;

            string str1 = "4|3|用途：电脑线路板用|稳压功能|已封装|TI牌|型号:TPS51200DRCR|非已蚀刻，且未切割、未封装的集成电路原片|量产|||非加密";
            string str3 = ToHalfAngle(str1);
            string str2 = "4|3|用途:电脑线路板用|稳压功能|已封装|TI牌|型号:TPS51200DRCR|非已蚀刻,且未切割、未封装的集成电路原片|量产|||非加密";
            string str4 = ToHalfAngle(str2);
            bool isEqual = str3 == str4;

            //TestLinqExpression();
            //TestSolrRestful();

            Type type1 = typeof(LinqTest);
            Type type2 = type1;
            type2 = typeof(ApiTest);

            //TestView();
            //TestLinqToSolr();
        }

        void Test1()
        {
            /*
            using (var reponsitory = new PvDataReponsitory())
            {
                string partnumber = "AD620";
                string manufacturer = "ADI";

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var cfpn = new XbjsTopView<PvDataReponsitory>(reponsitory)
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault(item => item.PartNumber == partnumber && item.Manufacturer == manufacturer);

                watch.Stop();
                TimeSpan span1 = watch.Elapsed;
                watch.Restart();

                if (cfpn == null)
                {
                    cfpn = new XbjsTopView<PvDataReponsitory>(reponsitory)
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault(item => item.PartNumber.StartsWith(partnumber) && item.Manufacturer.StartsWith(manufacturer));
                }

                watch.Stop();
                TimeSpan span2 = watch.Elapsed;
                watch.Restart();

                if (cfpn != null)
                {
                    var eccns = new EccnsTopView<PvDataReponsitory>(reponsitory).Where(item => item.PartNumber == cfpn.PartNumber)
                        .Select(item => item.Code).ToArray();
                    var pcs = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductControls>().Where(item => item.PartNumber == cfpn.PartNumber)
                        .Select(item => item.Type).ToArray();
                }

                watch.Stop();
                TimeSpan span3 = watch.Elapsed;
            }
            */
        }

        void Test2()
        {
            /*
            string partnumber = "AD620";
            string manufacturer = "ADI";

            Services.Models.XbjInfo cfpn1 = null;
            Services.Models.XbjInfo cfpn2 = null;

            Task task1 = new Task(() =>
            {
                using (var reponsitory = new PvDataReponsitory())
                {
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                    watch.Start();

                    cfpn1 = new XbjsTopView<PvDataReponsitory>(reponsitory)
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault(item => item.PartNumber == partnumber && item.Manufacturer == manufacturer);

                    watch.Stop();
                    long span = watch.ElapsedMilliseconds;
                }
            });
            task1.Start();

            Task task2 = new Task(() =>
            {
                using (var reponsitory = new PvDataReponsitory())
                {
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                    watch.Start();

                    cfpn2 = new XbjsTopView<PvDataReponsitory>(reponsitory)
                    .Where(item => item.PartNumber.Contains(partnumber) && item.Manufacturer.Contains(manufacturer))
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault();

                    watch.Stop();
                    long span = watch.ElapsedMilliseconds;
                }
            });
            task2.Start();

            Task.WaitAll(task1, task2);

            Services.Models.XbjInfo cfpn = cfpn1 ?? cfpn2;
            if (cfpn != null)
            {
                string[] eccns;
                int[] pcs;

                Task task3 = new Task(() =>
                {
                    eccns = new EccnsTopView<PvDataReponsitory>().Where(item => item.PartNumber == cfpn.PartNumber)
                    .Select(item => item.Code).ToArray();
                });
                task3.Start();

                Task task4 = new Task(() =>
                {
                    using (var reponsitory = new PvDataReponsitory())
                    {
                        pcs = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductControls>().Where(item => item.PartNumber == cfpn.PartNumber)
                        .Select(item => item.Type).ToArray();
                    }
                });
                task4.Start();

                Task.WaitAll(task3, task4);
            }
            */
        }

        void Test3()
        {
            /*
            using (var reponsitory = new PvDataReponsitory())
            {
                string partnumber = "AD620";
                string manufacturer = "ADI";

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                Services.Models.XbjInfo cfpn1 = null;
                Services.Models.XbjInfo cfpn2 = null;

                Task task1 = new Task(() =>
                {
                    cfpn1 = new XbjsTopView<PvDataReponsitory>()
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault(item => item.PartNumber == partnumber && item.Manufacturer == manufacturer);
                });
                task1.Start();

                Task task2 = new Task(() =>
                {
                    cfpn2 = new XbjsTopView<PvDataReponsitory>()
                    .OrderByDescending(item => item.OrderDate)
                    .FirstOrDefault(item => item.PartNumber.StartsWith(partnumber) && item.Manufacturer.StartsWith(manufacturer));
                });
                task2.Start();

                watch.Stop();
                TimeSpan span1 = watch.Elapsed;
                watch.Restart();

                task1.Wait();
                task2.Wait();

                watch.Stop();
                TimeSpan span2 = watch.Elapsed;
                watch.Restart();

                Services.Models.XbjInfo cfpn = cfpn1 ?? cfpn2;
                if (cfpn != null)
                {
                    var eccns = new EccnsTopView<PvDataReponsitory>(reponsitory).Where(item => item.PartNumber == cfpn.PartNumber)
                        .Select(item => item.Code).ToArray();
                    var pcs = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductControls>().Where(item => item.PartNumber == cfpn.PartNumber)
                        .Select(item => item.Type).ToArray();
                }

                watch.Stop();
                TimeSpan span3 = watch.Elapsed;
            }
            */
        }

        void Test4()
        {
            string strconn = System.Configuration.ConfigurationManager.ConnectionStrings["PvDataConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();

                string partnumber = "0034.3120";
                string manufacturer = "SCHURTER";

                string sql = @"DECLARE @VAL INT;
-- 查询结果
DECLARE @Code INT;
DECLARE @Success BIT;
DECLARE @Message NVARCHAR(200);

-- 型号、品牌
DECLARE @PartNumber NVARCHAR(150);
DECLARE @Manufacturer NVARCHAR(150);

-- 归类信息
DECLARE @HSCode	VARCHAR(50);
DECLARE @Name NVARCHAR(150);
DECLARE @LegalUnit1 NVARCHAR(10);
DECLARE @LegalUnit2 NVARCHAR(10);
DECLARE @VATRate DECIMAL(18,7);
DECLARE @ImportPreferentialTaxRate DECIMAL(18,7);
DECLARE @ImportGeneralTaxRate DECIMAL(18,7);
DECLARE @ExciseTaxRate DECIMAL(18,7);
DECLARE @Elements NVARCHAR(500);
DECLARE @CIQCode VARCHAR(3);
DECLARE @TaxCode VARCHAR(50);
DECLARE @TaxName NVARCHAR(150);

-- 特殊类型
DECLARE @Ccc BIT;
DECLARE @Embargo BIT;
DECLARE @HkControl BIT;
DECLARE @Coo BIT;
DECLARE @CIQ BIT;
DECLARE @CIQprice DECIMAL(18,7);

-- Eccn编码
DECLARE @EccnCode VARCHAR(500);

-- 如果型号、品牌可以精确匹配到数据，则使用精确匹配的；否则进行模糊匹配
SELECT @VAL = COUNT(1) FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr;
IF @VAL > 0
BEGIN
	SELECT @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
           @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
           @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
    FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr;
END
ELSE
BEGIN
    SELECT @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
           @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
           @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
    FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber LIKE (@paramPN + '%') AND Manufacturer LIKE (@paramMfr + '%');
END

-- 如果未查询到数据，返回Code=100
IF @PartNumber IS NULL
BEGIN
    SET @Code = 100; SET @Success = 0; SET @Message = '未查询到数据';
    SELECT Code = @Code, Success = @Success, Message = @Message;
END
-- 如果查询到数据，则继续查询海关税则和Eccn，返回Code=200
ELSE
BEGIN
    -- 查询海关税则
    SELECT @VATRate = VATRate / 100, @ImportPreferentialTaxRate = ImportPreferentialTaxRate / 100, 
           @ImportGeneralTaxRate = ImportGeneralTaxRate / 100, @ExciseTaxRate = ISNULL(ExciseTaxRate, 0) / 100
    FROM [PvData].[dbo].[Tariffs] WHERE HSCode = @HSCode;

    -- 查询型号的Eccn数据
    SET @EccnCode = '';
    SELECT @EccnCode = @EccnCode + Code + ',' FROM [PvData].[dbo].[EccnsTopView] WHERE PartNumber = @PartNumber;
    IF LEN(@EccnCode) > 0
    BEGIN
        SET @EccnCode = LEFT(@EccnCode, LEN(@EccnCode) - 1);
    END

    -- 返回查询结果
    SET @Code = 200; SET @Success = 1; SET @Message = '成功';
    SELECT Code = @Code, Success = @Success, Message = @Message,
           PartNumber = @PartNumber, Manufacturer = @Manufacturer,
           HSCode = @HSCode, Name = @Name, Elements = @Elements, CIQCode = @CIQCode, TaxCode = @TaxCode, TaxName = @TaxName,
           VATRate = @VATRate, ImportPreferentialTaxRate = @ImportPreferentialTaxRate,
           ImportGeneralTaxRate = @ImportGeneralTaxRate, ExciseTaxRate = @ExciseTaxRate,
           Ccc = @Ccc, Embargo = @Embargo, HkControl = @HkControl, Coo = @Coo, CIQ = @CIQ, CIQprice = @CIQprice,
           EccnCode = @EccnCode;
END";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = sql;
                sqlcmd.Connection = conn;
                sqlcmd.Parameters.AddWithValue("@paramPN", partnumber);
                sqlcmd.Parameters.AddWithValue("@paramMfr", manufacturer);

                using (var sdr = sqlcmd.ExecuteReader())
                {
                    sdr.Read();
                    //查询结果
                    int code = sdr.GetInt32(0);
                    bool success = sdr.GetBoolean(1);
                    string message = sdr.GetValue(2).ToString();

                    var jsonObj = new JSingle<object>()
                    {
                        code = code,
                        success = success
                    };
                    //查询到数据
                    if (code == 200)
                    {
                        var dic = new Dictionary<string, object>();
                        for (int i = 3; i < sdr.FieldCount; i++)
                        {
                            dic.Add(sdr.GetName(i), sdr[i]);
                        }
                        jsonObj.data = dic;
                    }
                    //未查询到数据
                    else
                    {
                        jsonObj.data = message;
                    }

                    string json = jsonObj.Json();
                }
            }
        }

        void TestLinqToSolr()
        {
            var linq = new LinqToSolr.SolrQuery<StandardProduct>()
                .Where(d => d.PartNumber.Contains("INA117KU") && d.Manufacturer == "Texas Instruments")
                .OrderBy(d => d.Manufacturer)
                .Skip(1)
                .Take(2)
                .Select(d => new { d.ID, d.PartNumber, d.Manufacturer, d.PackageCase, d.Packaging });

            var docs = linq.ToList();

            var doc = new LinqToSolr.SolrQuery<StandardProduct>()
                .Where(d => d.PartNumber == "INA117KU/2K5" && d.Manufacturer == "Texas Instruments")
                .OrderBy(d => d.Manufacturer)
                .Skip(0)
                .Take(2)
                .FirstOrDefault(d => d.ID == "283a4ec96966602543bd776026c36d21");

            var service = new LinqToSolr.SolrService();

            var doc1 = new StandardProduct()
            {
                ID = "1",
                PartNumber = "2",
                Manufacturer = "3",
                PackageCase = "4",
                Packaging = "5",
                CreateDate = DateTime.Now
            };

            var doc2 = new StandardProduct()
            {
                ID = "2",
                PartNumber = "3",
                Manufacturer = "4",
                PackageCase = "5",
                Packaging = "6",
                CreateDate = DateTime.Now
            };

            var list = new List<StandardProduct>();
            list.Add(doc1);
            list.Add(doc2);

            service.Save(list.ToArray());
            service.Delete<StandardProduct>(sp => sp.ID == "1" || sp.ID == "2");
        }

        void TestView()
        {
            var linq1 = new YaHv.PvData.Services.Views.Alls.TariffsAll()
               .OrderBy(d => d.HSCode)
               .Select(d => new { d.ID, hs_Code = d.HSCode, d.Name });

            var r1 = linq1.FirstOrDefault(t => t.hs_Code == "0101210090");

            var linq2 = new YaHv.PvData.Services.Views.Alls.TariffsAll()
                .OrderBy(d => d.HSCode)
                .Select(d => new { d.ID, d.HSCode, d.Name });
            var r2 = linq2.FirstOrDefault(t => t.HSCode == "0101210090");
        }

        void TestSolrRestful()
        {
            string url = "http://localhost:8980/solr/standardproducts/select";
            var queryByGet = Yahv.Utils.Http.ApiHelper.Current.Get<string>(url, new
            {
                q = "*:*",
                wt = "json",
                start = 0,
                rows = 10
            });

            var queryByPost = Yahv.Utils.Http.ApiHelper.Current.JPost<string>(url, new
            {
                @params = new
                {
                    q = "*:*",
                    wt = "json",
                    fl = "id,partnumber,manufacturer",
                    start = 10,
                    fq = "partnumber:CYRS1545AV18",
                    sort = "partnumber asc",
                    rows = 20
                }
            });
        }

        void TestLinqExpression()
        {
            var linq = new YaHv.PvData.Services.Views.Alls.ProductControlsAll()
                .Where(pc => pc.Manufacturer == "TI").Take(10).Select(pc => new
                {
                    pc.ID,
                    pc.PartNumber,
                    pc.Manufacturer,
                    pc.Name
                });

            var test = linq.ToString();
            var test1 = linq.Expression.ToString();

            string url = "http://hv.erp.b1b.com/PvDataApi/Linq/ValidateExpression";
            var result2 = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
            {
                expression = linq.ToString()
            });
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToHalfAngle(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
    }
}