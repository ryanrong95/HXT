<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Anomaly.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.Anomaly" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分拣异常</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
</head>
<script>
       var data = eval('(<%=this.Model.Summry%>)');
    $(function () {

          $('#Summary').textbox('setValue', data['Summary']);
        
    });

    function Close() {
        $.myWindow.close();
    }

    function Save() {
        if (!$("#form1").form('validate')) {
            return;
        }
        var data = new FormData($('#form1')[0]);
        var ID = getQueryString('ID');
        data.append("ID", ID);
        $.ajax({
            url: '?action=AbnormalSorting',
            type: 'POST',
            data: data,
            dataType: 'JSON',
            cache: false,
            processData: false,
            contentType: false,
            success: function (res) {
            }
        }).done(function (res) {
            if (res.success) {
                $.messager.alert('提示', res.message, 'info', function () {
                    $.myWindow.close();
                });
            } else {
                $.messager.alert('提示', res.message);
            }
        });
    }
</script>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin: 0 auto; height: 120px">
                <tr>
                    <td class="lbl" style="width: 80px; font-size: 14px">异常原因：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="required: true,validType:'length[1,300]',multiline:true,tipPosition:'bottom',missingMessage:'请输入分拣异常原因'" style="width: 250px; height: 60px" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
