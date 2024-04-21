<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsBeneficiaries.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selMethod').combobox({
                data: model.Methord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selStatus').combobox({
                data: model.Status,
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
                singleSelect: false,
                rownumbers: true,
                fit: true,
                nowrap: false,
                queryParams: getQuery()
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

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd" particle="Name:\'编辑\',jField:\'btnUpd\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('<a id="btnDetail" particle="Name:\'详情\',jField:\'btnDetail\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
            arry.push('<a id="btnDel" particle="Name:\'删除\',jField:\'btnDel\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#s_name').textbox("getText")),
                method: $('#selMethod').combobox("getValue"),
                currency: $('#selCurrency').combobox("getValue"),
                //status: $('#selStatus').combobox("getValue")
            };
            return params;
        };
        function showAddPage() {
            $.myDialog({
                title: "添加受益人",
                url: 'Edit.aspx?supplierid=' + model.SupplierID + '&clientid=' + model.ClientID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showDetailPage(id) {
            $.myWindow({
                title: "受益人详情",
                url: 'Details.aspx?id=' + id + '&clientid=' + model.ClientID + '&supplierid=' + model
                .SupplierID, onClose: function () {

                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myDialog({
                title: "编辑受益人信息",
                url: 'Edit.aspx?id=' + id + '&supplierid=' + model.SupplierID + '&clientid=' + model.ClientID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该受益人吗？', function (r) {
                if (r) {
                    $.post('?action=del', { id: id, clientid: model.ClientID, supplierid: model.SupplierID }, function () {
                        //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
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

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <%-- <input id="s_name" data-options="prompt:'企业名称/开户行/开户行地址/银行账号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 300px" />--%>
                    <td style="width: 90px;">开户行</td>
                    <td>
                        <input id="s_name" name="name" data-options="prompt:'开户行',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">汇款方式</td>
                    <td>
                        <select id="selMethod" name="method" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 90px;">支付币种</td>
                    <td colspan="3">
                        <select id="selCurrency" name="currency" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <%--<td style="width: 90px;">状态</td>
                    <td>
                        <select id="selStatus" name="status" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>--%>
                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" particle="Name:'添加',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field:'Ck',checkbox:true"></th>
                    <th data-options="field:'StatusName',width:50">状态</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field:'Admin',width:80">添加人</th>
                    <%
                        }
                    %>

                    <th data-options="field:'RealName',width:100">实际的企业名称</th>
                    <th data-options="field:'Bank',width:100">开户行</th>
                    <th data-options="field:'BankAddress',width:120">开户行地址</th>
                    <th data-options="field:'Account',width:120">银行账号</th>
                    <%--<th data-options="field: 'SwiftCode',width:50">银行编码 (国际)</th>--%>
                    <th data-options="field:'Methord',width:80">汇款方式</th>
                    <th data-options="field:'Currency',width:80">支付币种</th>
                    <th data-options="field:'TaxType',width:80">发票类型</th>
                    <th data-options="field:'Name',width:80">联系人</th>
                    <th data-options="field:'Mobile',width:80">手机号</th>
                    <th data-options="field:'Tel',width:80">电话</th>
                    <th data-options="field:'IsDefault',width:80">是否默认</th>
                    <th data-options="field:'Btn',formatter:btnformatter,width:195">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
