<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.OriginATRate.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#hsCode').combobox({
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
                hasDownArrow: false,
                onChange: function (value) {
                    $.post('?action=GetHSCodes', { hsCode: value }, function (data) {
                        $("#hsCode").combobox("loadData", data);
                    });
                },
            });

            $('#origin').combobox({
                data: model.Origin,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
            });

            $('#origin').combobox('setValue', model.Origin[43].value);

            $('#type').combobox('readonly', true);
            $('#type').combobox('textbox').css('background', '#EBEBE4');

            if (model.OriginATRate) {
                $('#hsCode').combobox('select', model.OriginATRate.HSCode);
                $('#origin').combobox('select', model.OriginATRate.Origin);
                $('#rate').numberbox('setValue', model.OriginATRate.Rate);
                $('#startDate').datebox('setValue', model.OriginATRate.StartDate);
                $('#endDate').datebox('setValue', model.OriginATRate.EndDate);

                $('#hsCode').combobox('readonly', true);
                $('#hsCode').combobox('textbox').css('background', '#EBEBE4');
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
                <td class="liebiao-label"> 海关编码：</td>
                <td>
                    <input id="hsCode" name="hsCode" class="easyui-combobox" style="width: 300px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 原产地: </td>
                <td>
                    <input id="origin" name="origin" class="easyui-combobox" style="width: 300px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 征税类型: </td>
                <td>
                    <input id="type" name="type" class="easyui-textbox" value="进口关税" style="width: 300px"/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 加征税率:</td>
                <td>
                    <input id="rate" name="rate" class="easyui-numberbox" data-options="precision:'2'" style="width: 300px" required/>
                </td>
            </tr> 
            <tr>
                <td class="liebiao-label"> 加征开始日期:</td>
                <td>
                    <input id="startDate" name="startDate" class="easyui-datebox" style="width: 300px" required/>
                </td>
            </tr>  
            <tr>
                <td class="liebiao-label"> 加征结束日期:</td>
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

