<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PackingList.aspx.cs" Inherits="WebApp.HKWarehouse.Report.PackingList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>装箱单</title>
    <uc:EasyUI runat="server" />
    <%-- <script>
        gvSettings.fatherMenu = '报表';
        gvSettings.menu = '装箱单';
        gvSettings.summary = '';
    </script>--%>

    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {
            $('#PackingDate').datebox('setValue', formatterDate(new Date()));

            var PackingDate = $('#PackingDate').datebox('getValue');
            var BoxIndex = $('#BoxIndex').textbox('getValue');
            $('#packedBoxList-datagrid').myDatagrid({
              
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
                 actionName: 'PackedBoxData',
                  queryParams: {
                    PackingDate: PackingDate,
                    BoxIndex: BoxIndex,
                },
            });

            //型号明细弹框初始化
            $('#boxDetail-dialog').dialog({
                buttons: [{
                    text: '关闭',
                    width: '52px',
                    handler: function () {
                        $('#boxDetail-dialog').dialog('close');
                    }
                }]
            });
        });

        //显示型号明细
        function ShowPackedBoxDetail(packingID) {
            $('#boxDetail-datagrid').datagrid({
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                fit:true,
                singleSelect:false,
                url: "?action=BoxDetail&PackingID=" + packingID,
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
                onLoadSuccess: function (data) {
                    var trs = $("#boxDetail-datagrid").prev().find("tr");
                    for (var i = 0; i < trs.length; i++) {
                        $(trs[i]).find("td:nth-child(1)").find("div").width(160);
                        $(trs[i]).find("td:nth-child(2)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(3)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(4)").find("div").width(70);
                        $(trs[i]).find("td:nth-child(5)").find("div").width(70);
                        $(trs[i]).find("td:nth-child(6)").find("div").width(110);
                    }
                },
            });

            $('#boxDetail-dialog').dialog('open');
        }

        function PackedBoxListDatagridOperation(val, row, index) {
            var buttons = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="ShowPackedBoxDetail(\'' + row.PackingID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">明细</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //查询
        function Search() {
            var PackingDate = $('#PackingDate').datebox('getValue');
            var BoxIndex = $('#BoxIndex').textbox('getValue');

            $('#packedBoxList-datagrid').myDatagrid({
                 actionName: 'PackedBoxData',
                queryParams: {
                    PackingDate: PackingDate,
                    BoxIndex: BoxIndex,
                },
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
        }

        //重置查询条件
        function Reset() {
            $('#PackingDate').datebox('setValue', formatterDate(new Date()));
            $('#BoxIndex').textbox('setValue', null);
            Search();
        }

        //显示当前日期
        function formatterDate(date) {
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0" + (date.getMonth() + 1);
            return date.getFullYear() + '-' + month + '-' + day;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar-packedBoxList-datagrid">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">装箱时间: </span>
                    <input class="easyui-datebox search" data-options="" id="PackingDate" />
                    <span class="lbl">箱号: </span>
                    <input class="easyui-textbox search" data-options="" id="BoxIndex" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="packedBoxList-datagrid" class="easyui-datagrid" title="装箱单" data-options="
            border:false,
            nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar-packedBoxList-datagrid'">
            <thead>
                <tr>
                    <th field="PackingDate" data-options="align:'center'" style="width: 10%">装箱日期</th>
                    <th field="BoxIndex" data-options="align:'left'" style="width: 10%">箱号</th>
                    <th field="StockCode" data-options="align:'center'" style="width: 10%">库位号</th>
                    <th field="OrderID" data-options="align:'left'" style="width: 10%">订单编号</th>
                    <th field="ClientCode" data-options="align:'center'" style="width: 10%">客户编号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 10%">客户名称</th>
                    <th field="SealedDate" data-options="align:'center'" style="width: 10%">封箱时间</th>
                    <th field="SealedName" data-options="align:'center'" style="width: 10%">封箱人</th>
                    <th data-options="field:'btn',width:'10%',formatter:PackedBoxListDatagridOperation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="boxDetail-dialog" class="easyui-dialog" title="明细" style="width: 800px; height: 450px;"
        data-options="iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <table id="boxDetail-datagrid" data-options="
            nowrap:false,
            border:false,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'Model',align:'center'" style="width: 118px;">型号</th>
                    <th data-options="field:'Name',align:'center'" style="width: 118px;">品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 112px;">品牌</th>
                    <th data-options="field:'Quantity',align:'left'" style="width: 40px;">数量</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 40px;">产地</th>
                    <th data-options="field:'GrossWeight',align:'left'" style="width: 90px;">毛重</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
