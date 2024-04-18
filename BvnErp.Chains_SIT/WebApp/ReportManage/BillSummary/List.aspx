<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ReportManage.BillSummary.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>账单汇总</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                queryParams: { action: "data" },
                nowrap: false,
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });

            $("#AllOrder").click(function () {
            if ($(this).is(":checked")) {
                $("#OutsideOrder").prop("checked", false);
                $("#InsideOrder").prop("checked", false);
                Search();
            }
        });

            $("#OutsideOrder").click(function () {
            if ($(this).is(":checked")) {
                $("#AllOrder").prop("checked", false);
                $("#InsideOrder").prop("checked", false);
                Search();
            }
        });

            $("#InsideOrder").click(function () {
            if ($(this).is(":checked")) {
                $("#AllOrder").prop("checked", false);
                $("#OutsideOrder").prop("checked", false);
                Search();
            }
        });

        });

        //查询
        function Search() {
            debugger;
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');

            var type = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
            }
            if ($('#OutsideOrder').is(':checked')) {
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            var parm = {
                StartTime: StartTime,
                EndTime: EndTime,
                OrderID: OrderID,
                ClientName: ClientName,
                ClientType: type,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#StartTime').datebox('setValue', null);
            $('#EndTime').datebox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $("#AllOrder").prop("checked", true);
            $("#OutsideOrder").prop("checked", false);
            $("#InsideOrder").prop("checked", false);

            Search();
        }

         //导出Excel
        function ExportExcel() {
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');

            var type = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
            }
            if($('#OutsideOrder').is(':checked')){
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            var param = {
                StartTime: StartTime,
                EndTime: EndTime,
                OrderID: OrderID,
                ClientName: ClientName,
                ClientType: type,
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', param, function (result) {
                MaskUtil.unmask();
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
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号: </td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">客户名称: </td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:150" />
                    </td>
                    <td style="padding-left: 15px">
                        <input type="checkbox" name="Order" value="0" id="AllOrder" title="全部订单" class="checkbox checkboxlist" checked="checked" />
                        <label for="AllOrder" style="margin-right: 20px">全部订单</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="B类" class="checkbox checkboxlist" />
                        <label for="OutsideOrder" style="margin-right: 20px">B类</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="checkbox checkboxlist" />
                        <label for="InsideOrder">A类</label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">报关日期: </td>
                    <td colspan="3">
                        <input class="easyui-datebox" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox" id="EndTime" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="账单汇总" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'MainOrderID',align:'left',width:130">主订单编号</th>
                    <th data-options="field:'OrderID',align:'center',width:150">订单编号</th>
                    <th data-options="field:'ClientName',align:'center',width:180">客户名称</th>
                    <th data-options="field:'ContrNo',align:'center',width:130">报关合同号</th>
                    <th data-options="field:'DDate',align:'center',width:90">报关日期</th>
                    <th data-options="field:'DeclarePrice',align:'center',width:80">货值</th>
                    <th data-options="field:'Currency',align:'center',width:60">币种</th>
                    <th data-options="field:'RMBDeclarePrice',align:'center',width:90">货值(RMB)</th>
                    <th data-options="field:'RealExchangeRate',align:'center',width:80">实时汇率</th>
                    <th data-options="field:'CustomsExchangeRate',align:'center',width:80">海关汇率</th>
                    <th data-options="field:'AgencyRate',align:'center',width:60">代理费率</th>
                    <th data-options="field:'AgencyFee',align:'center',width:60">代理费</th>
                    <th data-options="field:'AddedValueTax',align:'center',width:80">增值税</th>
                    <th data-options="field:'Tariff',align:'center',width:80">关税</th>
                    <th data-options="field:'Incidental',align:'center',width:60">杂费</th>
                    <th data-options="field:'TotalTariff',align:'center',width:80">税费合计</th>
                    <th data-options="field:'TotalDeclare',align:'center',width:100">报关总金额</th>
                    <th data-options="field:'SupplierName',align:'center',width:100">交货供应商</th>
                    <th data-options="field:'InvoiceType',align:'center',width:60">开票类型</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
