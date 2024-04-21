<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Brand.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
             if (!jQuery.isEmptyObject(model.Entity)) {
              
                $('#form1').form('load', model.Entity);
                   $('#Name').textbox('textbox').attr('disabled', true);
               
            }

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao" >
           <tr>
               <td>品牌名称</td>
               <td colspan="3"><input id="Name" name="Name" class="easyui-textbox" style="width: 350px;" data-options="required:true,missingMessage:'请输入品牌名称'" /></td>
               
           </tr>
            <tr>
                <td>品牌简称</td>
                <td><input id="Code" name="Code" class="easyui-textbox" style="width: 350px;" data-options="required:true,missingMessage:'请输入简称'" /></td>
            </tr>
            <tr>
                <td>中文名称</td>
               <td colspan="3">
                   <input id="ChineseName" name="ChineseName" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'" />
               </td>
              
            </tr>
           

            <tr>
                 <td>网址</td>
                <td colspan="3"><input id="Website" name="Website" class="easyui-textbox" style="width: 350px;" data-options="required:false,missingMessage:'请输入网址'" /></td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                  <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>
        </table>
         <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
            </div>
    </div>
</asp:Content>
