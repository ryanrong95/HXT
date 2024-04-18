<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderBillDisplay.aspx.cs" Inherits="WebApp.HKWarehouse.Finance.OrderBillDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>合同发票</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                scrollbarSize: 0,
                fit: false,
                border: true,
                pagination: false,
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
                }
            });
            $('#PiFile').myDatagrid({
                fitColumns: true,
                fit: false,
                scrollbarSize: 0,
                border: false,
                pagination: false,
                actionName:'filedata',
                loadFilter: function (data1) {
                    for (var index = 0; index < data1.rows.length; index++) {
                        var row = data1.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data1;
                },
                onLoadSuccess: function () {
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");
                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");
                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });
                }
            });
            $('#DecHeads').myDatagrid({
                fitColumns: true,
                fit: false,
                scrollbarSize: 0,
                border: false,
                pagination: false,
                actionName:'decdata',
                loadFilter: function (data1) {
                    for (var index = 0; index < data1.rows.length; index++) {
                        var row = data1.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data1;
                },

            });
        });
        //导出票据
        function Export(Index) {
            $('#DecHeads').datagrid('selectRow', Index);
            var rowdata = $('#DecHeads').datagrid('getSelected');
            var ID = rowdata.ID;

            $.post('?action=ExportPIFile', {
                ID: ID,
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
        //返回
        function Back() {
            var url = location.pathname.replace(/OrderBillDisplay.aspx/ig, 'OrderList.aspx');
            window.location = url;
        }
        //进项PI文件
        function Operation(val, row, index) {
            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.WebUrl + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        //销项PI文件
        function Operation1(val, row, index) {
            var buttons = '<a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Export(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">导出</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false">
        <%--返回--%>
        <div style="margin: 10px 0;">
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()"
                data-options="iconCls:'icon-back'">返回</a>
        </div>
        <div data-options="region:'center',border:false">
            <%--进项PI--%>
            <div>
                <table id="PiFile"  title="合同发票（进项）">
                    <thead>
                        <tr>
                            <th field="FileName" data-options="align:'left'" style="width: 50px">文件名称</th>
                            <th field="URL" data-options="align:'left'" style="width: 100px">文件地址</th>
                            <%--<th field="FileFormat" data-options="align:'left'" style="width: 50px">文件格式</th>--%>
                            <th field="CreateDate" data-options="align:'center'" style="width: 50px">上传时间</th>
                            <th data-options="field:'btn',width:50,formatter:Operation,align:'center'">操作</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <%--报关单--%>
            <div style="margin-top: 20px">
                <table id="DecHeads"  title="合同发票（销项）">
                    <thead>
                        <tr>
                            <th data-options="field:'ContrNo',align:'center'" style="width: 40px;">合同号</th>
                            <th data-options="field:'ConsigneeName',align:'left'" style="width: 60px;">收货人</th>
                            <th data-options="field:'TransMode',align:'center'" style="width: 20px;">成交方式</th>
                            <th data-options="field:'PackNo',align:'center'" style="width: 20px;">件数</th>
                            <th data-options="field:'CreateDate',align:'center'" style="width: 30px;">日期</th>
                            <th data-options="field:'btn',width:30,formatter:Operation1,align:'center'">操作</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <%--运单信息--%>
            <div style="margin-top: 20px; display: none">
                <table id="datagrid" class="mygrid" title="运单信息">
                    <thead>
                        <tr>
                            <th field="CompanyName" data-options="align:'center'" style="width: 50px">快递公司</th>
                            <th field="WaybillCode" data-options="align:'center'" style="width: 50px">运单编号</th>
                            <th field="ArrivalTime" data-options="align:'center'" style="width: 50px">到货时间</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
