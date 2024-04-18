<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.HKWarehouse.Temporary.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>综合查询暂存记录</title>
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '暂存通知';
        gvSettings.menu = '综合查询';
        gvSettings.summary = '';
    </script>--%>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns:true,
                fit: true,
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
            var EntryNumber = $("#EntryNumber").val();
            var CompanyName = $("#CompanyName").val();
            var WaybillCode = $("#WaybillCode").val();
            var Status = $("#Status").combobox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            $('#datagrid').myDatagrid('search', {
                EntryNumber: EntryNumber,
                CompanyName: CompanyName,
                WaybillCode: WaybillCode,
                Status: Status,
                StartDate: StartDate,
                EndDate: EndDate
            });
        }

        function Reset() {
            $("#EntryNumber").textbox('setValue', "");
            $("#CompanyName").textbox('setValue', "");
            $("#WaybillCode").textbox('setValue', "");
            $("#Status").combobox('setValue', "");
            $("#StartDate").datebox('setValue', "");
            $("#EndDate").datebox('setValue', "");
            Search();
        }

        //详情
        function Detial(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?ID=" + ID;;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '查看',
                width: '1000px',
                height: '500px',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetial" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Detial(\'' + row.ID + '\')" group >' +
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
                    <td class="lbl" style="padding-left: 0px">入仓号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="EntryNumber" />
                    </td>
                    <td class="lbl">公司名称：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="CompanyName" />
                    </td>
                    <td class="lbl">运单号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="WaybillCode" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">状态：</td>
                    <td>
                        <select class="easyui-combobox" id="Status" name="Status"
                            data-options="height:26,width:150">
                            <option value=""></option>
                            <option value="<%=Needs.Ccs.Services.Enums.TemporaryStatus.Untreated.GetHashCode()%>">未处理</option>
                            <option value="<%=Needs.Ccs.Services.Enums.TemporaryStatus.Treated.GetHashCode()%>">已处理</option>
                            <option value="<%=Needs.Ccs.Services.Enums.TemporaryStatus.Complete.GetHashCode()%>">已完成</option>
                        </select>
                    </td>
                    <td class="lbl">入库日期：</td>
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
        <table id="datagrid" class="mygrid" title="暂存记录列表" toolbar="#topBar" data-options="
            fitColumns:true,
            fit:true,
            nowrap:false,
            scrollbarSize:0,
            border:false,
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="EntryNumber" data-options="align:'center'" style="width: 8%">入仓号</th>
                    <th field="CompanyName" data-options="align:'left'" style="width: 16%">公司名称</th>
                    <th field="ShelveNumber" data-options="align:'center'" style="width: 10%">库位号</th>
                    <th field="WaybillCode" data-options="align:'left'" style="width: 10%">运单号</th>
                    <th field="PackNo" data-options="align:'center'" style="width: 8%">件数</th>
                    <th field="EntryDate" data-options="align:'center'" style="width: 10%">入库日期</th>
                    <th field="Status" data-options="align:'center'" style="width: 10%">状态</th>
                    <th data-options="field:'btnPacking',width:60,formatter:Operation,align:'center'" style="width: 10%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
