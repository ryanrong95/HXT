<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Service.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Attendance.Service" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //更新考勤结果
            $("#btnModify").click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                $.post("?action=ModifyPastsStatus", { date: $("#s_date").textbox("getValue") }, function (data) {
                    if (data.success) {
                        $.messager.alert("操作提示", data.data, 'info');
                    } else {
                        $.messager.alert("操作提示", data.data, 'error');
                    }
                });
            });

            //初始化考勤记录
            $("#btnInit").click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                $.post("?action=InitPasts", { date: $("#s_date").textbox("getValue") }, function (data) {
                    if (data.success) {
                        $.messager.alert("操作提示", data.data, 'info');
                    } else {
                        $.messager.alert("操作提示", data.data, 'error');
                    }
                });
            });

            //同步大赢家数据
            $("#btnSync").click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                $.post("?action=SyncData", { date: $("#s_date").textbox("getValue") }, function (data) {
                    if (data.success) {
                        $.messager.alert("操作提示", data.data, 'info');
                    } else {
                        $.messager.alert("操作提示", data.data, 'error');
                    }
                });
            });

            //根据日程安排更新考勤
            $("#btnSched").click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                $.post("?action=ModifyPastsStatusBySched", { date: $("#s_date").textbox("getValue") }, function (data) {
                    if (data.success) {
                        $.messager.alert("操作提示", data.data, 'info');
                    } else {
                        $.messager.alert("操作提示", data.data, 'error');
                    }
                });
            });
        });
    </script>
    <script>
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">考勤日期</td>
                <td style="width: 250px;" colspan="7">
                    <input id="s_date" name="s_date" type="text" class="easyui-datebox" required="true" editable="false" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 250px;" colspan="8">
                    <a id="btnModify" class="easyui-linkbutton" iconcls="icon-yg-edit">更新考勤结果</a>
                    <a id="btnInit" class="easyui-linkbutton" iconcls="icon-yg-edit">初始化考勤记录</a>
                    <a id="btnSync" class="easyui-linkbutton" iconcls="icon-yg-edit">同步大赢家数据</a>
                    <a id="btnSched" class="easyui-linkbutton" iconcls="icon-yg-edit">个人日程更新考勤</a>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
