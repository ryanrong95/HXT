<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.SysConfig.DomesticExpress.Type.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>快递方式</title>
<uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        //数据初始化
        $(function () {
            if (AllData["TypeName"] != null && AllData["TypeName"] != "") {
                $("#TypeName").textbox("setValue", AllData["TypeName"]);
                $("#TypeValue").textbox("setValue", AllData["TypeValue"]);
                $('#TypeValue').textbox('textbox').attr('readonly',true); 
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
            data.append('CompanyID', AllData["CompanyID"]);
            if (AllData != null && AllData != "") {
                data.append('ID', AllData["ID"]);
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
                    } else {
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
            <table style="margin:10px; line-height: 30px">
                <tr>
                    <td>快递方式名称：</td>
                    <td>
                        <input class="easyui-textbox" id="TypeName" name="TypeName"
                            data-options="required:true,validType:'length[1,25]',tipPosition:'bottom',missingMessage:'请输入快递方式名称'" style="width:280px;" />
                    </td>
                </tr>
                <tr>
                    <td>快递方式的值：</td>
                    <td>
                        <input class="easyui-numberbox" id="TypeValue" name="TypeValue"
                             data-options="min:1,precision:'0',required:true,tipPosition:'bottom',missingMessage:'请输入快递方式值'"  style="width:280px;" />
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