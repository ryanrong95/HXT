<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaiedList.aspx.cs" Inherits="WebApp.Finance.Payment.Paied.PaiedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var FeeType = eval('(<%=this.Model.FeeType%>)');
        var PayType = eval('(<%=this.Model.PayType%>)');

        $(function () {
            $("#PayDateStartDate").datebox("setValue", getLastMonthNowDate());
            $("#PayDateEndDate").datebox("setValue", getNowFormatDate());

            //下拉框数据初始化
            $('#FeeType').combobox({
                data: FeeType
            });
            $('#PayType').combobox({
                data: PayType
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
                    PayDateStartDate: $('#PayDateStartDate').datebox('getValue'),
                    PayDateEndDate: $('#PayDateEndDate').datebox('getValue'),
                },
                border: false,
                fitColumns: true,
                fit: true,
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
            var FeeType = $('#FeeType').combobox('getValue');
            var PayType = $('#PayType').combobox('getValue');
            var PayDateStartDate = $('#PayDateStartDate').datebox('getValue');
            var PayDateEndDate = $('#PayDateEndDate').datebox('getValue');
            var FinanceVaultID = $('#FinanceVault').combobox('getValue');
            var FinanceAccountID = $('#FinanceAccount').combobox('getValue');

            $('#datagrid').myDatagrid('search', {
                FeeType: FeeType,
                PayType: PayType,
                PayDateStartDate: PayDateStartDate,
                PayDateEndDate: PayDateEndDate,
                FinanceVaultID: FinanceVaultID,
                FinanceAccountID: FinanceAccountID,
            });
        }

        //重置查询条件
        function Reset() {
            $('#FeeType').combobox('setValue', null);
            $('#PayType').combobox('setValue', null);
            $('#PayDateStartDate').datebox('setValue', getLastMonthNowDate());
            $('#PayDateEndDate').datebox('setValue', getNowFormatDate());
            $('#FinanceVault').combobox('setValue', null);
            $('#FinanceAccount').combobox('setValue', null);
            Search();
        }

        //导出Excel
        function ExportExcel() {
            var FeeType = $('#FeeType').combobox('getValue');
            var PayType = $('#PayType').combobox('getValue');
            var PayDateStartDate = $('#PayDateStartDate').datebox('getValue');
            var PayDateEndDate = $('#PayDateEndDate').datebox('getValue');
            var FinanceVaultID = $('#FinanceVault').combobox('getValue');
            var FinanceAccountID = $('#FinanceAccount').combobox('getValue');

            var parm = {
                FeeType: FeeType,
                PayType: PayType,
                PayDateStartDate: PayDateStartDate,
                PayDateEndDate: PayDateEndDate,
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

        //纸质单据
        function PaperInvoice(val, row, index) {
            var buttons = '';

            if (row.IsPaperInvoiceUploadInt == '<%=Needs.Ccs.Services.Enums.UploadStatus.NotUpload.GetHashCode()%>') {
                //当前未上传
                buttons = '<span style="color: red; margin-right: 8px;">未上交</span>';

                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetUploaded(\'' + row.FinancePaymentID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">设置为已上交</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else if (row.IsPaperInvoiceUploadInt == '<%=Needs.Ccs.Services.Enums.UploadStatus.Uploaded.GetHashCode()%>') {
                //当前已上传
                buttons = '<span style="color: green;  margin-right: 8px;">已上交</span>';

                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetUnUpload(\'' + row.FinancePaymentID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">设置为未上交</span>' +
                    '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        //设置为已上传
        function SetUploaded(financePaymentID) {
            MaskUtil.mask();
            $.post('?action=SetUploaded', { FinancePaymentID: financePaymentID, }, function (data) {
                MaskUtil.unmask();
                var rel = JSON.parse(data);
                $.messager.alert('消息', rel.message, 'info', function () {
                    $('#datagrid').myDatagrid('reload');
                });
            });
        }

        //设置为未上传
        function SetUnUpload(financePaymentID) {
            MaskUtil.mask();
            $.post('?action=SetUnUpload', { FinancePaymentID: financePaymentID, }, function (data) {
                MaskUtil.unmask();
                var rel = JSON.parse(data);
                $.messager.alert('消息', rel.message, 'info', function () {
                    $('#datagrid').myDatagrid('reload');
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
                    <td class="lbl"><span style="padding-left: 10px;">费用类型:</span></td>
                    <td>
                        <input class="easyui-combobox" id="FeeType" name="FeeType" style="width: 200px"
                            data-options="editable: false, valueField:'Key',textField:'Value'" />
                    </td>

                    <td class="lbl"><span style="padding-left: 10px;">付款类型:</span></td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="FeeType" style="width: 200px"
                            data-options="editable: false, valueField:'Key',textField:'Value'" />
                    </td>

                    <td class="lbl"><span style="padding-left: 10px;">付款日期:</span></td>
                    <td>
                        <input class="easyui-datebox" id="PayDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="PayDateEndDate" data-options="width:200," />
                    </td>

                </tr>
                <tr>
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
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExcel()" style="margin-left: 10px;">导出Excel</a>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="付款" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 10%;">流水号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 10%;">收款方</th>
                    <th data-options="field:'FinanceVaultName',align:'left'" style="width: 6%;">付款金库</th>
                    <th data-options="field:'FinanceAccountName',align:'left'" style="width: 8%;">付款账户</th>
                    <th data-options="field:'FeeTypeName',align:'left'" style="width: 6%;">费用类型</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 6%;">金额</th>
                    <th data-options="field:'Currency',align:'left'" style="width: 4%;">币种</th>
                    <th data-options="field:'PayType',align:'left'" style="width: 4%;">付款类型</th>
                    <th data-options="field:'PayerName',align:'left'" style="width: 4%;">付款人</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 6%;">付款日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:PaperInvoice" style="width: 30px">纸质单据</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
