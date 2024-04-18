<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductCategoriesList.aspx.cs" Inherits="WebApp.SysConfig.ProductCategoriesList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品海关归类</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '产品海关归类';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript"> 
        $(function () {
            $('#datagrid').myDatagrid({
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
                idField: 'ID',
                nowrap: false,
            });
        });


        function Search() {
            var HSCode = $("#HSCode").val();
            var Model = $("#Model").val();
            var Name = $("#Name").val();

            $('#datagrid').myDatagrid('search', { HSCode: HSCode, Model: Model, Name: Name });
        }

        function Reset() {
            $("#HSCode").textbox("setValue", "");
            $("#Model").textbox("setValue", "");
            $("#Name").textbox("setValue", "");
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">品名: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox search" id="Model" />
                    <span class="lbl">海关编码:</span>
                    <input class="easyui-textbox search" id="HSCode" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="产品海关归类" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true" fitcolumns="true" data-options="border:false">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'left'" style="width: 100px">品名</th>
                    <th field="Model" data-options="align:'left'" style="width: 100px">型号</th>
                    <th field="HSCode" data-options="align:'center'" style="width: 100px">海关编码</th>
                    <th field="Elements" data-options="align:'left'" style="width: 200px">申报要素</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px">创建时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
