<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceDetail.aspx.cs" Inherits="WebApp.Finance.InvoiceDetail" %>

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
            var orderID = $('#OrderID').textbox('getValue');
            var comanyName = $('#ComanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {

                OrderID: orderID,
                ComanyName: comanyName,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ComanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
             var orderID = $('#OrderID').textbox('getValue');
            var comanyName = $('#ComanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            //验证成功
            $.post('?action=Export', {
                OrderID: orderID,
                ComanyName: comanyName,
                StartDate: startDate,
                EndDate: endDate
                // Data: JSON.stringify(data),
            }, function (result) {
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
            })
        }




    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td colspan="4">
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">订单编号:</td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="height:26,width:200,validType:'length[1,50]'" />
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
                    <th data-options="field:'ProductName',align:'left'" style="width:10%;">品名</th>
                    <th data-options="field:'ProductModel',align:'left'" style="width: 10%;">型号</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 6%;">单位</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 6%;">数量</th>
                    <th data-options="field:'DetailUnitPrice',align:'center'" style="width: 8%;">含税单价</th>
                    <th data-options="field:'DetailAmount',align:'center'" style="width: 8%;">含税金额</th>
                    <th data-options="field:'Name',align:'left'" style="width: 16%;">开票公司</th>
                    <th data-options="field:'InvoiceNo',align:'center'" style="width: 16%;">发票号码</th>
                    <th data-options="field:'UpdateDate',align:'center'" style="width: 8%;">开票日期</th>
                    <th data-options="field:'TaxAmount',align:'center'" style="width: 8%;">税额</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 8%;">订单号</th>
                    <%--  <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 50px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
