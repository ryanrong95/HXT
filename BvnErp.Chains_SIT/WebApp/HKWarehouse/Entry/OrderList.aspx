<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.OrderList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>香港库房查询订单状态</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        //gvSettings.fatherMenu = '入库通知(HK)';
        //gvSettings.menu = '报关订单';
        //gvSettings.summary = '';
    </script>
    <script>
        var OrderStatus = eval('(<%=this.Model.OrderStatus%>)');

        $(function () {
            $('#OrderStatus').combobox({
                data: OrderStatus,
            });

            Search();
        });

        //查询
        function Search() {
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var OrderStatus = $('#OrderStatus').combobox('getValue');

            $('#orderList-datagrid').myDatagrid({
                 actionName: 'OrderData',
                queryParams: {
                    ClientCode: ClientCode,
                    ClientName: ClientName,
                    OrderID: OrderID,
                    OrderStatus: OrderStatus,
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
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#OrderStatus').combobox('select', "");
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar-orderList-datagrid">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 22px;">入仓号: </span>
                    <input class="easyui-textbox search" data-options="" id="ClientCode" />
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox search" data-options="" id="ClientName" />
                </li>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox search" data-options="" id="OrderID" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" name="OrderStatus" data-options="valueField:'TypeValue',textField:'TypeText',required:false,editable:false" style="width: 248px" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orderList-datagrid" class="easyui-datagrid" title="报关订单" data-options="
            border:false,
            nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar-orderList-datagrid'">
            <thead>
                <tr>
                    <th field="OrderID" data-options="align:'center'" style="width: 35px">订单编号</th>
                    <th field="ClientCode" data-options="align:'center'" style="width: 25px">入仓号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 45px">客户名称</th>
                    <th field="OrderStatus" data-options="align:'center'" style="width: 20px">订单状态</th>
                    <th field="OrderConsigneeType" data-options="align:'center'" style="width: 20px">交货方式</th>
                    <th field="ClientSupplierName" data-options="align:'left'" style="width: 45px">供应商</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
