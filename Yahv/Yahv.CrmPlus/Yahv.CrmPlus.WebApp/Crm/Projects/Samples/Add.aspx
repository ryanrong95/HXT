<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Projects.Samples.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--   <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fixedCombobx.js"></script>--%>
    <script>
        $(function () {
            $('#Contact').contactCrmPlus({
                editable: true,
                required: true
            });
            $('#Address').consigneeCrmPlus({
                editable: false,
                required: true

            });
            $('#Contact').contactCrmPlus('setEnterpriseID', model.project.EndClientID);
            //$('#Contact').contactCrmPlus('setEnterpriseID', model.project ?.EndClientID);
            $("#Address").consigneeCrmPlus("setEnterpriseID",model.project.EndClientID);
            $("#SampleType").fixedCombobx({
                editable: false,
                required: true,
                type: "SampleType",
            });

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td>联系人：</td>
            <td>
                <input id="Contact" name="Contact" style="width: 200px; height: 28px" />
                <%--<input class="easyui-combobox" id="Contact" name="Contact" style="width: 200px;" data-options="required:true, editable:false" />--%>
            <td>寄送日期</td>
            <td>
                <input class="easyui-datebox" id="DeliveryDate" name="DeliveryDate" style="width: 200px; height: 28px" />
            </td>

        </tr>
        <tr>
            <td>单价</td>
            <td>
                <input id="UnitPrice" name="UnitPrice" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,min:0" /></td>
            <td>数量</td>
            <td>
                <input id="Qty" name="Qty" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,min:0" /></td>
        </tr>
        <tr>
            <td>送样类型</td>
            <td>
                <input id="SampleType" name="SampleType" style="width: 200px; height: 28px" /></td>

        </tr>
        <tr>
            <td>详细地址：</td>
            <td colspan="3">
                <input id="Address" name="Address" style="width: 350px; height: 28px" />
                <%-- <input class="easyui-combobox" id="Address" name="Address"
                    data-options="required:true, multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 700px;" />--%>
            </td>
        </tr>
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
    </div>
</asp:Content>
