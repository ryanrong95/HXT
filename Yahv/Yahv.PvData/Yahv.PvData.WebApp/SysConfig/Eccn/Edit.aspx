<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.Eccn.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#lastOrigin').combobox({
                data: model.LastOrigin,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
            });

            if (model.Eccn) {
                $('#partnumber').textbox('setValue', model.Eccn.PartNumber);
                $('#manufacturer').textbox('setValue', model.Eccn.Manufacturer);
                $('#code').textbox('setValue', model.Eccn.Code);
                $('#lastOrigin').combobox('select', model.Eccn.LastOrigin);

                $('#partnumber').textbox('readonly', true);
                $('#partnumber').textbox('textbox').css('background', '#EBEBE4');
                $('#manufacturer').textbox('readonly', true);
                $('#manufacturer').textbox('textbox').css('background', '#EBEBE4');
                $('#lastOrigin').combobox('readonly', true);
                $('#lastOrigin').combobox('textbox').css('background', '#EBEBE4');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label"> 型号：</td>
                <td>
                    <input id="partnumber" name="partnumber" class="easyui-textbox" style="width: 300px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 品牌: </td>
                <td>
                    <input id="manufacturer" name="manufacturer" class="easyui-textbox" style="width: 300px"/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> Eccn编码: </td>
                <td>
                    <input id="code" name="code" class="easyui-textbox" style="width: 300px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 限制地点:</td>
                <td>
                    <input id="lastOrigin" name="lastOrigin" class="easyui-combobox" style="width: 300px"/>
                </td>
            </tr>            
        </table>
        <div style="text-align:center; padding:5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 
        </div>
    </div>
</asp:Content>