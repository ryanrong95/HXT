<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Document.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //新增按钮
            $('#btnAdd').click(function () {
                $.myWindow({
                    title: '新增发布内容',
                    minWidth: 1200,
                    minHeight: 600,
                    url: '../Document/Edit.aspx',
                    onClose: function () {
                        grid.myDatagrid('search', getQuery());
                    },
                });
                return false;
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#CatalogID').combotree("clear");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //菜单类别
            $('#CatalogID').combotree({
                valueField: 'id',
                textField: 'text',
                data: model.CatalogData,
                multiple: true,
                panelWidth: 200,//宽度自适应
            })

            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true,
                scrollbarSize: 0,
            });
            
        });

        var getQuery = function () {
            var params = {
                action: 'data',
                CatalogID: $("#CatalogID").combotree("getValues").join(','),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };

        //编辑
        function Edit(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '编辑发布内容',
                minWidth: 1200,
                minHeight: 600,
                url: '../Document/Edit.aspx?ID=' + data.ID,
                onClose: function () {
                    grid.myDatagrid('search', getQuery());
                },
            });
            return false;
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除所选内容！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }

        //下单操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(' + index + ');return false;">编辑</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-cancel\'" onclick="Delete(\'' + row.ID + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">菜单类别</td>
                <td>
                    <input id="CatalogID" class="easyui-combotree" style="width: 200px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:''" class="easyui-datebox" />
                    &nbsp&nbsp&nbsp&nbsp<span>至</span>&nbsp&nbsp&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:''" class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">新增</a>
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="网站发布内容">
        <thead>
            <tr>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 150px;">操作</th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 150px;">创建日期</th>
                <th data-options="field:'Title',align:'center'" style="width: 300px">标题</th>
                <th data-options="field:'CatalogName',align:'center'" style="width: 100px;">类别</th>
                <th data-options="field:'Abstract',align:'center'" style="width: 300px;">摘要</th>
                <th data-options="field:'CreatorName',align:'center'" style="width: 130px;">创建人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
