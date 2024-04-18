<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ReportManage.ClassifiedStatistics.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            //归类产品列表初始化
            setCurrentDate();

            //订单列表初始化
            $('#products').myDatagrid({
                queryParams: { action: 'data', StartDate: dateMonth(0), EndDate: dateMonth(1) },
                nowrap: false,
                pagination: false
            });

        });

        //重置查询条件
        function Search() {
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                StartDate: StartDate,
                EndDate: EndDate
            };
            $('#products').myDatagrid({
                queryParams: parm,
                nowrap: false,
                pagination: false
            });
        }

        //重置查询条件
        function Reset() {
            setCurrentDate();
        }

        //导出归类统计数据
        function ExportExcel() {
            MaskUtil.mask();//遮挡层
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                StartDate: StartDate,
                EndDate: EndDate
            };
            debugger
            $.post("?action=ExportExcel", parm, function (data) {
                MaskUtil.unmask();
                var result = JSON.parse(data);
                if (result.result) {
                    Download(result.url);
                    $.messager.alert('消息', result.info);
                } else {
                    $.messager.alert('消息', result.info);
                }
            });
        }

        function Download(Url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = Url;
            a.download = "";
            a.click();
        }

        
    </script>
    <script>
        function setCurrentDate() {
            $("#StartDate").datebox("setValue", dateMonth(0));
            $("#EndDate").datebox("setValue", dateMonth(1));
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
        <div id="tool">
            <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">归类时间 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="归类统计" data-options="fitColumns:true,fit:true,nowrap:false,toolbar:'#topBar',rownumbers:true">
         <thead>
                <tr>
                    <th data-options="field:'AdminName',align:'left',width:130">归类人员</th>
                    <th data-options="field:'Pre1Count',align:'center',width:150">产品预归类_预处理一</th>
                    <th data-options="field:'Pre2Count',align:'center',width:160">产品预归类_预处理二</th>
                    <th data-options="field:'Consult1Count',align:'center',width:160">咨询归类_预处理一</th>
                    <th data-options="field:'Consult2Count',align:'center',width:160">咨询归类_预处理二</th>
                    <th data-options="field:'Classify1Count',align:'center',width:160">产品归类_预处理一</th>
                    <th data-options="field:'Classify2Count',align:'center',width:160">产品归类_预处理二</th>
                    <th data-options="field:'TotalCount',align:'center',width:100">归类条数</th>
                    <th data-options="field:'Percent',align:'center',width:100">归类占比</th>
                    <th data-options="field:'Summary',align:'center',width:180">备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
