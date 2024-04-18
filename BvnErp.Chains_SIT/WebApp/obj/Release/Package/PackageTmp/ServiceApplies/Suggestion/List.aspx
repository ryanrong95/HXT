<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ServiceApplies.Suggestion.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>投诉与建议</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '注册申请(XDT)';
        gvSettings.menu = '投诉与建议';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                toolbar: '#topBar',
                rownumbers: true,
                singleSelect: false,
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
                }
            });
        });

        //查询
        function Search() {
            var Summary = $('#Summary').textbox('getValue');
            var parm = {
                Summary: Summary,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Summary').textbox('setValue', null);
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">摘要:</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" data-options="height:26,width:200" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="投诉建议列表" >
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 60px;">姓名</th>
                    <th data-options="field:'Phone',align:'center'" style="width: 100px;">电话</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">创建日期</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 150px;">摘要</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
