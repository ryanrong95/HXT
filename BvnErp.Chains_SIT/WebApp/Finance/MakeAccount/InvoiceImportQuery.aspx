<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.InvoiceImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开票明细</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '开票通知';
        gvSettings.menu = '开票明细';
        gvSettings.summary = '';
    </script--%>>
    <script type="text/javascript">

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({ nowrap: false, pageSize: 50, });

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
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="开票明细" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                     <%--<th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>--%>
                    <th data-options="field:'InvoiceNo',align:'center'" style="width: 16%;">发票号码</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'InvoiceDate',align:'center'" style="width: 8%;">开票日期</th>
                    <th data-options="field:'InvoiceTypeName',align:'center'" style="width: 6%;">开票类型</th> 
                    <th data-options="field:'Je',align:'center'" style="width: 8%;">金额</th>
                    <th data-options="field:'Se',align:'center'" style="width: 6%;">税额</th>      
                    <th data-options="field:'InCreWord',align:'left'" style="width: 10%;">凭证字</th>
                    <th data-options="field:'InCreNo',align:'left'" style="width: 10%;">凭证号</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

