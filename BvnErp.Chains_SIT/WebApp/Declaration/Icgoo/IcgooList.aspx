<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IcgooList.aspx.cs" Inherits="WebApp.Declaration.Notice.ExcelList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = 'Icgoo导单(XTD)';
        gvSettings.menu = '导单';
        gvSettings.summary = '报关员导出Icgoo核对Excel';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#orders').myDatagrid();
            setCurrentDate();
        });

        //查询
        function Search() {
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            setCurrentDate();
            Search();
        }


        function DownloadExcel(Origin) {
            MaskUtil.mask();//遮挡层
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                StartDate: StartDate,
                EndDate: EndDate,
                Origin:Origin
            };
            debugger
            $.post("?action=DownloadExcel", parm, function (data) {
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
            var CurrentDate = getNowFormatDate();
            $("#StartDate").datebox("setValue", CurrentDate);
            $("#EndDate").datebox("setValue", CurrentDate);
        }

        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnDownload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="DownloadExcel('')">下载Excel</a>
            <a id="btnDownloadUSA" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="DownloadExcel('USA')">下载美国型号Excel</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">申报起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <%--<a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>--%>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="Icgoo核对列表" data-options="fitColumns:true,fit:true,nowrap:false,toolbar:'#topBar'">
        
        </table>
    </div>
</body>
</html>


