<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BookAccounts.List" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
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
                singleSelect: true
            });
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?id=' + model.EnterpriseID,
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnEnable" href="#" particle="Name:\'启用收款信息\',jField:\'btnEnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }
            else if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnClosed" href="#" particle="Name:\'停用收款信息\',jField:\'btnClosed\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function Enable(id)
        {
            $.messager.confirm('确认', '确定启用吗？', function (r) {
                if (r) {
                    $.post('?action=Enable', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        }
        function Closed(id)
        {
            $.messager.confirm('确认', '确定停用吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                        // grid.myDatagrid('search', getQuery());
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
                    <td colspan="8"><a id="btnCreator" particle="Name:'新增收款信息',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>

    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'BookAccountMethord',width:80">类型</th>
                <th data-options="field:'nature',width:80">性质</th>
                <th data-options="field:'Bank',width:120">开户行</th>
                <th data-options="field:'BankCode',width:150">行号</th>
                <th data-options="field:'Account',width:150">账号</th>
                <th data-options="field:'CurrencyDes',width:120">币种</th>
                <th data-options="field:'SwiftCode',width:120">SwiftCode</th>
                <th data-options="field:'Transfer',width:150">中转银行</th>
                <th data-options="field:'BankAddress',width:150">银行地址</th>
                <%--<th data-options="field:'Creator',width:100">创建人</th>--%>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
