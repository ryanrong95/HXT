<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="InvoiceDetail.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.InvoiceDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                nowrap: false,
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                $('#OrderID').textbox("setValue", "");
                $('#CompanyName').textbox("setValue", "");
                $('#StartDate').datebox("setValue", "");
                $('#EndDate').datebox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                OrderID: $('#OrderID').textbox('getValue'),
                ComanyName: $('#CompanyName').textbox('getValue'),
                StartDate: $('#StartDate').datebox('getValue'),
                EndDate: $('#EndDate').datebox('getValue'),
            };
            return params;
        };

        //导出Excel
        function ExportExcel() {
            var orderID = $('#OrderID').textbox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var data = $('#tab1').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            //验证成功
            $.post('?action=ExportExcel', {
                OrderID: orderID,
                ComanyName: companyName,
                StartDate: startDate,
                EndDate: endDate
                // Data: JSON.stringify(data),
            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td colspan="8">
                    <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td style="width: 280px;">
                    <input class="easyui-textbox" id="OrderID" data-options="width:280,validType:'length[1,50]'" />
                </td>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 280px;">
                    <input class="easyui-textbox" id="CompanyName" data-options="width:280,validType:'length[1,50]'" />
                </td>
                <td style="width: 90px;">开票日期</td>
                <td style="width: 280px;">
                    <input id="StartDate" data-options="prompt:'',width:110," class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'',width:110," class="easyui-datebox" />
                </td>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="开票明细">
        <thead>
            <tr>
                <th data-options="field:'DetailAmountDes',align:'left'" style="width: 8%;">含税金额</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 16%;">开票公司</th>
                <th data-options="field:'InvoiceNo',align:'left'" style="width: 16%;">发票号码</th>
                <th data-options="field:'InvoiceTimeDes',align:'center'" style="width: 8%;">开票日期</th>
                <th data-options="field:'TaxAmountDes',align:'left'" style="width: 8%;">税额</th>
                <th data-options="field:'OrderIDs',align:'left'" style="width: 16%;">订单号</th>
            </tr>
        </thead>
    </table>
</asp:Content>
