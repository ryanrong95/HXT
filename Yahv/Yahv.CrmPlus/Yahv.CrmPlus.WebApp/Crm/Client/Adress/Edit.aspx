<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Adress.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
     <script>
        $(function () {

            $("#AddressType").fixedCombobx({
                type: "AddressType",
                required:true
               // isAll: true
            });

            $("#District").fixedCombobx({
                type: "Origin",
                value: '<%=(int)Yahv.Underly.Origin.CHN%>',
                required:true
            });

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
  
        <table class="liebiao">
            <tr>
                <td>地址类型：</td>
                <td>
                    <input id="AddressType" name="AddressType"   style="width: 300px;" />
                </td>
            </tr>
             <tr>
                <td>收货人：</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 300px;" data-options="required:true" />
                </td>

            </tr>
            <tr>
                <td>联系电话：</td>
                <td>
                    <input id="Phone" name="Phone" class="easyui-textbox" style="width: 300px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>国家地区：</td>
                <td>
                    <input id="District" name="District"  style="width: 300px;"  /></td>
            </tr>
            <tr>
                <td>详细地址：</td>
                <td>
                    <input id="Context" name="Context" class="easyui-textbox" style="width: 300px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>邮政编码：</td>
                <td>
                    <input id="PostZip" name="PostZip" class="easyui-textbox" style="width: 300px;" data-options="required:false" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
 

</asp:Content>
