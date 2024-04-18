<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.ExcludeOriginTariffs.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>对美关税排除</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '收付款管理';
        gvSettings.menu = '账户流水';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            $('#AccountFlows').myDatagrid({
                fitColumns: true, fit: true,
                nowrap: false,
                toolbar: '#topBar',
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

            //注册上传产品信息filebox的onChange事件
            $('#uploadExcel').filebox({
                onClickButton: function () {
                    $('#uploadExcel').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadExcel').filebox('getValue') == '') {
                        return;
                    }
                    MaskUtil.mask();
                    var formData = new FormData($('#form1')[0]);
                    $.ajax({
                        url: '?action=ImportAccountFlow',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message, 'info', function () {
                                $('#AccountFlows').myDatagrid('reload');
                            });
                        }
                    }).done(function (res) {

                    });
                }
            });

        });

        function Search() {
            var hscode = $("#HSCode").textbox('getValue');
            var exclusionPeriod = $("#ExclusionPeriod").textbox('getValue');
            debugger
            var parm = {
                HSCode : hscode,
                ExclusionPeriod : exclusionPeriod               
            };
            $('#AccountFlows').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $("#HSCode").textbox('setValue', null);
            $('#ExclusionPeriod').textbox('setValue', null);

            Search();
        }

    </script>
</head>

<body class="easyui-layout">
    <div id="topBar">
        <form id="form1">
            <div id="search">
                <ul>
                    <li>
                        <div style="margin-left: 10px">
                            <%--<a id="btnDownload" href="../../Content/templates/银行流水导入模板.xlsx" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">下载导入模板</a>--%>
                            <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 102px; height: 26px; padding-left: 10px;"
                                data-options="region:'center',buttonText:'导入排除税号',buttonIcon: 'icon-add',
                                          accept:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'" />
                        </div>
                        <br />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">海关编码:</span>
                        <input class="easyui-textbox" id="HSCode" data-options="validType:'length[1,50]'" />
                        <span style="width: 80px; margin-left: 10px; display: inline-block">采购计划月:</span>
                        <input class="easyui-textbox" id="ExclusionPeriod" data-options="validType:'length[1,50]'" />

                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 10px;" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>

                    </li>
                </ul>
            </div>
        </form>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="AccountFlows" title="对美排除" data-options="fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'HSCode',align:'left'" style="width: 80px;">海关编码</th>
                    <th data-options="field:'Name',align:'left'" style="width: 250px;">名称</th>
                    <th data-options="field:'ExclusionPeriod',align:'left'" style="width: 100px;">采购计划月份</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
