<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Samples.SampleItems.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
     <script> 
        $(function () {
            $("#PartNumber").standardPartNumberCrmPlus({
                required: true
            });
            $("#SampleType").fixedCombobx({
                editable: false,
                required: true,
                type: "SampleType",
            });
            $("#PartNumber").standardPartNumberCrmPlus('setValue', model.Entity.SpnID);
            $("#SampleType").fixedCombobx("setValue", model.Entity.SampleType);
            $("#Qty").numberbox("setValue", model.Entity.Quantity);
            $("#UnitPrice").numberbox("setValue", model.Entity.Price);
            $("#Total").text(model.Entity.Total);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
     <div class="easyui-panel" id="tt" data-options="fit:true">
    <table class="liebiao">
        <tr>
            <td>型号</td>
              <td colspan="3">
                        <input id="PartNumber" name="PartNumber" style="width: 400px;" />
                    </td>
        </tr>
        <tr>
            <td>数量</td>
            <td>
                <input id="Qty" name="Qty" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0" /></td>
            <td>送样类型</td>
            <td>
                <input id="SampleType" name="SampleType"  style="width: 200px; "  /></td>

        </tr>
        <tr>
            <td>单价</td>
            <td>
                <input id="UnitPrice" name="UnitPrice" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0" /></td>
            <td>总价</td>
            <td>
                <%--<input id="Total" name="Total" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,min:0,editable:false" />--%>
                <label id="Total"></label>
            </td>
        </tr>
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
    </div>
          </div>
</asp:Content>
