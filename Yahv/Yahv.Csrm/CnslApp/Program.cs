using Newtonsoft.Json.Linq;
using ShencLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Views;

namespace CnslApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.Csrm\CnslApp\jdBackData.json", Encoding.UTF8);
            var jObject = JObject.Parse(json);


            //jObject.result.error_code


            var jResult = jObject["result"];
            var jSonResult = jResult["result"];

            var data = new
            {
                code = jObject["code"].Value<string>(),
                charge = jObject["charge"].Value<bool>(),
                result = new
                {
                    error_code = jResult["error_code"].Value<string>(),

                    result = new
                    {
                        investorList = jSonResult["investorList"].Select(item => new
                        {
                            name = item["name"].Value<string>(),
                            capitalActl = JArray.Parse(item["capitalActl"].Value<string>()).Select(capital => new
                            {
                                amomon = capital["amomon"].Value<decimal>()
                            })

                        })
                    }
                }
            };

            //data.result.result.investorList.Select(item => new 数据库对象类型
            //{

            //});



            //new DccPayer().Enter(new ShencLibrary.DccPayer.MyClientPayer
            //{
            //    ClientID = "B379DFBAAD593A682DB06CC16D002260",
            //    Contact = "陈翰",
            //    Currency = Yahv.Underly.Currency.CNY,
            //    Methord = Yahv.Underly.Methord.Check,
            //    Place = "CNY",
            //    RealName = "123456公司"
            //});
        }
    }
}
