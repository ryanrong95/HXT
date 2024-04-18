<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeclareImport.aspx.cs" Inherits="WebApp.Finance.MakeAccount.DeclareImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关进口</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--    <script>
        gvSettings.fatherMenu = '生成凭证';
        gvSettings.menu = '报关进口';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                pageSize: 200,
                rownumbers: true,
                singleSelect: false,
                onLoadSuccess: function (data) {
                },
            });

            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
            })
        });

        //查询
        function Search() {
            var invoiceType = $('#InvoiceType').combobox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {
                InvoiceType: invoiceType,
                CompanyName: companyName,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#InvoiceType').combobox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //
        function MakeAccount() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
            }

            //验证成功
            $.post('?action=MakeAccount', {
                Model: JSON.stringify(data)
            }, function (result) {
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', "生成凭证失败", 'info', function () { });
                }
            });
        }

         function MakeAccountAll() {
            var invoiceType = $('#InvoiceType').combobox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            //验证成功
            $.post('?action=MakeAccountAll', {
                InvoiceType: invoiceType,
                CompanyName: companyName,
                StartDate: startDate,
                EndDate: endDate
            }, function (result) {
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
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
          
                <tr>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">开票类型:</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">报关日期:</td>
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
                        <a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccount()">生成凭证</a>
                        <a id="btnMakeAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccountAll()">全部生成凭证</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报关进口" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 5%">报关日期</th>
                    <th data-options="field:'Tian',align:'center'" style="width: 4%">天</th>
                    <th data-options="field:'ImportPrice',align:'center'" style="width: 4%">委托报关（RMB)</th>
                    <th data-options="field:'YunBaoZa',align:'center'" style="width: 4%">运保杂</th>
                    <th data-options="field:'Tariff',align:'center'" style="width: 4%">应交关税RMB</th>
                    <th data-options="field:'ActualTariff',align:'center'" style="width: 4%">实交关税RMB</th>
                    <th data-options="field:'ExciseTax',align:'center'" style="width: 4%">消费税</th>
                    <th data-options="field:'ActualExciseTax',align:'center'" style="width: 4%">消费税实缴</th>
                    <th data-options="field:'ActualAddedValueTax',align:'center'" style="width: 4%">增值税RMB</th>
                    <th data-options="field:'ExchangeCustomer',align:'center'" style="width: 4%">委托金额-汇兑</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 12%">公司</th>
                    <th data-options="field:'ExchangeXDT',align:'center'" style="width: 4%">运保杂-汇兑</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%">币种</th>
                    <th data-options="field:'RealExchangeRate',align:'center'" style="width: 4%">汇率</th>
                    <th data-options="field:'DecAgentTotal',align:'center'" style="width: 6%">委托金额usd</th>
                    <th data-options="field:'ConsignorCode',align:'left'" style="width: 10%">物流方公司</th>
                    <th data-options="field:'DecYunBaoZaTotal',align:'center'" style="width: 4%">运保杂usd</th>
                    <th data-options="field:'InvoiceTypeName',align:'center'" style="width: 6%">开票类型</th>
                    <th data-options="field:'Check',align:'center'" style="width: 6%">检验</th>
                    <th data-options="field:'Addition1',align:'center'" style="width: 3%">留空</th>
                    <th data-options="field:'Tian1',align:'center'" style="width: 4%">天</th>
                    <th data-options="field:'DecAgentTotal1',align:'center'" style="width: 4%">委托金额usd</th>
                    <th data-options="field:'ActualTariff1',align:'center'" style="width: 4%">实交关税RMB</th>
                    <th data-options="field:'Addition2',align:'center'" style="width: 3%">留空</th>
                    <th data-options="field:'ExchangeCustomerOpposite',align:'center'" style="width: 6%">委托金额-汇兑</th>
                    <th data-options="field:'ImportPrice1',align:'center'" style="width: 6%">委托报关（RMB)</th>
                    <th data-options="field:'Tariff1',align:'center'" style="width: 6%">应交关税RMB</th>
                    <th data-options="field:'Addition3',align:'center'" style="width: 3%">留空</th>
                    <th data-options="field:'ClientName1',align:'left'" style="width: 12%">公司</th>
                    <th data-options="field:'RealExchangeRate1',align:'center'" style="width: 4%">汇率</th>
                    <th data-options="field:'Check1',align:'center'" style="width: 6%">检验</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
