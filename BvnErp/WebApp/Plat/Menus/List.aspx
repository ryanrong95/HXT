<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Plat.Menus.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>菜单列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '系统管理';
        gvSettings.menu = '菜单管理';
        gvSettings.summary = '';

    </script>
    <script>
        $(function () {
            var editId;
            var loadtree = function () {
                $('#tab1').treegrid({
                    url: '?',
                    method: 'post',
                    queryParams: {
                        action: 'data',
                        name: $.trim($('#_name').val())
                    },
                    lines: true,
                    idField: 'id',
                    treeField: 'name',
                    onClickCell: function (field, row) {
                        if (editId) {
                            $('#tab1').treegrid('endEdit', editId);
                        }
                        if (field == 'OrderIndex') {
                            $('#tab1').treegrid('beginEdit', row.id);
                            editId = row.id;
                            var ed = $('#tab1').treegrid('getEditor', { id: row.id, field: field });
                            var obj = ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target));
                            obj.focus();

                            obj.blur(function () {
                                if ($.trim(obj.val()) == row.OrderIndex) {
                                    $('#tab1').treegrid('cancelEdit', row.id);
                                }
                                //else {
                                //    $('#tab1').treegrid('endEdit', editId);
                                //}
                            });
                        }
                    },
                    onAfterEdit: function (row, changes) {
                        $.post("?",
                              {
                                  action: 'change',
                                  id: row.id,
                                  index: encodeURIComponent(changes.OrderIndex)
                              },
                              function (data) {

                              });
                    }
                });
            };

            loadtree();

            $('#btnSearch').click(function () {
                loadtree();
            });
            $('#btnClear').click(function () {
                location.href = '?';
            });


        });

        var btnformatter = function (val, rec) {
            var arry = [
                '<button style="cursor:pointer;" onclick="del(\'' + rec.id + '\');">删除</button>'
            ];

            return arry.join('');
        };
        var del = function (id) {
            console.log(id);
            $.messager.confirm('删除提示', '您确定要删除该记录吗?', function (r) {
                if (r) {
                    $.post('?action=del', { id: id }, function (response) {
                        if (response) {
                            $.messager.alert('提示', '删除成功', 'info', function () {
                                location.href = '?';
                            });
                        }
                        else {
                            $.messager.alert('提示', '删除失败', 'error');
                        }
                    }, 'json');
                }
            });
        };



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
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 110px; padding: 10px 0px;">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <th style="width: 90px;">菜单
                </th>
                <td>
                    <input type="text" class="easyui-textbox" prompt="名称&ID" id="_name" maxlength="50" style="width: 264px;" value="" />
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
        <table id="tab1" class="easyui-treegrid" title="菜单列表" data-options="fit:true">
            <thead>
                <tr>
                    <th data-options="field:'id'">ID</th>
                    <th data-options="field:'name'">名称</th>
                    <th data-options="field:'OrderIndex',width:40,editor:'numberbox'">排序</th>
                    <th data-options="field:'Url'">Url</th>
                    <th data-options="field:'CreateDate',formatter:dateformatter">创建时间</th>
                    <th v_name="option-btns" v_title="列表项操作按钮" data-options="field:'btns',formatter:btnformatter">操作</th>
                </tr>
            </thead>
        </table>
    </div>

</body>
</html>
