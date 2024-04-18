<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnReceiveList.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.UnReceiveList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款通知查询</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var pageNumber = getQueryString("pageNumber");
        var pageSize = getQueryString("pageSize");

        var initClientName = '<%=this.Model.ClientName%>';
        var initQuerenStatus = getQueryString("QuerenStatus");
        var initStartDate = getQueryString("StartDate");
        var initEndDate = getQueryString("EndDate");
    </script>
    <script type="text/javascript">
        $(function () {
            var comboboxQuerenStatusData = [
                { 'text': '全部', 'value': '0' },
                { 'text': '未确认', 'value': '1' },
                { 'text': '已确认', 'value': '2' },
            ];

            $('#comboboxQuerenStatus').combobox({
                data: comboboxQuerenStatusData,
                //onSelect: function (record) {
                //    Search();
                //}
            });


            if (pageNumber == null || pageNumber == "") {
                pageNumber = 1;
            }
            if (pageSize == null || pageSize == "") {
                pageSize = 10;
            }

            //初始化查询参数（返回来的）放入查询条件输入框内
            //$('#ClientName').textbox('setValue', initClientName);
            //if (initQuerenStatus == null || initQuerenStatus == "") {
            //    initQuerenStatus = "0";
            //}
            //$("#comboboxQuerenStatus").combobox("select", initQuerenStatus);
            //$('#StartDate').datebox('setValue', initStartDate);
            //$('#EndDate').datebox('setValue', initEndDate);

            $("#comboboxQuerenStatus").combobox("select", '0');

            //订单列表初始化
            $('#datagrid').myDatagrid({
                //url: location.pathname,  // + "?ClientName=" + initClientName + "&QuerenStatus=" + initQuerenStatus + "&StartDate=" + initStartDate + "&EndDate=" + initEndDate,
                //queryParams: {
                //ClientName: initClientName,
                //QuerenStatus: initQuerenStatus,
                //StartDate: initStartDate,
                //EndDate: initEndDate,
                //},
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                pageNumber: pageNumber,
                pageSize: pageSize,
                nowrap: false,
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
                onBeforeLoad: function (param) {
                    pageNumber = param.page;
                    pageSize = param.rows;
                },
            });


        });

        //查询
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            //$('#datagrid').myDatagrid({
            //    //url: location.pathname,  // + "?ClientName=" + ClientName + "&QuerenStatus=" + QuerenStatus + "&StartDate=" + StartDate + "&EndDate=" + EndDate,
            //    queryParams: {
            //        ClientName: ClientName,
            //        //QuerenStatus: QuerenStatus,
            //        StartDate: StartDate,
            //        EndDate: EndDate,
            //    },
            //});

            var parm = {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#datagrid').myDatagrid('search', parm);

        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#comboboxQuerenStatus').textbox('setValue', "全部");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //订单收款
        function AddReceiptDetail(id, clientid, SeqNo) {
            if (id) {
                $.post('?action=CheckReceive', { ReceiptID: id }, function (res) {
                    var result = JSON.parse(res);
                    if (result.success) {
                        var url = location.pathname.replace(/UnReceiveList.aspx/ig, '../Order/List.aspx?ID=' + id + "&ClientID=" + clientid + "&SeqNo=" + SeqNo
                            + "&UnReceiveListPageNumber=" + pageNumber + "&UnReceiveListPageSize=" + pageSize
                            + "&UnReceiveListClientName=" + encodeURI($('#ClientName').textbox('getValue'))
                            + "&UnReceiveListQuerenStatus=" + $("#comboboxQuerenStatus").combobox("getValue")
                            + "&UnReceiveListStartDate=" + $('#StartDate').datebox('getValue')
                            + "&UnReceiveListEndDate=" + $('#EndDate').datebox('getValue'));
                        window.location = url;
                    } else {
                        $.messager.alert('提示', "存在未审批的退款申请，不能订单收款");
                    }
                });

            }
        }

        //退款申请
        function RefundApply(financeReceiptId, clientName, SeqNo) {
            $.post('?action=CheckReceive', { ReceiptID: financeReceiptId }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.confirm('申请退款', '确定要申请 "' + clientName + '" 的这笔退款吗？', function (flag) {
                        if (flag) {
                            var url = location.pathname.replace(/UnReceiveList.aspx/ig, '../RefundApply/Apply.aspx?ReceiptID=' + financeReceiptId + '&PageSource=Apply');
                            top.$.myWindow({
                                iconCls: "icon-add",
                                url: url,
                                noheader: false,
                                title: '新增退款申请',
                                width: 850,
                                height: 700,
                                overflow: "hidden",
                                onClose: function () {
                                    $('#datagrid').datagrid('reload');
                                }
                            });
                        }
                    });
                } else {
                    $.messager.alert('提示', "存在未审批的退款申请，不能再次申请退款");
                }
            });
        }

        //取消收款
        function UnmackReceipt(financeReceiptId, clientId, clientName) {
            $.messager.confirm('取消收款', '确定要取消 "' + clientName + '" 的这笔收款吗？', function (flag) {
                if (flag) {
                    var url = location.pathname + "?action=UnmackReceipt";
                    var params = {
                        "FinanceReceiptId": financeReceiptId,
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
            var buttonsAddReceipt = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AddReceiptDetail(\''
                + row.ID + '\',\'' + row.ClientID + '\',\'' + row.SeqNo + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">订单收款</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            if (row.AvailableAmount - row.ClearAmount > 0) {
                buttonsAddReceipt += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="RefundApply(\''
                    + row.ID + '\',\'' + row.ClientName + '\',\'' + row.SeqNo + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">退款申请</span>' +
                    '<span class="l-btn-icon icon-back">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttonsAddReceipt;
        }
    </script>

    <style>
        table.search-condition td {
            padding-left: 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">付款人:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">状态:</td>
                    <td>
                        <select id="comboboxQuerenStatus" class="easyui-combobox" data-options="height:26,width:200,required:true,editable:false,">
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款明细" data-options="
            border:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'DyjID',align:'left'" style="width: 50px;">大赢家ID</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 70px;">流水号</th>
                    <th data-options="field:'VaultName',align:'left'" style="width: 70px;">金库</th>
                    <th data-options="field:'AccountName',align:'center'" style="width: 150px;">账户名称</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 150px;">付款人</th>
                    <th data-options="field:'FinanceReceiptFeeType',align:'center'" style="width: 70px;">类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 80px;">收款金额</th>
                    <th data-options="field:'AvailableAmount',align:'center'" style="width: 80px;">可核销金额</th>
                    <th data-options="field:'ClearAmount',align:'center'" style="width: 80px;">已确认金额</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 70px;">收款日期</th>
                    <th data-options="field:'QuerenStatus',align:'center'" style="width: 40px;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 110px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
