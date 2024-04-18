<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditPayee.aspx.cs" Inherits="WebApp.Finance.CostApply.Payee.EditPayee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>新增/编辑收款方</title>
    <uc:EasyUI runat="server" />
    <script src="../../../../Scripts/Ccs.js"></script>
    <link href="../../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var From = '<%=this.Model.From%>';
        var Payee = eval('(<%=this.Model.Payee%>)');

        $(function () {
            if (From == "Edit") {
                $("#PayeeName").textbox('setValue', Payee["PayeeName"]);
                $("#PayeeAccount").textbox('setValue', Payee["PayeeAccount"]);
                $("#PayeeBank").textbox('setValue', Payee["PayeeBank"]);
            }
        });

        function Save() {
            if (!Valid('form1')) {
                return;
            }

            var PayeeName = $("#PayeeName").textbox("getValue").trim(); //收款方名称
            var PayeeAccount = $("#PayeeAccount").textbox("getValue").trim(); //收款方账号
            var PayeeBank = $("#PayeeBank").textbox("getValue").trim(); //收款方银行

            MaskUtil.mask();
            $.post(location.pathname + '?action=Save', {
                From: From,
                CostApplyPayeeID: Payee["CostApplyPayeeID"],
                PayeeName: PayeeName,
                PayeeAccount: PayeeAccount,
                PayeeBank: PayeeBank,
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                        NormalClose();

                    });
                    alert1.window({
                        modal: true, onBeforeClose: function () {
                            NormalClose();
                        }
                    });
                } else {
                    $.messager.alert('提示', result.message, 'info', function () {

                    });
                }
            });
        }

        function Cancel() {
            $.myWindow.close();
        }

        function NormalClose() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1">
        <div style="margin-left: 35px;">
            <div style="margin-top: 20px;">
                <span class="lbl">收款方名称：</span>
                <input class="easyui-textbox" id="PayeeName" data-options="validType:'length[1,50]', width: 230,required:true,tipPosition:'bottom'," />
            </div>
            <div style="margin-top: 10px;">
                <span class="lbl">收款方账号：</span>
                <input class="easyui-textbox" id="PayeeAccount" data-options="validType:'length[1,50]', width: 230,required:true,tipPosition:'bottom'," />
            </div>
            <div style="margin-top: 10px;">
                <span class="lbl">收款方银行：</span>
                <input class="easyui-textbox" id="PayeeBank" data-options="validType:'length[1,50]', width: 230,required:true,tipPosition:'bottom'," />
            </div>
        </div>
        <div style="margin-left: 35px; margin-top: 15px;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="margin-left: 5px;" onclick="Save()">保存</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" style="margin-left: 5px;" onclick="Cancel()">取消</a>
        </div>
    </form>
</body>
</html>
