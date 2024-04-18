<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DualServiceList.aspx.cs" Inherits="WebApp.Finance.ReceivableBill.DualServiceList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $("#DDateStartDate").datebox("setValue", getLastMonthNowDate());
            $("#DDateEndDate").datebox("setValue", getNowFormatDate());

            $('#datagrid').myDatagrid({
                queryParams: {
		            DDateStartDate: $('#DDateStartDate').datebox('getValue'),
                    DDateEndDate: $('#DDateEndDate').datebox('getValue'),
	            },
                border:false,
                fitColumns:true,
                fit:true,
                scrollbarSize: 0,
                rownumbers: true,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

        });

        //查询
        function Search() {
            var DDateStartDate = $('#DDateStartDate').datebox('getValue');
            var DDateEndDate = $('#DDateEndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var InvoiceNo = $('#InvoiceNo').textbox('getValue');

            var parm = {
                DDateStartDate: DDateStartDate,
                DDateEndDate: DDateEndDate,
                OwnerName: OwnerName,
                OrderID: OrderID,
                InvoiceNo: InvoiceNo,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DDateStartDate').datebox('setValue', getLastMonthNowDate());
            $('#DDateEndDate').datebox('setValue', getNowFormatDate());
            $('#OwnerName').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#InvoiceNo').textbox('setValue', null);

            Search();
        }

        //导出Excel
        function ExportExcel() {
            var DDateStartDate = $('#DDateStartDate').datebox('getValue');
            var DDateEndDate = $('#DDateEndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var InvoiceNo = $('#InvoiceNo').textbox('getValue');

            var parm = {
                DDateStartDate: DDateStartDate,
                DDateEndDate: DDateEndDate,
                OwnerName: OwnerName,
                OrderID: OrderID,
                InvoiceNo: InvoiceNo,
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', parm, function (result) {
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

        //获取今天日期
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        }

        //获取上个月今天日期
        function getLastMonthNowDate() {
            var now = new Date();
            var year = now.getFullYear();//getYear()+1900=getFullYear()
            var month = now.getMonth() + 1;//0-11表示1-12月
            var day = now.getDate();
            if (parseInt(month) < 10) {
                month = "0" + month;
            }
            if (parseInt(day) < 10) {
                day = "0" + day;
            }

            now = year + '-' + month + '-' + day;

            if (parseInt(month) == 1) {//如果是1月份，则取上一年的12月份
                return (parseInt(year) - 1) + '-12-' + day;
            }

            var preSize = new Date(year, parseInt(month) - 1, 0).getDate();//上月总天数
            if (preSize < parseInt(day)) {//上月总天数<本月日期，比如3月的30日，在2月中没有30
                return year + '-' + month + '-01';
            }

            if (parseInt(month) <= 10) {
                return year + '-0' + (parseInt(month) - 1) + '-' + day;
            } else {
                return year + '-' + (parseInt(month) - 1) + '-' + day;
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl"><span>客户名称:</span></td>
                    <td>
                        <input class="easyui-textbox" id="OwnerName" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">订单编号:</span></td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="width:200," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"><span>发票号:</span></td>
                    <td>
                        <input class="easyui-textbox" id="InvoiceNo" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">报关日期:</span></td>
                    <td>
                        <input class="easyui-datebox" id="DDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="DDateEndDate" data-options="width:200," />
                    </td>
                    <td colspan="2">
                        <span style="padding-left: 10px;">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExcel()" style="margin-left: 10px;">导出Excel</a>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="应收账款-双抬头服务费" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 18%;">客户名称</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 6%;">报关日期</th>
                    <th data-options="field:'ServiceAmount',align:'left'" style="width: 6%;">服务费金额</th>
                    <th data-options="field:'UnInvoiceAmount',align:'left'" style="width: 6%;">未开票金额</th>
                    <th data-options="field:'InvoiceAmount',align:'left'" style="width: 6%;">发票金额</th>
                    <th data-options="field:'InvoiceNo',align:'left'" style="width: 10%;">发票号</th>
                    <th data-options="field:'InvoiceTime',align:'center'" style="width: 6%;">开票日期</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
