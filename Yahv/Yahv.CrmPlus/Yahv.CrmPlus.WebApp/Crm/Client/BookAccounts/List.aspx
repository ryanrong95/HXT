<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.BookAccounts.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
    $(function(){

       //客户付款账号
           var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
             
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: "新增",
                    url: 'Edit.aspx?&enterpriseid=' + model.ID, onClose: function () {
                        location.reload();
                    },
                    width: "50%",
                    height: "60%",
                });
            });
});

      //付款账号操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)Yahv.Underly.DataStatus.Normal%>') {
                arry.push('<a id="btnAccountUnable" href="#" particle="Name:\'付款账号停用\',jField:\'btnAccountUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="BookAccountClosed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)Yahv.Underly.DataStatus.Closed%>') {
                arry.push('<a id="btnAccountEnable" particle="Name:\'付款账号启用\',jField:\'btnAccountEnable\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="BookAccountEnable(\'' + rowData.ID + '\')">启用</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8"><a id="btnCreator" particle="Name:'付款账号新增',jField:'btnCreator1'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a></td>
                </tr>
                <tr></tr>
            </table>
            <table id="dg" data-options="rownumbers:true" style="width: 100%">
                <thead>
                    <tr>
                        <th data-options="field:'Account',width:120">账号</th>
                        <th data-options="field:'BookAccountType',width:120">账户类型</th>
                        <th data-options="field:'nature',width:220">性质</th>
                        <th data-options="field:'BookAccountMethord',width:120">支付方式</th>
                        <th data-options="field:'Bank',width:120">开户行</th>
                        <th data-options="field:'BankAddress',width:120">开户行地址</th>
                        <th data-options="field:'SwiftCode',width:120">银行代码</th>
                        <th data-options="field:'BankCode',width:120">行号</th>
                        <th data-options="field:'CurrencyDes',width:120">币种</th>
                        <th data-options="field:'StatusName',width:80">状态</th>
                        <th data-options="field:'Creator',width:100">创建人</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
                    </tr>
                </thead>
            </table>
</asp:Content>
