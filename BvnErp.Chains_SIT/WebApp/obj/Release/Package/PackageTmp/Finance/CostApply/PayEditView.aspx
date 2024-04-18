<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayEditView.aspx.cs" Inherits="WebApp.Finance.CostApply.PayEditView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑是否收到纸质票据</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var AllData = '<%=this.Model.AllData%>';
        var CostApplyID = '<%=Request.QueryString["CostApplyID"]%>';
        $(function () {
            var PaperNotesStatus = eval('(<%=this.Model.PaperNotesStatus%>)');
            $('#PaperNotesStatus').combobox({
                data: PaperNotesStatus
            });
            //初始化赋值
            if (PaperNotesStatus != null && PaperNotesStatus != "") {
                //$("#PaperNotesStatus").combobox("getValue");

            }
        });

        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }

        //保存校验

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var PaperNotesStatus = $('#PaperNotesStatus').combobox('getValue');

            if (PaperNotesStatus == null) {
                $.messager.alert("请选择是否收到纸质票据");
                return;
            }
             var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('PaperNotesStatus', PaperNotesStatus);
                data.append('CostApplyID',CostApplyID)
            }

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
                        $.messager.alert('消息', res.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">是否收到纸质票据：</td>
                    <td>
                        <input class="easyui-combobox" id="PaperNotesStatus" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />
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
