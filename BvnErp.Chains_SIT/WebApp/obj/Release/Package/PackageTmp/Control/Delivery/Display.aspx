<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Control.Delivery.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>深圳出库通知</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var order = eval('(<%=this.Model.OrderData%>)');

        $(function () {
            //初始化订单基本信息
            document.getElementById('OrderID').innerText = order['ID'];
            document.getElementById('CreateDate').innerText = order['CreateDate'];

            //分拣信息列表初始化
            window.grid = $('#sortings').myDatagrid({
                actionName: 'dataSortings',
                nowrap: false,
                singleSelect: false,
                pagination: false,
                fitcolumns: true,
                fit: false,
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
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        grid.myDatagrid('appendRow', { BoxIndex: '<div style="text-align:center;color:#0081d5">香港库房尚未出库</div>' }).datagrid('mergeCells', { index: 0, field: 'BoxIndex', colspan: 7 });
                        $('.datagrid-body').find("input[type='checkbox']")[0].hidden = true;
                        return;
                    }

                    var mark = 1;
                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].Quantity == data.rows[i].DeliveriedQuantity) {
                            $('.datagrid-btable').find("input[type='checkbox']")[i].disabled = 'disabled';
                        }

                        //合并箱号
                        if (i > 0) {
                            if (data.rows[i]['BoxIndex'] == data.rows[i - 1]['BoxIndex']) {
                                mark += 1;
                                $("#sortings").datagrid('mergeCells', {
                                    index: i + 1 - mark,
                                    field: 'BoxIndex',
                                    rowspan: mark
                                });
                            }
                            else {
                                mark = 1;
                            }
                        }
                    }
                },
                onSelect: function (index, row) {
                    if (row.Quantity == row.DeliveriedQuantity) {
                        $('#sortings').datagrid('unselectRow', index);

                        if (IsCheckAll()) {
                            $("input[type='checkbox']")[0].checked = true;
                        }
                    }
                },
                onCheck: function (index, row) {
                    if (IsCheckAll()) {
                        $("input[type='checkbox']")[0].checked = true;
                    }
                },
                onCheckAll: function (rows) {
                    for (var index = 0; index < rows.length; index++) {
                        var row = rows[index];
                        if (row.Quantity == row.DeliveriedQuantity) {
                            $('#sortings').datagrid('unselectRow', index);
                        }
                    }
                    $("input[type='checkbox']")[0].checked = true;
                },
            });

            //送货信息列表初始化
            $('#deliveries').myDatagrid({
                actionName: 'dataDeliveries',
                nowrap: false,
                pagination: false,
                fitcolumns: true,
                fit: false,
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

            //欠款情况列表初始化
            $('#debts').myDatagrid({
                actionName: 'DebtsData',
                nowrap: false,
                pagination: false,
                fitcolumns: true,
                fit: false,
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

            $.post('?action=isCanDelivery', { OrderID: order['ID'], ID: order['ClientID'] }, function (data) {
                var Result = JSON.parse(data);
                if (!Result.success) {
                    $("#btnAdd").show();
                    $("#showMsg").hide();
                }
                if (!Result.AdvanceMoney) {
                    $("#showOverDuePaymentMsg").hide();
                }
                else {
                    $("#btnAdd").hide();
                }
            });
        });

        //新增送货信息
        function AddDeliveryInfo() {
            var rows = $('#sortings').datagrid('getChecked');
            if (rows.length == 0) {
                $.messager.alert('提示', '请选择需要送货的产品！');
                return;
            }

            var ids = [];
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].DeliveriedQuantity >= rows[i].Quantity) {
                    $.messager.alert('提示', '选择的产品已全部送货！');
                    return;
                }
                ids.push(rows[i].ID);
            }

            var url = location.pathname.replace(/Display.aspx/ig, 'Edit.aspx?OrderID=' + order['ID'] + '&IDs=' + ids.join());
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '送货单',
                width: 1000,
                height: 622,
                onClose: function () {
                    $('#sortings').datagrid('reload');
                    $('#deliveries').datagrid('reload');
                }
            });
        }

        //删除送货信息
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除该送货信息！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('删除', result.message);
                            $('#sortings').datagrid('reload');
                            $('#deliveries').datagrid('reload');
                        } else {
                            $.messager.alert('删除', result.message);
                        }
                    })
                }
            });
        }

        //返回
        function Return() {
            var url = location.pathname.replace(/Display.aspx/ig, 'List.aspx');
            window.location = url;
        }

        //是否勾选全选框
        function IsCheckAll() {
            var isCheckAll = true;
            var rows = $('#sortings').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].Quantity != rows[i].DeliveriedQuantity) {
                    if (!$('.datagrid-btable').find("input[type='checkbox']")[i].checked) {
                        isCheckAll = false;
                        break;
                    };
                }
            }

            return isCheckAll;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            if (row.ExitNoticeStatus == '<%=Needs.Ccs.Services.Enums.ExitNoticeStatus.Exited.GetHashCode()%>'
                || row.ExitNoticeStatus == '<%=Needs.Ccs.Services.Enums.ExitNoticeStatus.Completed.GetHashCode()%>'
                || row.IsPrint == '<%=Needs.Ccs.Services.Enums.IsPrint.Printed.GetHashCode()%>') {
                return;
            }

            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <style>
        .span {
            font-size: 14px;
        }

        .label {
            font-size: 14px;
            font-weight: 500;
            color: dodgerblue;
            margin-right: 20px;
        }

        .datagrid-header-row,
        .datagrid-row {
            height: 30px;
        }

        /*.datagrid-row-selected {
            background: whitesmoke;
            color: #fff;
        }*/
    </style>
