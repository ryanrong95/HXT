<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="NoticeReport.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.NoticeReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var Source = "";
        $(function () {
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //通知
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                ajaxLoading();
                var Summary = $("#Summary").textbox("getValue");
                var ReportDate = $("#ReportDate").datebox("getValue");
                $.post('?action=Pass', { ID: id, ReportDate: ReportDate, Summary: Summary }, function (res) {
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
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">报到日期</td>
                    <td>
                        <input id="ReportDate" class="easyui-datebox" style="width: 300px;" data-options="required:true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">内容摘要</td>
                    <td>
                        <input id="Summary" class="easyui-textbox" style="width: 300px; height: 60px"
                            data-options="required:false,multiline:true" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-ok">已通知</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
