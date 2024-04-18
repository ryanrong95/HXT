<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContainerList.aspx.cs" Inherits="WebApp.BaseData.ContainerList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>       
        gvSettings.fatherMenu = '基础数据(XDT)';
        gvSettings.menu = '集装箱规格';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //集装箱规格列表初始化
            $('#containers').myDatagrid({
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

        //查询
        function Search() {
            var code = $('#Code').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            $('#containers').myDatagrid('search', { Code: code, Name: name });
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', null);
            $('#Name').textbox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">规格代码:</span>
                    <input class="easyui-textbox" id="Code" data-options="validType:'length[1,10]'" />
                    <span class="lbl">规格名称: </span>
                    <input class="easyui-textbox" id="Name" data-options="validType:'length[1,150]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="containers" title="集装箱规格" data-options="fitColumns:true,fit:true,border:false,scrollbarSize:0" toolbar="#topBar"
            singleselect="true">
            <thead>
                <tr>
                    <th data-options="field:'Code',align:'center'" style="width: 100px;">规格代码</th>
                    <th data-options="field:'Name',align:'center'" style="width: 100px;">规格名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
