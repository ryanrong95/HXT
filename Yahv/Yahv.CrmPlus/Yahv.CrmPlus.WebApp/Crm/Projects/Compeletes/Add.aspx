<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Projects.Compeletes.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script> 
        $(function () {

         $("#PartNumber").standardPartNumberCrmPlus({
                required: true
            });
              $("#PartNumber").standardPartNumberCrmPlus('setValue',model.Entity.SpnID);
           

        });

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td>竞品型号</td>
            <td colspan="3">
                <input id="PartNumber" name="PartNumber"  style="width: 350px;height: 28px"  />
            </td>
        </tr>
        <tr>
            <td>竞品单价</td>
            <td>
                <input id="UnitPrice" name="UnitPrice" class="easyui-numberbox" style="width: 350px; height: 28px" data-options="required:true,min:0" /></td>

        </tr>
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
    </div>
</asp:Content>
