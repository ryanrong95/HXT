<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            if (!jQuery.isEmptyObject(model.Entity)) {
                console.log(model.Entity.Name)
                $('#form1').form('load',
                   {
                       Name: model.Entity.Name,
                       Mobile: model.Entity.Mobile,
                       Tel: model.Entity.Tel,
                       Email: model.Entity.Email,
                       QQ: model.Entity.QQ,
                       Fax: model.Entity.Fax,
                   });
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true"  style="padding: 10px 10px 0px 10px;">
      
           <%-- <div style="padding: 10px 60px 20px 60px;">--%>
                <table class="liebiao">


                    <tr>
                        <td style="width: 100px">姓名</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,200]'">
                        </td>

                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'phoneNum'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">电话</td>
                        <td colspan="3">
                            <input name="Tel" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'telNum'">
                        </td>

                    </tr>
                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td colspan="3">
                            <input name="Email" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'email'">
                        </td>
                    </tr>
                     <tr>
                        <td style="width: 100px">传真</td>
                        <td colspan="3">
                            <input name="Fax" class="easyui-textbox" style="width: 200px;" data-options="required:false">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">QQ</td>
                        <td colspan="3">
                            <input name="QQ" class="easyui-textbox" style="width: 200px;" data-options="required:false">
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
           <%-- </div>--%>
    </div>
</asp:Content>
