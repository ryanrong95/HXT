<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ReduceRecords.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.ReduceRecords" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: true,
                pagination: false,
                fit: true,
                nowrap: false,
            });
        });
    </script>
    <script>
        //撤销当前减免
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;">撤销</a> '
                , '</span>'].join('');
        }
        //撤销减免
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除减免记录！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('提示', rel.message);
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <table id="tab1" title="">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 50px;">减免日期</th>
                    <th data-options="field:'Price',align:'center'" style="width: 50px;">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 50px;">币种</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 50px;">减免人</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 50px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
