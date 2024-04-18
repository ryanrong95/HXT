<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Plat.Roles.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '权限管理';
        gvSettings.menu = '角色管理';
        gvSettings.summary = '';

    </script>
    <script>
        var getParams = function () {
            var data = {
                action: 'data',
                rolename: $.trim($('#_rolename').val())
            };
            return data;
        };

        var loadgrid = function () {
            $("#tab1").bvgrid({
                queryParams: getParams(),
                toolbar: '#tbtool',
                OnLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        //循环判断操作为新增的不能选择
                        for (var i = 0; i < data.rows.length; i++) {
                            //根据isFinanceExamine让某些行不可选
                            if (data.rows[i].isFinanceExamine == 1) {
                                $("input[type='checkbox']")[i + 1].disabled = true;
                            }
                        }
                    }
                }
            });
        }

        $(function () {

            loadgrid();

            $("#brnSearch").click(function () {
                loadgrid();
            });
            $("#brnClear").click(function () {
                location.href = '?';
            });

        });

        /* 时间转换 */
        var dateformatter = function (value, row) {
            if (value) {
                return value;
                var date = new Date(value);
                //return new Date(parseInt(/(\d+)/.exec(value))).toDateTimeStr();
            }
            else {
                return "";
            }
        };


    </script>

    <script>
        /* 操作按钮 */
        var btnformatter = function (value, row) {
            var arry = [];
            if (row.Status != 210) {
                arry.push('<button v_name="option-btn-edit" v_title="列表项按钮-编辑" style="cursor:pointer;" onclick="edit(\'' + row.ID + '\');">编辑</button>');
                arry.push('<button v_name="option-btn-setting" v_title="列表项按钮-设置权限" style="cursor:pointer;" onclick="setting(\'' + row.ID + '\');">设置权限</button>');
            }
            arry.push('<button v_name="option-btn-allot" v_title="列表项按钮-管理员" style="cursor:pointer;" onclick="allot(\'' + row.ID + '\');">管理员</button>');
            return arry.join('');
        };
        var edit = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + id,
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };
        var setting = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Setting.aspx') + '?id=' + id,
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };
        var allot = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Allot.aspx') + '?id=' + id,
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };

        /* 工具栏按钮 */
        var add = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx'),
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };
        var del = function () {
            var arry = $('#tab1').datagrid('getChecked');
            if (arry && arry.length > 0) {
                $.messager.confirm('删除提示', '您确定要删除选中的记录吗?', function (r) {
                    if (r) {

                        var ids = arry.map(function (element, index) {
                            return element.ID;
                        });
                        $.post('?action=del', { ids: ids.join(',') }, function (response) {
                            if (response) {
                                $.messager.alert('提示', '删除成功', 'info');
                                loadgrid();
                            }
                            else {
                                $.messager.alert('提示', '删除失败', 'error');
                            }
                        }, 'json');
                    }
                });
            }
            else {
                $.messager.alert('提示', '请选择要删除的记录', 'warning');
            }

        };

    </script>
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 110px; padding: 10px 0px;">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <th style="width: 90px;">角色
                </th>
                <td>
                    <input type="text" class="easyui-textbox" prompt="名称&ID" id="_rolename" maxlength="50" style="width: 264px;" value="" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <button id="btnSearch" class="easyui-linkbutton" iconcls="icon-search">搜索</button>
                    <button id="btnClear" class="easyui-linkbutton" iconcls="icon-clear">清空</button>
                </td>
            </tr>
        </table>

    </div>
    <div data-options="region:'center',border:false">
        <table id="tab1" class="easyui-datagrid mygrid" title="角色列表" data-options="checkOnSelect:true,fitColumns:true,border:false,fit:true">
            <thead>
                <tr>
                    <th data-options="field:'ck',checkbox:true">选择</th>
                    <th g_name="ID" v_title="列表项列-ID" data-options="field:'ID'">ID</th>
                    <th g_name="Name" v_title="列表项列-角色名" data-options="field:'Name'">角色名</th>
                    <th g_name="Summary" v_title="列表项列-备注" data-options="field:'Summary'">备注</th>
                    <th g_name="CreateDate" v_title="列表项列-创建时间" data-options="field:'CreateDate',formatter:dateformatter">创建时间</th>
                    <th g_name="UpdateDate" v_title="列表项列-更新时间" data-options="field:'UpdateDate',formatter:dateformatter">更新时间</th>
                    <th g_name="option-btns" v_title="列表项按钮组" data-options="field:'option-btns',formatter:btnformatter">操作</th>
                </tr>
            </thead>
        </table>
       
        <div id="tbtool" style="height: auto;">
            <a href="javascript:void(0)" v_name="option-btn-add" v_title="按钮-新增" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="add()">Add</a>
            <a href="javascript:void(0)" v_name="option-btn-remove" v_title="按钮-删除" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="del()">Remove</a>
        </div>
    </div>


</body>
</html>
