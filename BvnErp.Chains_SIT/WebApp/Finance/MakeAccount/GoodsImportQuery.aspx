<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.GoodsImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>收款记录查询</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '收付款管理';
        gvSettings.menu = '收款';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        //数据初始化
        $(function () {
            //下拉框数据初始化
            //金库下拉框

            $('#FinanceVault').combobox({
                data: FinanceVaultData,            
                onHidePanel: function () {                   
                    var selectedVault = $("#FinanceVault").combobox("getValue");                   
                    if (selectedVault != null) {
                        $.post('?action=GetAccountByVault', { FinanceVault: selectedVault }, function (res) {
                            var accounts = JSON.parse(res.data)
                            $('#Account').combobox('setValue', '');
                            $('#Account').combobox('loadData', accounts );
                        });
                    }
                }
            });
            //账户下拉框初始化
            var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');
            $('#Account').combobox({
                data: FinanceAccountData,
            });

            //收款记录列表初始化
            $('#receiptRecords').myDatagrid({
                singleSelect: false,
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
        });

        //查询
        function Search() {          
            var payer = $('#Payer').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var financeVault = $('#FinanceVault').combobox('getValue');
            var account = $('#Account').combobox('getValue');
            var parm = {              
                Payer: payer,
                StartDate: startDate,
                EndDate: endDate,
                FinanceVault: financeVault,
                Account: account
            };
            $('#receiptRecords').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {         
            $('#Payer').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#FinanceVault').combobox('setValue', null);
            $('#Account').combobox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">       
        <div id="search">
            <table>
                <tr>                    
                    <td class="lbl">付款人:</td>
                    <td>
                        <input class="easyui-textbox" id="Payer"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">金库:</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" data-options="valueField:'Value',textField:'Text',editable:false" />
                    </td>
                    <td class="lbl">账户:</td>
                    <td>
                        <input class="easyui-combobox" id="Account" data-options="valueField:'Value',textField:'Text',editable:false"/>
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
        <table id="receiptRecords" title="预收账款" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar',singleSelect:false">
            <thead>
                <tr>                 
                    <th data-options="field:'RequestID',align:'left'" style="width: 13%;">请求ID</th>
                    <th data-options="field:'SerialNumber',align:'left'" style="width: 13%;">流水号</th>
                    <th data-options="field:'Vault',align:'left'" style="width: 5%;">金库</th>
                    <th data-options="field:'Account',align:'left'" style="width: 10%;">账户名称</th>
                    <th data-options="field:'Payer',align:'left'" style="width: 12%;">付款人</th>                   
                    <th data-options="field:'ReceiptType',align:'center'" style="width: 6%;">收款方式</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 6%;">金额</th>
                    <th data-options="field:'ExchangeRate',align:'center'" style="width: 3%;">汇率</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 3%;">币种</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 5%;">收款日期</th> 
                    <th data-options="field:'GoodsCreWord',align:'left'" style="width: 10%;">凭证字</th>
                    <th data-options="field:'GoodsCreNo',align:'left'" style="width: 10%;">凭证号</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
