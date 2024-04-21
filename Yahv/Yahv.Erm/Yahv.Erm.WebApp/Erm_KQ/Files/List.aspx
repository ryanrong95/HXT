<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Files.List" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>
<%@ Import Namespace="Yahv.Erm.Services" %>
<%@ Import Namespace="Yahv" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

        });
    </script>
    <style>
        .panel-header {
            border: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none" title="文件和模板下载">
            <table class="liebiao-compact">
                <tr>
                    <td class="lbl" style="width: 120px">员工手册：</td>
                    <td>
                        <a id="btn_Handbook" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_Handbook_Click">公司员工手册</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">管理规定：</td>
                    <td>
                        <a id="btn_RecruitmentRegulations" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_RecruitmentRegulations_Click">员工招聘管理规定</a>
                        <em class="toolLine"></em>
                        <a id="btn_TrainRegulations" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_TrainRegulations_Click">员工培训管理规定</a>
                        <em class="toolLine"></em>
                        <a id="btn_RPRegulations" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_RPRegulations_Click">员工奖惩管理制度</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">招聘相关：</td>
                    <td>
                        <a id="btn_RecruitmentNeeds" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_RecruitmentNeeds_Click">人员招聘需求表</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">应聘相关：</td>
                    <td>
                        <a id="btn_Application" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_Application_Click">应聘人员登记表</a>
                        <em class="toolLine"></em>
                        <a id="btn_Interview" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_Interview_Click">面试情况评估表</a>
                        <em class="toolLine"></em>
                        <a id="btn_Investigate" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_Investigate_Click">员工背景调查报告</a>
                        <em class="toolLine"></em>
                        <a id="btn_Register" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_Register_Click">入职登记表</a>
                        <em class="toolLine"></em>
                        <a id="btn_TurnNormal" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_TurnNormal_Click">员工转正申请表</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">培训相关：</td>
                    <td>
                        <a id="btn_PXNDJH" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXNDJH_Click">年度培训计划表</a>
                        <em class="toolLine"></em>
                        <a id="btn_PXBHG" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXBHG_Click">培训不合格人员通知单</a>
                        <em class="toolLine"></em>
                        <a id="btn_PXJDB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXJDB_Click">培训签到表</a>
                        <em class="toolLine"></em>
                        <a id="btn_PXSQB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXSQB_Click">培训申请表</a>
                        <em class="toolLine"></em>
                        <a id="btn_PXXMBGB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXXMBGB_Click">培训项目变更确认表</a>
                        <em class="toolLine"></em>
                        <a id="btn_PXXQB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_PXXQB_Click">培训需求表</a>
                        <em class="toolLine"></em>
                        <a id="btn_WXSQB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_WXSQB_Click">外训申请表</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">奖惩相关：</td>
                    <td>
                        <a id="btn_RPapply" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_RPapply_Click">员工奖惩申请表</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">工牌相关：</td>
                    <td>
                        <a id="btn_GPapply" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_GPapply_Click">工牌补办申请表</a>
                        <em class="toolLine"></em>
                        <a id="btn_GPFFDJB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_GPFFDJB_Click">工牌发放归还登记表</a>
                        <em class="toolLine"></em>
                        <a id="btn_LSGPFFDJB" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_LSGPFFDJB_Click">临时工牌发放归还登记表</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="width: 120px">离职相关：</td>
                    <td>
                        <a id="btn_ResignationApply" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_ResignationApply_Click">离职申请表</a>
                        <em class="toolLine"></em>
                        <a id="btn_ResignationHandover" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_ResignationHandover_Click">离职交接表</a>
                        <em class="toolLine"></em>
                        <a id="btn_ResignationConfirm" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_ResignationConfirm_Click">离职证明</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
