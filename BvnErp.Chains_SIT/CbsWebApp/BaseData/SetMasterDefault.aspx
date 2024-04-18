<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMasterDefault.aspx.cs" Inherits="Needs.Cbs.WebApp.BaseData.SetMasterDefault" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑申报地默认关联</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Cbs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var masterDefault = eval('(<%=this.Model.MasterDefault%>)');

        //数据初始化
        $(function () {
            if (masterDefault != null) {
                $("#Code").textbox("setValue", masterDefault["Code"]);
                $("#IEPortCode").textbox("setValue", masterDefault["IEPortCode"]);
                $("#EntyPortCode").textbox("setValue", masterDefault["EntyPortCode"]);
                $("#OrgCode").textbox("setValue", masterDefault["OrgCode"]);
                $("#VsaOrgCode").textbox("setValue", masterDefault["VsaOrgCode"]);
                $("#InspOrgCode").textbox("setValue", masterDefault["InspOrgCode"]);
                $("#PurpOrgCode").textbox("setValue", masterDefault["PurpOrgCode"]);

                $('#Code').textbox("readonly", true);
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
            if (masterDefault != null) {
                data.append('ID', masterDefault['ID']);
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
                    <td class="lbl">申报地海关代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="Code" name="Code" data-options="required:true,validType:'length[1,4]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">进境关别代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="IEPortCode" name="IEPortCode" data-options="required:true,validType:'length[1,4]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">入境口岸代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="EntyPortCode" name="EntyPortCode" data-options="required:true,validType:'length[1,8]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">检验检疫受理机关代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="OrgCode" name="OrgCode" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">领证机关代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="VsaOrgCode" name="VsaOrgCode" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">口岸检验检疫机关代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="InspOrgCode" name="InspOrgCode" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">目的地检验检疫机关代码：</td>
                    <td>
                        <input class="easyui-textbox input" id="PurpOrgCode" name="PurpOrgCode" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
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

