<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Receipt.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>收款记录查询</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '收付款管理';
        gvSettings.menu = '收款';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        //数据初始化
        $(function () {
            //下拉框数据初始化
            //金库下拉框

            $('#FinanceVault').combobox({
                data: FinanceVaultData,            
                onHidePanel: function () {                   
                    var selectedVault = $("#FinanceVault").combobox("getValue");                   
                    if (selectedVault != null) {
                        $.post('?action=GetAccountByVault', { FinanceVault: selectedVault }, function (res) {
                            var accounts = JSON.parse(res.data)
                            $('#Account').combobox('setValue', '');
                            $('#Account').combobox('loadData', accounts );
                        });
                    }
                }
            });
            //账户下拉框初始化
            var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');
            $('#Account').combobox({
                data: FinanceAccountData,
            });

            //收款记录列表初始化
            $('#receiptRecords').myDatagrid({
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
            var feeType = $('#FeeType').textbox('getValue');
            var payer = $('#Payer').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var financeVault = $('#FinanceVault').combobox('getValue');
            var account = $('#Account').combobox('getValue');
            var parm = {
                FeeType: feeType,
                Payer: payer,
                StartDate: startDate,
                EndDate: endDate,
                FinanceVault: financeVault,
                Account: account
            };
            $('#receiptRecords').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $("#FeeType").combobox('setValue', null);
            $('#Payer').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#FinanceVault').combobox('setValue', null);
            $('#Account').combobox('setValue', null);
            Search();
        }

        //新增费用
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '新增收款',
                width:680,
                height: 500,
                overflow:"hidden",
                onClose: function () {
                    $('#receiptRecords').datagrid('reload');
                }
            });
        }

        //查看收款记录
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '查看收款',
                width: 700,
                height: 800,
                onClose: function () {
                    $('#receiptRecords').datagrid('reload');
                }
            });
            //window.location = url;
        }

        //编辑收款
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                noheader: false,
                title: '编辑收款',
                width: 680,
                height:500,
                onClose: function () {
                    $('#receiptRecords').datagrid('reload');
                }
            });
        }

        //删除收款
        function Delete(id) {
            $.messager.confirm('确认', '您确定要删除这条财务收款！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#receiptRecords').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            //预收账款只能查看
            if (row.FeeType == "预收账款") {
                var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }

                //其他类型可编辑和删除
            else {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                   '<span class =\'l-btn-left l-btn-icon-left\'>' +
                   '<span class="l-btn-text">编辑</span>' +
                   '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                   '</span>' +
                   '</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }
        }     
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增收款</a>
        </div>
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">收款类型:</td>
                    <td>
                        <select class="easyui-combobox" id="FeeType" name="FeeType" data-options="editable:false">
                            <option value="" style="display: none"></option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived.GetHashCode()%>">预收账款</option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.FundTransfer.GetHashCode()%>">资金调拨</option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.BankInterest.GetHashCode()%>">银行利息</option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Loan.GetHashCode()%>">借款</option>
                            <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Repayment.GetHashCode()%>">还款</option>
                        </select>
                    </td>
                    <td class="lbl">付款人:</td>
                    <td>
                        <input class="easyui-textbox" id="Payer"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">金库:</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" data-options="valueField:'Value',textField:'Text',editable:false" />
                    </td>
                    <td class="lbl">账户:</td>
                    <td>
                        <input class="easyui-combobox" id="Account" data-options="valueField:'Value',textField:'Text',editable:false"/>
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="receiptRecords" title="财务收款" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'SerialNumber',align:'left'" style="width: 13%;">流水号</th>
                    <th data-options="field:'Vault',align:'left'" style="width: 10%;">金库</th>
                    <th data-options="field:'Account',align:'left'" style="width: 10%;">账户名称</th>
                    <th data-options="field:'Payer',align:'left'" style="width: 12%;">付款人</th>
                    <th data-options="field:'FeeType',align:'center'" style="width: 6%;">收款类型</th>
                    <th data-options="field:'ReceiptType',align:'center'" style="width: 6%;">收款方式</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 6%;">金额</th>
                    <th data-options="field:'ExchangeRate',align:'center'" style="width: 6%;">汇率</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 8%;">收款日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 13%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
