<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiptedList.aspx.cs" Inherits="WebApp.Finance.Receipt.Receipted.ReceiptedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {

            $("#ReceiptDateStartDate").datebox("setValue", getLastMonthNowDate());
            $("#ReceiptDateEndDate").datebox("setValue", getNowFormatDate());

            //下拉框数据初始化
            //收款类型
            var FinanceFeeType = eval('(<%=this.Model.FinanceFeeType%>)');
            $('#FeeType').combobox({
                data: FinanceFeeType,
            });

            //金库
            var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
            $('#FinanceVault').combobox({
                data: FinanceVaultData,
            });

            //账户
            var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');
            $('#FinanceAccount').combobox({
                data: FinanceAccountData,
            });

            $('#datagrid').myDatagrid({
                queryParams: {
		            ReceiptDateStartDate: $('#ReceiptDateStartDate').datebox('getValue'),
                    ReceiptDateEndDate: $('#ReceiptDateEndDate').datebox('getValue'),
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
                },
                onLoadSuccess: function (data) {
                    //合并单元格
			        var mark = 1;
                    for (var i = 0; i < data.rows.length; i++) {
                        //合并
                        if (i > 0) {
                            if (data.rows[i]['FinanceReceiptID'] == data.rows[i - 1]['FinanceReceiptID']) {
                                mark += 1;

						        $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'SeqNo',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'FinanceAccountName',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'Payer',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'FeeType',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'Amount',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'Currency',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'ReceiptDate',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'ThatDayExchangeRate',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'TariffAmount',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'AddedValueTaxAmount',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'ExciseTaxAmount',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'ServiceAmount',
							        rowspan: mark
                                });

                            } else {
						        mark = 1;
					        }
                        }
                    }
                },
            });

        });

        //查询
        function Search() {
            var FeeTypeInt = $('#FeeType').combobox('getValue');
            var Payer = $('#Payer').textbox('getValue');
            var ReceiptDateStartDate = $('#ReceiptDateStartDate').datebox('getValue');
            var ReceiptDateEndDate = $('#ReceiptDateEndDate').datebox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');
            var FinanceVaultID = $('#FinanceVault').combobox('getValue');
            var FinanceAccountID = $('#FinanceAccount').combobox('getValue');

            var parm = {
                FeeTypeInt: FeeTypeInt,
                Payer: Payer,
                ReceiptDateStartDate: ReceiptDateStartDate,
                ReceiptDateEndDate: ReceiptDateEndDate,
                SeqNo: SeqNo,
                FinanceVaultID: FinanceVaultID,
                FinanceAccountID: FinanceAccountID,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#FeeType').combobox('setValue', null);
            $('#Payer').textbox('setValue', null);
            $('#ReceiptDateStartDate').datebox('setValue', getLastMonthNowDate());
            $('#ReceiptDateEndDate').datebox('setValue', getNowFormatDate());
            $('#SeqNo').textbox('setValue', null);
            $('#FinanceVault').combobox('setValue', null);
            $('#FinanceAccount').combobox('setValue', null);

            Search();
        }

        //导出Excel
        function ExportExcel() {
            var FeeTypeInt = $('#FeeType').combobox('getValue');
            var Payer = $('#Payer').textbox('getValue');
            var ReceiptDateStartDate = $('#ReceiptDateStartDate').datebox('getValue');
            var ReceiptDateEndDate = $('#ReceiptDateEndDate').datebox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');
            var FinanceVaultID = $('#FinanceVault').combobox('getValue');
            var FinanceAccountID = $('#FinanceAccount').combobox('getValue');

            var parm = {
                FeeTypeInt: FeeTypeInt,
                Payer: Payer,
                ReceiptDateStartDate: ReceiptDateStartDate,
                ReceiptDateEndDate: ReceiptDateEndDate,
                SeqNo: SeqNo,
                FinanceVaultID: FinanceVaultID,
                FinanceAccountID: FinanceAccountID,
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
                    <td class="lbl"><span style="padding-left: 10px;">收款类型:</span></td>
                    <td>
                        <input class="easyui-combobox" id="FeeType" name="FeeType" style="width: 200px;"
                            data-options="editable: false, valueField:'Key',textField:'Value'" />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">付款人:</span></td>
                    <td>
                        <input class="easyui-textbox" id="Payer" name="Payer" style="width: 200px;" />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">收款日期:</span></td>
                    <td>
                        <input class="easyui-datebox" id="ReceiptDateStartDate" style="width: 200px;" />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="ReceiptDateEndDate" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"><span style="padding-left: 10px;">流水号:</span></td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" style="width: 200px;" />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">金库:</span></td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" data-options="valueField:'Value',textField:'Text',editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">账户:</span></td>
                    <td>
                        <input class="easyui-combobox" id="FinanceAccount" data-options="valueField:'Value',textField:'Text',editable:false" style="width: 200px;" />
                    </td>
                    <td colspan="2">
                        <span style="padding-left: 10px;">
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExcel()" style="margin-left: 15px;">导出Excel</a>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 10%;">银行流水号</th>
                    <th data-options="field:'FinanceVaultName',align:'left'" style="width: 6%;">金库名称</th>
                    <th data-options="field:'FinanceAccountName',align:'left'" style="width: 10%;">账户名称</th>
                    <th data-options="field:'Payer',align:'left'" style="width: 12%;">客户名称</th>
                    <th data-options="field:'FeeType',align:'left'" style="width: 6%;">收款类型</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 6%;">金额</th>
                    <th data-options="field:'Currency',align:'left'" style="width: 4%;">币种</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 6%;">收款日期</th>
                    <th data-options="field:'ThatDayExchangeRate',align:'left'" style="width: 4%;">当天汇率</th>
                    <th data-options="field:'ProductAmount',align:'left'" style="width: 6%;">货款</th>
                    <th data-options="field:'SwapExchangeRate',align:'left'" style="width: 4%;">换汇汇率</th>
                    <th data-options="field:'TariffAmount',align:'left'" style="width: 6%;">关税</th>
                    <th data-options="field:'AddedValueTaxAmount',align:'left'" style="width: 6%;">增值税</th>
                    <th data-options="field:'ExciseTaxAmount',align:'left'" style="width: 6%;">消费税</th>
                    <th data-options="field:'ServiceAmount',align:'left'" style="width: 6%;">服务费</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
