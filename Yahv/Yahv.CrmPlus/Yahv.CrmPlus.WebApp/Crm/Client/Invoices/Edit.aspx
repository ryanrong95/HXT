<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Invoices.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server" >
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.entity)) {
                $("#Name").text(model.entity.Name);
                $('#form1').form('load', {
               // Name: model.Entity.Enterprise.Name,//企业名称

                });
            }
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
 <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td style="width: 120px;">公司名称</td>
                <td>
                    <span id="Name"> </span>
         </tr>
            <tr>
                 <td>公司地址</td>
                <td>
                    <input id="Address" name="Address" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,1500]'" />
                </td>
            </tr>
            
               <tr>
                    <td>联系电话</td>
                  <td> <input id="Tel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="required:true" /></td>
               </tr>
            <tr>

            </tr>
                    
            <tr>
                  <td>开户行</td>
            <td> <input id="Bank" name="Bank" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" />   </td>
            </tr>
                   
            <tr>
                <td>开户行账号</td>
                <td>
                    <input id="Account" name="Account" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" /></td>
            </tr>
                
           

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
