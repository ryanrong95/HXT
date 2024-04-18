<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.MyApplies.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <UC:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '我的申请';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        //页面加载时
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });
    </script>
</head>
<body class="easyui-layout">
    <div title="我的申请" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" data-options="fit:true,scrollbarSize:0">
            <thead>
                <tr>
                    <th field="ID" data-options="align:'center'" style="width: 100px">ID</th>
                    <th field="TypeName"  data-options="align:'center'" style="width: 100px">申请类型</th>
                    <th field="MainID"  data-options="align:'center'" style="width: 100px">关联ID</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px;" >申请人</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;" >申请状态</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 150px;" >创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 150px;" >更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
