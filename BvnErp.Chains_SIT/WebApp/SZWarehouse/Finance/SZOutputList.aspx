<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SZOutputList.aspx.cs" Inherits="WebApp.SZWarehouse.Finance.SZOutputList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已出库产品数据</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '财务(SZ)';
        gvSettings.menu = '已出库产品数据';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
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
            var Name = $('#Name').textbox('getValue');
            var Model = $('#Model').textbox('getValue');
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            var parm = {
                Name: Name,
                Model: Model,
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            $('#Model').textbox('setValue', null);
            $('#InvoiceCompany').textbox('setValue', null);
            $('#StartTime').datebox('setValue', null);
            $('#EndTime').datebox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var Name = $('#Name').textbox('getValue');
            var Model = $('#Model').textbox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            //验证成功
            $.post('?action=Export', {
                Name: Name,
                Model: Model,
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany
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
            <table>
                <tr>
                    <td class="lbl">客户名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="InvoiceCompany" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">报关日期: </td>
                    <td>
                        <input class="easyui-datebox search" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox search" id="EndTime" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                    <td>
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">商品名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="Name" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">规格型号: </td>
                    <td>
                        <input class="easyui-textbox search" id="Model" data-options="height:26,width:150" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="已出库产品" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'UpdateDate',align:'center',width:150">出库日期</th>
                    <th data-options="field:'Name',align:'center',width:200">产品名称</th>
                    <th data-options="field:'Model',align:'center',width:200">规格型号</th>
                    <th data-options="field:'Brand',align:'center',width:100">品牌</th>
                    <th data-options="field:'TotalQuantity',align:'center',width:100">订单数量</th>
                    <th data-options="field:'Quantity',align:'center',width:100">出库数量</th>
                    <th data-options="field:'InUnitPrice',align:'center',width:100">进价</th>
                    <th data-options="field:'CostAmount',align:'center',width:100">成本</th>
                    <th data-options="field:'OutUnitPrice',align:'center',width:100">销价</th>
                    <th data-options="field:'SalesAmount',align:'center',width:100">销售金额</th>
                    <th data-options="field:'InterestRate',align:'center',width:100">利率</th>
                    <th data-options="field:'TaxRate',align:'center',width:100">税率</th>
                    <th data-options="field:'TaxAmount',align:'center',width:100">税额</th>
                    <th data-options="field:'TotalAmount',align:'center',width:100">价税</th>
                    <th data-options="field:'TotalInterestRate',align:'center',width:100">总利率</th>
                    <th data-options="field:'InvoiceCompany',align:'center',width:200">客户</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
