<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageList.aspx.cs" Inherits="WebApp.HKWarehouse.Finance.StorageList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库存(HK)</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '库存(HK)';
        gvSettings.menu = '库存产品数据';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                 nowrap: false
            });
            $('#datagrid_Input').myDatagrid({
                 nowrap: false
            });
            $('#datagrid_Output').myDatagrid({
                 nowrap: false
            });
        });

        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: OrderID, ClientCode: ClientCode });
            $('#datagrid_Input').myDatagrid('search', { OrderID: OrderID, ClientCode: ClientCode });
            $('#datagrid_Output').myDatagrid('search', { OrderID: OrderID, ClientCode: ClientCode });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#ClientCode").textbox('setValue', "");
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false,collapsible:false" title="香港库存产品" style="height: 70px">
        <table style="margin: 5px 0 5px 0">
            <tr>
                <td class="lbl" style="padding-left: 0px">订单编号：</td>
                <td>
                    <input class="easyui-textbox" data-options="height:26,width:180" id="OrderID" />
                </td>
                <td class="lbl">客户编号：</td>
                <td>
                    <input class="easyui-textbox" data-options="height:26,width:180" id="ClientCode" />
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
    <div id="tt" class="easyui-tabs" data-options="region:'center',border:false">
        <div title="在库产品数据">
            <table id="datagrid" title="" data-options="
                fitColumns:true,
                fit:true,
                border:false,
                scrollbarSize:0,
                queryParams:{ action: 'data' }">
                <thead>
                    <tr>
                        <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                        <th data-options="field:'ClientCode',align:'center'" style="width: 10%">客户编号</th>
                        <th data-options="field:'Model',align:'left',width:'10%'">型号</th>
                        <th data-options="field:'Name',align:'left',width:'10%'">报关品名</th>
                        <th data-options="field:'Manufacturer',align:'left',width:'10%'">品牌</th>
                        <th data-options="field:'Quantity',align:'center',width:'5%'">数量</th>
                        <th data-options="field:'UnitPrice',align:'center',width:'5%'">单价</th>
                        <th data-options="field:'Origin',align:'center',width:'10%'">产地</th>
                        <th data-options="field:'Currency',align:'center',width:'5%'">币种</th>
                        <th data-options="field:'BoxIndex',align:'center',width:'5%'">箱号</th>
                        <th data-options="field:'StockCode',align:'center',width:'10%'">库位号</th>
                        <th data-options="field:'CreateDate',align:'center',width:'5%'">入库时间</th>
                        <th data-options="field:'UpdateDate',align:'center',width:'5%'">更新时间</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div title="入库产品数据">
            <table id="datagrid_Input" title="" data-options="
                fitColumns:true,
                fit:true,
                border:false,
                scrollbarSize:0,
                queryParams:{ action: 'dataInput' }">
                <thead>
                    <tr>
                        <th data-options="field:'OrderID',align:'left'" style="width:10%">订单编号</th>
                        <th data-options="field:'ClientCode',align:'center'" style="width:10%">客户编号</th>
                        <th data-options="field:'Model',align:'left',width:'10%'">型号</th>
                        <th data-options="field:'Name',align:'left',width:'10%'">报关品名</th>
                        <th data-options="field:'Manufacturer',align:'left',width:'10%'">品牌</th>
                        <th data-options="field:'Quantity',align:'center',width:'10%'">数量</th>
                        <th data-options="field:'UnitPrice',align:'center',width:'10%'">单价</th>
                        <th data-options="field:'Origin',align:'center',width:'10%'">产地</th>
                        <th data-options="field:'Currency',align:'center',width:'5%'">币种</th>
                        <th data-options="field:'BoxIndex',align:'center',width:'5%'">箱号</th>
                        <th data-options="field:'CreateDate',align:'center',width:'10%'">入库时间</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div title="出库产品数据">
            <table id="datagrid_Output" title="" data-options="
                fitColumns:true,
                fit:true,
                border:false,
                scrollbarSize:0,
                queryParams:{ action: 'dataOutput' }">
                <thead>
                    <tr>
                        <th data-options="field:'OrderID',align:'left',width:'10%'">订单编号</th>
                        <th data-options="field:'ClientCode',align:'center',width:'10%'">客户编号</th>
                        <th data-options="field:'Model',align:'left',width:'10%'">型号</th>
                        <th data-options="field:'CustomsName',align:'left',width:'10%'">报关品名</th>
                        <th data-options="field:'Manufacturer',align:'left',width:'10%'">品牌</th>
                        <th data-options="field:'Quantity',align:'center',width:'5%'">数量</th>
                        <th data-options="field:'UnitPrice',align:'center',width:'5%'">单价</th>
                        <th data-options="field:'Origin',align:'center',width:'10%'">产地</th>
                        <th data-options="field:'Currency',align:'center',width:'5%'">币种</th>
                        <th data-options="field:'BoxIndex',align:'center',width:'5%'">箱号</th>
                        <th data-options="field:'CreateDate',align:'center',width:'10%'">创建时间</th>
                        <th data-options="field:'UpdateDate',align:'center',width:'10%'">出库时间</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</body>
</html>
