<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Drivers.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.Driver)) {
                $('#form1').form('load',
                    {
                        Name: model.Driver.Name,
                        IDCard: model.Driver.IDCard,
                        Mobile1: model.Driver.Mobile,
                        Mobile2: model.Driver.Mobile2,
                        CustomsCode: model.Driver.CustomsCode,
                        CardCode: model.Driver.CardCode,
                        PortCode: model.Driver.PortCode,
                        LBPassword: model.Driver.LBPassword
                    });
                $('#txtName').textbox('readonly');
                $('#txtIDCard').textbox('readonly');
                $('#txtMobile1').textbox('readonly');
                $("#txtIDCard").textbox({ required: false });
                $("#txtMobile1").textbox({ required: false });
                if (model.Driver.IsChcd) {
                    $("#IsChcd").checkbox('check');
                }
            }
            //中港贸易
            $('#IsChcd').checkbox({
                checked: true,
                onChange: function (checked) {
                    if (checked) {
                        $("#txtIDCard").textbox({ required: true });
                        $("#txtMobile1").textbox({ required: true });
                    }
                    else {
                        $("#txtIDCard").textbox({ required: false });
                        $("#txtMobile1").textbox({ required: false });

                    }
                }
            });

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">

        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td colspan="4">
                        <input class="easyui-checkbox" id="IsChcd" name="IsChcd" /><label for="IsDefault" style="margin-right: 30px">是否中港贸易</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">司机姓名</td>
                    <td colspan="3">
                        <input id="txtName" name="Name" class="easyui-textbox readonly_style" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">身份证号</td>
                    <td colspan="3">
                        <input id="txtIDCard" name="IDCard" class="easyui-textbox readonly_style" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">手机号(大陆)</td>
                    <td>
                        <input id="txtMobile1" name="Mobile1" class="easyui-textbox readonly_style" style="width: 200px;" data-options="required:true,validType:'telNum'">
                    </td>
                    <td style="width: 100px">手机号(香港或其他)</td>
                    <td>
                        <input id="txtMobile2" name="Mobile2" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'telNum'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">海关编码</td>
                    <td>
                        <input id="txtCustomsCode" name="CustomsCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                    <td style="width: 100px">司机卡号</td>
                    <td>
                        <input id="txtCardCode" name="CardCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">口岸电子编号</td>
                    <td>
                        <input id="txtPortCode" name="PortCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                    <td style="width: 100px">寮步密码</td>
                    <td>
                        <input id="txtLBPassword" name="LBPassword" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
            </table>
        </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <%-- <a id="btnsave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
        </div>
    </div>
</asp:Content>
