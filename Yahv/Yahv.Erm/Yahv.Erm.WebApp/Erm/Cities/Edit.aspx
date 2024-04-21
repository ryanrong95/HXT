<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Cities.Edit" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model && model.ID) {
                $('form').form('load', model);

                $('#txtName').textbox('readonly');
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
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>
