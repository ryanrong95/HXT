<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List1.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsContracts.List1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            $("#btnPreview").on('click', function () {
                // $.messager.alert('提示', '正在开发中！');
                $.post('?action=PreviewAgreement', { clientid: model.ClientID }, function (result) {
                    if (result.success) {
                        window.open(result.url);
                        // location.href = result.url;
                    }
                })
            })
            //导出协议
            $('#btnExport').on('click', function () {
                //$.messager.alert('提示', '正在开发中！');
                $.post('?action=ExportAgreement', { clientid: model.ClientID }, function (result) {
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = result.url;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                })
            });
            
        })
        function Pdf(url) {
            window.open(url, '', '', false)
            
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="margin: 8px;" id="ExportDiv">
        <a id="btnPreview" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">预览协议</a>
        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-exportFile'">导出协议</a>
    </div>

        <%
            string parta = this.Model.PartA;
            string partb = this.Model.PartB;

        %>
        <table class="liebiao">
            <tr>
                <td style="width: 100px">甲方</td>
                <td><%=parta %></td>
            </tr>
            <tr>
                <td style="width: 100px">乙方</td>

                <td><%=partb %></td>
            </tr>
            <%
                Yahv.Services.Models.CenterFileDescription agrement = this.Model.File as Yahv.Services.Models.CenterFileDescription;
            %>
            <tr>
                <td style="width: 100px">文件</td>
                <td><a href="#" onclick="Pdf('<%=agrement?.Url%>')"><%=agrement?.CustomName %></a>
                   </td>
            </tr>


        </table>
    </div>
</asp:Content>
