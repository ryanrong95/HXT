<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cleared.aspx.cs" Inherits="WebApp.OrderWaybill.Cleared" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>香港进口清关</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
 <%--   <script>
        gvSettings.fatherMenu = '香港进口清关';
        gvSettings.menu = '已清关';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
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

            //设置系统当前时间
            var curr_time = new Date();
            var str = curr_time.getMonth() + 1 + "/";
            str += curr_time.getDate() + "/";
            str += curr_time.getFullYear() + " ";
            str += curr_time.getHours() + ":";
            str += curr_time.getMinutes() + ":";
            str += curr_time.getSeconds();
            $('#EndTime').datebox('setValue', str);
        });

        //查询
        function Search() {
            var StartTime = $("#StartTime").datebox('getValue');
            var EndTime = $("#EndTime").datebox('getValue');

            $('#datagrid').myDatagrid('search', { StartTime: StartTime, EndTime: EndTime, });
        }

        //重置
        function Reset() {
            $("#StartTime").datebox('setValue', "");
            $("#EndTime").datebox('setValue', "");
            Search();
        }

        //导出Excel
        function Export() {
            var StartTime = $("#StartTime").datebox('getValue');
            var EndTime = $("#EndTime").datebox('getValue');
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            MaskUtil.mask();
            //验证成功
            $.post('?action=Export', {
                StartTime: StartTime,
                EndTime: EndTime,
                Data: JSON.stringify(data),
            }, function (result) {
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
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">运输批次号:</span>
                    <input class="easyui-textbox search" id="VoyageNo" />
                     <span class="lbl">到港日期:</span>
                    <input class="easyui-datebox search" id="StartTime" />
                     <span class="lbl">至:</span>
                    <input class="easyui-datebox search" id="EndTime" />
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="已清关" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="ArrivalDate" data-options="align:'center'" style="width: 12%">到港日期</th>
                    <th field="WaybillCode" data-options="align:'center'" style="width: 12%">运单号</th>
                    <th field="CarrierName" data-options="align:'center'" style="width: 12%">快递公司</th>
                    <th field="TotalCount" data-options="align:'center'" style="width: 12%">产品总数量</th>
                    <th field="TotalPrice" data-options="align:'center'" style="width: 12%">产品总金额</th>
                    <th field="Currency" data-options="align:'center'" style="width: 12%">币种</th>
                    <th field="Status" data-options="align:'center'" style="width: 12%">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
