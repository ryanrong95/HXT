<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivingImport.aspx.cs" Inherits="WebApp.Finance.MakeAccount.ReceivingImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款通知查询</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var pageNumber = getQueryString("pageNumber");
        var pageSize = getQueryString("pageSize");
        var initClientName = '<%=this.Model.ClientName%>';  //getQueryString("ClientName");
        var initQuerenStatus = getQueryString("QuerenStatus");
        var initStartDate = getQueryString("StartDate");
        var initEndDate = '<%=this.Model.EndDate%>';  
        var initLastReceiptDateStartDate = '<%=this.Model.LastReceiptDateStartDate%>';  
        var initLastReceiptDateEndDate = '<%=this.Model.LastReceiptDateEndDate%>';  

        var lastQuerenStatus = "0";
    </script>
    <script type="text/javascript">
        $(function () {
            if (pageNumber == null || pageNumber == "") {
                pageNumber = 1;
            }
            if (pageSize == null || pageSize == "") {
                pageSize = 50;
            }

            //初始化查询参数（返回来的）放入查询条件输入框内
            $('#ClientName').textbox('setValue', initClientName);
            if (initQuerenStatus == null || initQuerenStatus == "") {
                initQuerenStatus = "0";
            }
            $("#comboboxQuerenStatus").combobox("select", initQuerenStatus);
            $('#StartDate').datebox('setValue', initStartDate);
            $('#EndDate').datebox('setValue', initEndDate);
            $('#LastReceiptDateStartDate').datebox('setValue', initLastReceiptDateStartDate);
            $('#LastReceiptDateEndDate').datebox('setValue', initLastReceiptDateEndDate);


            //订单列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true, fit: true, toolbar: '#topBar', nowrap: false, rownumbers: true,
                singleSelect: false,
                queryParams: {
                    ClientName: initClientName,
                    QuerenStatus: initQuerenStatus,
                    StartDate: initStartDate,
                    EndDate: initEndDate,
                    LastReceiptDateStartDate: initLastReceiptDateStartDate,
                    LastReceiptDateEndDate: initLastReceiptDateEndDate
                },
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

            $('#comboboxQuerenStatus').combobox({
                onChange: function (newValue, oldValue) {
                    Search();
                }
            });

           
        });

        //查询
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

            if (!(StartDate != "" || EndDate != "" || ClientName != "" || QuerenStatus != "0")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            if (!(LastReceiptDateStartDate != "" || LastReceiptDateEndDate != "" )) {
                $.messager.alert({ title: '提示', msg: "请选择核销日期", icon: 'info', top: 300 });
                return;
            }

            var parm = {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
            }

            $('#datagrid').myDatagrid('search', parm);

        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#comboboxQuerenStatus').textbox('setValue', "全部");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#LastReceiptDateStartDate').datebox('setValue', null);
            $('#LastReceiptDateEndDate').datebox('setValue', null);
            Search();
        }

       

        //导出收款做账报表
        function ExportFinanceReport() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

            if (!(StartDate != "" || EndDate != "" || ClientName != "" || QuerenStatus != "0")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            if (!(LastReceiptDateStartDate != "" || LastReceiptDateEndDate != "" )) {
                $.messager.alert({ title: '提示', msg: "请选择核销日期导出做账报表", icon: 'info', top: 300 });
                return;
            }

            var queryParams = {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
            };
            MaskUtil.mask();
            $.post('?action=ExportFinanceReport', queryParams, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }

        function MakeAccount() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
            }

            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

             var AccInfo = [];
             for (i = 0; i < data.length; i++) {
                 AccInfo.push({
                     ID: data[i].ID,
                 });
             };

            //验证成功
             MaskUtil.mask();//遮挡层
            $.post('?action=MakeAccount', {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
                Model: JSON.stringify(AccInfo)
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息',rel.msg, 'info', function () { });
                }
            });
        }

        function MakeAccountAll() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

             var AccInfo = [];
             //验证成功
             MaskUtil.mask();//遮挡层
            $.post('?action=MakeAccount', {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
                Model: JSON.stringify(AccInfo)
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', rel.msg, 'info', function () { });
                }
            });
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
                            <option value="0">全部</option>
                            <option value="1">未确认</option>
                            <option value="2">已确认</option>
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
                    <td class="lbl">最近核销日期:</td>
                    <td>
                        <input class="easyui-datebox" id="LastReceiptDateStartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="LastReceiptDateEndDate" data-options="height:26,width:200," />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>                      
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="exportReport" data-options="iconCls:'icon-save'" onclick="ExportFinanceReport()">导出做账</a>
                        <a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccount()">生成凭证</a>
                        <a id="btnMakeAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccountAll()">生成全部凭证</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款统计" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true, singleSelect: false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ID',align:'center'" style="width: 10%;">收款ID</th>
                    <th data-options="field:'SeqNo',align:'center'" style="width: 7%;">流水号</th>
                    <th data-options="field:'AccountName',align:'center'" style="width: 15%;">收款账户名称</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 7%;">收款日期</th>
                    <th data-options="field:'ClientName',align:'center'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'InvoiceType',align:'left'" style="width: 5%;">客户类型</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 5%;">收款金额RMB</th>
                    <th data-options="field:'ClearAmount',align:'left'" style="width: 5%;">已确认金额</th>
                    <th data-options="field:'UnClearAmount',align:'center'" style="width: 5%;">未确认金额</th>
                    <th data-options="field:'TotalProduct',align:'center'" style="width: 5%;">货款</th>
                    <th data-options="field:'TotalAddTax',align:'center'" style="width: 5%;">增值税</th>
                    <th data-options="field:'TotalTariffTax',align:'left'" style="width: 5%;">关税</th>
                    <th data-options="field:'TotalExciseTax',align:'center'" style="width: 5%;">消费税</th>
                    <th data-options="field:'TotalAgency',align:'center'" style="width: 5%;">代理费</th>
                    <th data-options="field:'TotalExchangeSpread',align:'center'" style="width: 5%;">汇差</th>                                   
                </tr>
            </thead>
        </table>
    </div> 
</body>
</html>
