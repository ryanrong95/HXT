using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public  class DecHeadDefaultLoad
    {
        public  Dictionary<string, string> LoadElements  {get;set;}

        static object locker = new object();

        static DecHeadDefaultLoad current;


        private DecHeadDefaultLoad()
        {
            Dictionary<string, string> elemetnst = new Dictionary<string, string>();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var LinqCustomMaster = from customMaster in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustomMaster>()
                                       select new Models.BaseCustomMaster
                                       {
                                           ID = customMaster.ID,
                                           Code = customMaster.Code,
                                           Name = customMaster.Name,
                                       };

                string sCustomMaster = LinqCustomMaster.Select(item => new { ID = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.ID).Json();
                elemetnst.Add("CustomMaster", sCustomMaster);

                //运输方式
                var LinqTrafMode = from monitorWay in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTrafMode>()
                                   select new Models.BaseTrafMode
                                   {
                                       ID = monitorWay.ID,
                                       Code = monitorWay.Code,
                                       Name = monitorWay.Name
                                   };
                string sTrafMode = LinqTrafMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json(); ;
                elemetnst.Add("TrafMode", sTrafMode);

                //监管方式
                var LinqTradeMode = from baseTradeMode in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTradeMode>()
                                    select new Models.BaseTradeMode
                                    {
                                        ID = baseTradeMode.ID,
                                        Code = baseTradeMode.Code,
                                        Name = baseTradeMode.Name
                                    }; ;
                string sTradeMode = LinqTradeMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json(); ;
                elemetnst.Add("TradeMode", sTradeMode);

                //征免性质
                var LinqCutMode = from cutMode in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCutMode>()
                                  select new Models.BaseCutMode
                                  {
                                      ID = cutMode.ID,
                                      Code = cutMode.Code,
                                      Name = cutMode.Name,
                                      Detail = cutMode.Detail
                                  }; ;
                string sCutMode = LinqCutMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("CutMode", sCutMode);

                //启运国
                var LinqTradeCountry = from country in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>()
                                       select new Models.Country
                                       {
                                           ID = country.ID,
                                           Code = country.Code,
                                           Name = country.Name,
                                           EnglishName = country.EnglishName,
                                           EditionOneCode = country.EditionOne,
                                       }; ;
                string sTradeCountry = LinqTradeCountry.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("TradeCountry", sTradeCountry);

                //经停港口
                var LinqDistinatePort = from port in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePort>()
                                        select new Models.BasePort
                                        {
                                            ID = port.ID,
                                            Code = port.Code,
                                            Name = port.Name,
                                            EnglishName = port.EnglishName
                                        }; ;
                string sDistinatePort = LinqDistinatePort.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("DistinatePort", sDistinatePort);

                //成交方式
                var LinqTransMode = from baseTransMode in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTransMode>()
                                    select new Models.BaseTransMode
                                    {
                                        ID = baseTransMode.ID,
                                        Code = baseTransMode.Code,
                                        Name = baseTransMode.Name
                                    }; ;
                string sTransMode = LinqTransMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("TransMode", sTransMode);

                //币制
                var LinqCurrency = from entity in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCurrencies>()
                                   select new Models.Currency
                                   {
                                       ID = entity.ID,
                                       Code = entity.Code,
                                       Name = entity.Name,
                                       EnglishName = entity.EnglishName
                                   }; ;
                string sCurrency = LinqCurrency.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("Currency", sCurrency);

                //包装种类
                var LinqWrapType = from basePackType in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePackType>()
                                   select new Models.BasePackType
                                   {
                                       ID = basePackType.ID,
                                       Code = basePackType.Code,
                                       Name = basePackType.Name
                                   }; ;
                string sWrapType = LinqWrapType.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).OrderBy(item => item.Value).Json();
                elemetnst.Add("WrapType", sWrapType);

                //入境口岸
                var LinqEntyPortCode = from EntryPort in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseEntryPort>()
                                       select new Models.BaseEntryPort
                                       {
                                           ID = EntryPort.ID,
                                           Code = EntryPort.Code,
                                           Name = EntryPort.Name,
                                           RomanName = EntryPort.RomanName
                                       }; ;
                string sEntyPortCode = LinqEntyPortCode.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json(); ;
                elemetnst.Add("EntyPortCode", sEntyPortCode);
            }

            this.LoadElements = elemetnst;
        }

        public static DecHeadDefaultLoad Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new DecHeadDefaultLoad();
                        }
                    }
                }

                return current;
            }
        }
    }
}
