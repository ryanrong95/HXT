<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Dishonests.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            $("#cbbSupplier").combobox({
                data: model.Suppliers,
                valueField: 'EnterpriseID',
                textField: 'Name',
                panelHeight: 'auto', //自适应
                multiple: false,
                collapsible: true,
                required: true,
                editable: true,
                limitToList:true,
                panelheight: 'auto'
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td>供应商</td>
                <td>
                    <input id="cbbSupplier" name="Supplier" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>

            <tr>
                <td>失信原因</td>
                <td>
                    <input id="Reason" class="easyui-textbox" data-options="validType:'length[1,150]',required:true" name="Reason" style="width: 350px;" />
                </td>
            </tr>
            <tr>
                <td>发生时间</td>
                <td>
                    <input id="OccurTime" class="easyui-datebox" data-options="required:true"  name="OccurTime" style="width: 350px;" />
                </td>
            </tr>
            <tr>
                <td>相关单据</td>
                <td>
                    <input id="Code" class="easyui-textbox" data-options="validType:'length[1,50]',required:false" name="Code" style="width: 350px;" />
                </td>
            </tr>
            
            <tr>
                <td>备注</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
