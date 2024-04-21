<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("input", $("#txtName").next("span")).blur(function () {
                isExist();
            });
        });

        function isExist() {
            var companyName = $('#txtName').textbox('getValue');
            $.post('?action=GetEnterpriseName', { Name: companyName }, function (res) {
                //加载
                if (res) {
                    top.$.messager.alert('错误', "公司名称已存在");
                    return;
                }
            });
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td style="width: 120px;">公司名称</td>
            <td>
                <input id="txtName" name="Name" class="easyui-textbox" style="width: 350px;"
                    data-options="required:true,validType:'length[1,150]'" onblur="isExist()">
            </td>
        </tr>
        <tr>
            <td style="width: 100px">法人</td>
            <td colspan="3">
                <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
            </td>
        </tr>
        <tr>
            <td style="width: 100px">注册地址</td>
            <td colspan="3">
                <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
            </td>
        </tr>
        <tr>
            <td style="width: 100px">统一社会信用代码</td>
            <td colspan="3">
                <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
            </td>
        </tr>
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>

</asp:Content>
