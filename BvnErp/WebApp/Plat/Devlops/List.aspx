<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Plat.Devlops.List" %>

<!DOCTYPE html>


<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>语言列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        window.gvSettings.fatherMenu = '开发管理';
        window.gvSettings.menu = '开发手记';
        window.gvSettings.summary = '';

    </script>

    <script type="text/javascript">
        var edit = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + id, onClose: function () {
                    $("#tab1").bvgrid('reload');
                }
            }).open();
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

    <script>

        var btnformatter = function (val, rec) {
            var arry = [
                '<button style="cursor:pointer;" v_name="editbtn" v_title="列表项编辑按钮" onclick="edit(\'' + rec.ID + '\');">编辑</button>'
                //, '<button style="cursor:pointer;" v_name="deletebtn" v_title="列表项删除按钮" onclick="del(\'' + rec.ID + '\');">删除</button>'
            ];

            return arry.join('');
        };

        var getAjaxData = function () {
            var data = {
                action: 'data',
                CsProject: $('#cbbProject').combobox('getValue'),
                TypeName: $('#cbbType').combobox('getValue')
            };
            return data;
        };

        var myselector = function (rec) {
            var data = getAjaxData();
            data.action = 'selects_type';
            $('#cbbType').combobox('reload', '?' + $.param(data));

            var data = getAjaxData();
            $("#tab1").bvgrid('search', data);
            //$("#tab1").bvgrid('load', data);
        };

        $(function () {

            $('#cbbType').combobox({
                method: 'GET',
                valueField: 'id',
                textField: 'name',
                onSelect: function () {
                    myselector();
                },
            });

            $('#cbbProject').combobox({
                url: '?action=selects_project',
                //onSelect: function () {
                //    $('#cbbType').combobox('clear');
                //    myselector();
                //},
                onChange: function () {
                    $('#cbbType').combobox('clear');
                    myselector();
                },
                valueField: 'id',
                textField: 'name',
                onLoadSuccess: function () { //加载完成后,val[0]写死设置选中第一项
                    var val = $(this).combobox("getData");
                    $(this).combobox('clear');
                }
            });

            $("#tab1").bvgrid({ queryParams: getAjaxData(), singleSelect: false });
        });



    </script>


</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 75px">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td>项目：<input id="cbbProject" class="easyui-combobox" style="width: 200px" />
                    类：<input id="cbbType" class="easyui-combobox" style="width: 200px">
                    <%--<button id="btnSearch" class="easyui-linkbutton" iconcls="icon-search">搜索</button>--%>
                </td>
            </tr>
        </table>

    </div>
    <div data-options="region:'center',border:false">
        <table id="tab1" title="开发手记" data-options="fitColumns:true,border:false,fit:true" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'ID',checkbox:true"></th>
                    <%--<th data-options="field:'ID'">ID</th>--%>
                    <th data-options="field:'Devloper'">开发者</th>
                    <th data-options="field:'Number'">次序</th>
                    <th data-options="field:'CsProject'">项目</th>
                    <th data-options="field:'TypeName'">调用类型</th>
                    <th data-options="field:'MethodName'">调用方法</th>
                    <th data-options="field:'CreateDate'">创建时间</th>
                    <th data-options="field:'UpdateDate'">最后更新时间</th>
                    <th data-options="field:'btn',formatter:btnformatter" g_name="btn" v_title="列表项操作按钮组">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
