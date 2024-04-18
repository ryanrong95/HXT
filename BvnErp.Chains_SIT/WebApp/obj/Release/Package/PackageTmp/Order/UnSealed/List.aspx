<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.UnSealed.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待封箱订单</title>
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '我的跟单(XDT)';
        gvSettings.menu = '待封箱';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //订单列表初始化
            $('#orders').myDatagrid({ queryParams: { ClientType: '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>', action: "data" }, });

            $("#AllOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#OutsideOrder").prop("checked", false);
                    $("#InsideOrder").prop("checked", false);
                    Search();
                }
            });
            $("#OutsideOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#AllOrder").prop("checked", false);
                    $("#InsideOrder").prop("checked", false);
                    Search();
                }
            });
            $("#InsideOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#AllOrder").prop("checked", false);
                    $("#OutsideOrder").prop("checked", false);
                    Search();
                }
            });
            //初始化内单客户下拉框
            var clients = eval('(<%=this.Model.Clients%>)');
            $('#Client').combobox({
                valueField: 'ID',
                textField: 'Name',
                data: clients,
                onSelect: function () {
                    if ($('#InsideOrder').is(':checked')) {
                        Search();
                    }
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var type = "";
            var clinetID = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
                clinetID = $("#Client").combobox('getValue');
            }
            if($('#OutsideOrder').is(':checked')){
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            var parm = {
                OrderID: orderID,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate,
                ClientType: type,
                ClientID:clinetID,
            };
            $('#orders').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看到货
        function View(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Display.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '待封箱信息',
                width: '80%',
                height: '80%'
            });
        }

        //订单退回
        function ReturnOrder(id) {
            var url = location.pathname.replace(/List.aspx/ig, '../Reason.aspx') + '?ID=' + id + '&Source=Return';

            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '订单退回原因',
                width: '400px',
                height: '260px',
                onClose: function () {
                    $('#orders').datagrid('reload');
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">详情</span>' +
                        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick = "ReturnOrder(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">退回</span>' +
                    '<span class="l-btn-icon icon-undo">&nbsp;</span>' +
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
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <br />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    <br />
                    <span class="lbl"></span>
                    <input type="checkbox" name="Order" value="0" id="AllOrder" title="全部订单" class="checkbox checkboxlist" /><label for="AllOrder" style="margin-right: 20px">全部订单</label>
                    <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="仅限B类" class="checkbox checkboxlist" checked="checked" /><label for="OutsideOrder" style="margin-right: 20px">仅限B类</label>
                    <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="checkbox checkboxlist" /><label for="InsideOrder">A类</label>
                    <input id="Client" class="easyui-combobox" style="width: 280px;" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="待封箱" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 6%;">币种</th>
                    <th data-options="field:'SupplierName',align:'left'" style="width: 14%;">供应商</th>
                    <th data-options="field:'HKWay',align:'center'" style="width: 10%;">香港交货方式</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">下单日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 14%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
