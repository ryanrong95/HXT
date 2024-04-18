<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Swap.Bank.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>银行管理</title>
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
                $("#Summary").textbox("setValue", AllData["Summary"]);
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
            <table id="editTable" style="margin:10px; line-height: 30px">
                <tr>
                    <td class="lbl">名称：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入名称'" style="width:280px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">系统代码：</td>
                    <td>
                        <input class="easyui-textbox" id="Code" name="Code"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入系统代码'"  style="width:280px;"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">摘要：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="required:false,validType:'length[1,200]',tipPosition:'bottom',multiline:true,missingMessage:'请输入摘要'" style="width:280px;height:40px;" />
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
