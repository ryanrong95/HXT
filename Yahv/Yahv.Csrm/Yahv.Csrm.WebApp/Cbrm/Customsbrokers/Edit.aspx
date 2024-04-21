<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Cbrm.Customsbrokers.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //if (!jQuery.isEmptyObject(model)) {
            //    $('#form1').form('load', model);
            //    $('#txtName').textbox('readonly');
            //}
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
   <%-- <input type="hidden" id="CustomsbrokersID" class="easyui-textbox" name="ID" value="" />--%>
<%--    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 700px">
            <div style="padding: 10px 60px 20px 60px;">
                <table>
                    <tr>
                        <td style="width: 100px">报关公司名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox"
                                data-options="prompt:'报关公司,名称要保证全局唯一',fit:true,required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td style="width: 100px">级别</td>
                        <td colspan="3">
                            <select id="selGrade" name="Grade" runat="server" class="easyui-combobox" data-options="editable:false" panelheight="auto" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td colspan="3">
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 200px;"
                                data-options="prompt:'管理员自行保障正确性',required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3">
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 200px;" data-options="prompt:'自行保障正确性',required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">是否内部报关公司</td>
                        <td colspan="3">
                            <input id="chbOwn" runat="server" type="checkbox" name="chbOwn" />
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                </div>
            </div>
        </div>
    </div>--%>
</asp:Content>

