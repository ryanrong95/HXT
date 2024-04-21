<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ChangeScheduling.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Attends.ChangeScheduling" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //实际班别
            $('#SchedulingID').combobox({
                data: model.Schedulings,
                textField: 'text',
                valueField: 'value',
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td>实际班别</td>
                <td>
                    <input id="SchedulingID" name="SchedulingID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
        </div>
    </div>
</asp:Content>
