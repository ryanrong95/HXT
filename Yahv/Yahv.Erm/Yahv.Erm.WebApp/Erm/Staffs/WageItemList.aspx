<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="WageItemList.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.PayBill" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //页面加载
        $(function () {
            fetchData()

        });
        function fetchData() {
            var s = "";
            s = "[[";
            $.each(model.PositionItems, function (index, value, array) {

                s += "{field:'" + value.ID + "',title:'" + value.Name + "',width:100,editor: { type: 'textbox', options: { validType: 'length[1,50]' }}},";

            });
            s += "{ field: 'Btn', title: '操作', width: 100, align: 'center', formatter: Operation }";
            s = s + "]]";
            //使用js动态创建easyui的datagrid
            $('#tab1').myDatagrid({
                fit: true,
                pagination: false,
                nowrap: true,
                columns: eval(s)

            });

        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        var editIndex = undefined;
        function Edit(index) {
            $('#tab1').datagrid('selectRow', index)
                .datagrid('beginEdit', index);
            editIndex = index;
        }
        function EndEdit() {
            if (editIndex == undefined) { return true }
            $('#tab1').datagrid('endEdit', editIndex);
            debugger;
            var rows = $('#tab1').datagrid('getEditors', editIndex);

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tab1" title="工资项列表">
    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClientClick="EndEdit()" OnClick="btnSave_Click" />
    </div>
</asp:Content>
