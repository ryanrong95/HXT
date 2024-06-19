<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WrapTypeList.aspx.cs" Inherits="WebApp.BaseData.WrapTypeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>包装信息</title>
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>       
        gvSettings.fatherMenu = '基础数据(XDT)';
        gvSettings.menu = '包装类型';
        gvSettings.summary = ''
    </script>--%>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //数据初始化
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
                }
            });
        });

        //查询
        function Search() {
            var code = $('#Code').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            $('#datagrid').myDatagrid('search', { Code: code, Name: name });
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', "");
            $('#Name').textbox('setValue', "");
            Search();
        }
        function Add() {
            var url = location.pathname.replace(/WrapTypeList.aspx/ig, 'WrapTypeInfo.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增包装类型',
                url: url,
                width: '450px',
                height: '180px',
                onClose: function () {
                    Search();
                }
            });
        }


    </script>
</head>
<body class="easyui-layout">

    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">包装代码:</span>
                    <input class="easyui-textbox search" id="Code" data-options="validType:'length[1,10]'" />
                    <span class="lbl">包装名称: </span>
                    <input class="easyui-textbox search" id="Name" data-options="validType:'length[1,150]'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="包装类型" class="easyui-datagrid" data-options="fitColumns:true,fit:true,border:false,scrollbarSize:0" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true">
            <thead>
                <tr>
                    <th field="Code" data-options="align:'center'" style="width: 200px">包装代码</th>
                    <th field="Name" data-options="align:'center'" style="width: 200px">包装名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>