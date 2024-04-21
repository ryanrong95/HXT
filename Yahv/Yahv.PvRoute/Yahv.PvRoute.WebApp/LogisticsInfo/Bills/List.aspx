<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvRoute.WebApp.LogisticsInfo.Bills.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
     <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.css" rel="stylesheet">

     <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            $('#s_carrier').combobox({
                data: model.Sources,
                valueField: "value",
                textField: "text",
                multiple: false,
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            //刷新按钮
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

             $("#btnImport").on("click",
                function () {
                    $("#<%=fileUpload.ClientID%>").click();
                });

            $("#<%=fileUpload.ClientID%>").on("change", function () {
                if (this.value === "") {
                    top.$.messager.alert('提示', '请选择要上传的Excel文件');
                    return;
                } else {
                    var index = this.value.lastIndexOf(".");
                    var extention = this.value.substr(index);
                    if (extention !== ".xls" && extention !== ".xlsx") {
                        top.$.messager.alert('提示', '请选择excel格式的文件!');
                        return;
                    }

                    $("#<%= btn_Import.ClientID %>").click();
                    this.value = '';        //清空值，确保每次导入都触发change事件
                }
            });
        });
    </script>

     <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                faceOrderID: $.trim($('#faceOrderID').textbox("getText")),
                source: $.trim($('#s_carrier').combobox('getValue')),
                dateIndex: $.trim($('#dateIndex').textbox("getText")),
            };
            return params;
        };

      

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
     <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>

      <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">快递单号</td>
                <td style="width: 300px;">
                    <input id="faceOrderID" data-options="prompt:'快递单号'" runat="server" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">承运商</td>
                <td style="width: 300px;">
                    <select id="s_carrier" data-options="editable: false," class="easyui-combobox" runat="server" style="width: 200px;" />
                </td>
                 <td style="width: 90px;">期号</td>
                <td style="width: 300px;">
                    <input id="dateIndex" data-options="prompt:'期号'" style="width: 200px;" runat="server" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                <em class="toolLine"></em>
                     <a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" data-options="iconCls:'icon-yg-excelExport'">Excel导出</a>
                    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>
                </td>
                <td colspan="2">
                     <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="对账单">
        <thead>
            <tr>
                <th data-options="field:'FaceOrderID',align:'center',width:fixWidth(10)">快递单号</th>
                <th data-options="field:'Quantity',align:'left',width:fixWidth(4)">件数</th>
                <th data-options="field:'Weight',align:'left',width:fixWidth(4)">重量</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(6)">金额</th>
                <th data-options="field:'CurrencyDes',align:'left',width:fixWidth(8)">币种</th>
                <th data-options="field:'CarrierName',align:'left',width:fixWidth(6)">承运商</th>
                <th data-options="field:'Checker',align:'left',width:fixWidth(6)">核对人</th>
                <th data-options="field:'CheckTime',align:'left',width:fixWidth(12)">核对时间</th>
                <th data-options="field:'Reviewer',align:'left',width:fixWidth(6)">审核人</th>
                <th data-options="field:'ReviewTime',align:'left',width:fixWidth(12)">审核时间</th>
                <th data-options="field:'Cashier',align:'left',width:fixWidth(6)">出纳人</th>
                <th data-options="field:'CashierTime',align:'left',width:fixWidth(12)">出纳时间</th>
                <th data-options="field:'DateIndex',align:'left',width:fixWidth(6)">期号</th>
                <%--<th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(12)">操作</th>--%>
            </tr>
        </thead>
    </table>
</asp:Content>
