<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageList.aspx.cs" Inherits="WebApp.ReportManage.FeeReport.StorageList" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>账单汇总</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var OrderPremiumType = eval('(<%=this.Model.OrderPremiumType%>)');
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                queryParams: { action: "data" },
                nowrap: false,
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });

            //$('#FeeType').combobox({
            //    data: OrderPremiumType,
            //});

        });



        //查询
        function Search() {
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var FeeType = $('#FeeType').textbox('getValue');

            var parm = {
                StartTime: StartTime,
                EndTime: EndTime,
                ClientName: ClientName,
                FeeType: FeeType
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#StartTime').datebox('setValue', null);
            $('#EndTime').datebox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#FeeType').textbox('setValue', null);

            Search();
        }

        //导出Excel
        function ExportExcel() {
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var FeeType = $('#FeeType').textbox('getValue');
            

            var param = {
                StartTime: StartTime,
                EndTime: EndTime,
                ClientName: ClientName,
                FeeType: FeeType,
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
                    <td class="lbl">客户名称: </td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">费用类型: </td>
                    <td>
                        <%--<input class="easyui-combobox" id="FeeType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />--%>
                        <input class="easyui-textbox" id="FeeType" data-options="height:26,width:150" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">日期: </td>
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
        <table id="datagrid" title="费用汇总" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CreateTime',align:'left',width:100">日期</th>
                    <th data-options="field:'payCompanyName',align:'center',width:200">客户名称</th>
                    <th data-options="field:'OrderID',align:'center',width:180">订单号</th>
                    <th data-options="field:'Subject',align:'center',width:130">费用科目明细</th>
                    <th data-options="field:'ReceivableAmount',align:'center',width:130">应付金额</th>
                    <th data-options="field:'ReceivableCNYAmount',align:'center',width:130">应付金额(CNY)</th>
                    <th data-options="field:'PayableAmount',align:'center',width:130">应收金额(CNY)</th>
                    <th data-options="field:'PayableTaxedAmount',align:'center',width:130">应收含税金额(CNY)</th>
                   <%-- <th data-options="field:'PayableCurrency',align:'center',width:60">应付币种</th>--%>
                    <th data-options="field:'ReceiptsAmount',align:'center',width:130">实收金额(CNY)</th>
                    <th data-options="field:'OwedMoney',align:'center',width:130">欠款(CNY)</th>
                    <th data-options="field:'Discount',align:'center',width:90">费用优惠(CNY)</th>
                    <th data-options="field:'AdminName',align:'center',width:80">制单人</th>
                  <%--  <th data-options="field:'Mechandiser',align:'center',width:80">跟单客服</th>
                    <th data-options="field:'Salesman',align:'center',width:60">业务员</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>