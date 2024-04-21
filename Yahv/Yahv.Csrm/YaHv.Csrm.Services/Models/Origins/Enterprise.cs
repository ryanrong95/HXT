using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class Enterprise : Yahv.Linq.IUnique
    {
        public Enterprise()
        {
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                if (this.Name == "个人承运商")
                {
                    return this.id ?? "Personal";
                }
                else if (this.Name == "芯达通物流部")
                {
                    return this.id ?? "XdtPCL";
                }
                else
                {
                    return this.id;//?? this.Name.MD5();
                }
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 管理编码
        /// </summary>
        /// <chenhan>保障局部唯一</chenhan>
        public string AdminCode { get; set; }
        //public string OldName { set; get; }
        string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                this.name = regex.Replace(Yahv.Utils.Extends.StringExtend.ToHalfAngle(value), " ").Trim();
            }
        }
        //public string Name { set; get; }
        ///// <summary>
        ///// 地域、地区（废弃）
        ///// </summary>
        public string District { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 企业法人
        /// </summary>

        public string Corporation { set; get; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var view = repository.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>().
                    Where(item => item.Name == this.Name);
                bool isUpdate = view.Any();
                if (isUpdate)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                    {
                        AdminCode = this.AdminCode,
                        District = this.District,
                        Status = (int)this.Status,
                        RegAddress = this.RegAddress,
                        Corporation = this.Corporation,
                        Uscc = this.Uscc,
                        Place = this.Place
                    }, item => item.Name == this.Name);

                    this.id = view.ToArray().OrderBy(item => item.Name).First().ID;

                }
                else
                {
                    this.ID = PKeySigner.Pick(PKeyType.Enterprise);
                    //新增逻辑
                    repository.Insert(this.ToLinq());
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        public bool IsExist()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                return repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == this.ID);
            }
        }
        /// <summary>
        /// 谨慎修改名称
        /// </summary>
        /// <param name="Name"></param>
        public bool UpdateName(string NewName)
        {
            bool result = false;
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //只有reg-开头的企业可以修改名称
                if (this.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase)
                    && !repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.Name == NewName))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new { Name = NewName }, item => item.ID == this.ID);
                    result = true;
                }
            }
            return result;
        }
        #endregion  
    }
}


//var old = repository.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
//                        .Where(item => item.Name == this.OldName).SingleOrDefault();
////Layers.Data.Sqls.PvbCrm.Enterprises updater;
//object updater;
//                if (old != null)
//                {
//                    if (this.OldName.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
//                    {
//    updater = new
//    {
//        Name = this.Name,
//        AdminCode = this.AdminCode,
//        District = this.District,
//        Status = (int)this.Status,
//        RegAddress = this.RegAddress,
//        Corporation = this.Corporation,
//        Uscc = this.Uscc,
//        Place = this.Place
//    };
//}
//                    else
//                    {
//    updater = new
//    {
//        AdminCode = this.AdminCode,
//        District = this.District,
//        Status = (int)this.Status,
//        RegAddress = this.RegAddress,
//        Corporation = this.Corporation,
//        Uscc = this.Uscc,
//        Place = this.Place
//    };
//}
//                    this.ID = old.ID;
//repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(updater,
//                        item => item.ID == old.ID);
//}
//                else
//                {

//                    this.ID = PKeySigner.Pick(PKeyType.Enterprise);
//                    //新增逻辑
//                    repository.Insert(this.ToLinq());

//var enterprise = repository.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
//                       .Where(item => item.Name == this.Name).SingleOrDefault();
//                    if (enterprise == null)
//                    {
//    this.ID = PKeySigner.Pick(PKeyType.Enterprise);
//    //新增逻辑
//    repository.Insert(this.ToLinq());
//}
//                    else
//                    {
//    updater = new
//    {
//        AdminCode = this.AdminCode,
//        District = this.District,
//        Status = (int)this.Status,
//        RegAddress = this.RegAddress,
//        Corporation = this.Corporation,
//        Uscc = this.Uscc,
//        Place = this.Place
//    };
//    this.ID = enterprise.ID;
//    repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(updater,
//   item => item.ID == enterprise.ID);
//}

//}
