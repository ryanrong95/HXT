<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnExited.aspx.cs" Inherits="WebApp.ExitOrder.Exit.UnExited" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待出库-出库单</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
  <%--  <script>
        gvSettings.fatherMenu = '出库单';
        gvSettings.menu = '待出库';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var exitType = eval('(<%=this.Model.ExitType%>)');

        //页面加载时
        $(function () {
            $('#ExitType').combobox({
                data: exitType
            });

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
            var entryNumber = $('#EntryNumber').textbox('getValue');
            var exitType = $('#ExitType').combobox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber, ExitType: exitType });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            $("#ExitType").combobox('setValue', "");
            Search();
        }

        function Printing(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                switch (rowdata.ExitType) {
                    case "送货上门":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'DeliveryBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                    case "快递":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'ExpressBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                    case "自提":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'LadingBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                        window.location = url;
                        break;
                }
            }
        }


        function Operation(val, row, index) {
            if (row.ExitType == "自提") {
                var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Printing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">打印提货单</span>' +
                    '<span class="l-btn-icon icon-print">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {
                var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Printing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">打印送货单</span>' +
                    '<span class="l-btn-icon icon-print">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="OrderID" />
                    </td>
                    <td class="lbl">客户编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryNumber"/>
                    </td>
                    <td class="lbl">送货类型: </td>
                    <td>
                        <input class="easyui-combobox search" id="ExitType" data-options="valueField:'Key',textField:'Value'" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="待出库" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 50px">订单编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 50px">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px">客户名称</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 50px">件数</th>
                    <th data-options="field:'ExitType',align:'center'" style="width: 50px">送货类型</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 50px">制单员</th>
                    <th data-options="field:'NoticeStatus',align:'center'" style="width: 50px">状态</th>
                    <th data-options="field:'btnPacking',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
