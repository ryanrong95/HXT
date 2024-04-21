<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Approvals.Specials.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
           
            $("#cbbType").fixedCombobx({
                required: false,
                type: "nBrandType",
                isAll: true
            })
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($('#txtname').textbox("getText")),
                    Brand: $('#txtBrand').textbox("getText"),
                    Pn: $('#txtPn').textbox("getText"),
                    nBrandType: $('#cbbType').combobox("getValue")
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
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
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalPass\'" onclick="approval(true,\'' + rowData.ID + '\')">审批通过</a> ');
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalNopass\'" onclick="approval(false,\'' + rowData.ID + '\')">审批不通过</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function approval(result, id) {
            $.post('?action=Approve', { id: id, result: result }, function (success) {
                if (success) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "操作成功",
                        type: "success"
                    });
                    grid.myDatagrid('flush');
                }
                else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "操作失败",
                        type: "error"
                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td>供应商名称:</td>
                    <td>
                        <input id="txtname" class="easyui-textbox" /></td>
                    <td>品牌:</td>
                    <td>
                        <input id="txtBrand" class="easyui-textbox" /></td>
                    <td>型号:</td>
                    <td>
                        <input id="txtPn" class="easyui-textbox" /></td>
                    <td>特色类型:</td>
                    <td>
                        <input id="cbbType" class="easyui-combobox">
                    </td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
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
                <th data-options="field:'SupplierName',width:200,sortable:true">供应商名称</th>
                <th data-options="field:'Brand',width:200,sortable:true">品牌</th>
                <th data-options="field:'PartNumber',width:200,sortable:true">型号</th>
                <th data-options="field:'Type',width:80">特色类型</th>
                <th data-options="field:'Summary',width:220">备注</th>
                <th data-options="field:'CreateDate',width:120,sortable:true">申请时间</th>
                <th data-options="field:'CreatorName',width:80,sortable:true">申请人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:300">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
