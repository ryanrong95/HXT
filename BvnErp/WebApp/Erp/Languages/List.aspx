<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Erp.Languages.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>语言列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        window.gvSettings.fatherMenu = '系统管理';
        window.gvSettings.menu = '语言管理';
        window.gvSettings.summary = '';

    </script>

    <script type="text/javascript">
        $(function () {
            $.maintable = $("#tab1");
            if ($.maintable.length != 1) {
                return;
            }
            $.queryUrl = typeof ($.queryUrl) == 'undefined' ? "?" : $.queryUrl; // url
            $.maintablePaging = true; //typeof ($.maintablePaging) == 'undefined' ? false : $.maintablePaging; // 是否分页

            $.Paging = function (pageIndex, pageSize) {
                if (!pageIndex) {
                    pageIndex = $.maintable.datagrid('getPager').pagination('options').pageNumber;
                }
                if (!pageSize) {
                    pageSize = $.maintable.datagrid('getPager').pagination('options').pageSize;
                }

                if (!!!$.datagrid_param) {
                    $.datagrid_param = {};
                }
                $.datagrid_param.pi = pageIndex;
                $.datagrid_param.ps = pageSize;

                $.datagrid_param._ = Math.random();
                $.datagrid_param.action = 'data';
                $.maintable.datagrid('loading');
                $.get($.queryUrl, $.datagrid_param, function (data) {
                    $.maintable.datagrid('loadData', data);
                }, 'json');
            };
            $.maintable.datagrid({
                method: "post",
                striped: true,
                rownumbers: true,
                pageList: [1, 10, 20, 50, 100],
                pagination: $.maintablePaging,
                //toolbar: '#tb',
                singleSelect: true,
                checkOnSelect: false,
                selectOnCheck: false,
                onLoadSuccess: function (data) {
                    $.maintable.datagrid("loaded");
                    //$.maintable_loadsuccess(val, rec);
                },
                onSelect: function (rowData) {
                    //navgrid.datagrid("unselectAll");
                }
            });
            $.maintable_loadsuccess = function () { };

            $.maintable.datagrid('getPager').pagination({
                showRefresh: true,
                pageSize: parseInt('20'),
                onSelectPage: $.Paging
            });

            $.Paging();
        });
        var btnformatter = function (val, rec) {
            var arry = [
                '<button style="cursor:pointer;" v_name="editbtn" v_title="列表项编辑按钮" onclick="edit(\'' + rec.ID + '\');">编辑</button>',
                '<button style="cursor:pointer;" v_name="deletebtn" v_title="列表项删除按钮" onclick="del(\'' + rec.ID + '\');">删除</button>'
            ];

            return arry.join('');
        };

        var edit = function (id) {
            top.$.myWindow({ url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + id, onClose: function () { $.Paging(); } }).open();
        };

        var del = function (id) {
            $.messager.confirm('删除提示', '您确定要删除选中的记录吗?', function (r) {
                if (r) {
                    $.post("?action=del", { id: id }, function (data) {
                        $.messager.alert('提示', '删除成功');
                        $.Paging();
                    })
                }
            });
        };

    </script>

    <script type="text/javascript">
        $(function () {
            $('#btnNew').click(function () {
                top.$.myWindow({ url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx'), onClose: function () { $.Paging(); } }).open();
            });

            //alert($('[v_name="ShortName"]').html());
        })
    </script>

</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 75px">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td>
                    <%--<button id="btnSearch" class="easyui-linkbutton" iconcls="icon-search">搜索</button>
                    <button id="btnClear" class="easyui-linkbutton" iconcls="icon-clear">清空</button>--%>
                    <button id="btnNew" v_name="addnewbtn" v_title="搜索区新增按钮" class="easyui-linkbutton" iconcls="icon-add">
                        新增
                    </button>
                </td>
            </tr>
        </table>

    </div>
    <div data-options="region:'center',border:false">
        <table id="tab1" title="语言列表" data-options="fitColumns:true,border:false,fit:true" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'ShortName'">短名称</th>
                    <th data-options="field:'DisplayName'">显示名称</th>
                    <th data-options="field:'EnglishName'">英文名称</th>
                    <th data-options="field:'EnglishName'">数据库名称</th>
                    <th data-options="field:'btn',formatter:btnformatter" g_name="btn" v_title="列表项操作按钮组" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
