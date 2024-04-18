<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExitPrint.aspx.cs" Inherits="WebApp.HKWarehouse.Exit.ExitPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
<%--    <script>
        gvSettings.fatherMenu = '出库通知(HK)';
        gvSettings.menu = '打印出库单';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            $('#StartDDate').datebox('setValue', formatterDate(new Date()));
            $('#EndDDate').datebox('setValue', formatterDate(new Date()));

            var url = location.pathname.replace(/ExitPrint.aspx/ig, 'ExitPrint.aspx?subAction=exitList');
            url = url + '&StartDDate=' + $('#StartDDate').textbox('getValue') + '&EndDDate=' + $('#EndDDate').textbox('getValue');
            $('#exitList-datagrid').myDatagrid({
                url: url,
                actionName:'data',
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });
        });

        //查询按钮
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var VoyNo = $('#VoyNo').textbox('getValue');
            var StartDDate = $('#StartDDate').textbox('getValue');
            var EndDDate = $('#EndDDate').textbox('getValue');

            var url = location.pathname.replace(/ExitPrint.aspx/ig, 'ExitPrint.aspx?subAction=exitList');
            url = url + '&ClientName=' + ClientName + '&VoyNo=' + VoyNo + '&StartDDate=' + StartDDate + '&EndDDate=' + EndDDate;
            //$('#exitList-datagrid').myDatagrid({
            //    url: url,
            //});
                var params = { url: url };
            //$('#exitList-datagrid').myDatagrid({
            //    url: url,
            //});
              this.datagrid(params);
        }

        //重置按钮
        function Reset() {
            $("#ClientName").textbox('setValue', "");
            $("#VoyNo").textbox('setValue', "");
            $('#StartDDate').datebox('setValue', formatterDate(new Date()));
            $('#EndDDate').datebox('setValue', formatterDate(new Date()));
           
            var url = location.pathname.replace(/ExitPrint.aspx/ig, 'ExitPrint.aspx?subAction=exitList');
            url = url + '&StartDDate=' + $('#StartDDate').textbox('getValue') + '&EndDDate=' + $('#EndDDate').textbox('getValue');
            var params = { url: url };
            //$('#exitList-datagrid').myDatagrid({
            //    url: url,
            //});
              this.datagrid(params);
        }

        //显示当前日期
        function formatterDate(date) {
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0" + (date.getMonth() + 1);
            return date.getFullYear() + '-' + month + '-' + day;
        };

        //导出Excel
        function Export(clientID, voyNo, dDate) {
            MaskUtil.mask();
            var url = '?action=Export&ClientID=' + clientID + "&VoyNo=" + voyNo + "&DDate=" + dDate;
            $.post(url, {

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
            });
        }

        //显示报关日期
        function ShowDDate(val, row, index) {
            return row.DDate.substring(0, 10);
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" '
                + 'onclick="Export(\'' + row.ClientID + '\', \'' + row.VoyNo + '\', \'' + row.DDate.substring(0, 10) + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">导出 Excel</span>' +
                '<span class="l-btn-icon icon-yg-excelExport">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <style>
        #exitList-topBar table td {
            height: 30px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="exitList-topBar">
        <table>
            <tr>
                <td class="lbl" style="padding-left: 5px;">客户名称：</td>
                <td>
                    <input class="easyui-textbox" data-options="height:26,width:200" id="ClientName" />
                </td>
                <td class="lbl" style="padding-left: 5px;">运输批次号：</td>
                <td>
                    <input class="easyui-textbox" data-options="height:26,width:200" id="VoyNo" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="padding-left: 5px;">报关日期：</td>
                <td>
                    <input class="easyui-datebox" data-options="height:26,width:200" id="StartDDate" />
                </td>
                <td class="lbl" style="padding-right: 12px; text-align: right;">至</td>
                <td>
                    <input class="easyui-datebox" data-options="height:26,width:200" id="EndDDate" />
                </td>
                <td style="padding-left: 5px">
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                </td>
                <td style="padding-left: 5px">
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false,">
        <table id="exitList-datagrid" title="打印出库单" data-options="
            nowrap:false,
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#exitList-topBar'">
            <thead>
                <tr>
                    <th field="ClientName" data-options="align:'left'" style="width: 20%">客户名称</th>
                    <th field="VoyNo" data-options="align:'center'" style="width: 20%">运输批次号</th>
                    <th field="DDate" data-options="align:'center',formatter:ShowDDate" style="width: 20%">报关日期</th>
                    <th field="TotalPackNo" data-options="align:'center'" style="width: 20%">总件数</th>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 20%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
