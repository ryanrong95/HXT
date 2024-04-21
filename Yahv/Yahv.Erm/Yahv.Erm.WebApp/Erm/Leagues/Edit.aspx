<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Leagues.Edit" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //下拉
            $('#cbbType').combobox({
                url: '?action=selects_type',
                valueField: 'id',
                textField: 'name',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function (newValue, oldValue) {
                    $('#tr_role').hide();
                    $('#tr_enterprise').hide();

                    $('#slctRole').combobox({ 'required': false });
                    $('#slctEnterprise').combobox({ 'required': false });

                    if (newValue == '<%=(int)LeagueType.Position %>') {
                        $('#tr_role').show();
                        $('#slctRole').combobox({ 'required': true });
                    }

                    if (newValue == '<%=(int)LeagueType.Company %>') {
                        $('#tr_enterprise').show();
                        $('#slctEnterprise').combobox({ 'required': true });
                    }
                }
            });

            if (model && model.ID) {
                $('form').form('load', model);

                $('#txtName').textbox('readonly');
            } else {
                $('#tr_role').hide();
                $('#tr_enterprise').hide();
            }
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="display: none;">
        <input id="txtFatherid" name="FatherID" class="easyui-textbox">
    </div>
    <div class="easyui-panel" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>名称</td>
                    <td>
                        <input id="txtName" name="Name" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,75]'" />
                    </td>
                </tr>
                <tr>
                    <td>类型</td>
                    <td>
                        <input id="cbbType" name="Type" class="easyui-combobox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,75]'">
                    </td>
                </tr>
                <tr id="tr_role">
                    <td>角色</td>
                    <td>
                        <select id="slctRole" name="RoleID" class="easyui-combobox"
                            data-options="editable: false, url:'?action=roles', valueField: 'id', textField: 'name',panelHeight:'160px'"
                            style="width: 200px">
                        </select>
                    </td>
                </tr>
                <tr id="tr_enterprise">
                    <td>内部公司</td>
                    <td>
                        <select id="slctEnterprise" name="EnterpriseID" class="easyui-combobox"
                            data-options="editable: true, url:'?action=enterprises', valueField: 'id', textField: 'name',panelHeight:'160px'"
                            style="width: 200px">
                        </select>
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>
