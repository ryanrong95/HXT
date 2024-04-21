using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Usually;

namespace Wms.Services.Models
{

    /// <summary>
    /// 库区
    /// </summary>
    public class Region : BaseShelves
    {

        #region 事件
        //enter成功
        public event SuccessHanlder ShelvesSuccess;
        //enter失败
        public event ErrorHanlder ShelvesFailed;
        //不支持修改
        public event ErrorHanlder NotSupportedUpdate;
        //删除成功
        public event SuccessHanlder AbandonSuccess;
        //删除失败
        public event ErrorHanlder AbandonFailed;
        //名称重复
        public event ErrorHanlder CheckNameRepeated;
        //ID不支持修改
        public event ErrorHanlder IDNotSupportModify;

        #endregion
        public override void Abandon()
        {
            //throw new NotImplementedException();
        }

        public override void Enter()
        {

            using (var repository = new PvWmsRepository())
            {

                //id为空是添加
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = this.FatherID + "-" + this.Name;

                    if (CheckNameRepeated != null)
                    {
                        //ID的个数大于1是名字重复
                        if (new Views.ShelvesView()[this.ID] != null)
                        {
                            CheckNameRepeated.Invoke(this, new ErrorEventArgs("Name Repeated!!"));
                            return;
                        }
                    }
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    this.Status = ShelvesStatus.Normal;
                    repository.Insert(this.ToLinq());
                }

                //否则是修改
                else
                {

                    var oldID = this.ID;//原有的ID
                    if (new Views.ShelvesView()[oldID] == null)
                    {
                        this.NotSupportedUpdate?.Invoke(this, new ErrorEventArgs("所要修改的数据不存在"));
                        return;
                    }

                    var newID = this.FatherID + "-" + this.Name;//FatherID和this.Name组合而成的编号

                    //原有的ID == Shelves.FatherID的个数>0的时候不可以进行编辑ID
                    if (new Views.ShelvesView().Where(item => item.FatherID == oldID).Count() > 0)
                    {
                        //id不支持修改
                        if (newID != oldID)
                        {
                            this.IDNotSupportModify?.Invoke(this, new ErrorEventArgs("ID does not support modification!!"));
                            return;
                        }
                    }

                    if (CheckNameRepeated != null)
                    {
                        //newID的个数大于1并且newID不是this.ID是名字重复
                        if (new Views.ShelvesView()[newID] != null && newID != oldID)
                        {
                            CheckNameRepeated.Invoke(this, new ErrorEventArgs("Name Repeated!!"));
                            return;
                        }
                    }

                    this.UpdateDate = DateTime.Now;
                    repository.Update(this.ToLinq(), item => item.ID == oldID);
                }

            }

        }
    }
}
