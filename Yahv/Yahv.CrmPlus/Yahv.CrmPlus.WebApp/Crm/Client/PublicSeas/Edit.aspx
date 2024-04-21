<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas.Edit" %>

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
            });
            $("#ConductType").combobox({
                data: model.ConductType
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">

 
    <table class="liebiao">
        <tr>
            <td>认领人：</td>
            <td>
                <label id="Claiman" style="height:30px"></label>
            </td>
        </tr>
        <tr>
            <td>我方合作公司：</td>
            <td>
                <input id="Company" name="Company" class="easyui-combobox" style="width: 350px;height:30px" data-options="required:true, editable:false,panelheight:'auto'" />
            </td>
        </tr>
        <tr>
            <td>业务类型：</td>
            <td>
                <input id="ConductType" name="ConductType" class="easyui-combobox" style="width: 350px;height:30px"" data-options="required:true, editable:false,panelheight:'auto'" />
            </td>
        </tr>

    </table>
       </div>
</asp:Content>
