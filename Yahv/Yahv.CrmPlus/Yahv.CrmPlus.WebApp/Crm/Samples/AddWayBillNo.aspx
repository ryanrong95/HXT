<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="AddWayBillNo.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Samples.AddWayBillNo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Entity.Summary != null) {
                $("#Summary").textbox("setValue", model.Entity.Summary);
                }
                if (model.Entity.WaybillCode) {
                    $("#WaybillCode").textbox("setValue",model.Entity.WaybillCode);
                }
            if (model.Entity.DeliveryDate != null) {
                $("#DeliveryDate").datebox("setValue", model.Entity.DeliveryDate);
            }
                
                
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
     <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>运单号：</td>
                <td>
                    <input class="easyui-textbox" id="WaybillCode" name="WaybillCode" style="width: 300px" data-options="required:true,validType:'length[1,50]'" /></td>
             
            </tr>
            <tr>
                   <td>快递公司：</td>
                <td>
                     <input class="easyui-textbox" id="Summary" name="Summary" style="width:300px" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>

            <tr>
                <td>寄送日期</td>
                <td>
                    <input class="easyui-datebox" id="DeliveryDate" name="DeliveryDate" style="width: 300px"  data-options="required:true"/>
                </td>
            </tr>
         
        </table>
         <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>

</asp:Content>
