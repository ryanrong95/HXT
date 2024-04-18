<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.SysConfig.CustomsQuarantine.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>地区管理</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var Alldata = eval('(<%=this.Model.RoleData%>)');
        var OriginData = eval('(<%=this.Model.OriginData%>)');
        //数据初始化
        $(function () {
            $("#Origin").combobox({
                data: OriginData
            });
            if (Alldata != null) {
                $('#Origin').combobox('setValue', Alldata['Origin']);
                $('#StartDate').datebox('setValue', Alldata['StartDate']);
                $('#EndDate').datebox('setValue', Alldata['EndDate']);
                $('#Summary').textbox('setValue', Alldata['Summary']);
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
            if (Alldata != null) {
                data.append('ID', Alldata["ID"])
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
    <div id="content" data-options="region:'center',border:false">
        <form id="form1" runat="server">
            <table style="margin: 10px; line-height: 30px">               
                <tr>
                    <td>检疫起始日期：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" name="StartDate"
                            data-options="required:true,tipPosition:'bottom',missingMessage:'请输入日期'" style="width:200px;"/>
                    </td>
                </tr>
                <tr>
                    <td>检疫结束日期：</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" name="EndDate"
                            data-options="required:true,tipPosition:'bottom',missingMessage:'请输入日期'" style="width:200px;"/>
                    </td>
                </tr>
                <tr>
                    <td>需要检疫的产地：</td>
                    <td>
                        <input class="easyui-combobox" id="Origin" name="Origin"
                            data-options="valueField:'value',textField:'text',required:true,tipPosition:'bottom',missingMessage:'请选择产地',panelHeight:120" style="width:200px;"/>
                    </td>
                </tr>
                <tr>
                    <td>摘要：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="validType:'length[1,200]',tipPosition:'bottom',multiline:true,missingMessage:'请输入摘要'" style="width:350px; height: 80px;" />
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

