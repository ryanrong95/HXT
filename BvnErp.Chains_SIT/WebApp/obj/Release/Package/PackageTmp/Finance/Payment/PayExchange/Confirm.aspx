<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="WebApp.Finance.Payment.PayExchange.Confirm" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var PayerData = eval('(<%=this.Model.PayerData%>)');
        var ID = getQueryString("ID");
        var IsPass = getQueryString("IsPass");
        var IsAdvanceMoney = getQueryString("IsAdvanceMoney");//是否垫款

        var PeriodType = '<%=this.Model.PeriodType%>';
        var ReceivableAmountTotal = '<%=this.Model.ReceivableAmountTotal%>';
        var ReceivedAmountTotal = '<%=this.Model.ReceivedAmountTotal%>';
        var WaiBiPrice = '<%=this.Model.WaiBiPrice%>';
        var TheExchangeRate = '<%=this.Model.TheExchangeRate%>';
        var AvailableProductFee = '<%=this.Model.AvailableProductFee%>';
        var RmbPrice = '<%=this.Model.RmbPrice%>';

        $(function () {
            $("#PeriodType").html(PeriodType);
            $("#ReceivableAmountTotal").html(ReceivableAmountTotal);
            $("#ReceivedAmountTotal").html(ReceivedAmountTotal);
            $("#WaiBiPrice").html(WaiBiPrice);
            $("#TheExchangeRate").html(TheExchangeRate);
            $("#AvailableProductFee").html(AvailableProductFee);

            $('#Payer').combobox({
                data: PayerData,
                editable: false,
                valueField: 'ID',
                textField: 'ByName'
            });
            if (IsPass != "true") {
                $("#IsShow").css({
                    display: "none",
                });
            }
        });
        //保存
        function Save() {
            if (IsPass == "true") {
                if (!$("#form1").form('validate')) {
                    $.messager.alert("提示", "请指定付款人")
                    return;
                }
            }

            var data = new FormData($('#form1')[0]);
            var Payer = $("#Payer").combobox("getValue");
            var Summary = $("#Summary").textbox("getValue");
            data.append("ID", ID);
            data.append("Payer", Payer);
            data.append("Summary", Summary);
            data.append("ReceivableAmountTotal", ReceivableAmountTotal);
            data.append("ReceivedAmountTotal", ReceivedAmountTotal);
            data.append("IsAdvanceMoney", IsAdvanceMoney);
            data.append("RmbPrice", RmbPrice);

            if (IsPass == "true") {
                $.ajax({
                    url: '?action=Save',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        if (res.success) {
                            $.messager.alert('提示', res.message, 'info', function () {
                                Back();
                            });
                        }
                    }
                });
            }
            else {
                $.ajax({
                    url: '?action=Cancel',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        if (res.success) {
                            $.messager.alert('提示', res.message, 'info', function () {
                                Back();
                            });
                        }
                    }
                });
            }
        }
        //关闭
        function Close() {
            var ewindow = $.myWindow.getMyWindow("Approval2Confirm");
            ewindow.ConfirmBackUrl = '';
            $.myWindow.close();
        }
        //退回列表界面
        function Back() {
            var ewindow = $.myWindow.getMyWindow("Approval2Confirm");
            var url = location.pathname.replace(/Confirm.aspx/ig, 'UnApprovedList.aspx');
            ewindow.ConfirmBackUrl = url;
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-top: 10px; margin: 0 auto; height: 150px">
                <tr>
                    <td colspan="2">
                        <span style="font-weight: bold;">账期类型：</span>
                        <span>
                            <label id="PeriodType"></label>
                        </span>
                        <span style="margin-left: 5px; font-weight: bold;">应收总额：</span>
                        <span>
                            <label id="ReceivableAmountTotal"></label>
                        </span>
                        <span style="margin-left: 5px; font-weight: bold;">实收总额：</span>
                        <span>
                            <label id="ReceivedAmountTotal"></label>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span style="font-weight: bold;">客户要付外币金额：</span>
                        <span>
                            <label id="WaiBiPrice"></label>
                        </span>
                        <span style="margin-left: 5px; font-weight: bold;">汇率：</span>
                        <span>
                            <label id="TheExchangeRate"></label>
                        </span>
                        <span style="margin-left: 5px; font-weight: bold;">货款可用垫款：</span>
                        <span>
                            <label id="AvailableProductFee"></label>
                        </span>
                    </td>
                </tr>
                <tr id="IsShow">
                    <td class="lbl">付款人：</td>
                    <td>
                        <input class="easyui-combobox" id="Payer"
                            data-options="required:true,height:26,width:300,missingMessage:'请选择付款人',tipPosition:'bottom'," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary"
                            data-options="multiline:true,height:60,width:300,validType:'length[1,250]'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
