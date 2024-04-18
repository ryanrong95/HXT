<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ViewList.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.BasePrice.ViewList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                pagination: false,
                fitColumns: true,
                scrollbarSize: 0,
                rownumbers: true,
                onLoadSuccess: onLoadSuccess,
            });
        });
    </script>
    <script>
        //合并单元格
        function onLoadSuccess(data) {
            var rowspan1 = 1;
            var rowspan2 = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['SpecID'] == data.rows[i - 1]['SpecID']) {
                    rowspan2 += 1;
                    $("#tab1").datagrid('mergeCells', {
                        index: i + 1 - rowspan2,
                        field: 'SpecID',
                        rowspan: rowspan2
                    });
                }
                if (data.rows[i]['Name'] == data.rows[i - 1]['Name']) {
                    rowspan1 += 1;
                    $("#tab1").datagrid('mergeCells', {
                        index: i + 1 - rowspan1,
                        field: 'Name',
                        rowspan: rowspan1
                    });
                }
                else {
                    rowspan1 = 1;
                    rowspan2 = 1;
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tab1" title="">
        <thead>
            <tr>
                <th data-options="field:'Name',align:'center'" style="width: 100px;">库位名称</th>
                <th data-options="field:'SpecID',align:'center'" style="width: 100px">库位级别</th>
                <th data-options="field:'Load',align:'center'" style="width: 100px">承重(kg)</th>
                <th data-options="field:'Volume',align:'center'" style="width: 100px;">容积(cm³)</th>
                <th data-options="field:'Month',align:'center'" style="width: 100px">租赁时长(月)</th>
                <th data-options="field:'Price',align:'center'" style="width: 100px;">租赁单价</th>
                <th data-options="field:'Summary',align:'center'" style="width: 150px">备注</th>
            </tr>
        </thead>
    </table>
</asp:Content>


