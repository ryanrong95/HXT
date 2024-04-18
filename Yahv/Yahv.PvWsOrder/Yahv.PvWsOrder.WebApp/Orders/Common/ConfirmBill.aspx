<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ConfirmBill.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.ConfirmBill" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var firstLoad = true;
        //已收款（账单最后一次确认之前）
        var oldReceived = 0;
        //数组，记录优惠券正式提交之前已选的优惠券index
        var selectedCoupouIndex = [];

        $(function () {
            //应收账款初始化
            $('#tab1').myDatagrid({
                fitColumns: true,
                fit: true,
                singleSelect: true,
                pagination: false,
                scrollbarSize: 0,
                nowrap: false,
                rownumbers: true,
                onLoadSuccess: onLoadSuccess,
            });
            //优惠券初始化
            $('#tab2').myDatagrid({
                fitColumns: true,
                fit: true,
                nowrap: false,
                singleSelect: false,
                pagination: false,
                actionName: 'vouchersData',
                checkOnSelect: false,
                onLoadSuccess: function (data) {
                    //去掉全选复选框
                    $(".datagrid-header-check").html("");
                    //过滤不能用的优惠券
                    var rows = $('#tab1').datagrid('getRows');
                    for (var i = 0; i < data.rows.length; i++) {
                        var exit = false;
                        for (var k = 0; k < rows.length; k++) {
                            if (data.rows[i].Subject == rows[k].Subject && data.rows[i].Catalog == rows[k].Catalog) {
                                exit = true;
                                break;
                            }
                        }
                        if (!exit) {
                            $("input[type='checkbox']")[i].disabled = true;
                        }
                    }
                },
                onCheck: function (rowIndex, rowData) {
                    var rows = $('#tab2').datagrid('getRows');
                    for (var i = 0; i < rows.length; i++) {
                        //目前不考虑通用优惠券
                        if (rows[i].Subject == rowData.Subject && rows[i].Catalog == rowData.Catalog) {
                            if (i != rowIndex) {
                                $("input[type='checkbox']")[i].disabled = true;
                            }
                        }
                    }

                    AddCoupon(rowData, rowIndex);
                },
                onUncheck: function (rowIndex, rowData) {
                    var rows = $('#tab2').datagrid('getRows');
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].Subject == rowData.Subject && rows[i].Catalog == rowData.Catalog) {
                            if (i != rowIndex) {
                                $("input[type='checkbox']")[i].disabled = false;
                            }
                        }
                    }
                    CancelCoupon(rowData, rowIndex);
                }
            })
            //关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //提交
            $("#btnSubmit").click(function () {
                var data = new FormData();
                var rows = $('#tab1').datagrid('getRows');
                var receivables = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    receivables.push(rows[i]);
                }
                data.append('receivables', JSON.stringify(receivables));
                data.append('orderId', id);

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $.myWindow.close();
                    }
                })
            })
        });
    </script>
    <script>
        //减免操作按钮Formatter
        function Operation(val, row, index) {
            if (row.OriginDate == '<span class="subtotal">合计：</span>') {
                return;
            }
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="Deduction(\'' + row.ID + '\');return false;">减免</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="ReduceRecords(' + index + ');return false;">减免记录</a> '
                , '</span>'].join('');
        }

        //加载tabl结束后的操作：添加合计行，自动勾选优惠券
        function onLoadSuccess(data) {
            if (firstLoad) {
                firstLoad = false;
                AddTotalRow();
                if (data.rows.length > 0) {
                    $('#payee').textbox('setValue', data.rows[0].PayeeName);
                    $('#payer').textbox('setValue', data.rows[0].PayerName);
                }
                for (var j = 0; j < selectedCoupouIndex.length; j++) {
                    $('#tab2').datagrid('checkRow', selectedCoupouIndex[j]);
                };
            }
        }

        //添加优惠券
        function AddCoupon(data, index) {
            var rows = $('#tab1').datagrid('getRows');
            var array = new Array();
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                //目前不考虑通用优惠券
                if (row.Catalog == data.Catalog && row.Subject == data.Subject) {
                    //计算优惠金额
                    var price = 0;
                    if (data.TypeValue == '<%=Yahv.Underly.CouponType.Quota.GetHashCode()%>') {
                        //无门槛  优惠券实付金额（未收）
                        var remainsPrice = Number(row.Remains);
                        var couponPrice = Number(data.Price);
                        price = remainsPrice > couponPrice ? couponPrice : remainsPrice
                    }
                    if (data.TypeValue == '<%=Yahv.Underly.CouponType.Fact.GetHashCode()%>') {
                        //记录原始值
                        price = Number(row.Remains);
                    }

                    var obj = new Object();
                    obj.ID = row.ID;
                    obj.Index = i;
                    obj.CouponID = data.ID;
                    obj.Price = price
                    array.push(obj);
                    //记录勾选的优惠券(非重复)
                    if (selectedCoupouIndex.indexOf(index) == -1) {
                        selectedCoupouIndex.push(index);
                    }
                }
            }
            //绑定优惠券应收项
            var band = array[0];
            for (var k = 1; k < array.length; k++) {
                if (Number(array[k].Price) > Number(band.Price)) {
                    band = array[k];
                }
            }
            var row = rows[band.Index];
            row.CouponID = band.CouponID;
            //优惠券金额
            row.CouponPrice = Number(band.Price);
            //实际应收 = 应收-减免-优惠
            row.RealLeftPrice = row.LeftPrice - row.ReducePrice - row.CouponPrice;
            //未收 = 应收-减免-优惠-实际已收
            row.Remains = row.LeftPrice - row.ReducePrice - row.CouponPrice - row.RealRightPrice;

            Reload();
        }

        //取消优惠券
        function CancelCoupon(data, index) {
            var rows = $('#tab1').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i]
                if (row.CouponID == data.ID) {
                    row.CouponID = "";
                    row.CouponPrice = 0;
                    row.RealLeftPrice = row.LeftPrice - row.ReducePrice - row.CouponPrice;
                    row.Remains = row.LeftPrice - row.ReducePrice - row.CouponPrice - row.RealRightPrice;
                    //撤销勾选的优惠券
                    deleteEleInArray(index);
                }
            }
            Reload();
        }
        //账单确认减免
        function Deduction(receiveId) {
            $.myDialog({
                title: "减免确认",
                width: 800,
                height: 400,
                url: '/Pays/pays/ReductionAccount/Edit.aspx?type=rec&id=' + receiveId,
                onClose: function () {
                    $('#tab1').datagrid("reload");
                    $('#tab2').datagrid("uncheckAll");
                    firstLoad = true;
                },
            });
        }
        //查看减免记录
        function ReduceRecords(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "减免记录",
                url: location.pathname.replace('ConfirmBill.aspx', 'ReduceRecords.aspx?ID=' + data.ID + '&Currency=' + data.Currency),
                onClose: function () {
                    $('#tab1').datagrid("reload");
                    $('#tab2').datagrid("uncheckAll");
                    firstLoad = true;
                },
            });
        }
    </script>
    <script>
        function Reload() {
            RemoveTotalRow();
            loadData();
            AddTotalRow();
        }
        //添加合计行
        function AddTotalRow() {
            $('#tab1').datagrid('appendRow', {
                OriginDate: '<span class="subtotal">合计：</span>',
                Catalog: '<span class="subtotal">--</span>',
                Subject: '<span class="subtotal">--</span>',
                Currency: '<span class="subtotal">--</span>',
                LeftPrice: '<span class="subtotal">' + compute('LeftPrice') + '</span>',
                Currency: '<span class="subtotal">--</span>',
                ReducePrice: '<span class="subtotal">' + compute('ReducePrice') + '</span>',
                CouponPrice: '<span class="subtotal">' + compute('CouponPrice') + '</span>',
                RealLeftPrice: '<span class="subtotal">' + compute('RealLeftPrice') + '</span>',
                RealRightPrice: '<span class="subtotal">' + compute('RealRightPrice') + '</span>',
                Remains: '<span class="subtotal">' + compute('Remains') + '</span>',
                Btn: '<span class="subtotal">--</span>',
            });
        }
        //删除合计行
        function RemoveTotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
        }
        //重新加载数据
        function loadData() {
            var data = $('#tab1').datagrid('getData');
            $('#tab1').datagrid('loadData', data);
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //删除指定优惠券
        function deleteEleInArray(index) {
            for (var m = 0; m < selectedCoupouIndex.length; m++) {
                if (selectedCoupouIndex[m] == index) {
                    selectedCoupouIndex.splice(m, 1);
                }
            };
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north'" style="height: 60px; border: none">
            <table class="liebiao">
                <tr>
                    <td style="width:100px">收款公司</td>
                    <td>
                        <input id="payee" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td style="width:100px">付款公司</td>
                    <td>
                        <input id="payer" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center',title:'客户应收账款',split:true" style="border: none">
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'OriginDate',align:'center'" style="width: 150px;">应收日期</th>
                        <th data-options="field:'Catalog',align:'center'" style="width: 110px;">分类</th>
                        <th data-options="field:'Subject',align:'center'" style="width: 110px;">科目</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 100px">币种</th>
                        <th data-options="field:'LeftPrice',align:'center'" style="width: 130px;">应收金额</th>
                        <th data-options="field:'CouponID',align:'center',hidden:true" style="width: 180px;">优惠券编号</th>
                        <th data-options="field:'ReducePrice',align:'center'" style="width: 130px">减免金额</th>
                        <th data-options="field:'CouponPrice',align:'center'" style="width: 130px">优惠金额</th>
                        <th data-options="field:'RealLeftPrice',align:'center'" style="width: 130px;">实际应收</th>
                        <th data-options="field:'RealRightPrice',align:'center'" style="width: 130px;">实际已收</th>
                        <th data-options="field:'Remains',align:'center'" style="width: 130px;">未收金额</th>
                        <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 300px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'east',title:'客户优惠券',split:true" style="width: 30%; border: none">
            <table id="tab2">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:'true'"></th>
                        <th data-options="field:'ID',align:'center',hidden:true" style="width: 180px;">编号</th>
                        <th data-options="field:'Name',align:'left'" style="width: 150px;">名称</th>
                        <th data-options="field:'Type',align:'center'" style="width: 100px;">类型</th>
                        <th data-options="field:'Catalog',align:'center'" style="width: 100px">分类</th>
                        <th data-options="field:'Subject',align:'center'" style="width: 100px;">科目</th>
                        <th data-options="field:'Price',align:'center'" style="width: 90px;">金额</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5;">
            <div style="float: right; margin-right: 2px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">取消</a>
            </div>
        </div>
    </div>
</asp:Content>
