<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.UnInvoiced.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待申请开票订单</title>
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '我的跟单(XDT)';
        gvSettings.menu = '待开票';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            $('#OrderStatus').combobox({
                data: orderStatus,
            });

            //代理订单列表初始化
            $('#orders').myDatagrid({
                border: false,
                nowrap: false,
                fitColumns: true,
                fit: true,
                singleSelect: false,
                queryParams: { ClientType:'<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>', action: "data" },
                onClickRow: onClickRow,
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
            var orderStatus = $('#OrderStatus').combobox("getValue");
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var type = "";
            var clinetID = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
                clinetID = $("#Client").combobox('getValue');
            }
            if ($('#OutsideOrder').is(':checked')) {
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            $('#orders').myDatagrid('search', {
                OrderID: orderID, ClientCode: clientCode, OrderStatus: orderStatus, StartDate: startDate, EndDate: endDate, ClientType: type,
                ClientID: clinetID,
            });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#OrderStatus').combobox("setValue", null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //多单一起开票
        function BatchApplyInvoice() {
            var rows = $('#orders').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请先勾选需要申请开票的订单!');
                return;
            }

            var clients = [];
            var invoiceTypes = [];
            var invoiceTaxRates = [];
            var orderAgreementIDs = [];
            var ids = [];
            for (var i = 0; i < rows.length; i++) {
                clients.push(rows[i].ClientName);
                invoiceTypes.push(rows[i].InvoiceType);
                invoiceTaxRates.push(rows[i].InvoiceTaxRate);
                orderAgreementIDs.push(rows[i].ClientAgreementID);
                ids.push(rows[i].ID);
            }

            if (!isAllEqual(clients, invoiceTypes, orderAgreementIDs)) {
                $.messager.alert('提示', '多个订单一起开票，需要选择相同客户、相同协议的订单！');
                return;
            }

            MaskUtil.mask();
            $.post('?action=CheckOrderDeclare', {
                IDs: ids.join(','),
                ClientCode: rows[0].ClientCode
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.confirm("提示", rel.message, function (res) {
                        if (res) {
                            var url = location.pathname.replace(/List.aspx/ig, '../../Finance/Invoice/Apply.aspx') + '?IDs=' + ids;
                            window.location = url;
                        }
                    });
                } else {
                    $.messager.alert('消息', rel.message, 'info', function () {

                    });
                }
            });
        }

        //查看订单详情
        function View(id) {
            var url = location.pathname.replace(/List.aspx/ig, '../Detail.aspx') + '?ID=' + id;

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '订单详情',
                width: '80%',
                height: '80%'
            });
        }

        //申请开票
        function ApplyInvoice(index) {
            var rows = $('#orders').datagrid('getRows');
            var row = rows[index];

            MaskUtil.mask();
            $.post('?action=CheckOrderDeclare', {
                IDs: row.ID,
                ClientCode: row.ClientCode
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.confirm("提示", rel.message, function (res) {
                        if (res) {
                            var url = location.pathname.replace(/List.aspx/ig, '../../Finance/Invoice/Apply.aspx') + '?IDs=' + ids;
                            window.location = url;
                        }
                    });
                } else {
                    $.messager.alert('消息', rel.message, 'info', function () {

                    });
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ApplyInvoice(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">申请开票</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //数组元素是否全部一致
        function isAllEqual(array, types, agreementIDs) {
            var flag = true;
            if (array.length > 0) {
                flag = !array.some(function (value, index) {
                    return (value !== array[0]);
                });
            }

            if (types.length > 0) {
                flag &= !types.some(function (value, index) {
                    return (value !== types[0]);
                });
            }

            if (agreementIDs.length > 0) {
                flag &= !agreementIDs.some(function (value, index) {
                    return (value !== agreementIDs[0]);
                });
            }

            return flag;
        }

        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#orders').datagrid('validateRow', editIndex)) {
                $('#orders').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#orders').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#orders').datagrid('selectRow', editIndex);
                }
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BatchApplyInvoice()">申请开票</a>
            <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*多个订单一起开票，需要选择相同客户的订单</span>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    <br />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
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
        <table id="orders" title="待开票" data-options="border:false,nowrap:false,fitColumns:true,fit:true,singleSelect:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 5%;">全选</th>
                    <th data-options="field:'ID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 8%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'InvoiceStatus',align:'center'" style="width: 5%;">开票状态</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 8%;">开票类型</th>
                    <th data-options="field:'InvoiceTaxRate',align:'center'" style="width: 5%;">开票税率</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 5%;">订单状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">下单日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
