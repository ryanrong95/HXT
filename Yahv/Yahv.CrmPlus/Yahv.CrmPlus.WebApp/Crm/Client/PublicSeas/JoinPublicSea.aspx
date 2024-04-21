<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="JoinPublicSea.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas.JoinPublicSea" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#Claiman").text(model.Claiman);
            $("#Company").combobox({
                data: model.Companys,
                valueField: 'ID',
                textField: 'Name',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].ID);
                }
            });
            $("#ConductType").combobox({
                data: model.ConductType,
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].value);
                }
            });
            $("#Owner").combobox({
                data: model.Owners,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].value);
                }
            });

        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="fit:true" style="padding: 10px 10px 0px 10px">
        <table class="liebiao">
            <tr>
                <td>业务类型：</td>
                <td>
                    <input id="ConductType" name="ConductType" class="easyui-combobox" style="width: 250px; height: 25px" data-options="required:true, editable:false,panelheight:'auto'" />
                </td>
            </tr>
           <%-- <tr>
                <td>我方合作公司：</td>
                <td>
                    <input id="Company" name="Company" class="easyui-combobox" style="width: 250px; height: 25px" data-options="required:true, editable:false,panelheight:'auto'" />
                </td>
            </tr>
            <tr>
                <td>客户所有人：
                </td>
                <td>
                    <input id="Owner" name="Owner" class="easyui-combobox" style="width: 250px; height: 25px" data-options="required:true,editable:false,panelheight:'auto'" />
                </td>
            </tr>--%>
            <tr>
                <td>加入公海的原因：</td>
                <td colspan="2">
                    <input id="Summary" name="Summary" class="easyui-textbox" style="width: 400px; height: 100px;" data-option="required:false;multiline:true" />
                </td>
            </tr>
        </table>
           <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>

    </div>

</asp:Content>
