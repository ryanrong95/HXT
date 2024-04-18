<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditBank.aspx.cs" Inherits="WebApp.Finance.Swap.EditBank" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改银行</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var BankData = eval('(<%=this.Model.BankData%>)');
        var SwapNoticeID = '<%=this.Model.SwapNoticeID%>';

        $(function () {
            $("#Bank").combobox({
                data: BankData,
                required: true,
                valueField: 'value',
                textField: 'text',
                onSelect: function (record) {
                    //window.parent.cleanIDs = "";

                    var BankID = record.value;   //$("#Bank").combobox("getValue");
                    var BankName = record.text;  //$("#Bank").combobox("getText");

                    //window.parent.bankID = BankID;
                    //window.parent.bankName = BankName;

                    //var IDs = getQueryString("IDs");
                    MaskUtil.mask();
                    $("#check-limit-country-result").html('');

                    //canClickNext = false;

                    $.post('?action=CheckLimitCountry', {
                        BankID: BankID,
                        BankName: BankName,
                        //IDs: JSON.stringify(IDs),
                        SwapNoticeID: SwapNoticeID,
                    }, function (result) {
                        MaskUtil.unmask();

                        //canClickNext = true;

                        var resultJson = JSON.parse(result);
                        if (resultJson.success) {
                            $("#check-limit-country-result").html(resultJson.message);
                            //window.parent.cleanIDs = resultJson.ids;
                            $("#CleanDecHeadIDs").val(resultJson.ids);
                        } else {
                            $.messager.alert('消息', resultJson.message, 'info', function () {
                                
                            });
                        }

                    });
                },
            });
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        //确定修改银行
        function Save() {
            var ewindow = $.myWindow.getMyWindow("EditInfo2EditBank");

            var CleanDecHeadIDs = $("#CleanDecHeadIDs").val();
            var BankName = $("#Bank").combobox("getText");

            if (CleanDecHeadIDs == '' || CleanDecHeadIDs == 'undefined') {
                $.messager.alert('消息', '已无可用的报关单', 'info', function () {
                    
                });
                return;
            }

            MaskUtil.mask();

            $.post('?action=ChangeBank', {
                CleanDecHeadIDs: CleanDecHeadIDs,
                SwapNoticeID: SwapNoticeID,
                BankName: BankName,
            }, function (result) {
                MaskUtil.unmask();

                var resultJson = JSON.parse(result);
                if (resultJson.success) {
                    ewindow.changeBankOK = true;

                    Close();
                } else {
                    $.messager.alert('消息', resultJson.message, 'info', function () {

                    });
                }
            });

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:50px;margin-left:100px">
                <tr>
                    <td>选择银行：</td>
                    <td>
                        <input class="easyui-combobox" id="Bank" name="Bank" panelHeight="120"
                            data-options="required:true,editable:false" style="height: 30px; width: 250px"" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td style="padding-top: 5px;">选择换汇银行，自动过滤换汇黑名单国家的报关单</td>
                </tr>
            </table>
            <table style="margin-top:10px;margin-left:10px">
                <tbody>
                    <tr>
                        <td><div id="check-limit-country-result" style="word-break: break-all;width: 515px;"></div></td>
                    </tr>
                </tbody>
            </table>
        </form>

        <input id="CleanDecHeadIDs" type="hidden" value="" />
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'," onclick="Save()">确定</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'," onclick="Close()">取消</a>
    </div>
</body>
</html>
