<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WaybillTracking.aspx.cs" Inherits="WebApp.Express100.WaybillTracking" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>物流轨迹</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ExpCompany = eval('(<%=this.Model.ExpCompany%>)');

        $(function () {

            //列表初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                fit: true,
            });
            $("#com").combobox({
                data: ExpCompany
            });
        });
    </script>
</head>

<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">快递单号：</span>
                    <input class="easyui-textbox" id="num" data-options="height:26,width:180" />
                    <span class="lbl">快递公司：</span>
                    <input class="easyui-combobox" id="com" data-options="valueField:'Key',textField:'Value',editable:false, width: 180" />
                    <span class="lbl">收/寄件人号码：</span>
                    <input class="easyui-textbox" id="Phone" data-options="height:26,width:180" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="物流详细" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'WaybillTrackingTime',align:'center'" style="width: 20%;">物流时间</th>
                    <th data-options="field:'WaybillTrackingContext',align:'center'" style="width: 10%;">物流详细</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
