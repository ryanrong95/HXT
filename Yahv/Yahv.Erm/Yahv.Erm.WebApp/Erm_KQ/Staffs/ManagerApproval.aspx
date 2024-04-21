<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="ManagerApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ManagerApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var Source = "";
        $(function () {
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //通过
            $("#btnSubmit").click(function () {
                ajaxLoading();
                var Summary = $("#Summary").textbox("getValue");
                $.post('?action=Pass', { ID: id, Summary: Summary }, function (res) {
                    ajaxLoadEnd();
                    var res = JSON.parse(res);
                    if (res.success) {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                })
            })
            //不通过
            $("#btnReject").click(function () {
                var Summary = $("#Summary").textbox("getValue");
                $.post('?action=Fail', { ID: id, Summary: Summary }, function (res) {
                    var res = JSON.parse(res);
                    if (res.success) {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                })
            })
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">审批意见：</td>
                    <td colspan="7">
                        <input id="Summary" class="easyui-textbox" style="width: 300px; height: 60px"
                            data-options="required:false,multiline:true" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-ok">通过</a>
                <a id="btnReject" class="easyui-linkbutton" iconcls="icon-no">不通过</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
