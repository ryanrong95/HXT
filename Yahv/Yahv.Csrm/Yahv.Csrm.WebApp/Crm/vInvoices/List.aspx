<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.vInvoices.List" %>

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
                rownumbers: true,
                singleSelect: false,
                fit: true,
                queryParams: getQuery(),
                nowrap: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
        })
    </script>
    <script>

        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '确认删除吗？', function (r) {
                if (r) {
                    $.post('?action=del', { id: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            });
        }
        function showAddPage() {
            $.myDialog({
                title: "新增",
                url: 'Add.aspx?EnterpriseID=' + model.EnterpriseID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?id=' + id + "&EnterpriseID=" + model.EnterpriseID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function defaultformmater(value, rec) {
            return value ? "是" : "否";
        }
        function personformmater(value, rec) {
            return value ? "个人/非企业单位" : "企业"
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div id="tb">
            <div>
                <table class="liebiao-compact">

                    <tr>
                        <td colspan="8">
                            <a id="btnCreator" particle="Name:'添加',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
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
                    <th data-options="field: 'IsPersonal',formatter:personformmater,width:100">抬头类型</th>
                    <th data-options="field: 'Type',width:100">发票类型</th>
                    <th data-options="field: 'Title',width:100">抬头</th>
                    <th data-options="field: 'DeliveryType',width:100">交付方式</th>
                    <%-- <th data-options="field: 'BankName',width:100">开户行</th>
                    <th data-options="field: 'Account',width:120">银行账号</th>--%>
                    <th data-options="field: 'PostRecipient',width:80">收票人</th>
                    <th data-options="field: 'PostTel',width:120">联系电话</th>
                    <th data-options="field: 'PostAddress',width:180">邮寄地址</th>
                    <th data-options="field: 'IsDefault',formatter:defaultformmater,width:80">是否默认</th>
                    <%--<th data-options="field: 'TaxpNumber',width:120">纳税人识别号</th>--%>

                    <%-- <th data-options="field: 'PostZipCode',width:80">邮编</th>--%>

                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'RealName',width:80">添加人</th>
                    <%
                        }
                    %>
                    <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>

