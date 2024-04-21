<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Contacts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selType').combobox({
                data: model.ContactType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Type);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#txtName').textbox({ readonly: true });
                $('#txtTel').textbox({ readonly: true });
                $('#txtMobile').textbox({ readonly: true });
                $('#txtEmai').textbox({ readonly: true });
                $('#selType').combobox({ readonly: true });
                $('#form1').form('load', model.Entity);
                $("#btnDiv").hide();
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">姓名</td>
                    <td colspan="3">
                        <input id="txtName" name="Name" class="easyui-textbox readonly_style" style="width: 350px;" data-options="validType:'length[1,50]',required:true,prompt:'必填'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">类型</td>
                    <td colspan="3">
                        <select id="selType" name="Type" class="easyui-combobox readonly_style" data-options="editable:false" style="width: 350px"></select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">电话</td>
                    <td colspan="3">
                        <input id="txtTel" name="Tel" class="easyui-textbox readonly_style" style="width: 350px;" data-options="validType:'telNum',required:false">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">手机号</td>
                    <td colspan="3">
                        <input id="txtMobile" name="Mobile" class="easyui-textbox readonly_style" style="width: 350px;" data-options="validType:'phoneNum',required:false">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">邮箱</td>
                    <td colspan="3">
                        <input id="txtEmai" name="Email" class="easyui-textbox readonly_style" style="width: 350px;" data-options="validType:'email',required:false">
                    </td>
                </tr>

            </table>
            <div id="btnDiv" style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
            </div>
        </div>
    </div>

</asp:Content>

