<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Assign.aspx.cs" Inherits="WebApp.Client.Assign.Assign" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .lbl {
            text-align: right;
        }
    </style>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
        var Merchandiser = eval('(<%=this.Model.Merchandiser%>)');
        var ClientAssignData = eval('(<%=this.Model.ClientAssignData%>)');
        var ClientReferrerData = eval('(<%=this.Model.ClientReferrerData%>)');
        var Referrer = eval('(<%=this.Model.ClientReferrer%>)');

        $(function () {
            $("#ServiceManager").combobox({
                data: ServiceManager
            });

            $("#Merchandiser").combobox({
                data: Merchandiser
            });

            $("#Referrers").combobox({
                data: ClientReferrerData
            });

            $.each(ClientAssignData, function (index, val) {
                if (val.Type == '<%=Needs.Ccs.Services.Enums.ClientAdminType.Merchandiser.GetHashCode()%>') {
                    $("#Merchandiser").combobox('setValue', val.Admin.ID);

                }
                else if (val.Type ==<%=Needs.Ccs.Services.Enums.ClientAdminType.ServiceManager.GetHashCode()%>) {
                    $("#ServiceManager").combobox('setValue', val.Admin.ID);
                }
                // else {

                //    $("#Referrers").combobox('setValue', val.Admin.ID);
                //}
                $("#Referrers").combobox('setValue', Referrer);
                $("#Summary").textbox('setValue', val.Summary);
            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var values = FormValues("form1");
                values['ID'] = ID;//会员ID 
                if ($("#Referrers").combobox('getValue') == "") {
                    values['Referrers'] = "";//引荐人
                    values['ReferrersID'] = "";//引荐人ID
                }
                //提交后台
                MaskUtil.mask();//遮挡层
                $.post('?action=SaveClientAssign', { Model: JSON.stringify(values) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, "info", function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable">
                <tr>
                    <td class="lbl">业务员：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:true,missingMessage:'请选择业务员'" id="ServiceManager" name="ServiceManager" style="width: 400px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">跟单员：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:true,missingMessage:'请选择跟单员'" id="Merchandiser" name="Merchandiser" style="width: 400px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">引荐人：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:false,editable:true,missingMessage:'请选择引荐人'" id="Referrers" name="Referrers" style="width: 400px;" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">摘要备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,500]',tipPosition:'bottom',multiline:true" style="width: 400px; height: 60px;" />
                    </td>
                </tr>

            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="closeWin()">取消</a>
    </div>
</body>
</html>
