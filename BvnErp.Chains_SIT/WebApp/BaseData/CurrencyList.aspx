<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CurrencyList.aspx.cs" Inherits="WebApp.BaseData.CustomsRateList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>币种</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '基础数据(XDT)';
        gvSettings.menu = '币种';
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
                }
            });
        });

        function Search() {
            var Code = $("#Code").val();
            var Name = $("#Name").val();
            var EnglishName = $("#EnglishName").val();

            $('#datagrid').myDatagrid('search', { Code: Code, Name: Name, EnglishName: EnglishName });
        }

        function Reset() {
            $("#Code").textbox("setValue", "");
            $("#Name").textbox("setValue", "");
            $("#EnglishName").textbox("setValue", "");
            Search();
        }

        function Add() {
            var url = location.pathname.replace(/CurrencyList.aspx/ig, 'CurrencyInfo.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增海关汇率',
                url: url,
                width: '450px',
                height: '200px',
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
                    <span class="lbl">币种代码:</span>
                    <input class="easyui-textbox search" id="Code" />
                    <span class="lbl">中文名称: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <%--                    <span class="lbl">英文名称: </span>
                    <input class="easyui-textbox search" id="EnglishName" />--%>

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="币种" class="easyui-datagrid" data-options="fitColumns:true,fit:true,border:false,scrollbarSize:0" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true">
            <thead>
                <tr>
                    <th field="Code" data-options="align:'center'" style="width: 150px">币种代码</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">中文名称</th>
                    <th field="EnglishName" data-options="align:'center'" style="width: 200px">英文名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
