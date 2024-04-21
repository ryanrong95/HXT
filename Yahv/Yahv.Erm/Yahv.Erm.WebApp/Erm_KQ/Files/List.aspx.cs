using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Application = Yahv.Erm.Services.Models.Origins.Application;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.Files
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 员工手册

        protected void btn_Handbook_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\芯达通公司员工手册.docx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 管理规定

        protected void btn_RecruitmentRegulations_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工招聘管理规定.docx";
            DownLoadFile(fileName);
        }

        protected void btn_TrainRegulations_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训管理制度.doc";
            DownLoadFile(fileName);
        }

        protected void btn_RPRegulations_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工奖惩管理制度.docx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 离职相关文件

        protected void btn_ResignationApply_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职申请表.xlsx";
            DownLoadFile(fileName);
        }
        
        protected void btn_ResignationHandover_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职交接表.docx";
            DownLoadFile(fileName);
        }
       
        protected void btn_ResignationConfirm_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\离职证明.docx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 应聘相关文件

        protected void btn_Application_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\应聘人员登记表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_Interview_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\面试情况评估表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_Investigate_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工背景调查报告.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_Register_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\入职登记表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_TurnNormal_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\试用期转正申请表.xlsx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 招聘相关文件

        protected void btn_RecruitmentNeeds_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\人员招聘需求表.docx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 培训相关文件

        protected void btn_PXNDJH_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\年度培训计划表.xlsx";
            DownLoadFile(fileName);
        }

        protected void btn_PXBHG_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训不合格人员通知单.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_PXJDB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训签到表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_PXSQB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训申请表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_PXXMBGB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训项目变更确认表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_PXXQB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\培训需求表.xlsx";
            DownLoadFile(fileName);
        }
        protected void btn_WXSQB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\外训申请表.xlsx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 奖惩相关文件

        protected void btn_RPapply_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工奖惩申请表.xlsx";
            DownLoadFile(fileName);
        }

        #endregion

        #region 奖惩相关文件

        protected void btn_GPapply_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\工牌补办申请表.xlsx";
            DownLoadFile(fileName);
        }

        protected void btn_GPFFDJB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\工牌发放归还登记表.xlsx";
            DownLoadFile(fileName);
        }

        protected void btn_LSGPFFDJB_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\临时工牌发放归还登记表.xlsx";
            DownLoadFile(fileName);
        }

        #endregion
    }
}