<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutStock.aspx.cs" Inherits="WebApp.HKWarehouse.Exit.OutStock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>出库通知展示</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotices%>)');
        var NoticeStatus = getQueryString("Status");
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
            initExitNotice();
        });

        function initExitNotice() {
            if (NoticeStatus == 4) {
                $(".HideDiv").hide();
            } else {
                $(".HideDiv").show();
            }

            $("#ExitNoticeID").text(ExitNotice.NoticeID);
            $("#DriverName").text(ExitNotice.DriverName);
            $("#DriverCode").text(ExitNotice.DriverCode);
            $("#Address").text(ExitNotice.Address);
            $("#VehicleNo").text(ExitNotice.VehicleNo);
            $("#PackNo").text(ExitNotice.PackNo);
            $("#VoyageNo").text(ExitNotice.VoyageNo);
            $("#BillNo").text(ExitNotice.BillNo);

        }
        //确认出库
        function ConfirmExitStock() {
            var id = getQueryString('ExitNoticeID');
            MaskUtil.mask();//遮挡层
            $.post('?action=SaveOutStock', {
                ID: id,
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        Back();
                    }
                });
            });
        }
        //返回
        function Back() {
            var Status = getQueryString("Status");
            if (Status == 1)
            {
                var url = location.pathname.replace(/OutStock.aspx/ig, 'UnExitedList.aspx');
                window.location = url;
            }
            else
            {
                var url = location.pathname.replace(/OutStock.aspx/ig, 'ExitedList.aspx');
                window.location = url;
            }
        }

        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['CaseNumber'] == data.rows[i - 1]['CaseNumber']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'CaseNumber',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
        }
    </script>
    <style>
        html {
            height: 100%;
        }

        body {
            min-height: 100%;
        }
    </style>
</head>
<body>
    <div class="easyui-panel" style="width: 100%; overflow-y: scroll;" title="出库" data-options="fit:true,border:false">
        <div data-options="region:'north'" data-options="fit:true,border:false">
            <div class="easyui-panel" data-options="fit:true,border:false">
                <div id="topBar" style="margin-bottom: 15px; margin-top: 15px; margin-left: 20px">
                    <div id="tool">
                        <a href="javascript:void(0);" class="easyui-linkbutton HideDiv" data-options="iconCls:'icon-ok'" onclick="ConfirmExitStock()">确认出库</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()" data-options="iconCls:'icon-back'">返回</a>
                    </div>
                </div>
            </div>
        </div>
        <div class="easyui-layout" data-options="fit:true,border:false" style="margin-left: 5px; height: 80%">
            <div data-options="region:'west',collapsible:false,border:false,split:true" style="width: 75%; height: 100%">
                <div id="data" class="easyui-panel" data-options="fit:true,border:false">
                    <%--列表--%>
                    <table id="datagrid" title="出库产品" data-options="
                    fitColumns:true,
                    fit:true,
                    border:true,
                    scrollbarSize:0,
                    nowrap:false,
                    onLoadSuccess: onLoadSuccess,">
                        <thead>
                            <tr>
                                <th field="StockCode" data-options="align:'center'" style="width: 150px">库位号</th>
                                <th field="PackingDate" data-options="align:'center'" style="width:150px">装箱日期</th>
                                <th field="CaseNumber" data-options="align:'left'" style="width: 160px">箱号</th>
                                <th field="Model" data-options="align:'left'" style="width: 160px">型号</th>
                                <th field="ProductName" data-options="align:'left'" style="width: 180px">报关品名</th>
                                <th field="Manufactor" data-options="align:'left'" style="width: 150px">品牌</th>
                                <th field="Quantity" data-options="align:'center'" style="width:100px">数量</th>
                                <th field="NetWeight" data-options="align:'center'" style="width: 100px">净重(KG)</th>
                                <th field="GrossWeight" data-options="align:'center'" style="width: 100px">毛重(KG)</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
            <div data-options="region:'center',border:false,split:true" style="min-width: 24%">
                <div id="panel1" class="easyui-panel" title="运单" fit="true">
                    <form id="form5" style="margin-bottom: 20px">
                        <table id="table1" style="margin-left: 10px; line-height: 30px;">
                            <tr style="height: 30px;">
                                <td>
                                    <label>运输批次号：</label></td>
                                <td>
                                    <label id="VoyageNo"></label>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    <label>车牌号：</label></td>
                                <td>
                                    <label id="VehicleNo"></label>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    <label>运单号：</label></td>
                                <td>
                                    <label id="BillNo"></label>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    <label>司机代码：</label></td>
                                <td>
                                    <label id="DriverCode"></label>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    <label>司机姓名：</label></td>
                                <td>
                                    <label id="DriverName"></label>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    <label>总件数：</label></td>
                                <td>
                                    <label id="PackNo"></label>
                                </td>
                            </tr>
                        </table>
                    </form>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
