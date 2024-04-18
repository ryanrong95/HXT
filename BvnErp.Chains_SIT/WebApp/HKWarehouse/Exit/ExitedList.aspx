<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExitedList.aspx.cs" Inherits="WebApp.HKWarehouse.Exit.ExitedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>出库通知—已出库</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '出库通知(HK)';
        gvSettings.menu = '已出库';
        gvSettings.summary = '';
    </script>--%>

    <script>
        //页面加载时
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
            var orderID = $('#OrderID').textbox('getValue');
            var VoyNo = $('#VoyNo').textbox('getValue');

            $('#datagrid').myDatagrid('search', { OrderID: orderID, VoyNo: VoyNo });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#VoyNo").textbox('setValue', "");
            $("#AdminName").textbox('setValue', "");
            Search()
        }

        function Detail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/ExitedList.aspx/ig, 'OutStock.aspx') + "?ExitNoticeID=" + rowdata.ID + "&Status=" + 4;
                window.location = url;
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Detail(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout" data-options="fit:true,border:false">
    <div id="topBar">
        <div id="search">
            <table id="table1">
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="OrderID" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="VoyNo" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="已出库" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="VoyNo" data-options="align:'center'" style="width: 250px">运输批次号</th>
                    <th field="OrderID" data-options="align:'center'" style="width: 250px">订单编号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 200px">客户名称</th>
                    <th field="PackNo" data-options="align:'center'" style="width: 130px">件数</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 130px">创建日期</th>
                    <th field="NoticeStatus" data-options="align:'center'" style="width: 130px">状态</th>
                    <th data-options="field:'btnPacking',width:130,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
