<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Transports.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#cboType').combobox({
                data: model.VehicleType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Transports == null ? data[0].value : model.Transports.Type);
                }
            });
            if (!jQuery.isEmptyObject(model.Transports)) {
                $('#form1').form('load',
                    {
                        CarNumber1: model.Transports.CarNumber1,
                        CarNumber2: model.Transports.CarNumber2,
                        Weight: model.Transports.Weight
                    });
                $('#txtCarNumber1').textbox('readonly');
                $('#cboType').combobox({ readonly: true });
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">类型</td>
                    <td colspan="3">
                        <select id="cboType" name="Type" class="easyui-combobox readonly_style" data-options="editable:false,panelheight:'auto',required:false" style="width: 350px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px">车牌号</td>
                    <td colspan="3">
                        <input id="txtCarNumber1" name="CarNumber1" class="easyui-textbox readonly_style" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">临时车牌号</td>
                    <td colspan="3">
                        <input id="txtCarNumber2" name="CarNumber2" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">车重/KGS</td>
                    <td colspan="3">
                        <input id="txtWeight" name="Weight" class="easyui-numberbox" style="width: 350px;" data-options="required:false">
                    </td>
                </tr>
            </table>
        </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
        </div>
    </div>
</asp:Content>
