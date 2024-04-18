<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminList.aspx.cs" Inherits="WebApp.Order.Query.AdminList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单列表</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '订单管理(XDT)';
        gvSettings.menu = '订单列表';
        gvSettings.summary = '订单查询查看';
    </script>--%>
    <style type="text/css">
        .rowlink {
            color: blue;
            cursor: pointer;
            text-decoration: underline
        }
    </style>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            $('#OrderStatus').combobox({
                data: orderStatus
            });

            var payExchangeStatus = eval('(<%= this.Model.PayExchangeStatus%>)')
            $('#PayExchangeStatus').combobox({
                data: payExchangeStatus
            });

            //订单列表初始化
            $('#orders').myDatagrid({
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
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var orderStatus = $('#OrderStatus').combobox('getValue');
            var payExChangeStatus = $('#PayExchangeStatus').combobox('getValue');
            var parm = {
                OrderID: orderID,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate,
                OrderStatus: orderStatus,
                PayExChangeStatus: payExChangeStatus
            };
            $('#orders').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OrderStatus').combobox('setValue', null);
            $('#PayExchangeStatus').combobox('setValue', null);
            Search();
        }

        function ViewMianOrderBill(val, row, index) {
            return "<a class=\"rowlink\" onclick=\"ViewMainOrderBill('" + row.MainOrderID + "')\" >" + row.MainOrderID + "</a>";
        }

        function ViewMainOrderBill(ID) {
            var url = location.pathname.replace(/AdminList.aspx/ig, '../MainOrder/Tab.aspx?ID=' + ID + '&From=AdminQuery');
            window.location = url;
        }

        //查看订单信息
        function View(ID) {
            var url = location.pathname.replace(/AdminList.aspx/ig, '../MainOrder/Tab.aspx?ID=' + ID + '&From=AdminQuery');
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                 
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
                    <br />
                    <span class="lbl">付汇状态: </span>
                    <input class="easyui-combobox" id="PayExchangeStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
                
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="orders" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'" title="订单列表">
            <thead>
                <tr>
                    <th data-options="field:'MainOrderID',align:'left',formatter:ViewMianOrderBill" style="width: 8%;">主订单编号</th>
                    <th data-options="field:'ID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 7%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 8%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">下单日期</th>
                    <th data-options="field:'SpecialType',align:'center'" style="width: 7%;">特殊类型</th>
                    <th data-options="field:'InvoiceStatus',align:'center'" style="width: 7%;">开票状态</th>
                    <th data-options="field:'PayExchangeStatus',align:'center'" style="width: 7%;">付汇状态</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 7%;">订单状态</th>
                   <%-- <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
