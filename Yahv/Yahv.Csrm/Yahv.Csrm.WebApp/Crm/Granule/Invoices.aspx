<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Invoices.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Granule.Invoices" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selType').combobox({
                data: model.Type,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            $('#selStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            var result = "";
            if (model.Client == null) {
                result += "<span style='color:red'>请从左侧客户列表选择客户</span>"
            }
            else {
                if (model.Client.Vip) {
                    result += "当前客户：<span class='vip'></span>";
                }
                result += model.Client.Enterprise.Name
                result += '<span class="level' + model.Client.Grade + '"></span>';
            }
            $("#currentClient").html(result)
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                rownumbers: true,
                singleSelect: false,
                checkOnSelect: false,
                fit: true,
                queryParams: getQuery()
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
        })
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#txtname').textbox("getText")),
                taxperNumber: $.trim($('#txtTaxperNumber').textbox("getText")),
                type: $('#selType').combobox("getValue"),
                // status: $('#selStatus').combobox("getValue"),
            };
            return params;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div id="currentClient" style="padding: 5px"></div>
        <!--工具-->
        <div id="tb">
            <table class="liebiao">
                <tr>
                    <td>开户行</td>
                    <td>
                        <input id="txtname" data-options="prompt:'开户行',validType:'length[1,75]'" class="easyui-textbox" /></td>
                    <td>纳税人识别号</td>
                    <td>
                        <input id="txtTaxperNumber" data-options="prompt:'纳税人识别号',validType:'length[1,75]'" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td>发票类型</td>
                    <td>
                        <select id="selType" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 130px"></select>
                    </td>
                    <td colspan="2">
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
                    <%-- <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>--%>
                    <th data-options="field: 'Type',width:100">发票类型</th>
                    <th data-options="field: 'Bank',width:100">开户行</th>
                    <th data-options="field: 'BankAddress',width:280">开户行地址</th>
                    <th data-options="field: 'Account',width:120">银行账号</th>
                    <th data-options="field: 'TaxperNumber',width:80">纳税人识别号</th>
                    <th data-options="field: 'Address',width:100">收票地址</th>
                    <th data-options="field: 'Postzip',width:80">邮编</th>
                    <th data-options="field: 'ContactName',width:80">联系人</th>
                    <th data-options="field: 'Mobile',width:120">手机号</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
