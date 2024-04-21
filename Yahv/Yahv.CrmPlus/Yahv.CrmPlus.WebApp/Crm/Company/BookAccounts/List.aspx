<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.BookAccounts.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#txtname').textbox("getText")),
                    selStatus: $('#selStatus').combobox("getValue"),
                    bookAccountType:$('#txtType').combobox("getValue"),
                };
                return params;
            };

            $('#selStatus').combobox({
                data: model.DataStatus,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });
            $('#txtType').combobox({
                data: model.BookAccountType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        });
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }

            arry.push('</span>');
            return arry.join('');
        }

        function showAddPage() {
            $.myDialog({
                title: "新增",
                url: 'Edit.aspx?&companyid=' + model.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }

        function Closed(id) {
            $.messager.confirm('确认', '您确认想要停用收付款信息吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id, CompanyID: model.ID }, function () {

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
        function Enable(id) {
            $.messager.confirm('确认', '您确认启用该收付款信息吗？', function (r) {
                if (r) {
                    $.post('?action=Enable', { ID: id, CompanyID: model.ID }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 100px;">账号</td>
                    <td>
                        <input id="txtname" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td>账户类型</td>
                    <td>
                        <input id="txtType" data-options="prompt:'账户类型',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">状态</td>

                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Bank',width:120">开户行</th>
                <th data-options="field:'nature',width:220">性质</th>
                <th data-options="field:'BankAddress',width:120">开户行地址</th>
                <th data-options="field:'Account',width:120">账号</th>
                <th data-options="field:'BookAccountType',width:120">账户类型</th>
                <th data-options="field:'BookAccountMethord',width:120">支付方式</th>
                <th data-options="field:'SwiftCode',width:120">银行代码</th>
                <th data-options="field:'BankCode',width:120">行号</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'Creator',width:100">创建人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

