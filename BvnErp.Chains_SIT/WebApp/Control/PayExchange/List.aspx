<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Control.PayExchange.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>付汇查看</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                border: false,
                scrollbarSize: 0,
                toolbar: '#topBar',
                nowrap: false,
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

        function Search() {
            var ClientCode = $("#ClientCode").textbox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            $('#datagrid').myDatagrid('search', { ClientCode: ClientCode, StartDate: StartDate, EndDate: EndDate });
        }

        function Reset() {
            $("#ClientCode").textbox('setValue', "");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看
        function Details(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?ID=" + rowdata.ID;
                window.location = url;
            }
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetails" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Details(' + index + ')" group >' +
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
        <div id="search">
            <table id="table1" style="margin: 5px 0 5px 0">
                <tr>
                    <td class="lbl">客户编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="ClientCode" />
                    </td>
                    <td class="lbl">申请时间：</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="StartDate" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="EndDate" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="付汇申请记录" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="ID" data-options="align:'center'" style="width: 120px">申请编号</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 120px">申请日期</th>
                    <th field="ClientCode" data-options="align:'center'" style="width: 80px">客户编号</th>
                    <th field="TotalAmount" data-options="align:'center'" style="width: 80px">金额</th>
                    <th field="Currency" data-options="align:'center'" style="width: 80px">币种</th>
                    <th field="ExchangeRate" data-options="align:'center'" style="width: 80px">汇率</th>
                    <th field="SupplierName" data-options="align:'left'" style="width: 200px">供应商名称</th>
                    <th field="BankName" data-options="align:'left'" style="width: 160px">银行名称</th>
                    <th field="BankAccount" data-options="align:'left'" style="width: 160px">银行账号</th>
                    <th field="Status" data-options="align:'center'" style="width: 50px">状态</th>
                    <th field="FatherID" data-options="align:'center'" style="width: 50px">付汇类型</th>
                    <th data-options="field:'btn',width:150,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
