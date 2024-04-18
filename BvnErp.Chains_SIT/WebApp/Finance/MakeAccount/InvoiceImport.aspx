<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceImport.aspx.cs" Inherits="WebApp.Finance.MakeAccount.InvoiceImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开票明细</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>    
   <%-- <script>
        gvSettings.fatherMenu = '开票通知';
        gvSettings.menu = '开票明细';
        gvSettings.summary = '';
    </script--%>>
    <script type="text/javascript">

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({ nowrap: false, pageSize: 50,  singleSelect: false,});

        });

        //查询
        function Search() {
            var invoiceNo = $('#InvoiceNo').textbox('getValue');
            var comanyName = $('#ComanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {

                InvoiceNo: invoiceNo,
                ComanyName: comanyName,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#InvoiceNo').textbox('setValue', null);
            $('#ComanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

          function MakeAccount() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
            }

            //验证成功
            MaskUtil.mask();//遮挡层
            $.post('?action=MakeAccount', {
                Model: JSON.stringify(data)
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
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
            var invoiceNo = $('#InvoiceNo').textbox('getValue');
            var comanyName = $('#ComanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeAccountAll', {               
                InvoiceNo: invoiceNo,
                ComanyName: comanyName,
                StartDate: startDate,
                EndDate: endDate
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
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
                    <td class="lbl">订单编号:</td>
                    <td>
                        <input class="easyui-textbox" id="InvoiceNo" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">客户名称:</td>
                    <td>
                        <input class="easyui-textbox" id="ComanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">开票日期:</td>
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
                        <a id="btnMakeAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccountAll()">生成全部凭证</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="开出发票" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                     <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'InvoiceNo',align:'center'" style="width: 16%;">发票号码</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'InvoiceDate',align:'center'" style="width: 8%;">开票日期</th>
                    <th data-options="field:'InvoiceTypeName',align:'center'" style="width: 6%;">开票类型</th>        
                    <th data-options="field:'Je',align:'center'" style="width: 8%;">金额</th>
                    <th data-options="field:'Se',align:'center'" style="width: 6%;">税额</th>        
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
