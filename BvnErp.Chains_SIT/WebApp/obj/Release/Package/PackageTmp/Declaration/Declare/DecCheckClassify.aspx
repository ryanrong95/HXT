<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecCheckClassify.aspx.cs" Inherits="WebApp.Declaration.Declare.DecCheckClassify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        $(function () {

            //列表初始化
            $('#tbitems').myDatagrid({
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                rownumbers: false,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });
        });
    </script>

</head>
<body  class="easyui-layout">
     <div id="data" data-options="region:'center',border:false">
        <table id="tbitems" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'GNo',align:'center'" style="width: 3%;">序号</th>
                    <th data-options="field:'HsCode',align:'left'" style="width: 7%;">商品编码</th>
                    <th data-options="field:'Name',align:'left'" style="width: 10%;">商品名称</th>
                    <th data-options="field:'Elements',align:'left'" style="width: 40%;">规格型号</th>
                    <th data-options="field:'StandardElements',align:'left'" style="width: 40%;">申报要素</th>
                    <th data-options="field:'Operate1',align:'center'" style="width: 6%;">归类一人员</th>
                    <th data-options="field:'Operate2',align:'center'" style="width: 6%;">归类二人员</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
