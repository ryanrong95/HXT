<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Needs.Cbs.WebApp.BaseData.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑海关基础数据</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Cbs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var setting = eval('(<%=this.Model.Setting%>)');

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var types = eval('(<%=this.Model.Types%>)');
            $('#Type').combobox({
                data: types
            });

            if (setting != null)
            {
                $("#Code").textbox("setValue", setting["Code"]);
                $("#Type").combobox("setValue", setting["Type"]);
                $("#Name").textbox("setValue", setting["Name"]);
                $("#EnglishName").textbox("setValue", setting["EnglishName"]);
                $("#Summary").textbox("setValue", setting["Summary"]);

                $('#Code').textbox("readonly", true);
                $("#Type").combobox("readonly", true);
            }
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (setting != null) {
                data.append('ID', setting['ID']);
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
            <table>
                <tr>
                    <td class="lbl">代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="Code" name="Code" data-options="required:true,validType:'length[1,20]',tipPosition:'bottom',missingMessage:'请输入代码'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">类型：</td>
                    <td>
                        <input class="easyui-combobox input" id="Type" name="Type" data-options="valueField:'Key',textField:'Value',tipPosition:'bottom',required:true,missingMessage:'请选择数据类型',panelHeight:'100px'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">中文名称：</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入中文名称'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">英文名称：</td>
                    <td>
                        <input class="easyui-textbox input" id="EnglishName" name="EnglishName" data-options="required:false,validType:'length[0,300]',missingMessage:'请输入英文名称'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">摘要备注：</td>
                    <td>
                        <input class="easyui-textbox input" id="Summary" name="Summary" data-options="multiline:true,required:false,validType:'length[0,300]'" style="height: 60px" />
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
