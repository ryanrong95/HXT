<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts.List" %>
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
            $.myWindow({
                title: "新增联系人",
                url: 'Edit.aspx?clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "50%",
            });
            return false;
        }
        function showEditPage(payeeid) {
            $.myWindow({
                title: "修改联系人信息",
                url: 'Edit.aspx?clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID + '&contactid=' + payeeid, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "50%",
            });
            return false;
        }
        function showDetailPage(id) {
            $.myWindow({
                title: "联系人详细信息",
                url: 'Details.aspx?contactid=' + id + '&clientid=' + model.ClientID + '&supplierid=' + model.nSupplierID,
                onClose: function () { },
                width: "60%",
                height: "50%",
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
        function deleteItem(contactid) {
            $.messager.confirm('确认', '您确认想要删除该联系人吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { id: contactid, supplierid: model.nSupplierID, clientid: model.ClientID }, function (success) {
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
                <%--<th data-options="field:'ID',width:100">ID</th>--%>
                <th data-options="field:'Name',width:100">姓名</th>
                <th data-options="field:'Mobile',width:140">手机号</th>
                <th data-options="field:'Tel',width:70">电话</th>
                <th data-options="field:'Email',width:120">邮箱</th>
                <th data-options="field:'QQ',width:150">QQ</th>
                <th data-options="field:'Fax',width:120">传真</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',width:150">修改时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>

