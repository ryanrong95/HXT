<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.UnApproved.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待审批</title>
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '库房费用';
        gvSettings.menu = '待审批';
        gvSettings.summary = '';
    </script>--%>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var WarehousePremiumsStatus = eval('(<%=this.Model.WarehousePremiumsStatus%>)');
        //页面加载
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
                toolbar: '#topBar',
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
            //初始化费用状态
            $('#Status').combobox({
                data: WarehousePremiumsStatus,
            })

        });
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var Status = $('#Status').combobox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: OrderID, Status: Status });
        }
        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $('#Status').combobox('setValue', "");
            Search();
        }
        //审批
        function Approval(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Approval.aspx') + '?FeeID=' + ID;
            self.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '审批费用',
                width: '800px',
                height: '600px',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }
        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnApproval" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Approval(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">审批</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span></span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="OrderID" />
                    </td>
                    <td class="lbl">费用名称：</td>
                    <td>
                        <input class="easyui-combobox" id="Status" name="Status"
                            data-options="height:26,width:180,valueField:'Key',textField:'Value',limitToList:true," />
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
        <%--费用列表--%>
        <table id="datagrid" class="mygrid" title="库房费用列表" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar',
            queryParams:{ action:'data' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px">订单编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px">客户名称</th>
                    <%--<th data-options="field:'Payer',align:'center'" style="width: 20px">付款人</th>--%>
                    <th data-options="field:'Type',align:'center'" style="width: 100px">费用名称</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 50px">单价</th>
                    <th data-options="field:'Count',align:'center'" style="width:50px">数量</th>
                    <th data-options="field:'ApprovalPrice',align:'center'" style="width: 100px">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width:100px">币种</th>
                    <th data-options="field:'Creater',align:'center'" style="width: 100px">录入人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 100px">录入时间</th>
                    <th data-options="field:'btnRemove',width:20,formatter:Operation,align:'center'" style="width: 100px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
