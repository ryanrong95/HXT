<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.TaxCategory.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>税务分类</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '税务归类';
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
                nowrap: false,
            });
        });

        function Search() {
            var TaxCode = $("#TaxCode").val();
            var TaxName = $("#TaxName").val();
            var KeyWords = $("#KeyWords").val();

            $('#datagrid').myDatagrid('search', { TaxCode: TaxCode, TaxName: TaxName, KeyWords: KeyWords });
        }

        function Reset() {
            $("#TaxCode").textbox("setValue", "");
            $("#TaxName").textbox("setValue", "");
            $("#KeyWords").textbox("setValue", "");
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">税号: </span>
                    <input class="easyui-textbox search" id="TaxCode" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <span class="lbl">名称: </span>
                    <input class="easyui-textbox search" id="TaxName" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <span class="lbl">关键词:</span>
                    <input class="easyui-textbox search" id="KeyWords" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="税务归类" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true" fitcolumns="true" data-options="border:false">
            <thead>
                <tr>
                    <th field="TaxCode" data-options="align:'center'" style="width: 100px">税号</th>
                    <th field="TaxName" data-options="align:'left'" style="width: 100px">名称</th>
                    <th field="KeyWords" data-options="align:'left'" style="width: 150px">关键词</th>
                    <th field="Description" data-options="align:'left'" style="width: 300px">描述</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
