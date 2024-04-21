<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Schedulings.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("id");
        $(function () {
            $("#btnSave").click(function () {
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Name', $("#Name").textbox("getValue"));
                data.append('IsMain', $('#IsMain').checkbox('options').checked);
                data.append('AmStartTime', $("#AmStartTime").timespinner("getValue"));
                data.append('AmEndTime', $("#AmEndTime").timespinner("getValue"));
                data.append('PmStartTime', $("#PmStartTime").timespinner("getValue"));
                data.append('PmEndTime', $("#PmEndTime").timespinner("getValue"));
                data.append('DomainValue', $("#DomainValue").numberbox("getValue"));
                data.append('Summary', $("#Summary").textbox("getValue"));
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
            //初始化
            if (model) {
                $("form").form("load", model);
                $('#IsMain').checkbox({ checked: model.IsMain })
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
            <tr>
                <td>班别名称</td>
                <td colspan="3">
                    <input id="Name" name="Name" class="easyui-textbox" required="required" style="width: 250px;" />&nbsp&nbsp&nbsp&nbsp
                    <input id="IsMain" name="IsMain" class="easyui-checkbox" data-options="label:'主班别',labelPosition:'after',checked:true">
                </td>
            </tr>
            <tr>
                <td>上午开始时间</td>
                <td>
                    <input id="AmStartTime" name="AmStartTime" class="easyui-timespinner" data-options="showSeconds:true" style="width: 250px;" />
                </td>
                <td>上午结束时间</td>
                <td>
                    <input id="AmEndTime" name="AmEndTime" class="easyui-timespinner" data-options="showSeconds:true" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>下午开始时间</td>
                <td>
                    <input id="PmStartTime" name="PmStartTime" class="easyui-timespinner" required="required" data-options="showSeconds:true" style="width: 250px;" />
                </td>
                <td>下午结束时间</td>
                <td>
                    <input id="PmEndTime" name="PmEndTime" class="easyui-timespinner" required="required" data-options="showSeconds:true" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>阈值</td>
                <td colspan="3">
                    <input id="DomainValue" name="DomainValue" class="easyui-numberbox" required="required" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" style="width: 350px; height: 40px;"
                        data-options="validType:'length[1,500]',multiline:true" />
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
    <%--<div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td>班别名称</td>
                <td colspan="3">
                    <input id="Name" name="Name" class="easyui-textbox" required="required" style="width: 250px;" />&nbsp&nbsp&nbsp&nbsp
                    <input id="IsMain" name="IsMain" class="easyui-checkbox" data-options="label:'主班别',labelPosition:'after',checked:true">
                </td>
            </tr>
            <tr>
                <td>上午开始时间</td>
                <td>
                    <input id="AmStartTime" name="AmStartTime" class="easyui-timespinner" data-options="showSeconds:true" style="width: 250px;" />
                </td>
                <td>上午结束时间</td>
                <td>
                    <input id="AmEndTime" name="AmEndTime" class="easyui-timespinner" data-options="showSeconds:true" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>下午开始时间</td>
                <td>
                    <input id="PmStartTime" name="PmStartTime" class="easyui-timespinner" required="required" data-options="showSeconds:true" style="width: 250px;" />
                </td>
                <td>下午结束时间</td>
                <td>
                    <input id="PmEndTime" name="PmEndTime" class="easyui-timespinner" required="required" data-options="showSeconds:true" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>阈值</td>
                <td colspan="3">
                    <input id="DomainValue" name="DomainValue" class="easyui-numberbox" required="required" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" style="width: 350px; height: 40px;"
                        data-options="validType:'length[1,500]',multiline:true" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save">保存</a>
            <%--<asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
        </div>
    </div>--%>
</asp:Content>
