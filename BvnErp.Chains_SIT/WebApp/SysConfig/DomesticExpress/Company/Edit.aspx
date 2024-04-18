<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.SysConfig.DomesticExpress.Company.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>快递公司编辑</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        //数据初始化
        $(function () {
            if (AllData != null && AllData != "") {
                $("#Name").textbox("setValue", AllData["Name"]);
                $("#Code").textbox("setValue", AllData["Code"]);
                $("#CustomerName").textbox("setValue", AllData["CustomerName"]);
                $("#CustomerPwd").textbox("setValue", AllData["CustomerPwd"]);
                $("#MonthCode").textbox("setValue", AllData["MonthCode"]);
            }
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"])
            }
            MaskUtil.mask();
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                }
            }).done(function (res) {
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin: 10px; line-height: 30px">
                <tr>
                    <td>承运商名称：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                            style="width: 280px;" readonly="true" />
                    </td>
                </tr>
                <tr>
                    <td>承运商简称：</td>
                    <td>
                        <input class="easyui-textbox" id="Code" name="Code"
                            style="width: 280px;" readonly="true" />
                    </td>
                </tr>
                <tr>
                    <td>账号名称：</td>
                    <td>
                        <input class="easyui-textbox" id="CustomerName" name="CustomerName"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入账号名称'" style="width: 280px;" />
                    </td>
                </tr>
                <tr>
                    <td>账号密码：</td>
                    <td>
                        <input class="easyui-textbox" id="CustomerPwd" name="CustomerPwd"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入账号密码'" style="width: 280px;" />
                    </td>
                </tr>
                <tr>
                    <td>月结账号：</td>
                    <td>
                        <input class="easyui-textbox" id="MonthCode" name="MonthCode"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom'" style="width: 280px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
