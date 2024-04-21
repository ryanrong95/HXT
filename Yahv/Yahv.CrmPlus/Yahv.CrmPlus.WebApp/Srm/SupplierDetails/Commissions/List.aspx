<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions.List" %>
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
            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?id=' + model.Supplierid,
                    width: '600px',
                    height: '450px',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)AuditStatus.Normal%>') {
                arry.push('<a id="btnDel" href="#" particle="Name:\'删除返款信息\',jField:\'btnDel\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Del(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        
        function Del(id) {
            $.messager.confirm('确认', '确定删除吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已删除!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
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
                    <td colspan="8"><a id="btnAdd" particle="Name:'新增返款信息',jField:'btnAdd'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Currency',width:100">币种</th>
                <th data-options="field:'Msp',width:120">最小金额</th>
                <th data-options="field:'Type',width:120">类型</th>
                <th data-options="field:'Methord',width:120">返款方式</th>
                <th data-options="field:'Radio',width:120">率值</th>
                <th data-options="field:'StatusDes',width:120">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
