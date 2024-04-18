<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.PayableBill.List" %>

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
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: false,
                queryParams: {
		            DDateStartDate: $('#DDateStartDate').datebox('getValue'),
                    DDateEndDate: $('#DDateEndDate').datebox('getValue'),
	            },
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
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
                }
            });

        });

        //查询
        function Search() {
            var DDateStartDate = $('#DDateStartDate').datebox('getValue');
            var DDateEndDate = $('#DDateEndDate').datebox('getValue');
            var ConsignorCode = $('#ConsignorCode').textbox('getValue');

            var parm = {
                DDateStartDate: DDateStartDate,
                DDateEndDate: DDateEndDate,
                ConsignorCode: ConsignorCode,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DDateStartDate').datebox('setValue', getLastMonthNowDate());
            $('#DDateEndDate').datebox('setValue', getNowFormatDate());
            $('#ConsignorCode').textbox('setValue', null);

            Search();
        }

        //导出Excel
        function ExportExcel() {
            var DDateStartDate = $('#DDateStartDate').datebox('getValue');
            var DDateEndDate = $('#DDateEndDate').datebox('getValue');
            var ConsignorCode = $('#ConsignorCode').textbox('getValue');

            var parm = {
                DDateStartDate: DDateStartDate,
                DDateEndDate: DDateEndDate,
                ConsignorCode: ConsignorCode,
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

        ////列表框按钮加载
        //function Operation(val, row, index) {
        //    var buttons = '';

        //    return buttons;
        //}

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

        //导出发票
        function ExportInvoice() {
            var rows = $('#datagrid').myDatagrid('getChecked');
            if (rows.length <= 0) {
                $.messager.alert('提示', '请勾选！');
                return;
            }

            //拼接 DecHeadIDs 字符串
            var DecHeadIDs = "";
            for (var i = 0; i < rows.length; i++) {
                DecHeadIDs += rows[i].DecHeadID + ",";
            }
            DecHeadIDs = DecHeadIDs.substr(0, DecHeadIDs.length - 1);

            var parm = {
                DecHeadIDs: DecHeadIDs,
            };

            MaskUtil.mask();
            $.post('?action=ExportInvoice', parm, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
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

        //导出合同
        function ExportContract() {
            var rows = $('#datagrid').myDatagrid('getChecked');
            if (rows.length <= 0) {
                $.messager.alert('提示', '请勾选！');
                return;
            }

            //拼接 DecHeadIDs 字符串
            var DecHeadIDs = "";
            for (var i = 0; i < rows.length; i++) {
                DecHeadIDs += rows[i].DecHeadID + ",";
            }
            DecHeadIDs = DecHeadIDs.substr(0, DecHeadIDs.length - 1);

            var parm = {
                DecHeadIDs: DecHeadIDs,
            };

            MaskUtil.mask();
            $.post('?action=ExportContract', parm, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
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

        //导出报关单
        function ExportDecHeadFile() {
            var rows = $('#datagrid').myDatagrid('getChecked');
            if (rows.length <= 0) {
                $.messager.alert('提示', '请勾选！');
                return;
            }

            //拼接 DecHeadIDs 字符串
            var DecHeadIDs = "";
            for (var i = 0; i < rows.length; i++) {
                DecHeadIDs += rows[i].DecHeadID + ",";
            }
            DecHeadIDs = DecHeadIDs.substr(0, DecHeadIDs.length - 1);

            var parm = {
                DecHeadIDs: DecHeadIDs,
            };

            MaskUtil.mask();
            $.post('?action=ExportDecHeadFile', parm, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
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

        //导出增值税发票
        function ExportDecHeadVatFile() {
            var rows = $('#datagrid').myDatagrid('getChecked');
            if (rows.length <= 0) {
                $.messager.alert('提示', '请勾选！');
                return;
            }

            //拼接 DecHeadIDs 字符串
            var DecHeadIDs = "";
            for (var i = 0; i < rows.length; i++) {
                DecHeadIDs += rows[i].DecHeadID + ",";
            }
            DecHeadIDs = DecHeadIDs.substr(0, DecHeadIDs.length - 1);

            var parm = {
                DecHeadIDs: DecHeadIDs,
            };

            MaskUtil.mask();
            $.post('?action=ExportDecHeadVatFile', parm, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
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
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">报关日期:</td>
                    <td>
                        <input class="easyui-datebox" id="DDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="DDateEndDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">供应商:</span></td>
                    <td>
                        <input class="easyui-textbox" id="ConsignorCode" data-options="width:200," />
                    </td>
                    <td colspan="2">
                        <span style="padding-left: 10px;">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExcel()">导出Excel</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportInvoice()" style="margin-left: 10px;">导出发票</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportContract()" style="margin-left: 10px;">导出合同</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportDecHeadFile()" style="margin-left: 10px;">导出报关单</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportDecHeadVatFile()" style="margin-left: 10px;">导出增值税发票</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="应付账款" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ConsignorCode',align:'center'" style="width: 12%;">供应商</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 6%;">币种</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 10%;">报关日期</th>
                    <th data-options="field:'DeclarationAmount',align:'left'" style="width: 8%;">报关金额</th>
                    <th data-options="field:'YunBaoZa',align:'left'" style="width: 6%;">运保杂</th>
                    <th data-options="field:'AttorneyAmount',align:'left'" style="width: 8%;">委托金额</th>
                    <th data-options="field:'ThatDayExchangeRate',align:'center'" style="width: 6%;">当天汇率</th>
                    <th data-options="field:'CustomsExchangeRate',align:'center'" style="width: 6%;">海关汇率</th>
                    <%--<th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 110px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
