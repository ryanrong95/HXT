<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WageItems.Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                fitColumns: true,
                pagination: false,
                singleSelect: true,
                onLoadSuccess: function () {
                    $("#tab1").datagrid('enableDnd');
                },
            });
        });


        //保存顺序
        function Save() {
            var data = $("#tab1").datagrid("getRows");
            var ids = "", orderindex = "";
            data.map(function (element, index) {
                ids += element.ID + ",";
                var value = index + 1;
                orderindex += value + ",";
            });
            $("#hidids").val(ids);
            $("#hidIndexs").val(orderindex);

            //formdata方式传回后台
            //var form = new FormData($("#form1")[0]);
            //form.append("ids", ids);
            //form.append("orderindex", orderindex);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'ID',width:100">ID</th>
                <th data-options="field:'Name',width:100">名称</th>
                <th data-options="field:'OrderIndex',width:100">顺序</th>
                <th data-options="field:'Description',width:150">描述</th>
                <th data-options="field:'AdminName',width:100">录入人</th>
                <th data-options="field:'CreateDate',width:100">创建日期</th>
                <th data-options="field:'StatusName',width:100">状态</th>
            </tr>
        </thead>
    </table>
    <div data-options="region:'south',border:false" style="text-align: center; padding: 5px; height: 5%">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="return Save();" OnClick="btnSubmit_Click" />
    </div>
    <input type="hidden" runat="server" id="hidids" />
    <input type="hidden" runat="server" id="hidIndexs" />
</asp:Content>
