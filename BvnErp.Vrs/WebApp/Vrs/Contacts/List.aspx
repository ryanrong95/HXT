<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Vrs.Contacts.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:easyui runat="server" />
    <style>
        .ven_type {
            height: 40px;
        }
            .ven_type button {
                margin-top: 8px;
            }
    </style>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '供应商管理.';
        gvSettings.menu = '联系人管理';
        gvSettings.summary = '';

    </script>
    <script>

        var edit = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?id=" + id, onClose: function () {
                    $("#table1").bvgrid('reload');
                }
            }).open();
        };
        var del = function (id) {
            $.messager.confirm('删除提示', '确定要删除吗?', function (r) {
                if (r) {
                    $.post("?action=del", { id: id }, function (data) {
                        $.messager.alert('提示', '删除成功');
                        $.Paging();
                    })
                }
            });
        }

        function add() {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx'),
                onClose: function () {
                    $("#table1").bvgrid('reload');
                }
            }).open();
        }
        var getAjaxData = function () {
            var data = {
                action: 'data',

            };
            return data;
        };
        $(function () {
            $('#table1').bvgrid({ queryParams: getAjaxData() });
        });

        var btnformatter = function (val, rec) {
            var arry = [];
            arry.push('<button style="cursor:pointer;" onclick="edit(\'' + rec.ID + '\');">编辑</button>'
                , '<button style="cursor:pointer;" onclick="del(\'' + rec.ID + '\');">删除</button>')

            return arry.join('|');
        };

    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="联系人管理">
        <div class="ven_type">
            <button onclick="add()">添加</button>
        </div>
        <%--<input type="button" name="txtadd" value="添加"  />--%>
        <table id="table1" class="easyui-datagrid">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:100,align:'center'">名字</th>
                    <th data-options="field:'Sex',width:50,align:'center'">性别</th>
                    <th data-options="field:'Birthday',width:90,align:'center'">生日</th>
                    <th data-options="field:'Tel',width:100,align:'center'">电话</th>
                    <th data-options="field:'Mobile',width:100,align:'center'">手机号</th>
                    <th data-options="field:'Email',width:150,align:'center'">邮箱</th>
                    <th data-options="field:'CompanyName',width:160,align:'center'">公司</th>
                    <th data-options="field:'Status',width:80,align:'center'">状态</th>
                    <th data-options="field:'Job',width:80,align:'center'">职务</th>
                    <th data-options="field:'Btns',formatter:btnformatter,align:'center',width:200">操作</th>                    
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
