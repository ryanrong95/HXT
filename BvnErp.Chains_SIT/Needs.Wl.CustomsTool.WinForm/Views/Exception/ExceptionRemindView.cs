using Needs.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Views
{
    public class ExceptionRemindView
    {
        private static int MaxSetCount = 50;

        private RedisHelper _redis;
        private Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey _redisKey;
        private string _macAddr = string.Empty;
        private string _processName = string.Empty;
        private Needs.Ccs.Services.Enums.BalanceQueueBusinessType _businessType;

        //public ExceptionRemindView(string macAddr, string processName, Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType)
        public ExceptionRemindView(string macAddr, Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType)
        {
            this._redis = new RedisHelper();
            this._redisKey = new Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey(businessType.ToString());
            this._macAddr = macAddr;
            //this._processName = processName;
            this._businessType = businessType;
        }

        public void LoadExceptionDBInfo()
        {
            var keys = this._redis.HashKeys(this._redisKey.RemindDBSet).ToList();
            foreach (var key in keys)
            {
                this._redis.HashDelete(this._redisKey.RemindDBSet, key);
            }

            var top = new Needs.Ccs.Services.Views.BalanceQueueRedis.BalanceQueueView().GetBalanceQueueInfo()
                .Where(t => t.Status == Needs.Ccs.Services.Enums.Status.Normal
                        && t.ProcessStatus == Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.NeedRemind
                        && t.MacAddr == this._macAddr
                        //&& t.ProcessName == this._processName
                        && t.BusinessType == this._businessType)
                .OrderByDescending(t => t.UpdateDate).Take(MaxSetCount).ToList();

            foreach (var item in top)
            {
                this._redis.HashSet<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(_redisKey.RemindDBSet, item.InfoID, item);
            }
        }

        public IEnumerable<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo> GetInfos(out int total, int page, int rows)
        {
            var keys = this._redis.HashKeys(this._redisKey.RemindDBSet).ToList();

            List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo> listInfo = new List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>();

            foreach (var key in keys)
            {
                listInfo.Add(this._redis.HashGet<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.RemindDBSet, key));
            }

            listInfo = listInfo.OrderByDescending(t => t.UpdateDate).ToList();


            if (listInfo.Count > MaxSetCount)
            {
                for (int i = 0; i < listInfo.Count - MaxSetCount; i++)
                {
                    this._redis.HashDelete(this._redisKey.RemindDBSet, listInfo[MaxSetCount + i].InfoID);
                }

                listInfo.RemoveRange(MaxSetCount, listInfo.Count - MaxSetCount);
            }



            total = listInfo.Count();

            var result = listInfo.Skip(rows * (page - 1)).Take(rows);
            return result;
        }
    }
}
