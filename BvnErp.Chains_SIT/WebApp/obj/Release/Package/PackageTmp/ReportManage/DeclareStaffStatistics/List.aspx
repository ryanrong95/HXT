<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ReportManage.DeclareStaffStatistics.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>工作量汇总</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {

            $('#StartTime').datebox('setValue', dateMonth(0));
            $('#EndTime').datebox('setValue', dateMonth(1));

            //订单列表初始化
            $('#datagrid').myDatagrid({
                queryParams: { action: 'data', StartTime: dateMonth(0), EndTime: dateMonth(1) },
                nowrap: false,
            });


        });

        //查询
        function Search() {
            //debugger;
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');

            var parm = {
                StartTime: StartTime,
                EndTime: EndTime,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#StartTime').datebox('setValue', dateMonth(0));
            $('#EndTime').datebox('setValue', dateMonth(1));

            Search();
        }

         //导出Excel
        function ExportExcel() {
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');

            var param = {
                StartTime: StartTime,
                EndTime: EndTime,
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', param, function (result) {
                MaskUtil.unmask();
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

        function dateMonth(number) {
            var nowdays = new Date();
            var year = nowdays.getFullYear();
            var month = nowdays.getMonth();
            if (month == 0) {
                month = 12;
                year = year - 1;
            }
            if (month < 10) {
                month = "0" + month;
            }
            if (number == 0) {
                return year + "-" + month + "-" + "01";
            }
            else {
                var myDate = new Date(year, month, 0);
                return year + "-" + month + "-" + myDate.getDate();
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">报关日期: </td>
                    <td colspan="3">
                        <input class="easyui-datebox" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox" id="EndTime" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
<%--                        <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="统计汇总" data-options="fitColumns:false,fit:true,toolbar:'#topBar',rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'StaffName',align:'left',width:130">岗位</th>
                    <th data-options="field:'AdminName',align:'center',width:150">人员</th>
                    <th data-options="field:'Quantity',align:'center',width:180">工作量(单)</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