</head>
<body>
    <div style="text-align: center; margin: 5px;">
        <table id="debts" title="欠款情况">
            <thead>
                <tr>
                    <th data-options="field:'ClientName',align:'center'" style="width: 20%">客户名称</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 20%">币种</th>
                    <th data-options="field:'CostSum',align:'center'" style="width: 20%">总共欠款（花费）</th>
                    <th data-options="field:'TotalSum',align:'center'" style="width: 20%">总额度</th>
                    <th data-options="field:'IsOverDue',align:'center'" style="width: 20%">逾期</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddDeliveryInfo()" style="display: none">新增送货信息</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            <span id="showMsg" style="color: red">订单超过垫款上限</span>
            <span id="showOverDuePaymentMsg" style="color: red">客户存在垫资超期</span>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="span">订单编号: </span>
                    <label id="OrderID" class="label"></label>
                    <span class="span">下单日期: </span>
                    <label id="CreateDate" class="label"></label>
                </li>
            </ul>
        </div>
    </div>
    <div style="text-align: center; margin: 5px">
        <table id="sortings" title="到货信息">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 5%">全选</th>
                    <th data-options="field:'BoxIndex',align:'center'" style="width: 10%">箱号</th>
                    <th data-options="field:'Name',align:'left'" style="width: 15%">报关品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 20%">品牌</th>
                    <th data-options="field:'Model',align:'center'" style="width: 20%">规格型号</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 15%">产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 5%">数量</th>
                    <th data-options="field:'DeliveriedQuantity',align:'center'" style="width: 10%">已送货数量</th>
                </tr>
            </thead>
        </table>
    </div>

    <div style="text-align: center; margin: 5px;">
        <table id="deliveries" title="送货信息">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center'" style="width: 20%">送货单号</th>
                    <th data-options="field:'DeliveryDate',align:'center'" style="width: 10%">送货时间</th>
                    <th data-options="field:'DeliveryType',align:'center'" style="width: 20%">送货方式</th>
                    <%-- <th data-options="field:'DeliveryQuantity',align:'center'" style="width: 10%">送货数量</th>--%>
                    <th data-options="field:'PackNo',align:'center'" style="width: 10%">件数</th>
                    <th data-options="field:'CenterExeStatus',align:'center'" style="width: 15%">送货状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
