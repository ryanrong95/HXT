<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.AccountFlow.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>收付款流水</title>    
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '收付款管理';
        gvSettings.menu = '账户流水';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //初始化
            var CurrData = eval('(<%=this.Model.CurrData%>)');
            var VaultData = eval('(<%=this.Model.VaultData%>)');
            var FeeTypeData = eval('(<%=this.Model.FeeType%>)');
            var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');

            //费用类型初始化
            $('#FeeType').combobox({
                data: FeeTypeData,
            })

            //币种初始化
            $('#Currency').combobox({
                data: CurrData,
            })

            //金库初始化
            $('#Vault').combobox({
                data: VaultData,
                onHidePanel: function () {
                    var selectedVault = $("#Vault").combobox("getValue");
                    if (selectedVault != null) {
                        $.post('?action=GetAccountByVault', { FinanceVault: selectedVault }, function (res) {
                            var accounts = JSON.parse(res.data)
                            $('#Account').combobox('setValue', '');
                            $('#Account').combobox('loadData', accounts);
                        });
                    }
                }
            });

            //账户下拉框初始化           
            $('#Account').combobox({
                data: FinanceAccountData,
            });

            //账户流水列表初始化
            $('#AccountFlows').myDatagrid({
                fitColumns:true,fit:true,
                nowrap: false,
                toolbar:'#topBar',
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

            //注册上传产品信息filebox的onChange事件
            $('#uploadExcel').filebox({
                onClickButton: function () {
                    $('#uploadExcel').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadExcel').filebox('getValue') == '') {
                        return;
                    }
                    MaskUtil.mask();
                    var formData = new FormData($('#form1')[0]);
                    $.ajax({
                        url: '?action=ImportAccountFlow',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message, 'info', function () {
                                $('#AccountFlows').myDatagrid('reload');
                            });
                        }
                    }).done(function (res) {

                    });
                }
            });

        });

        function Search() {
            var feeType = $('#FeeType').combobox('getValue');
            var payType = $('#PayType').combobox('getValue');
            var currency = $('#Currency').combobox('getValue');
            var type = $('#Type').combobox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var vault = $('#Vault').combobox('getValue');
            var account = $('#Account').combobox('getValue');
            var parm = {
                FeeType: feeType,
                PayType: payType,
                Currency: currency,
                ReceipType: type,
                StartDate: startDate,
                EndDate: endDate,
                Vault: vault,
                Account: account
            };
            $('#AccountFlows').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $("#FeeType").combobox('setValue', null);
            $('#PayType').combobox('setValue', null);
            $('#Currency').combobox('setValue', null);
            $('#Type').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#Vault').combobox('setValue', null);
            $('#Account').combobox('setValue', null);
            Search();
        }

        //导出Excel
        function ExportExcel() {
            var feeType = $('#FeeType').combobox('getValue');
            var payType = $('#PayType').combobox('getValue');
            var currency = $('#Currency').combobox('getValue');
            var type = $('#Type').combobox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var vault = $('#Vault').combobox('getValue');
            var account = $('#Account').combobox('getValue');

            var param = {
                FeeType: feeType,
                PayType: payType,
                Currency: currency,
                ReceipType: type,
                StartDate: startDate,
                EndDate: endDate,
                Vault: vault,
                Account: account
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
        <form id="form1">
            <div id="search">
                <ul>
                    <li>
                        <div style ="margin-left:10px">
                            <a id="btnDownload" href="../../Content/templates/银行流水导入模板.xlsx" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">下载导入模板</a>
                            <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 102px; height: 26px;padding-left:10px;"
                                data-options="region:'center',buttonText:'导入账户流水',buttonIcon: 'icon-add',
                                          accept:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'" />
                        </div>
                        <span style="width: 70px; margin-left: 10px; display: inline-block">费用类型:</span>
                        <input class="easyui-combobox" id="FeeType" name="FeeType" data-options="valueField:'Key',textField:'Value',editable:false,width:255" />
                        <%--  <select class="easyui-combobox" id="FeeType" name="FeeType" data-options="width:300,editable:false">
                        <option value="" style="display: none"></option>
                        <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived.GetHashCode()%>">预收账款</option>
                        <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.FundTransfer.GetHashCode()%>">资金调拨</option>
                        <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.BankInterest.GetHashCode()%>">银行利息</option>
                        <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Loan.GetHashCode()%>">借款</option>
                        <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Repayment.GetHashCode()%>">还款</option>
                        </select>--%>
                        <span style="width: 70px; margin-left: 10px; display: inline-block">费用方式:</span>
                        <select class="easyui-combobox" id="PayType" name="PayType" data-options="width:255,editable:false">
                            <option value="" style="display: none"></option>
                            <option value="<%=Needs.Ccs.Services.Enums.PaymentType.Cash.GetHashCode()%>">现金</option>
                            <option value="<%=Needs.Ccs.Services.Enums.PaymentType.Check.GetHashCode()%>">支票</option>
                            <option value="<%=Needs.Ccs.Services.Enums.PaymentType.TransferAccount.GetHashCode()%>">转账</option>
                            <option value="<%=Needs.Ccs.Services.Enums.PaymentType.AcceptanceBill.GetHashCode()%>">转账</option>
                        </select>
                        <span style="width: 70px; margin-left: 10px; display: inline-block">币种:</span>
                        <input class="easyui-combobox" id="Currency" name="Currency" data-options="valueField:'Value',textField:'Text',editable:false,width:255" />
                        <br />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">收款/付款:</span>
                        <select class="easyui-combobox" id="Type" name="Type" data-options="width:255,editable:false">
                            <option value="" style="display: none"></option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceType.Receipt.GetHashCode() %>">收款</option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceType.Payment.GetHashCode() %>">付款</option>
                        </select>

                        <span style="width: 70px; margin-left: 10px; display: inline-block">发生日期:</span>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false,width:255" />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">至 </span>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false,width:255" />
                        <br />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">金库:</span>
                        <input class="easyui-combobox" id="Vault" data-options="valueField:'Value',textField:'Text',editable:false,width:255" />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">账户:</span>
                        <input class="easyui-combobox" id="Account" data-options="valueField:'Value',textField:'Text',editable:false,width:255" />

                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 10px;" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
                    </li>
                </ul>
            </div>
        </form>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="AccountFlows" title="收付款流水列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 80px;">流水号</th>
                    <th data-options="field:'Vault',align:'left'" style="width: 50px;">金库</th>
                    <th data-options="field:'Account',align:'left'" style="width: 80px;">账户名称</th>
                    <th data-options="field:'OtherAccount',align:'left'" style="width: 80px;">对方户名</th>
                    <th data-options="field:'Type',align:'center'" style="width: 50px;">收款/付款</th>
                    <th data-options="field:'FeeType',align:'center'" style="width: 50px;">费用类型</th>
                    <th data-options="field:'PayType',align:'center'" style="width: 50px;">费用方式</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 50px;">币种</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 50px;">金额</th>
                    <th data-options="field:'Balance',align:'center'" style="width: 50px;">账户余额</th>
                    <th data-options="field:'Date',align:'center'" style="width: 50px;">发生日期</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
