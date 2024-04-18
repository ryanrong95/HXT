<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancePaymentImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.FinancePaymentImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   
    <script type="text/javascript">
      
        $(function () {
          
            //代理订单列表初始化
            $('#payments').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
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
                }
            });
        });

        //查询
        function Search() {
           
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#payments').myDatagrid('search', {  StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
          
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

     

        //
        function MakeAccount() {
            var data = $('#payments').myDatagrid('getSelections');
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
                        $('#payments').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', "生成凭证失败", 'info', function () { });
                }
            });
        }

      

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 26px">              
                <tr>
                    <td class="lbl">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" style="height: 26px; width: 200px" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" style="height: 26px; width: 200px" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <%-- <a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccount()">生成凭证</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="payments" title="财务付款" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 60px">流水号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 60px">收款方</th>
                    <th data-options="field:'FinanceVault',align:'center'" style="width: 40px">付款金库</th>
                    <th data-options="field:'FinanceAccount',align:'center'" style="width: 40px">付款账户</th>
                    <th data-options="field:'BankName',align:'center'" style="width: 60px">银行</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 40px">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 20px">币种</th>
                    <th data-options="field:'Payer',align:'center'" style="width: 30px">付款人</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 40px">付款日期</th>
                    <th data-options="field:'FinPCreWord',align:'center'" style="width: 30px">凭证字</th>
                    <th data-options="field:'FinPCreNo',align:'center'" style="width: 40px">凭证号</th>
                   <%-- <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 60px">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
