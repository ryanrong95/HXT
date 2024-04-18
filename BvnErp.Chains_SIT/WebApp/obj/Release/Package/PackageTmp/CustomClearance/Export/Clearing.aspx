<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Clearing.aspx.cs" Inherits="WebApp.CustomClearance.Clearing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>清关</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
  <%--  <script>
        gvSettings.fatherMenu = '香港出口清关';
        gvSettings.menu = '待清关';
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
            var VoyageNo = $("#VoyageNo").textbox('getValue');
            var StartTime = $("#StartTime").datebox('getValue');
            var EndTime = $("#EndTime").datebox('getValue');

            $('#datagrid').myDatagrid('search', { VoyageNo: VoyageNo, StartTime: StartTime, EndTime: EndTime, });
        }

        //重置
        function Reset() {
            $("#VoyageNo").textbox('setValue', "");
            $("#StartTime").datebox('setValue', "");
            $("#EndTime").datebox('setValue', "");
            Search();
        }

        //导出Excel
        function Export() {
            var VoyageNo = $("#VoyageNo").textbox('getValue');
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
                VoyageNo: VoyageNo,
                StartTime: StartTime,
                EndTime: EndTime,
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

        //一键清关
        function Declared() {
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);
            //验证成功
            $.post('?action=Declared', {
                IDs: JSON.stringify(strIds),
            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        $('#datagrid').myDatagrid('reload');
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
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Declared()">一键清关</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">运输批次号:</span>
                    <input class="easyui-textbox search" id="VoyageNo" />
                     <span class="lbl">出口日期:</span>
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
        <table id="datagrid" class="easyui-datagrid" title="待清关" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'center'" style="width: 50px">运输批次号</th>
                    <th field="BillNo" data-options="align:'center'" style="width: 50px">运单号</th>
                    <th field="HKLicense" data-options="align:'center'" style="width: 50px">车牌号</th>
                    <th field="LoadingDate" data-options="align:'center'" style="width: 50px">出口日期</th>
                    <th field="CarrierName" data-options="align:'center'" style="width: 50px">承运商</th>
                    <th field="ProductQty" data-options="align:'center'" style="width: 50px">产品数量</th>
                    <th field="TotalPrice" data-options="align:'center'" style="width: 50px">总金额</th>
                    <th field="Currency" data-options="align:'center'" style="width: 50px">币种</th>
                    <th field="Status" data-options="align:'center'" style="width: 50px">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
