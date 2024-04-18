<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="WebApp.HKWarehouse.Agency2Declare.ShoppingCart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script>
        var PvWsOrderUrl = '<%=this.Model.PvWsOrderUrl%>';
        var WindowName = '<%=this.Model.WindowName%>';
        var OutClientCode = '<%=this.Model.OutClientCode%>';
        var OutClientID = '<%=this.Model.OutClientID%>';
        var editIndex = undefined;
        var check = false;
        var SelectedInfo = [];
        $(function () {
            var ewindow = $.myWindow.getMyWindow(WindowName);
            SelectedInfo = ewindow.SelectedInfo;

            //订单信息初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                pagination: true, //启用分页
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: true,
                onCheck: onCheck,
                onUncheck: onUncheck,
                onClickRow: onClickRow,
            });

            $("input", $("#ClientNo").next("span")).blur(function () {
                var clientNo = $("#ClientNo").val();
                $.post('?action=CompanyName', { ClientNo: clientNo }, function (res) {
                    var result = JSON.parse(res);
                    if (result.success) {
                        $("#CompanyName").textbox("setValue", result.message);
                    } else {
                        $.messager.alert('提示', result.message);
                    }
                });
            });
        });

        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#datagrid').datagrid('validateRow', editIndex)) {
                $('#datagrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#datagrid').datagrid('selectRow', index);
                    $('#datagrid').datagrid('beginEdit', index);
                    editIndex = index;

                    //var ed = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'SaleQuantity' });
                    //if (ed) {
                    //    $(ed.target).numberbox({
                    //        onChange: function (newValue, oldValue) {
                    //            if (editIndex == undefined)
                    //                return;
                    //            var rows = $("#datagrid").datagrid("getRows");
                    //            var row = rows[index]
                    //            var qty = row.Quantity;
                    //            if (Number(newValue) > Number(qty)) {
                    //                $.messager.alert('提示', '下单数量不能大于库存数量');
                    //                $(ed.target).numberbox("setValue", qty);
                    //                return;
                    //            } else {
                    //                $('#datagrid').datagrid('updateRow', {
                    //                    index: index,
                    //                    row: {
                    //                        SaleQuantity: newValue
                    //                    }
                    //                });
                    //            }
                    //        }
                    //    });
                    //}

                } else {
                    $('#datagrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                editIndex = undefined;
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick = "removeRow(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function removeRow(ID) {
            var datas = $('#datagrid').datagrid('getData');
            for (var i = 0; i < datas.rows.length; i++) {
                if (datas.rows[i].ID == ID) {
                    var rowIndex = $('#datagrid').datagrid('getRowIndex', datas.rows[i]);
                    $('#datagrid').datagrid('deleteRow', rowIndex);
                    break;
                }
            }
            var ewindow = $.myWindow.getMyWindow(WindowName);
            if (WindowName == "StockData") {
                var test = ewindow.SelectedInfo;
                var index = ewindow.findIndex(ewindow.SelectedInfo, ID);
                    if (index > -1) {
                        ewindow.SelectedInfo.splice(index, 1);
                    }                  
            }
        }

        function NormalClose() {
            var ewindow = $.myWindow.getMyWindow(WindowName);
            if (WindowName == "StockData") {
                ewindow.Search();
            }
            $.myWindow.close();
        }

        function AddOrder() {                   
            var CompanyName = $("#CompanyName").textbox("getValue");
            var ClientCode = $("#ClientNo").textbox("getValue");
            if (CompanyName == "" || CompanyName == null || CompanyName == undefined) {
                $.messager.alert('提示', "请先选择客户!");
                return;
            }
            var cart = "";
            for (var i = 0; i < SelectedInfo.length; i++) {
                cart += SelectedInfo[i].ID + ",";
            }          
            $.myWindow({
                title: '新增报关订单',
                minWidth: 1200,
                minHeight: 600,
                url: PvWsOrderUrl+ 'Orders/AddAgency2Declare.aspx?Name=' + CompanyName + '&EnterCode=' + ClientCode+'&OutEnterCode='+OutClientCode+'&OutClientID='+OutClientID+'&CartInfo=' + cart,
                onClose: function () {
                },
            });           
        }
       
    </script>
    <script type="text/javascript">
        var index = -1;
        function onCheck(rowIndex, rowData) {
            if (index == -1) {
                index = rowIndex;
                var productid = rowData["ID"];
                var rows = $("#datagrid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i]["ID"] == productid && i != rowIndex) {
                        $("#datagrid").datagrid("checkRow", i);
                    }
                }
                index = -1;
            }
        }
        function onUncheck(rowIndex, rowData) {
            if (index == -1) {
                index = rowIndex;
                var productid = rowData["ID"];
                var rows = $("#datagrid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i]["ID"] == productid && i != rowIndex) {
                        $("#datagrid").datagrid("uncheckRow", i);
                    }
                }
                index = -1;
            }
        }
    </script>
</head>
<body>
    <div class="easyui-panel" style="width: 100%; height: 100%; border: 0px;">
        <div style="text-align: center; height: 70%; margin: 5px;">
            <table id="datagrid">
                <thead>
                    <tr>
                        <th data-options="field:'ID',align:'center'" style="width: 200px">入库单号</th>
                        <th data-options="field:'PartNumber',align:'center'" style="width: 200px">型号</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 100px">库存数量</th>
                       <%-- <th data-options="field:'SaleQuantity',align:'center', editor:'numberbox'," style="width: 100px">下单数量</th>--%>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div>
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">入仓号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientNo" data-options="width:100" />
                    </td>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="width:300,disabled:true" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="sub-container" style="height: 20px;">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="AddOrder()">转报关</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" style="margin-left: 5px;" onclick="NormalClose()">取消</a>
        </div>
    </div>
</body>
</html>
