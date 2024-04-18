<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.BasePrice.List" %>

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
            //新增
            $('#btn').bind('click', function () {
                $.myWindow({
                    title: "新增配置",
                    url: location.pathname.replace('List.aspx', 'Edit.aspx'),
                    width: 600,
                    height: 400,
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                });
                return false;
            });
        });
    </script>
    <script>
        //操作
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\',\'' + row.SpecID + '\',\'' + row.Month + '\');return false;">编辑</a> ')
            buttons.push('</span>')
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;">删除</a> ')
            buttons.push('</span>')
            return buttons.join('');
        }
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
    <script>
        //编辑
        function Edit(id, specID, month) {
            $.myWindow({
                title: "编辑价格配置",
                url: location.pathname.replace('List.aspx', 'Edit.aspx?ID=' + id + '&SpecID=' + specID + '&Month=' + month),
                width: 600,
                height: 400,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除所选项。', function (success) {
                if (success) {
                    $.post('?action=Delete', { id: id }, function (result) {
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <a id="btn" class="easyui-linkbutton" data-options="iconCls:'icon-add'" style="margin: 5px 0;">新增配置</a>
    </div>
    <table id="tab1" title="库位租赁价格配置表">
        <thead>
            <tr>
                <th data-options="field:'Name',align:'center'" style="width: 100px;">库位名称</th>
                <th data-options="field:'SpecID',align:'center'" style="width: 100px">库位级别</th>
                <th data-options="field:'Load',align:'center'" style="width: 100px">承重(kg)</th>
                <th data-options="field:'Volume',align:'center'" style="width: 100px;">容积(cm³)</th>
                <th data-options="field:'Month',align:'center'" style="width: 100px">租赁时长(月)</th>
                <th data-options="field:'Price',align:'center'" style="width: 100px;">租赁单价</th>
                <th data-options="field:'CreatorName',align:'center'" style="width: 80px;">操作人</th>
                <th data-options="field:'Summary',align:'center'" style="width: 120px">备注</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 110px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

