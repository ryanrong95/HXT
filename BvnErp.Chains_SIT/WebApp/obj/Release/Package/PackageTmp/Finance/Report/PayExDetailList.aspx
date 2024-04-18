<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayExDetailList.aspx.cs" Inherits="WebApp.Finance.Report.PayExDetailList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>付汇申请明细</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {

            //订单列表初始化
            $('#datagrid').myDatagrid({
                //queryParams: { StartTime: GetToday(), EndTime: GetToday(), action: "data" },
                nowrap: false,
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });
        });

        //查询
        function Search() {
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');

            var parm = {
                StartDate: StartDate,
                EndDate: EndDate,
                ClientName: ClientName,
                OrderID: OrderID,
                ContrNo: ContrNo
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {          
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ContrNo').textbox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {          
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
                   
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                StartDate: StartDate,
                EndDate: EndDate,
                ClientName: ClientName,
                OrderID: OrderID,
                ContrNo: ContrNo
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        if (rel.url.length > 1) {
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
                        <input class="easyui-textbox search" id="ClientName" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">订单号: </td>
                    <td>
                        <input class="easyui-textbox search" id="OrderID" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">合同号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ContrNo" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">申请时间: </td>
                    <td colspan="3">
                        <input class="easyui-datebox search" id="StartDate" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox search" id="EndDate" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>                   
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="付汇申请明细" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>                  
                    <th data-options="field:'PayExchangeApplyID',align:'left',width:160">付汇编号</th>
                    <th data-options="field:'ClientName',align:'center',width:220">客户名称</th> 
                    <th data-options="field:'OrderID',align:'center',width:150">订单号</th>
                    <th data-options="field:'ContrNo',align:'center',width:150">合同号</th>  
                    <th data-options="field:'Amount',align:'center',width:120">付汇金额</th>
                    <th data-options="field:'PayExchangeRate',align:'center',width:80">付汇汇率</th>
                    <th data-options="field:'AmountRMB',align:'center',width:100">付汇RMB</th>
                    <th data-options="field:'Currency',align:'center',width:80">币种</th>       
                    <th data-options="field:'CreateDate',align:'center',width:200">申请时间</th>              
                </tr>
            </thead>            
        </table>
    </div>
</body>
</html>