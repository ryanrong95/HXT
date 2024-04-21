<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                    {
                        Corporation: model.Entity.EnterpriseRegister.Corperation,
                        RegAddress: model.Entity.EnterpriseRegister.RegAddress,//注册地址
                        Uscc: model.Entity.EnterpriseRegister.Uscc,//统一社会信用代码
                    });
                //$('#txtName').textbox('textbox').attr('disabled', true);
                // document.getElementById('lab_Name').innerHTML = model.Entity.Enterprise.Name;
                $("#lab_Name").text(model.Entity.Name);
            }

        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td style="width: 120px;">公司名称</td>
            <td>
                <label id="lab_Name"></label>
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
        <%-- <a class="easyui-linkbutton" id="submint" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>--%>
    </div>

</asp:Content>
