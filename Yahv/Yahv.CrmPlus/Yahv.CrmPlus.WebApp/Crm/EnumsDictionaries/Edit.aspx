<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.EnumsDictionaries.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
     <script>
        $(function ()
        {
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', model.Entity);
                document.getElementById('lab_Field').innerHTML = model.Entity.Field;
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <%--<tr>
                <td>类别</td>
                <td>
                    <label id="lab_Enum"></label>
            </tr>--%>
            <tr>
                <td>名称</td>
                <td>
                   <%-- <input id="Field" name="Field" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,50]'" />--%>
                      <label id="lab_Field"></label>
                </td>
            </tr>
            <tr>

                <td>描述</td>
                <td>
                    <input id="Description" name="Description" class="easyui-textbox" style="width: 200px;" data-options="required:true" /></td>
            </tr>
            

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
