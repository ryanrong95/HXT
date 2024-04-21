<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nPayees.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
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
        })
        function showAddPage() {
            $.myWindowFuse({
                title: "新增收款人",
                url: 'Edit.aspx?clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "56%",
                height: "48%",
            });
            return false;
        }
        function showEditPage(payeeid) {
            $.myWindowFuse({
                title: "修改收款人信息",
                url: 'Edit.aspx?clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID + '&payeeid=' + payeeid, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "56%",
                height: "48%",
            });
            return false;
        }
        function showDetailPage(id) {
            $.myWindow({
                title: "受益人详情",
                url: 'Details.aspx?payeeid=' + id + '&clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID,
                onClose: function () { },
                width: "56%",
                height: "48%",
            });
            return false;
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                arry.push('<a id="btnUpd" particle="Name:\'编辑\',jField:\'btnUpd\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDetail" particle="Name:\'详情\',jField:\'btnDetail\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
                arry.push('<a id="btnDel" particle="Name:\'删除\',jField:\'btnDel\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');


        }
        function deleteItem(payeeid) {
            $.messager.confirm('确认', '您确认想要删除该收款人吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { id: payeeid, supplierid: model.nSupplierID, clientid: model.ClientID }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除失败!",
                                type: "error"
                            });
                        }

                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8">

                        <a id="btnCreator" class="easyui-linkbutton" particle="Name:'添加',jField:'btnCreator'" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
               <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                <th data-options="field:'Methord',width:100">支付方式</th>
               <%-- <th data-options="field:'Currency',width:70">币种</th>--%>
                <th data-options="field:'Account',width:140">账号</th>
                 <th data-options="field:'Place',width:120">国家/地区</th>
                <th data-options="field:'Bank',width:120">开户行</th>
                <th data-options="field:'BankAddress',width:150">开户行地址</th>
                <th data-options="field:'SwiftCode',width:120">SwiftCode</th>
                <th data-options="field:'Contact',width:120">联系人</th>
                <th data-options="field:'Mobile',width:100">手机号</th>
                <th data-options="field:'Tel',width:90">电话</th>
                <th data-options="field:'Email',width:100">邮箱</th>
                <th data-options="field:'IsDefault',width:100">是否默认</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',width:150">修改时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
