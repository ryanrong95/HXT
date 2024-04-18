<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.DollarEquityApply.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var IsPaidOption = eval('(<%=this.Model.IsPaidOption%>)');

        var BalanceValue = '0';

        $(function () {
            showBalanceValue();

            //是否处理下拉框初始化           
            $('#IsPaid').combobox({
                data: IsPaidOption,
            })

            //订单列表初始化
            $('#dollarEquityApplys').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,
                onCheck: function (index, row) {
                    calcSomeSum($('#dollarEquityApplys').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#dollarEquityApplys').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    calcSomeSum($('#dollarEquityApplys').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#dollarEquityApplys').datagrid('getChecked'));
                },
                pageSize: 50,
            });

        });

        //查询
        function Search() {
            var IsPaid = $('#IsPaid').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');           

            var parm = {
                IsPaid: IsPaid,
                StartDate: StartDate,
                EndDate: EndDate,               
            };
            $('#dollarEquityApplys').myDatagrid('search', parm);

            showBalanceValue();
        }

        //重置查询条件
        function Reset() {
            $('#IsPaid').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);

            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Detail(\''
                + row.DollarEquityApplyID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if(row.IsPaid == "1") {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">付款</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Payment(\''
                    + row.DollarEquityApplyID + '\',\'' + row.Amount + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">付款</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (row.FileUrl != null && row.FileUrl != '') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="File(\''
                    + row.FileUrl + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">文件</span>' +
                    '<span class="l-btn-icon icon-save">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">文件</span>' +
                    '<span class="l-btn-icon icon-save">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        //详情
        function Detail(dollarEquityApplyID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?DollarEquityApplyID=" + dollarEquityApplyID;

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '详情',
                width: '700',
                height: '290',
                url: url,
                onClose: function () {
                    $('#dollarEquityApplys').datagrid('reload');
                }
            });
        }

        //付款
        function Payment(dollarEquityApplyID, amount) {
            if (Number(BalanceValue) < Number(amount)) {
                $.messager.alert('提示', '剩余美金权益不足！');
                return;
            }

            var url = location.pathname.replace(/List.aspx/ig, 'Payment.aspx') + "?DollarEquityApplyID=" + dollarEquityApplyID;

            window.location.href = url; 
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            //console.log(rows);

            var totalApplyAmount = 0; //总申请金额

            for (var i = 0; i < rows.length; i++) {
                var currentAmount = Number(rows[i].Amount);

                totalApplyAmount += currentAmount;
            }

            $("#TotalApplyAmount").html(totalApplyAmount); //总申请金额
        }

        //查看付款凭证文件
        function File(fileUrl) {
            $('#viewfileImg').css("display", "none");
            $('#viewfilePdf').css("display", "none");
            if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', fileUrl);
                $('#viewfilePdf').css("display", "block");

            }
            else {
                $('#viewfileImg').attr('src', fileUrl);
                $('#viewfileImg').css("display", "block");
            }
            $("#viewFileDialog").window('open').window('center');
        }

        //显示剩余美金权益
        function showBalanceValue() {
            $.post(location.pathname + '?action=GetBalanceValue', {
                
            }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $("#BalanceValue").html(result.BalanceValue);
                    BalanceValue = result.BalanceValue;
                } else {
                    
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl" style="padding-left: 5px">是否处理：</td>
                    <td>
                        <input class="easyui-combobox" id="IsPaid" name="IsPaid" data-options="valueField:'Value',textField:'Text',editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">申请日期：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 200px;" />
                    </td>
                    <td style="padding-left: 5px">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="lbl">
                        <span style="font-size: 14px;">客户剩余美金权益：</span>
                        <label id="BalanceValue" style="font-size: 14px;"></label>
                        <span style="font-size: 14px; margin-left: 10px;">客户本次申请总金额：</span>
                        <label id="TotalApplyAmount" style="font-size: 14px;">0</label>
                        <%--<a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="margin-left: 20px;" onclick="Swap()">一键付汇</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="dollarEquityApplys" title="转付汇申请" data-options="
            border: false,
            fitColumns:true,
            fit:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 10px">全选</th>
                    <th data-options="field:'ApplyID',align:'center'" style="width: 6%;">申请ID</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 6%;">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%;">币制</th>
                    <th data-options="field:'SupplierChnName',align:'left'" style="width: 12%;">供应商中文名称</th>
                    <th data-options="field:'SupplierEngName',align:'center'" style="width: 12%;">供应商英文名称</th>
                    <th data-options="field:'BankName',align:'center'" style="width: 12%;">银行名称</th>
                    <th data-options="field:'BankAccount',align:'center'" style="width: 6%;">银行账号</th>
                    <th data-options="field:'SwiftCode',align:'center'" style="width: 6%;">银行代码</th>
                    <th data-options="field:'BankAddress',align:'left'" style="width: 12%;">银行地址</th>
                    <th data-options="field:'IsPaidStr',align:'center'" style="width: 4%;">是否付款</th>
                    <th data-options="field:'btn',width:100,formatter:Operation,align:'center'" style="width: 12%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 600px;">
        <img id="viewfileImg" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

</body>
</html>
