<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Chains.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            $("#btnCreator").click(function ()
            {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx',
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })
        })
        function btnformatter(value,rowData)
        {

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
   
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a>
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
                <th data-options="field: 'Name',width:120">客户名称</th>
                <th data-options="field: 'WsCode',width:120">入仓号</th>
                <th data-options="field: 'Type',width:120">类型</th>
                <th data-options="field: 'Owner',width:120">业务员</th>
                <th data-options="field: 'Tracker',width:120">跟单员</th>
                <th data-options="field: 'Referrer',width:120">引荐人</th>
                <th data-options="field: 'StatusDes',width:80">状态</th>
                <th data-options="field: 'CreteDate',width:150">创建时间</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
