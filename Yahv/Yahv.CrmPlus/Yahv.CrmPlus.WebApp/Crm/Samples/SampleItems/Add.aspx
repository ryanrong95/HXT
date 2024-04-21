<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Samples.SampleItems.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script> 
        $(function () {
            $("#PartNumber").standardPartNumberCrmPlus({
                required: true
            });

             $("#SampleType").fixedCombobx({
                type: "SampleType",
                required:true

            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td>型号</td>
            <td colspan="3">
                <input id="PartNumber" name="PartNumber" class="easyui-combogrid" style="width: 350px;" data-options="required:true,valueField:'SpnID',textField:'PartNumber',editable:false" />
            </td>
        </tr>
        <tr>
            <td>数量</td>
            <td>
                <input id="Qty" name="Qty" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,min:0" /></td>
            <td>送样类型</td>
            <td>
                <input id="SampleType" name="SampleType" class="easyui-combobox" style="width: 200px; height: 28px" data-options="required:true,min:0" /></td>

        </tr>
        <tr>
            <td>单价</td>
            <td>
                <input id="UnitPrice" name="UnitPrice" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,min:0" /></td>
            <td>总价</td>
            <td>
                <input id="Total" name="Total" class="easyui-numberbox" style="width: 200px; height: 28px" data-options="required:true,e min:0,editable:false" /></td>
        </tr>
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
    </div>
</asp:Content>
