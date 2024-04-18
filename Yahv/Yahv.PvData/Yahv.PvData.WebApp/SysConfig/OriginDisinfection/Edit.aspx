<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.OriginDisinfection.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#origin').combobox({
                data: model.Origin,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
            });

            if (model.OriginDisinfection) {
                $('#origin').combobox('select', model.OriginDisinfection.Origin);
                $('#startDate').datebox('setValue', model.OriginDisinfection.StartDate);
                $('#endDate').datebox('setValue', model.OriginDisinfection.EndDate);

                $('#origin').combobox('readonly', true);
                $('#origin').combobox('textbox').css('background', '#EBEBE4');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label"> 原产地: </td>
                <td>
                    <input id="origin" name="origin" class="easyui-combobox" style="width: 300px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 检疫开始日期:</td>
                <td>
                    <input id="startDate" name="startDate" class="easyui-datebox" style="width: 300px" required/>
                </td>
            </tr>  
            <tr>
                <td class="liebiao-label"> 检疫结束日期:</td>
                <td>
                    <input id="endDate" name="endDate" class="easyui-datebox" style="width: 300px"/>
                </td>
            </tr>            
        </table>
        <div style="text-align:center; padding:5px">
            <%--<asp:Button ID="btnSubmit" runat="server" Visible="false" Text="保存" OnClick="btnSubmit_Click"/> --%>
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 

        </div>
    </div>
</asp:Content>
