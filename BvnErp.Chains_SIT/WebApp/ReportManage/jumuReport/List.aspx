<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ReportManage.jumuReport.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>智能报表</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
 
    <script type="text/javascript"> 
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
        });

       

     

        function Add() {
            var url = location.pathname.replace(/CountryList.aspx/ig, 'CountryInfo.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增国家或地区',
                url: url,
                width: '450px',
                height: '200px',
                onClose: function () {
                    Search();
                }
            });
        }

        function View(reportUrl,reportName) {
            var url = location.pathname.replace(/List.aspx/ig, 'JimuTemplate.aspx?reportUrl='+reportUrl);
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: reportName,
                url: url,
                width: '450px',
                height: '200px',
                onClose: function () {
                    
                }
            });
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ReportUrl + '\',\'' + row.ReportName+'\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
           <%-- <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>--%>
        </div>
       
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报表" class="easyui-datagrid" data-options="fitColumns:true,fit:true,border:false,scrollbarSize:0"  style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true">
            <thead>
                <tr>
                    <th field="ReportName" data-options="align:'center'" style="width: 150px">报名名称</th>
                    <th field="ReportUrl" data-options="align:'center'" style="width: 100px">报名地址</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>