<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoicedList.aspx.cs" Inherits="WebApp.Finance.InvoicedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已开票</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%-- <script>
        gvSettings.fatherMenu = '开票通知';
        gvSettings.menu = '已开票';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var ApplyData = eval('(<%=this.Model.ApplyData%>)');
        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                singleSelect: false,
                onCheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onLoadSuccess: function (data) {
                    var totalSum = 0; //总件数
                    var rows = data.rows;
                    for (var i = 0; i < rows.length; i++) {
                        var currentTotal = Number(Number(Number(rows[i].Amount)).toFixed(2));
                        totalSum += currentTotal;
                    }

                    $("#total-sum").html(totalSum.toFixed(2)); //勾选金额
                },
            });

            //初始化Combobox
            $('#Apply').combobox({
                data: ApplyData,
            })
            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
            })
        });

        //查询
        function Search() {
            var Apply = $('#Apply').combobox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var InvStartDate = $('#InvStartDate').datebox('getValue');
            var InvEndDate = $('#InvEndDate').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var InvoiceNo = $('#InvoiceNo').textbox('getValue');
            var OutStockStatus = $('#OutStockStatus').combobox('getValue');
            var parm = {
                Apply: Apply,
                InvoiceType: InvoiceType,
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                StartDate: StartDate,
                EndDate: EndDate,
                InvStartDate: InvStartDate,
                InvEndDate: InvEndDate,
                OrderID: OrderID,
                InvoiceNo: InvoiceNo,
                OutStockStatus: OutStockStatus
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Apply').combobox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#InvStartDate').datebox('setValue', null);
            $('#InvEndDate').datebox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#InvoiceNo').textbox('setValue', null);
            $('#OutStockStatus').combobox('setValue', null);
            
            Search();
        }

        //查看
        function See(id) {
            if (id) {
                var url = location.pathname.replace(/InvoicedList.aspx/ig, 'Details.aspx?ID=' + id);
                window.location = url;
            }
        }

        //打印确认单
        function Print(id) {
            var url = location.pathname.replace(/InvoicedList.aspx/ig, 'PrintInvoiceConfirm.aspx?ID=' + id + '&From=Finance');
            window.location = url;
        }

        //打印发票运单
        function PrintWaybill() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选开票通知！');
                return;
            }
            //验证是否同同客户
            for (var i = 0; i < data.length; i++) {
                if (data[0].ClientCode != data[i].ClientCode) {
                    $.messager.alert('提示', '请选择相同客户的开票通知！');
                    return;
                }
            }
            //拼接字符串
            var strIds = "";
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            var url = location.pathname.replace(/InvoicedList.aspx/ig, 'PrintInvoice.aspx?IDs=' + strIds);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印发票运单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }


        //维护发票运单号
        function AddWaybill() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选开票通知！');
                return;
            }
            //验证是否同同客户
            for (var i = 0; i < data.length; i++) {
                if (data[i].WaybillCode != '') {
                    $.messager.alert('提示', data[i].ID + ' 已有运单号，不能录入！');
                    return;
                }
            }
            //拼接字符串
            var strIds = "";
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            var url = location.pathname.replace(/InvoicedList.aspx/ig, 'AddWaybill.aspx?IDs=' + strIds);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '维护发票运单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //
        function OutStock() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选开票通知！');
                return;
            }
            //验证是否可以出库
            for (var i = 0; i < data.length; i++) {
                if (data[i].InvoiceType == '服务费发票' || data[i].IsExStock == true) {
                    $.messager.alert('提示', data[i].ID + ' 此开票不需出库！');
                    return;
                }
            }
            //拼接字符串
            var strIds = "";
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            MaskUtil.mask();//遮挡层
            $.post('?action=OutStock', {
                IDs: JSON.stringify(strIds)
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        $('#datagrid').myDatagrid('reload');
                    }
                });
            });
        }

        //查看发票文件
        function File(invoiceNoticeID) {
            var url = location.pathname.replace(/InvoicedList.aspx/ig, 'InvoiceNoticeFile.aspx') + "?InvoiceNoticeID=" + invoiceNoticeID;

            $.myWindow.setMyWindow("InvoicedList2InvoiceNoticeFile", window);
            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '发票',
                width: '840',
                height: '460',
                url: url,
                onClose: function () {

                }
            });
        }

        //批量打印发票确认确认单
        function BatchPrintInvoiceConfirm() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选开票通知！');
                return;
            }
            //拼接字符串
            var strIds = "";
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            var url = location.pathname.replace(/InvoicedList.aspx/ig, 'BatchPrintInvoiceConfirm.aspx?IDs=' + strIds);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印确认单',
                width: '750px',
                height: '550px',
                onClose: function () {
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="See(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            //buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Print(\'' + row.ID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">打印确认单</span>' +
            //    '<span class="l-btn-icon icon-print">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

            if (row.InvoiceType == '<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>' && row.IsExStock != true) {

            }

            if (row.InvoiceNoticeFileCount > 0) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="File(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">发票</span>' +
                    '<span class="l-btn-icon icon-tip">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        function FMTIsExStock(val, row, index) {
            var flag = "";
            if (row.InvoiceType == '全额发票') {
                flag = row.IsExStock == true ? "已出库" : "未出库";
            } else {
                flag = "-";
            }
            return flag;
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            var totalSum = 0.00; //
            var count = 0;
            for (var i = 0; i < rows.length; i++) {
                var currentTotal = Number(Number(rows[i].Amount).toFixed(2));
                totalSum += currentTotal;
                count++;
            }

            $("#check-count").html("勾选 " + count + "单");
            $("#check-sum").html(totalSum.toFixed(2)); //勾选金额          
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">申请人:</td>
                    <td>
                        <input class="easyui-combobox" id="Apply" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">开票类型:</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">订单号:</td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">发票号:</td>
                    <td>
                        <input class="easyui-textbox" id="InvoiceNo" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">申请日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">开票日期:</td>
                    <td>
                        <input class="easyui-datebox" id="InvStartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="InvEndDate" data-options="height:26,width:200," />
                    </td>
                      <td class="lbl">出库状态:</td>
                    <td>
                       <%-- <input class="easyui-combobox" id="OutStockStatus" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />--%>
                        <select id="OutStockStatus" class="easyui-combobox" name="OutStockStatus" style="width:200px;">
                        <option value="0">未出库</option>
                        <option value="1">已出库</option>
                        <option selected="selected">全部</option>
                    </select>
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="PrintWaybill()"
                            style="margin-left: 5px;">打印发票运单</a>
                        <a id="btnBatchPrintInvoiceConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="BatchPrintInvoiceConfirm()"
                            style="margin-left: 5px;">打印发票确认单</a>
                        <a id="btnAddWaybill" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="AddWaybill()"
                            style="margin-left: 5px;">维护运单号</a>
                        <a id="btnOutStock" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="OutStock()"
                            style="margin-left: 5px;">财务出库</a>
                        <span id="sum-container" style="margin-left: 55px; color: red; font-weight: 600">
                            <label>合计</label>
                            <label style="margin-left: 25px;">本页合计:</label>
                            <label id="total-sum">0</label>
                            <label id="check-count" style="margin-left: 25px;">勾选 0单</label>
                            <label style="margin-left: 25px;">勾选合计:</label>
                            <label id="check-sum">0</label>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="已开票" data-options="
            fitColumns:true,
            fit:true,
            singleSelect:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ID',align:'center'" style="width: 8%;">开票编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 15%;">公司名称</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 8%;">开票类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 8%;">含税金额</th>
                    <th data-options="field:'Difference',align:'center'" style="width: 90px;">差额</th>
                    <th data-options="field:'DeliveryType',align:'center'" style="width: 8%;">交付方式</th>
                    <th data-options="field:'WaybillCode',align:'left'" style="width: 10%;">发票运单</th>
                    <th data-options="field:'Status',align:'center'" style="width: 7%;">开票状态</th>
                    <th data-options="field:'ApplyName',align:'center'" style="width: 7%;">申请人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">申请日期</th>
                    <th data-options="field:'InvoiceDate',align:'center'" style="width: 8%;">开票日期</th>
                    <th data-options="field:'IsExStock',align:'center',formatter:FMTIsExStock" style="width: 8%;">出库状态</th>
                    <th data-options="field:'Btn',align:'left',width:80,formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
