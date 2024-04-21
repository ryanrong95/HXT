<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceComments.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //新增
            //$("#btnadd").click(function () {
            //    $.myDialog({
            //        title: '添加',
            //        url: 'Add.aspx?TraceID=' + id,
            //        width: "750px",
            //        height: "400px",
            //        isHaveOk: true,
            //        onClose: function () {
            //             grid.myDatagrid('flush');
            //           // window.location.reload();
            //        }
            //    });
            //})
        })
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table>
        <tr>
            <td style="width: 80px; vertical-align: top;">点评内容：</td>
            <td>
                <input class="easyui-textbox" id="Comments" name="Comments" style="width: 600px; height: 300px"
                    data-options="required:true, multiline:true,validType:'length[1,300]',tipPosition:'right'" />
            </td>
        </tr>
    </table>
    <div style="text-align: center; padding: 5px; display: none;">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>
    <%-- <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px;">
    </div>--%>
</asp:Content>
