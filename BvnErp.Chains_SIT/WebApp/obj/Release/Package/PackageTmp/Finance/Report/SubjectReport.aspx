<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectReport.aspx.cs" Inherits="WebApp.Finance.Report.SubjectReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //gvSettings.fatherMenu = '财务报表(XDT)';
        //gvSettings.menu = '科目明细';
        //gvSettings.summary = '科目明细报表';
    </script>
    <script type="text/javascript">
        $(function () {
          
            //订单列表初始化
            $('#orders').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,
                pageSize: 20,
                rownumbers: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
              
            });

        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                ContrNo: ContrNo,
                ClientName: ClientName,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#ClientName').textbox('ClientName', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //导出
        function Export() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var pageNumber = $("#orders").datagrid("options").pageNumber;
            var pagesise = $("#orders").datagrid("options").pageSize;

            if (!(StartDate != "" || EndDate != "" || ContrNo != "" || ClientName != "")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', { ContrNo: ContrNo, StartDate: StartDate, EndDate: EndDate,page:pageNumber,rows:pagesise}, function (result) {
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
            });
        }

        //导出统计
        function ExportStatistics() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var pageNumber = $("#orders").datagrid("options").pageNumber;
            var pagesise = $("#orders").datagrid("options").pageSize;

            if (!(StartDate != "" || EndDate != "" || ContrNo != "" || ClientName != "")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportStatistics', { ContrNo: ContrNo, StartDate: StartDate, EndDate: EndDate, page: pageNumber, rows: pagesise }, function (result) {
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
            });
        }

    </script>

</head>
<body class="easyui-layout">
    <div id="topBar">

        <div style="margin-left: 15px;">
            <ul style="list-style-type: none;">
                <li style="margin-top: 5px;">
                    <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" style="width: 250px;" />
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 34px;">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl" style="margin-left: 34px;">至: </span>
                    <input class="easyui-datebox" id="EndDate" />
                </li>
                <li style="margin-top: 5px;">
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Export()">导出Excel</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportStatistics()">导出汇总</a>
                </li>
            </ul>
        </div>
    </div>

    <table id="orders" title="科目明细报表" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                <th data-options="field:'InvoiceTypeName',align:'center'" style="width: 5%">开票类型</th>
                <th data-options="field:'ConsignorCode',align:'left'" style="width: 10%">供应商</th>
                <th data-options="field:'DeclareDate',align:'left'" style="width: 6%">报关日期</th>
                <th data-options="field:'ContrNo',align:'left'" style="width: 8%">合同号</th>
                <th data-options="field:'DecForeignTotal',align:'center'" style="width: 6%">报关外币</th>
                <th data-options="field:'DecAgentTotal',align:'center'" style="width: 6%">委托外币</th>
                <th data-options="field:'DecYunBaoZaTotal',align:'center'" style="width: 6%">运保杂外币</th>
                <th data-options="field:'DecTotalPriceRMB',align:'center'" style="width: 6%">报关总价</th>
                <th data-options="field:'ImportPrice',align:'center'" style="width: 6%">进口</th>
                <th data-options="field:'SalePrice',align:'center'" style="width: 6%">运保杂</th>
                <th data-options="field:'Tariff',align:'left'" style="width: 6%">应交关税</th>
                <th data-options="field:'ActualExciseTax',align:'left'" style="width: 6%">实缴消费税</th>
                <th data-options="field:'ActualAddedValueTax',align:'left'" style="width: 6%">实缴增值税</th>
                <th data-options="field:'ExchangeCustomer',align:'left'" style="width: 6%">汇兑-客户</th>
                <th data-options="field:'ExchangeXDT',align:'left'" style="width: 6%">汇兑-芯达通</th>
                <th data-options="field:'RealExchangeRate',align:'left'" style="width: 6%">实时汇率</th>
                <th data-options="field:'DueCustomerFC',align:'center'" style="width: 6%">应付-客户外币</th>
                <th data-options="field:'DueCustomerRMB',align:'center'" style="width: 6%">应付-客户RMB</th>
                <th data-options="field:'DueXDTFC',align:'left'" style="width: 6%">应付-芯达通外币</th>
                <th data-options="field:'DueXDTRMB',align:'center'" style="width: 6%">应付-芯达通RMB</th>
                <th data-options="field:'ActualTariff',align:'center'" style="width: 6%">实交关税</th>
                <th data-options="field:'DutiablePrice',align:'left'" style="width: 6%">完税价格</th>
<%--                <th data-options="field:'InvoiceNo',align:'left'" style="width: 6%">发票号</th>
                <th data-options="field:'InvoiceDate',align:'left'" style="width: 5%">开票日期</th>--%>
            </tr>
        </thead>
    </table>
</body>
</html>
