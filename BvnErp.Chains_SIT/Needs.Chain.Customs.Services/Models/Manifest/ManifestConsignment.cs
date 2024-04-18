using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单的运单
    /// </summary>
    [Serializable]
    public class ManifestConsignment : IUnique, IPersist
    {
        /// <summary>
        /// 运单号
        /// </summary>
        public string ID { get; set; }

        public Manifest Manifest { get; set; }

        /// <summary>
        /// 运单状态
        /// </summary>
        public string CusMftStatus { get; set; }

        /// <summary>
        /// 运输条款
        /// </summary>
        public int? ConditionCode { get; set; }

        public int PaymentType { get; set; }

        public string GovProcedureCode { get; set; }

        public string TransitDestination { get; set; }

        public int PackNum { get; set; }

        public string PackType { get; set; }

        public decimal? Cube { get; set; }

        public decimal GrossWt { get; set; }

        public decimal GoodsValue { get; set; }

        /// <summary>
        /// 产品总数量
        /// </summary>
        public decimal GoodsQuantity { get; set; }

        public string Currency { get; set; }

        public string Consolidator { get; set; }

        /// <summary>
        /// 收货人名称
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 发货人名称
        /// </summary>
        public string ConsignorName { get; set; }

        /// <summary>
        /// 制单报文地址
        /// </summary>
        public string MarkingUrl { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 提运单-商品列表
        /// </summary>
        ManifestConsignmentItems items;
        public ManifestConsignmentItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.ManifestConsignmentItemsView())
                    {
                        var query = view.Where(item => item.ManifestConsignmentID == this.ID);
                        this.Items = new ManifestConsignmentItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new ManifestConsignmentItems(value, new Action<ManifestConsignmentItem>(delegate (ManifestConsignmentItem item)
                {
                    item.ManifestConsignmentID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 提运单-集装箱列表
        /// </summary>
        ManifestConsignmentContainers containers;
        public ManifestConsignmentContainers Containers
        {
            get
            {
                if (containers == null)
                {
                    using (var view = new Views.ManifestConsignmentContainersView())
                    {
                        var query = view.Where(item => item.ManifestConsignmentID == this.ID);
                        this.Containers = new ManifestConsignmentContainers(query);
                    }
                }
                return this.containers;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.containers = new ManifestConsignmentContainers(value, new Action<ManifestConsignmentContainer>(delegate (ManifestConsignmentContainer item)
                {
                    item.ManifestConsignmentID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 舱单-回执报文轨迹
        /// </summary>
        ManifestConsignmentTraces traces;
        public ManifestConsignmentTraces Traces
        {
            get
            {
                if (traces == null)
                {
                    using (var view = new Views.ManifestConsignmentTracesView())
                    {
                        var query = view.Where(item => item.ManifestConsignmentID == this.ID);
                        this.Traces = new ManifestConsignmentTraces(query);
                    }
                }
                return this.traces;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.traces = new ManifestConsignmentTraces(value, new Action<ManifestConsignmentTrace>(delegate (ManifestConsignmentTrace item)
                {
                    item.ManifestConsignmentID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 舱单创建
        /// </summary>
        public event ManifestCreatedHanlder ManifestCreated;

        /// <summary>
        /// 舱单制单成功
        /// </summary>
        public event ManifestMakedHanlder ManifestMaked;

        /// <summary>
        /// 舱单删除
        /// </summary>
        public event ManifestMakedHanlder ManifestDeleted;

        /// <summary>
        /// 舱单申报（报文准备就绪）
        /// </summary>
        public event ManifestApplyHanlder ManifestApply;

        /// <summary>
        /// 舱单取消
        /// </summary>
        public event ManifestCancelHandler ManifestCancel;

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public ManifestConsignment()
        {
            //舱单制单成功
            this.ManifestMaked += ManifestConsignment_ManifestMaked;

            //舱单申报（报文准备就绪）
            this.ManifestApply += ManifestConsignment_ManifestApply;

            //舱单取消
            this.ManifestCancel += ManifestConsignment_ManifestCancel;

            //舱单删除
            this.ManifestDeleted += ManifestConsignment_ManifestDeleted;

            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 转换舱单-构造函数
        /// </summary>
        /// <param name="decHead"></param>
        public ManifestConsignment(DecHead decHead) : this()
        {
            var model = decHead.Lists.OrderBy(t => t.GNo).FirstOrDefault();
            this.ID = decHead.BillNo;
            this.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Draft);
            //this.ConditionCode = this.ConditionCode;
            this.PaymentType = 1;//1-Direct payment
            this.GovProcedureCode = "RD01";//公路口岸直通
            //this.TransitDestination = this.TransitDestination;
            this.PackNum = decHead.PackNo;
            //TODO:从报关单的22 如何转换到舱单的CT？？
            this.PackType = "CT";//CT 纸板箱
            //this.Cube = this.Cube;
            this.GrossWt = decHead.GrossWt;
            this.GoodsValue = decHead.Lists.Sum(t => t.DeclTotal);
            this.GoodsQuantity = decHead.Lists.Sum(t => t.GQty);
            this.Currency = model.TradeCurr;
            //this.Consolidator = this.Consolidator;
            this.ConsigneeName = decHead.ConsigneeName;
            this.ConsignorName = decHead.ConsignorName;
            this.Admin = new Admin
            {
                ID = decHead.Inputer.ID
            };

            this.Manifest = new Manifest();
            this.Manifest.ID = decHead.VoyNo;
            this.Manifest.TrafMode = 3;//3	公路运输 road transport
            //this.Manifest.CustomsCode = decHead.CustomMaster;
            this.Manifest.CustomsCode = decHead.IEPort;//使用报关单中的 实际进境关别
            //manifest.CarrierCode = this.CarrierCode;
            //manifest.TransAgentCode = this.TransAgentCode;
            this.Manifest.LoadingDate = DateTime.Now;
            this.Manifest.LoadingLocationCode = decHead.IEPort;//使用报关单中的 实际进境关别
            this.Manifest.ArrivalDate = DateTime.Now;
            this.Manifest.CustomMaster = PurchaserContext.Current.CustomMaster;//使用芯达通配置的 备案关别？？？
            this.Manifest.UnitCode = decHead.ConsigneeScc;
            this.Manifest.MsgRepName = decHead.ConsigneeName;
            //manifest.AdditionalInformation = this.AdditionalInformation;

            var item = new ManifestConsignmentItem();

            //以下按照当前做单方式，全部默认只写一行商品项信息
            item.GoodsSeqNo = 1;
            item.GoodsPackNum = this.PackNum;
            item.GoodsPackType = this.PackType;
            item.GoodsGrossWt = this.GrossWt;
            item.GoodsBriefDesc = model.GName;//第一项品名
            //item.UndgNo
            //item.HsCode
            //item.GoodsDetailDesc
            this.Items.Add(item);

            if (decHead.Containers.Count > 0)
            {
                var contain = new ManifestConsignmentContainer();
                contain.ContainerNo = decHead.Containers.FirstOrDefault().ContainerID;
                contain.ManifestConsignmentID = this.ID;
                this.Containers.Add(contain);
            }

            this.ManifestCreated += ManifestConsignment_ManifestCreated;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //this.Manifest.Enter();//舱单目前没有在界面编辑

                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    //新增提运单
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    //更新提运单
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    //清除项数据
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignmentContainers>(i => i.ManifestConsignmentID == this.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignmentItems>(i => i.ManifestConsignmentID == this.ID);
                }
                
                //集装箱信息
                foreach (var container in this.Containers)
                {
                    container.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ManifestConsignmentContainers
                    {
                        ID = container.ID,
                        ManifestConsignmentID = this.ID,
                        ContainerNo = container.ContainerNo
                    });
                }

                //商品项信息
                foreach (var item in this.Items)
                {
                    item.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ManifestConsignmentItems
                    {
                        ID = item.ID,
                        ManifestConsignmentID = this.ID,
                        GoodsSeqNo = item.GoodsSeqNo,
                        GoodsPackNum = item.GoodsPackNum,
                        GoodsPackType = item.GoodsPackType,
                        GoodsGrossWt = item.GoodsGrossWt,
                        GoodsBriefDesc = item.GoodsBriefDesc,
                        UndgNo = item.UndgNo,
                        HsCode = item.HsCode,
                        GoodsDetailDesc = item.GoodsDetailDesc
                    });
                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #region 舱单创建

        /// <summary>
        /// 生成舱单，提运单
        /// </summary>
        public void CreateManifestConsignment()
        {
            this.Manifest.Enter();
            this.Enter();
            this.OnCreated(new ManifestCreatedEventArgs(this));
        }

        public virtual void OnCreated(ManifestCreatedEventArgs args)
        {
            this.ManifestCreated?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestCreated(object sender, ManifestCreatedEventArgs e)
        {
            e.ManifestConsignment.Trace("舱单新增");
            //设置复核人          
            string adminID = GetDeclareCreatorAdminID(e.ManifestConsignment.ID);
            //更新运单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {               
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { DoubleCheckerAdminID = adminID }, item => item.ID == this.ID);
            }
        }

        private string GetDeclareCreatorAdminID(string billNo) 
        {
            string adminID = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var dechead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.BillNo == billNo).FirstOrDefault();
                if (dechead != null) 
                {
                    adminID = dechead.DoubleCheckerAdminID;
                }
            }
            return adminID;
        }

        private string GetDeclareCreatorAdminID(List<CurrentUnDecNoticeCountViewModel> listModel)
        {
            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.UnDecNoticeCount).FirstOrDefault().UnDecNoticeCount;
            int[] serialNos = listModel.Where(t => t.UnDecNoticeCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].UnDecNoticeCount = listModel[i].UnDecNoticeCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }

        #endregion

        #region 舱单复核
        public void DoubleCheck()
        {
            //更新运单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.DoubleChecked);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus }, item => item.ID == this.ID);
            }
        }
        #endregion 

        #region 舱单制单

        /// <summary>
        /// 生成Xml报文
        /// </summary>
        public void Make()
        {
            var message = new ManifestMessage.Manifest(this);
            var fileName = message.SaveAs(this.Manifest.ID + '_' + this.ID + ".xml");

            //更新运单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Make);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus, MarkingUrl = fileName }, item => item.ID == this.ID);
            }

            this.OnMaked(new ManifestMakedEventArgs(this, fileName));
        }

        public virtual void OnMaked(ManifestMakedEventArgs args)
        {
            this.ManifestMaked?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestMaked(object sender, ManifestMakedEventArgs e)
        {
            e.ManifestConsignment.Trace("制单，生成报文，报文名称:" + e.FileName);
        }

        #endregion

        #region 舱单申报

        /// <summary>
        /// 生成.zip，提交海关
        /// </summary>
        public void Apply()
        {
            var FunCode = this.CusMftStatus == MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Deleting) ? "_03" : string.Empty;

            string fileName = this.Manifest.ID + "_" + this.ID + FunCode + ".zip.temp";

            //创建文件夹
            FileDirectory file = new FileDirectory();
            System.Net.WebClient wbClient = new System.Net.WebClient();
            List<string> files = new List<string>();
            var clientPath = string.Empty;

            var ManifestMessageRootPath = System.Configuration.ConfigurationManager.AppSettings["ManifestMessageRootPath"];

            //原则：谁使用谁拼接
            clientPath = ManifestMessageRootPath + SysConfig.DecMessageDirectory + @"\" + this.Manifest.ID + "_" + this.ID + FunCode + ".xml";
            
            files.Add(clientPath);
            var filepath = file.FileServerUrl + @"\" + this.MarkingUrl;
            wbClient.DownloadFile(filepath, clientPath);

            //压缩单个文件
            ZipSingleFile zip = new ZipSingleFile(fileName);
            zip.File = files[0];
            zip.ContainedFileName = this.Manifest.ID + "_" + this.ID + FunCode + ".xml";
            zip.ZipedPath = ManifestMessageRootPath + SysConfig.MeaasgeFolder + @"\";
            //zip.Zip();
            zip.ZipFileManifest();


            //删除已被压缩的源文件
            files.ForEach(t =>
            {
                File.Delete(t);
            });

            //.temp文件重命名
            File.Move(zip.ZipedPath + fileName, zip.ZipedPath + this.Manifest.ID + "_" + this.ID + FunCode + ".zip");

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Declare);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus }, item => item.ID == this.ID);
            }

            this.OnApplied(new ManifestApplyEventArgs(this));
        }

        public virtual void OnApplied(ManifestApplyEventArgs args)
        {
            this.ManifestApply?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestApply(object sender, ManifestApplyEventArgs e)
        {
            e.ManifestConsignment.Trace("导出报文.zip至文件夹，等待发送或自动发送至海关");
        }

        #endregion

        #region 取消
        public void Cancel()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更改舱单状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { Status = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Cancel) }, item => item.ID == this.ID);
            }

            this.OnCanceled(new ManifestApplyEventArgs(this));
        }

        public virtual void OnCanceled(ManifestApplyEventArgs args)
        {
            this.ManifestCancel?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestCancel(object sender, ManifestApplyEventArgs e)
        {
            e.ManifestConsignment.Trace("舱单取消!");
        }

        #endregion

        #region 舱单删除
        public void Delete()
        {
            var message = new Needs.Ccs.Services.Models.ManifestMessageDelete.Manifest(this);
            var fileName = message.SaveAs(this.Manifest.ID + '_' + this.ID + '_' + SysConfig.MessageDelete + ".xml");

            //更新运单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Deleting);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus, MarkingUrl = fileName }, item => item.ID == this.ID);
            }
            this.OnDeleted(new ManifestMakedEventArgs(this, fileName));
        }

        public virtual void OnDeleted(ManifestMakedEventArgs args)
        {
            this.ManifestDeleted?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestDeleted(object sender, ManifestMakedEventArgs e)
        {
            e.ManifestConsignment.Trace("删单，生成报文，报文名称:"+e.FileName);
        }
        #endregion
    }

}
