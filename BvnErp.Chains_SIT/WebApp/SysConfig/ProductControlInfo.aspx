<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductControlInfo.aspx.cs" Inherits="WebApp.SysConfig.ProductControlInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管控产品-系统配置</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {
            var ProductControlTypeData = eval('(<%=this.Model.ProductControlType%>)');
            $('#Type').combobox({
                data: ProductControlTypeData,
            });
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
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
            <table id="editTable">
                <tr>
                    <td>管控类型：</td>
                    <td>
                        <input class="easyui-combobox input" id="Type" name="Type" data-options="valueField:'ID',textField:'Name',tipPosition:'bottom',required:true,editable:false,missingMessage:'请选择管控类型'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">品名</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入品名'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">型号</td>
                    <td>
                        <input class="easyui-textbox input" id="Model" name="Model"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入型号'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="padding-right: 10px">品牌</td>
                    <td>
                        <input class="easyui-textbox input" id="Manufacturer" name="Manufacturer"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入品牌'" />
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
