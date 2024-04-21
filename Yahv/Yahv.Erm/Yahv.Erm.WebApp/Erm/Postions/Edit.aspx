<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Postions.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                pagination: false,
                singleSelect: false,
                rownumbers: false,
                fitColumns: true,
            });
        });

        //保存
        function Save() {
            var array = $('#tab1').datagrid('getChecked');
            var ids = array.map(function (element, index) {
                return element.ID;
            }).join(',');
            $("#hCheckId").val(ids);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" data-options="border:false" style="width:100%;height:100%">
        <div data-options="region:'north',border:false,collapsed:false">
            <table class="liebiao">
                <tr>
                    <td>岗位名称</td>
                    <td>
                        <input id="txtName" name="Name" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,50]'" />
                    </td>
                </tr>
            </table>
        </div>
        <div title="工资项分配" data-options="region:'center',border:false" style="height:75%">
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true"></th>
                        <th data-options="field:'Name',width:50">工资项名</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',border:false" style="text-align: center; padding: 5px;height:5%">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="return Save();" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
    <input type="hidden" runat="server" id="hCheckId" />
</asp:Content>
