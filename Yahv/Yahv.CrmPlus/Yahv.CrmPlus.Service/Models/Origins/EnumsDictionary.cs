using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.CrmPlus.Service.Models.Origins
{

    public class EnumsDictionary : IUnique
    {
        public EnumsDictionary()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 存储枚举的fullName
        /// </summary>
        public string Enum { set; get; }
        /// <summary>
        /// 是否固定
        /// </summary>
        public bool IsFixed { set; get; }
        /// <summary>
        /// 枚举名字
        /// </summary>
        public string Field { set; get; }
        /// <summary>
        /// 枚举描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 枚举值(不要用,暂时留着,使用ID)
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
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
        /// <summary>
        /// Repeat
        /// </summary>
        public event ErrorHanlder Repeat;

        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {

                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    var exsits = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>().
                   Any(item => item.Enum == this.Enum && item.Field == this.Field);
                    if (exsits)
                    {
                        this.Repeat(this, new ErrorEventArgs());
                        return;
                    }
                    this.ID = PKeySigner.Pick(PKeyType.Enums);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.EnumsDictionaries()
                    {
                        ID = this.ID,
                        Enum = this.Enum,
                        IsFixed = this.IsFixed,
                        Field = this.Field,
                        Value = this.Value,
                        Description = this.Description,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>(new
                    {
                        Description = this.Description
                    }, item => item.ID == this.ID);
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>(item => item.ID == this.ID);
                this.AbandonSuccess?.Invoke(this, new SuccessEventArgs());
            }
        }
        #endregion

        void test()
        {
            //EnumsDictionary item = null;
            ////FixedArea area = FixedArea.Abroad;
            //if (item == FixedArea.Abroad)
            //{
            //    //默认为美元
            //    //至少可以判断库房必须是香港等
            //}
        }

        public static implicit operator FixedArea(EnumsDictionary dictionary)
        {
            return ExtendsEnum.ToArray<FixedArea>().SingleOrDefault(item => item.GetFixedID() == dictionary.ID);
        }
        public static implicit operator FixedIndustry(EnumsDictionary dictionary)
        {
            return ExtendsEnum.ToArray<FixedIndustry>().SingleOrDefault(item => item.GetFixedID() == dictionary.ID);
        }
        public static implicit operator FixedSource(EnumsDictionary dictionary)
        {
            return ExtendsEnum.ToArray<FixedSource>().SingleOrDefault(item => item.GetFixedID() == dictionary.ID);
        }
    }
}
