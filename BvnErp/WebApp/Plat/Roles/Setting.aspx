<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setting.aspx.cs" Inherits="WebApp.Plat.Roles.Setting" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>权限设置</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '';
        gvSettings.menu = '角色管理';
        gvSettings.summary = '';

    </script>

    <script>
        $(function () {
            var isTreeLoad = true; /* 是否在初始化，是就不进行post调用menusave */
            $('.datagrid ').hide();
            var json = eval(<%=this.Model %>); /* menu tree data */
            var roleMenu = eval(<%=this.RoleMenus %>); /* role menu data */
            var roleUnites = eval(<%=this.RoleUnites %>);/* role unites data */
            var roleID = '<%=ViewState["RoleID"] %>';
            $('#menu').tree({
                data: json,
                onLoadSuccess: function (node, data) {
                    $.each(roleMenu, function (index, element) {
                        var node = $("#menu").tree("find", element);
                        if (node) {
                            $("#menu").tree("check", node.target);
                        }
                    });
                    isTreeLoad = false;
                },
                onCheck: function (node, checked) {
                    if (!isTreeLoad) {
                        var action = checked ? 'menuSave' : 'menuRemove';
                        var ids = [];
                        ids.push(node.id);
                        var parent = $('#menu').tree('getParent', node.target);
                        if (parent == null) { /* 一级菜单全选全不选 */
                            var children = $('#menu').tree('getChildren', node.target);
                            $.each(children, function (index, element) {
                                ids.push(element.id);
                            });
                        }
                        else {
                            if (parent.checkState == 'checked' && checked) {
                                ids.push(parent.id);
                            }
                            if (parent.checkState == 'unchecked' && !checked) {
                                ids.push(parent.id);
                            }
                        }
                        $.post("?action=" + action, { id: roleID, 'ids': ids.join(',') }, function () {

                        });
                    }
                },
                onClick: function (node) {
                    $('.datagrid ').show();
                    var parent = $('#menu').tree('getParent', node.target);
                    if (parent == null) {
                        $('.datagrid').hide();
                        return false;
                    }
                    loadUnites(node.text);
                }
            });

            var loadUnites = function (menu) {
                var isLoad = true;
                $('#tab1').datagrid({
                    url: "?action=roleUnites",
                    method: "post",
                    queryParams: {
                        id: roleID,
                        menu: menu
                    },
                    onLoadSuccess: function (data) {
                        /* 默认选中 */
                        $.each(data.rows, function (index, element) {
                            var idx = roleUnites.indexOf(element.ID);
                            if (roleUnites.indexOf(element.ID) >= 0) {
                                $("#tab1").datagrid("checkRow", index);
                            }
                        });
                        isLoad = false;
                    },
                    onCheck: function (rowIndex, rowData) {
                        if (!isLoad) {
                            checkevent("save", rowData.ID);
                        }
                    },
                    onUncheck: function (rowIndex, rowData) {
                        checkevent("remove", rowData.ID);
                    },
                    onCheckAll: function (rows) {
                        if (!isLoad) {
                            var arry = [];
                            for (var index = 0; index < rows.length; index++) {
                                arry.push(rows[index].ID);
                            }
                            checkevent("save", arry.join(','));
                        }
                    },
                    onUncheckAll: function (rows) {
                        var arry = [];
                        for (var index = 0; index < rows.length; index++) {
                            arry.push(rows[index].ID);
                        }
                        checkevent("remove", arry.join(','));
                    }
                });

                // checkbox change function
                var checkevent = function (action, ids) {
                    $.post("?action=" + action, { id: roleID, ids: ids }, function () {

                    });
                };
            };
        });
    </script>
    <script>
        var typeformatter = function (value, row) {
            if (value == 1) {
                return "正常";
            }
            else if (value == 2) {
                return "表格列";
            }
            else {
                return "";
            }
        };
    </script>

</head>
<body>
    <div class="easyui-panel" title="权限设置" style="width: 100%; min-height: 400px; height: auto; padding: 10px;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'west',split:true" style="width: 26%; padding: 10px">

                <ul class="easyui-tree" id="menu" data-options="animate: true,checkbox:true,lines:true">
                </ul>

            </div>
            <div data-options="region:'center'" style="padding: 10px">
                <table id="tab1" class="easyui-datagrid" style="display: none;" title="颗粒化" data-options="fit:true,checkOnSelect:true">
                    <thead>
                        <tr>
                            <th data-options="field:'ck',checkbox:true">选择</th>
                            <th data-options="field:'Type',formatter:typeformatter">类型</th>
                            <th data-options="field:'Menu'">所属菜单</th>
                            <th data-options="field:'Url'">菜单Url</th>
                            <th data-options="field:'Name'">名称</th>
                            <th data-options="field:'Title'">显示名称</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
</body>
</html>
