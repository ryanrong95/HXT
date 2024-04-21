<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="UpdateName.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsClients.UpdateName" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            $("#txtName").textbox('setValue', model.Entity.Name);
            //var getQuery = function () {
            //    var params = {
            //        action: 'data'
            //    };
            //    return params;
            //};
            ////设置表格
            //window.grid = $("#dg").myDatagrid({
            //    toolbar: '#tb',
            //    pagination: false,
            //    singleSelect: false,
            //    method: 'get',
            //    queryParams: getQuery(),
            //    fit: true,
            //    rownumbers: true,
            //    nowrap: false
            //});
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td class="lbl">客户名称：</td>
            <td>
               <input id="txtName" name="NewName" class="easyui-textbox readonly_style"
                                data-options="required:true,validType:'length[1,75]'" style="width: 350px;">
            </td>
        </tr>
       
    </table>
    <%-- <table id="dg" title="" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field:'UserName',width:180,nowrap:false">用户名</th>
                    <th data-options="field:'RealName',width:80">真实姓名</th>
                    <th data-options="field:'Mobile',width:100">手机号</th>
                    <th data-options="field:'CreateDate',width:100">创建时间</th>
                </tr>
            </thead>
        </table>--%>
    <div id="btnDiv" style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
