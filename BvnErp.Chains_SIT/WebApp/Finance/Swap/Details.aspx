<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="WebApp.Finance.Swap.Details" %>

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
            debugger;
            //列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true, pagination: false, fit: false, rownumbers: true, singleSelect: false,

                onLoadSuccess: function (data) {

                    $("#datagrid").find(".datagrid-wrap").height(32 * (irow + 1));
                    $("#datagrid").find(".datagrid-view").height(32 * (irow + 1));
                    $("#datagrid").find(".datagrid-body").height(32 * irow);
                    var leftTrs = $(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $(".datagrid-view2>.datagrid-body tr");

                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

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

        function Operation(val, row, index) {
            var buttons = row.IsBackwardExtrusion;

            return buttons;
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
                            <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同协议号</th>
                            <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                            <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                            <th data-options="field:'SwapAmount',align:'center'" style="width: 100px;">换汇金额</th>
                            <th data-options="field:'换汇委托金额',align:'center'" style="width: 100px;">换汇委托金额</th>
                            <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">报关日期</th>
                            <th data-options="field:'Op',align:'center',formatter:Operation" style="width: 10px;"></th>
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
                            <input class="easyui-combobox" id="AccountOut" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
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
                            <input class="easyui-combobox" id="AccountMid" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
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
                            <input class="easyui-combobox" id="AccountIn" data-options="height:26,width:120,valueField:'Value',textField:'Text'" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
