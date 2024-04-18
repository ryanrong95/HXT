<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnExited.aspx.cs" Inherits="WebApp.SZWareHouse.Exit.UnExited" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待出库-出库通知</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '出库通知(SZ)';
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
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                singleSelect: true,
                pageSize: 20,
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

        function Search() {
            var exitNoticeID = $('#ExitNoticeID').textbox('getValue');
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            var exitType = $('#ExitType').combobox('getValue');
            var clientName = $('#ClientName').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber, ExitType: exitType, ExitNoticeID: exitNoticeID, ClientName: clientName, });
        }

        function Reset() {
            $("#ExitNoticeID").textbox('setValue', "");
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            $("#ExitType").combobox('setValue', "");
            $("#ClientName").textbox('setValue', "");
            Search();
        }

        function Printing(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                switch (rowdata.ExitType) {
                    case "送货上门":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'DeliveryBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 1;
                        window.location = url;
                        break;
                    case "快递":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'ExpressBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 1;
                        window.location = url;
                        break;
                    case "自提":
                        var url = location.pathname.replace(/UnExited.aspx/ig, 'LadingBill.aspx') + "?ExitNoticeID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID + "&ExitStatus=" + 1;
                        window.location = url;
                        break;
                }
            }
        }


        function Operation(val, row, index) {
            if (row.ExitType == "自提") {
                var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Printing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">打印</span>' +
                    '<span class="l-btn-icon icon-print">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {
                var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Printing(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">打印</span>' +
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
                    <td class="lbl">单号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ExitNoticeID" />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">送货类型: </td>
                    <td>
                        <input class="easyui-combobox search" id="ExitType" data-options="valueField:'Key',textField:'Value',editable:false," />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">订单编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="OrderID" />
                    </td>
                    <td style="padding-left: 5px;">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">客户编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryNumber" />
                    </td>
                    <td class="lbl" style="padding-left: 5px;">客户名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="ClientName" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="待出库" data-options="border:false,fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true,pageSize:20,nowrap:false," class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center'" style="width: 50px">单号</th>
                    <th data-options="field:'ExitType',align:'center'" style="width: 30px">送货类型</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 30px">件数</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 50px">订单编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 30px">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px">客户名称</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 30px">制单人</th>
                    <%--<th data-options="field:'NoticeStatus',align:'center'" style="width: 30px">出库状态</th>--%>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 30px">制单日期</th>
                    <th data-options="field:'IsPrint',align:'center'" style="width: 30px">打印状态</th>
                    <th data-options="field:'btnPacking',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
