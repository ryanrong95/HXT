<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceRecords.TraceComments.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {

            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                //toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: true,
                pagination: false,
            });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-layout" data-options="fit:true">
  <%--      <div data-options="region:'north'" style="height: 210px;width:810px">--%>
            <table class="liebiao">
                <tr>
                    <td>点评内容：</td>
                    <td>
                        <input class="easyui-textbox input" id="Comments" name="Comments"
                            data-options="required:true, multiline:true,validType:'length[1,300]',tipPosition:'right'" style="width: 600px;height:200px" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px; display: none;">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            </div>
       <%-- </div>--%>
       <%-- <div data-options="region:'center'" style="height: 553px">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" title="点评记录">
                <thead>
                    <tr>
                        <th data-options="field:'Reader',width:150">点评人</th>
                        <th data-options="field:'CreateDate',width:100">点评时间</th>
                        <th data-options="field:'Comments',width:260">点评内容</th>
                    </tr>
                </thead>
            </table>

        </div>--%>
    </div>

</asp:Content>
