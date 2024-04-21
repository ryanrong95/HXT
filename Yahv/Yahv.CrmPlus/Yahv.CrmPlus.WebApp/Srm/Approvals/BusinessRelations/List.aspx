<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Approvals.BusinessRelations.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($('#txtname').textbox("getText")),
                    RelationsType: $('#cbbType').combobox("getValue")
                };
                return params;
            };
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            $("#cbbType").fixedCombobx({
                required: false,
                type: "BusinessRelationType",
                isAll: true
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            //arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalPass\'" onclick="approval(true,\'' + rowData.ID + '\')">审批通过</a> ');
            //arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalNopass\'" onclick="approval(false,\'' + rowData.ID + '\')">审批不通过</a> '); 
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnApproval" href="#" particle="Name:\'审批\',jField:\'Approval\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="approval(\'' + rowData.ID + '\',\'' + rowData.MainID + '\')">审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function approval(id,mainid) {
            $.myWindow({
                title: '审批',
                url: 'Edit.aspx?subid=' + id + '&enterpriseid=' + mainid,
                width: '800px',
                height: '400px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        //function approval(result, id) {
        //    $.post('?action=Approve', { id: id, result: result }, function (success) {
        //        if (success) {
        //            top.$.timeouts.alert({
        //                position: "TC",
        //                msg: "操作成功",
        //                type: "success"
        //            });
        //            grid.myDatagrid('flush');
        //        }
        //        else {
        //            top.$.timeouts.alert({
        //                position: "TC",
        //                msg: "操作失败",
        //                type: "error"
        //            });
        //        }
        //    });
        //}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td>企业</td>
                    <td>
                        <input id="txtname" class="easyui-textbox" /></td>
                    <td>关联类型</td>
                    <td>
                        <input id="cbbType" class="easyui-combobox"></td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <%-- <em></em>
                          <a id="btnApplyLog" class="easyui-linkbutton" data-options="iconCls:'icon-yg-details'">审批记录</a>--%>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'MainName',width:200,sortable:true">主体公司</th>
                <th data-options="field:'SubName',width:200,sortable:true">关联公司</th>
                <th data-options="field:'Relation',width:200,sortable:true">关联类型</th>
                <th data-options="field:'CreateDate',width:180,sortable:true">申请时间</th>
                <th data-options="field:'CreatorName',width:80,sortable:true">申请人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
