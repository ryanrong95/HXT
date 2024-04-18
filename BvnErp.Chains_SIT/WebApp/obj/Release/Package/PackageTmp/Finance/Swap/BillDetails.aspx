<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BillDetails.aspx.cs" Inherits="WebApp.Finance.Swap.BillDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script type="text/javascript">

        var SwapNoticeData = eval('(<%=this.Model.SwapNoticeData%>)');
        var VaultData = eval('(<%=this.Model.VaultData%>)');
        var AccountData = eval('(<%=this.Model.AccountData%>)');

        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({     
                fitColumns: true,
                pagination: false,
                fit: false,
                rownumbers: true,
                singleSelect: false,
                onLoadSuccess: function (data) {
                    debugger
                    var SumTransPremiumInsuranceAmount = 0, LastSumTransPremiumInsuranceAmount = 0;
                    var SumTransPremiumInsuranceAmountRMB = 0, LastSumTransPremiumInsuranceAmountRMB = 0;
                    var SumAcceptanceCustomer = 0, LastSumAcceptanceCustomer = 0;
                    var SumAcceptanceXDT = 0, LastSumAcceptanceXDT = 0;
                    var currentDate = data.rows[0].DDate;
                    var i = 0;
                    for (i = 0; i < data.rows.length; i++) {
                        if (currentDate == data.rows[i].DDate) {                          
                            SumTransPremiumInsuranceAmount += data.rows[i].TransPremiumInsuranceAmount;
                            SumTransPremiumInsuranceAmountRMB += data.rows[i].TransPremiumInsuranceAmountRMB;
                            SumAcceptanceCustomer += data.rows[i].AcceptanceCustomer;
                            SumAcceptanceXDT += data.rows[i].AcceptanceXDT;
                        } else {                           
                            LastSumTransPremiumInsuranceAmount = SumTransPremiumInsuranceAmount;
                            LastSumTransPremiumInsuranceAmountRMB = SumTransPremiumInsuranceAmountRMB;
                            LastSumAcceptanceCustomer = SumAcceptanceCustomer;
                            LastSumAcceptanceXDT = SumAcceptanceXDT;
                            currentDate =data.rows[i].DDate;                           
               
                            $('#datagrid').myDatagrid('insertRow', {
                                index: i,
                                row: {
                                    ContrNo: '<span>合计</span>',
                                    OrderID: '',
                                    ClientName: '',
                                    Currency: '',
                                    RealExchangeRate: '',                                  
                                    TransPremiumInsuranceAmount: '<span>' + LastSumTransPremiumInsuranceAmount.toFixed(2) + '</span>',
                                    TransPremiumInsuranceAmountRMB: '<span>' + LastSumTransPremiumInsuranceAmountRMB.toFixed(2) + '</span>',
                                    AcceptanceCustomer: '<span>' + LastSumAcceptanceCustomer.toFixed(2) + '</span>',
                                    AcceptanceXDT: '<span>' + LastSumAcceptanceXDT.toFixed(2) + '</span>',
                                }                               
                            });
                             i++;
                        }
                    }
                    $('#datagrid').myDatagrid('insertRow', {
                                index: i,
                                row: {
                                    ContrNo: '<span>合计</span>',
                                    OrderID: '',
                                    ClientName: '',
                                    Currency: '',
                                    RealExchangeRate: '',                                                                     
                                    TransPremiumInsuranceAmount: '<span>' + SumTransPremiumInsuranceAmount.toFixed(2) + '</span>',
                                    TransPremiumInsuranceAmountRMB: '<span>' + SumTransPremiumInsuranceAmountRMB.toFixed(2) + '</span>',
                                    AcceptanceCustomer: '<span>' + SumAcceptanceCustomer.toFixed(2) + '</span>',
                                    AcceptanceXDT: '<span>' + SumAcceptanceXDT.toFixed(2) + '</span>',
                                }                               
                            });
                },
            });
            //初始化快递信息
            $('#VaultIn').combobox({
                data: VaultData,
                disabled: true,
            });
            $('#VaultOut').combobox({
                data: VaultData,
                disabled: true,
            });
            $('#VaultMid').combobox({
                data: VaultData,
                disabled: true,
            });
            $('#AccountIn').combobox({
                data: AccountData,
                disabled: true,
            });
            $('#AccountOut').combobox({
                data: AccountData,
                disabled: true,
            });
            $('#AccountMid').combobox({
                data: AccountData,
                disabled: true,
            });
            $('#BankName').combobox({
                disabled: true,
            });

            Init()
        });

        function Init() {
            if (SwapNoticeData != null) {
                $('#TotalAmount').textbox("setValue", SwapNoticeData.TotalAmount);
                $('#BankName').combobox("setValue", SwapNoticeData.BankName);
                $('#TotalRmb').textbox("setValue", Number(SwapNoticeData.TotalRmb).toFixed(2));
                $('#VaultOut').combobox("setValue", SwapNoticeData.VaultOut);
                $('#VaultIn').combobox("setValue", SwapNoticeData.VaultIn);
                $('#VaultMid').combobox("setValue", SwapNoticeData.VaultMid);
                $('#AccountOut').combobox("setValue", SwapNoticeData.AccountOut);
                $('#AccountIn').combobox("setValue", SwapNoticeData.AccountIn);
                $('#AccountMid').combobox("setValue", SwapNoticeData.AccountMid);
                $('#ExchangeRate').textbox("setValue", SwapNoticeData.ExchangeRate);
                $('#Poundage').textbox("setValue", SwapNoticeData.Poundage);
                $('#ExchangeDate').datebox('setValue', SwapNoticeData.ExchangeDate);
            }
        }

        function compute(colName) {
            var rows = $('#datagrid').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                total += parseFloat(rows[i][colName]);
            }
            return total.toFixed(2);
        }
    </script>
    <style>
        .lab {
            padding-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 0 5px">
            <div style="margin: 10px 0">
                <label style="font-size: 16px; font-weight: 600; color: orangered">换汇明细</label>
            </div>
            <div data-options="region:'center',border:false">
                <table id="datagrid" title="" data-options="fitColumns:true,pagination:false,fit:false,rownumbers:true,singleSelect:false">
                    <thead>
                        <tr>
                            <th data-options="field:'ContrNo',align:'center'" style="width: 150px;">合同号</th>
                            <th data-options="field:'OrderID',align:'center'" style="width: 180px;">订单编号</th>
                            <th data-options="field:'ClientName',align:'center'" style="width: 200px;">客户名称</th>
                            <th data-options="field:'Currency',align:'center'" style="width: 50px;">币种</th>
                            <th data-options="field:'RealExchangeRate',align:'center'" style="width: 80px;">实时汇率</th>                          
                            <th data-options="field:'TransPremiumInsuranceAmount',align:'center'" style="width: 80px;">运保杂</th>
                            <th data-options="field:'TransPremiumInsuranceAmountRMB',align:'center'" style="width: 100px;">运保杂RMB</th>
                            <th data-options="field:'AcceptanceCustomer',align:'center'" style="width: 120px;">汇兑-客户(借)</th>
                            <th data-options="field:'AcceptanceXDT',align:'center'" style="width: 120px;">汇兑-芯达通(借)</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div style="margin: 10px 0">
                <label style="font-size: 16px; font-weight: 600; color: orangered">换汇汇率</label>
            </div>
            <div id="SwapInf">
                <table style="line-height: 30px">
                    <tr>
                        <td class="lbl">外币总金额：</td>
                        <td>
                            <input class="easyui-textbox" id="TotalAmount" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">换汇银行：</td>
                        <td>
                            <select id="BankName" class="easyui-combobox" style="height: 26px; width: 200px">
                                <option>农业银行</option>
                                <option>建设银行</option>
                                <option>平安银行</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">换汇汇率：</td>
                        <td>
                            <input class="easyui-textbox" id="ExchangeRate" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">RMB总金额：</td>
                        <td>
                            <input class="easyui-textbox" id="TotalRmb" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl">换汇日期：</td>
                        <td>
                            <input class="easyui-datebox" id="ExchangeDate" data-options="required:true,height:26,width:150,editable:false" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="margin: 10px 0">
                <label style="font-size: 16px; font-weight: 600; color: orangered">换汇账户</label>
            </div>
            <div id="SwapAcc">
                <table style="line-height: 30px">
                    <tr>
                        <td class="lbl" colspan="4">人民币账户:</td>
                    </tr>
                    <tr>
                        <td class="lbl">金库：</td>
                        <td>
                            <input class="easyui-combobox" id="VaultOut" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
                        </td>
                        <td class="lbl lab">账户：</td>
                        <td>
                            <input class="easyui-combobox" id="AccountOut" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                        </td>
                        <td class="lbl lab">手续费：</td>
                        <td>
                            <input class="easyui-textbox" id="Poundage" data-options="editable:false,height:26,width:120,valueField:'Value',textField:'Text'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" colspan="4">外币账户：</td>
                    </tr>
                    <tr>
                        <td class="lbl">金库：</td>
                        <td>
                            <input class="easyui-combobox" id="VaultMid" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
                        </td>
                        <td class="lbl lab">账户：</td>
                        <td>
                            <input class="easyui-combobox" id="AccountMid" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" colspan="4">供应商账户：</td>
                    </tr>
                    <tr>
                        <td class="lbl">金库：</td>
                        <td>
                            <input class="easyui-combobox" id="VaultIn" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
                        </td>
                        <td class="lbl lab">账户：</td>
                        <td>
                            <input class="easyui-combobox" id="AccountIn" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
