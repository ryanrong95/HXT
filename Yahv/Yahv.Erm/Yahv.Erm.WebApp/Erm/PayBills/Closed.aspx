<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Closed.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PayBills.Closed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                fitColumns: true,
                pagination: false,
                singleSelect: false,
                rownumbers: false
            });
        });

        //保存
        function Save() {
            var array = $('#tab1').datagrid('getChecked');
            //if (array.length == 0) {
            //    $.messager.alert("提示", "请选择分配的工资项！");
            //    return false;
            //}
            var names = array.map(function (element, index) {
                return element.Name;
            }).join(',');
            $("#hNames").val(names);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" title="选择清空项" data-options="border:false" style="width: 98%; height: 100%">
        <table id="tab1" style="width: 30%; height: 98%">
            <thead>
                <tr>
                    <th data-options="field:'ck',checkbox:true"></th>
                    <th data-options="field:'Name',width:50">工资项名称</th>
                </tr>
            </thead>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClientClick="return Save();" OnClick="btnSave_Click" />
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
    <input type="hidden" runat="server" id="hNames" />
</asp:Content>
