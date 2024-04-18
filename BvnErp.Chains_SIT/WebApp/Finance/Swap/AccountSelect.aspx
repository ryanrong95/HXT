<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountSelect.aspx.cs" Inherits="WebApp.Finance.AccountSelect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var VaultData = eval('(<%=this.Model.VaultData%>)');
        var chargeBearerData = eval('(<%=this.Model.ChargeBearer%>)');
        var consignorCode = '<%=this.Model.consignorCode%>';
        var partyName = '<%=this.Model.partyName%>';


        $(function () {
            var UID = getQueryString("UID");
            var SwapNoticeID = getQueryString("SwapNoticeID");
            var txnAmount = getQueryString("txnAmount");
            var txnRefId = getQueryString("txnRefId");
            $("#SwapNoticeID").val(SwapNoticeID);
            $("#txnAmount").val(txnAmount);
            $("#uid").val(UID);
            $("#txnRefId").val(txnRefId);
            $("#senderPartyName").val("XINDATONGSUPPLYCHAIN");

            $('#VaultIn').combobox({
                data: VaultData,
                onSelect: function (record) {
                    $.post('?action=ForeignAccountSelectIn', { ID: record.Value, NoticeID: SwapNoticeID }, function (data) {
                        data = eval(data);
                        $('#AccountIn').combobox({
                            data: eval(data)
                        });
                    })
                }
            });

            $('#chargeBearer').combobox({
                data: chargeBearerData,
            });

            $('#chargeBearer').combobox("setValue", 3);
            $('#PartyName').textbox("setValue", consignorCode);
            $('#PartyNameEnglish').textbox("setValue", partyName);
             //$('#PartyName').combobox({
             //    onSelect: function (record) {
             //        $('#PartyNameEnglish').textbox("setValue", record.Value);
             //   }
             //});

             $('#AccountIn').combobox({
                 onSelect: function (record) {
                     AccountInfo(record.Value);
                }
            });
        });
    </script>
    <script>
        function TT() {
            var txnRefId = $("#txnRefId").val();
            var txnAmount = $("#txnAmount").val();
            var ChargeBearer = $("#chargeBearer").combobox("getValue");
            var PartyName = $("#PartyNameEnglish").textbox("getValue");
            var AccountNo = $("#AccountNo").textbox("getValue");
            var SwiftCode = $("#SwiftCode").textbox("getValue");
            var BankName = $("#BankName").textbox("getValue");
            var SenderPartyName = $("#senderPartyName").val();
            MaskUtil.mask();
            $.post('?action=TT', {
                txnAmount: txnAmount,               
                txnRefId: txnRefId,
                ChargeBearer: ChargeBearer,
                PartyName: PartyName,
                AccountNo: AccountNo,
                SwiftCode: SwiftCode,
                BankName: BankName,
                SenderPartyName:SenderPartyName
            }, function (data) {
                var Result = JSON.parse(data);
                MaskUtil.unmask();
                $.messager.confirm('确认', Result.message, function (success) {
                    //window.parent.SearchBooking(uid);
                    //self.parent.$('iframe').parent().window('close');
                    Close();
                });
            });
        }

        function AccountInfo(AccountID) {
            $.post('?action=AccountInfo', {AccountID: AccountID}, function (data) {
                var Result = JSON.parse(data);
                $('#AccountNo').textbox("setValue", Result.AccountNo);
                $('#SwiftCode').textbox("setValue", Result.SwiftBic);
                $('#BankName').textbox("setValue", Result.BankName);
            });
        }

        //关闭窗口
        function Close() {
            $.myWindow.close();
            //var ewindow = $.myWindow.getMyWindow("FX2AccountSelect");

            //ewindow.childParam = "onlyClose";
            //$.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form2" runat="server" method="post" onsubmit="return CheckSubmit()">
            <div>
                <input type="hidden" id="SwapNoticeID" />
                <input type="hidden" id="txnAmount" />
                <input type="hidden" id="uid" />
                <input type="hidden" id="txnRefId" />
                <input type="hidden" id="senderPartyName" />
            </div>
            <table id="editTable" style="margin-top: 30px">
                <tr>
                    <td class="lbl">收款方名称：</td>
                    <td>
                        <%--<select class="easyui-combobox" id="PartyName" data-options="required:true,height:26,width:200,valueField:'Value',textField:'Text',tipPosition:'right'">
                            <option value="HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED">香港畅运</option>
                            <option value="HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO.,LIMITED">香港万路通</option>
                        </select>--%>
                        <input class="easyui-textbox" id="PartyName" name="PartyName" data-options="required:true,height:28,width:430,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款方英文名称：</td>
                    <td>
                        <input class="easyui-textbox" id="PartyNameEnglish" name="PartyNameEnglish" data-options="required:true,height:28,width:430,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">金库：</td>
                    <td>
                        <input class="easyui-combobox" id="VaultIn" data-options="required:true,height:26,width:200,valueField:'Value',textField:'Text',tipPosition:'right'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">账户：</td>
                    <td>
                        <input class="easyui-combobox" id="AccountIn" data-options="required:true,height:26,width:200,valueField:'Value',textField:'Text',tipPosition:'right'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">账号：</td>
                    <td>
                        <input class="easyui-textbox" id="AccountNo" name="AccountNo" data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行代码：</td>
                    <td>
                        <input class="easyui-textbox" id="SwiftCode" name="SwiftCode" data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行名称：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName" data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行费用：</td>
                    <td>
                        <input class="easyui-combobox" id="chargeBearer" data-options="required:true,height:26,width:200,valueField:'Value',textField:'Text',tipPosition:'right'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="TT()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
