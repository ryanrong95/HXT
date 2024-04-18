<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddDeclarantCandidate.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.AddDeclarantCandidate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增人员</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var from = '<%=this.Model.From%>';
        var CandidateData = eval('(<%=this.Model.CandidateData%>)');

        $(function () {
            $("#Selectable").combobox({
                data: CandidateData,
                required: true,
                valueField: 'value',
                textField: 'text',
                onChange: function (record) {

                },
            });
        });

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var AdminID = $("#Selectable").combobox("getValue");

            MaskUtil.mask();

            $.post('?action=Save', {
                From: from,
                AdminID: AdminID,
            }, function (result) {
                var rel = JSON.parse(result);
                MaskUtil.unmask();
                $.messager.alert('提示', rel.message, 'info', function () {
                    Close();
                });
            });

        }

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:50px;margin-left:100px">
                <tr>
                    <td>可选人员：</td>
                    <td>
                        <input class="easyui-combobox" id="Selectable" name="Bank" panelHeight="120"
                            data-options="required:true,editable:false" style="height: 30px; width: 250px"" />
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
