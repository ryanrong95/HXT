<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecRequestCertEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecRequestCertEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {
            var ID = getQueryString("ID");
            var IfDelete = getQueryString("IfDelete");
            var DeclarationID = getQueryString("DeclarationID");
            var DocuCodeEdit = getQueryString("DocuCode");
            var CertCode = getQueryString("CertCode");
            var EdocSource = getQueryString("EdocSource");
            var DocuCode = eval('(<%=this.Model.DocuCode%>)');
            $("#ID").val(ID);
            $("#DeclarationID").val(DeclarationID);
            $("#IfDelete").val(IfDelete);
            $("#DocuCode").combobox({
                data: DocuCode
            });
            if (DocuCodeEdit != null && DocuCodeEdit != '' && DocuCodeEdit != undefined) {
                $("#DocuCode").combobox("setValue", DocuCodeEdit.substring(0, 1));
            };
            if (CertCode != null && CertCode != '' && CertCode != undefined) {
                $("#CertCode").textbox("setValue", CertCode);
            };
            if (EdocSource == "Search") {
                setDisable();
            }
        });

        function Save() {
            //验证表单数据
            //if (!$("#form1").form('validate')) {
            //    return;
            //}

            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                //$.messager.alert('提示', '请按提示输入数据！');
                return;
            }

            var model = {
                ID: $("#ID").val(),
                IfDelete: $("#IfDelete").val(),
                DeclarationID: $("#DeclarationID").val(),
                DocuCode: $("#DocuCode").combobox('getValue'),
                CertCode: $("#CertCode").textbox('getValue'),
            };

            $.post("?action=Save", model, function (res) {
                var result = JSON.parse(res);
                $.messager.alert('消息', result.message, 'info', function (r) {
                    if (result.success) {
                        ParentSearch();
                        Cancel();
                    } else {
                    }
                });
            });
        }

        function Cancel() {
            $.myWindow.close();
        }

        function ParentSearch() {
            var ewindow = $.myWindow.getMyWindow("DecRequestCert2DecRequestCertEdit");
            ewindow.SearchButton();
        }

        function setDisable() {
            $("#dlg-buttons").css("display", "none");
            $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
            $('input[class*=combobox]').attr('readonly', true).attr('disabled', true);
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">单证代码：</td>
                    <td>
                        <input class="easyui-combobox" id="DocuCode" name="DocuCode"
                            data-options="valueField:'Value',textField:'Text',required:true,tipPosition:'bottom',missingMessage:'请选择单证代码'" style="width: 300px;" />
                        <input type="hidden" id="ID" />
                        <input type="hidden" id="IfDelete" />
                        <input type="hidden" id="DeclarationID" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">单证编号：</td>
                    <td>
                        <input class="easyui-textbox" id="CertCode" name="CertCode" data-options="required:true,validType:'NoCHS',tipPosition:'bottom',missingMessage:'请输入单证编号'" style="width: 300px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
