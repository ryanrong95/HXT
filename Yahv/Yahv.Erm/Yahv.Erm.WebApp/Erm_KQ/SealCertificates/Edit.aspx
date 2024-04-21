<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.SealCertificates.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("id");
        $(function () {
            $("#Type").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.SealCertificateType,
            })
            $("#Staff").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            //保存
            $("#btnSave").click(function () {
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Name', $("#Name").textbox("getValue"));
                data.append('Type', $("#Type").combobox("getValue"));
                data.append('ProcessingDate', $("#ProcessingDate").datebox("getValue"));
                data.append('DueDate', $("#DueDate").datebox("getValue"));
                data.append('StaffID', $("#Staff").combobox("getValue"));
                ajaxLoading();
                $.ajax({
                    url: '?action=Save',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })

            Init();
        });
    </script>
    <script>
        function Init() {
            $("#Name").textbox("setValue", model.SealModel.Name);
            $("#Type").combobox("setValue", model.SealModel.Type);
            $("#ProcessingDate").datebox("setValue", model.SealModel.ProcessingDate);
            $("#DueDate").datebox("setValue", model.SealModel.DueDate);
            $("#Staff").combobox("setValue", model.SealModel.StaffID);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td>印章证照名称</td>
                    <td>
                        <input id="Name" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td>类型</td>
                    <td>
                        <input id="Type" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td>办理日期</td>
                    <td>
                        <input id="ProcessingDate" class="easyui-datebox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td>到期日期</td>
                    <td>
                        <input id="DueDate" class="easyui-datebox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td>保管人</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
