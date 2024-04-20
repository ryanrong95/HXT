<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Bom.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server"></uc:EasyUI>
</head>
<body>

    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="Excel显示">
        <table id="table1" class="easyui-datagrid">
            <thead>
                <tr>
                    <th data-options="field:'ID',width:100,align:'center'">Bom单号</th> 
                    <th data-options="field:'CreateDate',width:100,align:'center'">上传时间</th>  
                    <th data-options="field:'Contact',width:150,align:'center'">联系人名称</th>
                    <th data-options="field:'Email',width:100,align:'center'">邮箱</th>     
                    <th data-options="field:'Btns',formatter:btnformatter,align:'center',width:100">操作</th>                     
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
  <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'Bom单管理.';
        gvSettings.menu = 'Bom单';
        gvSettings.summary = '';

    </script>
<script>
    var edit = function (id) {
        top.$.myWindow({
            url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + id,
            onClose: function () {
                $("#table1").bvgrid('reload');
            }
        }).open();
    };
    var getAjaxData = function () {
        var data = {
            action: 'data',
        };
        return data;
    };
    $(function () {
        $('#table1').bvgrid({ queryParams: getAjaxData() });
    });

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
    var btnformatter = function (val, rec) {
        var arry = [];
        arry.push(
            '<a href="' + rec.Uri + '" target="_blank">下载</a>'
            //,'<button style="cursor:pointer;" onclick="del(\'' + rec.ID + '\');">删除</button>'
             )
        return arry.join('|');
    };
</script>
