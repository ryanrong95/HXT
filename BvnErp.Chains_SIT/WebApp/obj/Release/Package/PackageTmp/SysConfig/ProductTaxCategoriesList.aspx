<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductTaxCategoriesList.aspx.cs" Inherits="WebApp.SysConfig.ProductTaxCategoriesList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品税务归类</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '产品税务归类';
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
            var Model = $("#Model").val();
            var Name = $("#Name").val();
            $('#datagrid').myDatagrid('search', { Name: Name, Model: Model, TaxCode: TaxCode, TaxName: TaxName });
        }

        function Reset() {
            $("#TaxCode").textbox("setValue", "");
            $("#TaxName").textbox("setValue", "");
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
                    <span class="lbl">品     名: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox search" id="Model" />
                    <span class="lbl">税务编码: </span>
                    <input class="easyui-textbox search" id="TaxCode" />
                    <br />
                    <span class="lbl">税务名称: </span>
                    <input class="easyui-textbox search" id="TaxName" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="产品税务归类" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true" fitcolumns="true" data-options="border:false">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'left'" style="width: 100px">品名</th>
                    <th field="Model" data-options="align:'left'" style="width: 100px">型号</th>
                    <th field="TaxCode" data-options="align:'center'" style="width: 100px">税务编码</th>
                    <th field="TaxName" data-options="align:'left'" style="width: 100px">税务名称</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px">创建时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
