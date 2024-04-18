<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Receipt.Record.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            var orderFeeType = eval('(<%=this.Model.OrderFeeType%>)');
            $('#FeeType').combobox({
                data: orderFeeType
            });


            $('#datagrid').myDatagrid({
                border:false,
                fitColumns:true,
                fit:true,
                scrollbarSize: 0,
                rownumbers:true,
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
                    //合并单元格
			        var mark = 1;
			        for (var i = 0; i < data.rows.length; i++) {
				        //合并 订单编号、费用类型
				        if (i > 0) {
					        if (data.rows[i]['OrderID'] == data.rows[i - 1]['OrderID'] && data.rows[i]['FeeTypeName'] == data.rows[i - 1]['FeeTypeName']) {
						        mark += 1;
						        $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'OrderID',
							        rowspan: mark
						        });
						        $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'FeeTypeName',
							        rowspan: mark
                                });
                                $("#datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'Btn',
							        rowspan: mark
                                });
					        }
					        else {
						        mark = 1;
					        }
				        }
			        }
                },
            });


        });

        //查询
        function Search() {
            var CreateDateStartDate = $('#CreateDateStartDate').datebox('getValue');
            var CreateDateEndDate = $('#CreateDateEndDate').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ReceiptDateStartDate = $('#ReceiptDateStartDate').datebox('getValue');
            var ReceiptDateEndDate = $('#ReceiptDateEndDate').datebox('getValue');
            var FeeType = $('#FeeType').combobox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');

            var parm = {
                CreateDateStartDate: CreateDateStartDate,
                CreateDateEndDate: CreateDateEndDate,
                OrderID: OrderID,
                ReceiptDateStartDate: ReceiptDateStartDate,
                ReceiptDateEndDate: ReceiptDateEndDate,
                FeeType: FeeType,
                SeqNo: SeqNo,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#CreateDateStartDate').datebox('setValue', null);
            $('#CreateDateEndDate').datebox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ReceiptDateStartDate').datebox('setValue', null);
            $('#ReceiptDateEndDate').datebox('setValue', null);
            $('#FeeType').combobox('setValue', null);
            $('#SeqNo').textbox('setValue', null);

            Search();
        }

        //取消收款
        function UnmackReceipt(OrderReceiptID, OrderID, FeeTypeName, FeeTypeInt) {
            $.messager.confirm('取消收款', '确定要取消该收款吗？<br>'
                + '订单号：' + OrderID + '<br>'
                + '费用类型：' + FeeTypeName, function (flag) {
                if (flag) {
                    var url = location.pathname + "?action=UnmackReceipt";
                    var params = {
                        OrderID: OrderID,
                        FeeTypeInt: FeeTypeInt,
                    };
                    MaskUtil.mask();
                    $.post(url, params, function (res) {
                        MaskUtil.unmask();
                        var resData = JSON.parse(res);
                        if (resData.success == "true") {
                            $('#datagrid').datagrid('reload');
                        } else {
                            $.messager.alert('提示', resData.message);
                        }
                    });
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';

            buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="UnmackReceipt(\''
                + row.OrderReceiptID + '\',\''
                + row.OrderID + '\',\''
                + row.FeeTypeName + '\',\''
                + row.FeeTypeInt + '\',\''
                + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">取消收款</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            return buttons;
        }

        //导出
        function Export() {
            var CreateDateStartDate = $('#CreateDateStartDate').datebox('getValue');
            var CreateDateEndDate = $('#CreateDateEndDate').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ReceiptDateStartDate = $('#ReceiptDateStartDate').datebox('getValue');
            var ReceiptDateEndDate = $('#ReceiptDateEndDate').datebox('getValue');
            var FeeType = $('#FeeType').combobox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');

            if (ReceiptDateStartDate == null || ReceiptDateStartDate == '' || ReceiptDateEndDate == null || ReceiptDateEndDate == '') {
                $.messager.alert('消息', '请选择银行收款日期，再导出excel', 'info');
                return;
            }

            var parm = {
                CreateDateStartDate: CreateDateStartDate,
                CreateDateEndDate: CreateDateEndDate,
                OrderID: OrderID,
                ReceiptDateStartDate: ReceiptDateStartDate,
                ReceiptDateEndDate: ReceiptDateEndDate,
                FeeType: FeeType,
                SeqNo: SeqNo,
            };
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', parm, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="CreateDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="CreateDateEndDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">订单编号:</span></td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">银行流水号:</span></td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" data-options="width:200," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行流水收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="ReceiptDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="ReceiptDateEndDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">费用类型:</span></td>
                    <td>
                        <select id="FeeType" class="easyui-combobox" data-options="valueField:'Key',textField:'Value',width:200,editable:false,">

                        </select>
                    </td>

                    <td colspan="2">
                        <span style="padding-left: 10px;">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                           <%-- <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Export()">导出Excel</a>--%>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款记录" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">收款时间</th>
                    <th data-options="field:'AdminName',align:'left'" style="width: 5%;">收款人</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'FeeTypeName',align:'left'" style="width: 6%;">费用类型</th>
                    <th data-options="field:'ReceiptTypeName',align:'center'" style="width: 6%;">收款类型</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 7%;">实收</th>
                    <th data-options="field:'FinanceReceiptID',align:'left'" style="width: 12%;">收款编号</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 8%;">银行流水号</th>
                    <th data-options="field:'ReceiptAmount',align:'left'" style="width: 7%;">收款金额</th>
                     <th data-options="field:'Payer',align:'center'" style="width: 12%;">客户名称</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 8%;">银行流水收款时间</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 110px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
