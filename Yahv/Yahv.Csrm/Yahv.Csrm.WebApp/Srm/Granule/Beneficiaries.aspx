<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Beneficiaries.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Granule.Beneficiaries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var isInit = true;
            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            $('#selMethod').combobox({
                data: model.Methord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            var result = "";
            if (model.Supplier == null) {
                result += "<span style='color:red'>请从左侧供应商列表选择供应商</span>"
            }
            else {
                result += "当前供应商：";
                result += model.Supplier.Enterprise.Name
                result += '<span class="level' + model.Supplier.Grade + '"></span>';
            }
            $("#currentSupplier").html(result)
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                checkOnSelect: false,
                rownumbers: true,
                fit: true,
                queryParams: getQuery(),
                onLoadSuccess: function (data) {
                    data.rows.map(function (element, index) {
                        if (element.IsChecked) {
                            $("#dg").datagrid("checkRow", index);
                            console.log(element)
                        }
                    });
                    isInit = false;
                },
                onCheck: function (index, row) {
                    if (row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认将该受益人分配给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=Bind', { benefitid: row.ID, adminid: model.Admin.ID, sipplierid: model.Supplier.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                },
                onUncheck: function (index, row) {
                    if (!row.IsChecked) {
                        return;
                    }
                    $.messager.confirm('确认', '确认取消分配该受益人给' + model.Admin.RealName + '吗？', function (r) {
                        if (r) {
                            $.post('?action=UnBind', { benefitid: row.ID, adminid: model.Admin.ID, sipplierid: model.Supplier.ID }, function (result) {
                                top.$.messager.alert('操作提示', result.data, 'info');
                            });
                        }
                    })
                }
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div id="currentSupplier" style="padding: 5px"></div>
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">开户行</td>
                    <td>
                        <input id="s_name" name="name" data-options="prompt:'开户行',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">汇款方式</td>
                    <td>
                        <select id="selMethod" name="method" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                </tr>
                <tr>
                    <td>支付币种</td>
                    <td colspan="3">
                        <select id="selCurrency" name="currency" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'StatusName',width:50">状态</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <%--<th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>--%>
                    <th data-options="field: 'RealName',width:100">实际的企业名称</th>
                    <th data-options="field: 'Bank',width:100">开户行</th>
                    <th data-options="field: 'BankAddress',width:120">开户行地址</th>
                    <th data-options="field: 'Account',width:120">银行账号</th>
                    <%--<th data-options="field: 'SwiftCode',width:50">银行编码 (国际)</th>--%>
                    <th data-options="field: 'Methord',width:80">汇款方式</th>
                    <th data-options="field: 'Currency',width:80">支付币种</th>
                    <th data-options="field: 'TaxType',width:80">发票类型</th>
                    <th data-options="field: 'Name',width:80">联系人</th>
                    <th data-options="field: 'Mobile',width:80">手机号</th>
                    <th data-options="field: 'Tel',width:80">电话</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
