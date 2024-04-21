<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Assign.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#ServiceManager").combobox({
                valueField: 'ID',
                textField: 'RealName',
                limitToList: true,
                required: true,
                editable: false,
                missingMessage: '请选择业务员',
                data: model.ServiceManager,
                onLoadSuccess: function () {
                    if (model.ServiceManagerID != null) {
                        $(this).combobox('select', model.ServiceManagerID)
                    }
                }
            });

            $("#Merchandiser").combobox({
                valueField: 'ID',
                textField: 'RealName',
                limitToList: true,
                required: false,
                editable: false,
                missingMessage: '请选择跟单员',
                data: model.Merchandiser,
                onLoadSuccess: function () {
                    if (model.MerchandiserID != null) {
                        $(this).combobox('select', model.MerchandiserID)
                    }
                }
            });
            $("#Referrer").combobox({
                valueField: 'ID',
                textField: 'RealName',
                limitToList: true,
                required: false,
                editable: false,
                missingMessage: '请选择引荐人',
                data: model.ServiceManager,
                onLoadSuccess: function () {
                    if (model.ReferrerID != null) {
                        $(this).combobox('select', model.ReferrerID)
                    }
                }
            });
            $("#Merchandiser").combobox({ readonly: model.isAssignMerchandiser });
            $("#Referrer").combobox({ readonly: model.isAssignMerchandiser });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="editTable" class="liebiao">
        <tr>
            <td class="lbl">业务员：</td>
            <td>
                <input class="easyui-combobox" data-options="" id="ServiceManager" name="ServiceManager" style="width: 350px;" />
            </td>
        </tr>
        <tr>
            <td class="lbl">跟单员：</td>
            <td>
                <input class="easyui-combobox readonly_style" data-options="" id="Merchandiser" name="Merchandiser" style="width: 350px;" />
            </td>
        </tr>
         <tr>
            <td class="lbl">引荐人：</td>
            <td>
                <input class="easyui-combobox readonly_style" data-options="" id="Referrer" name="Referrer" style="width: 350px;" />
            </td>
        </tr>
    </table>
    <div id="btnDiv" style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
    </div>
</asp:Content>
