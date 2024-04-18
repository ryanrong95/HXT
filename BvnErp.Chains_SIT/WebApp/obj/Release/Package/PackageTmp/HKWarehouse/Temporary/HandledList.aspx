<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HandledList.aspx.cs" Inherits="WebApp.HKWarehouse.Temporary.HandledList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已处理暂存记录</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
     <%--<script>
         gvSettings.fatherMenu = '暂存通知';
        gvSettings.menu = '已处理';
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
        });

        //查询
        function Search() {
            var EntryNumber = $("#EntryNumber").val();
            var CompanyName = $("#CompanyName").val();
            $('#datagrid').myDatagrid('search', { EntryNumber: EntryNumber, CompanyName: CompanyName });
        }

        //重置
        function Reset() {
            $("#EntryNumber").textbox('setValue', "");
            $("#CompanyName").textbox('setValue', "");
            Search();
        }

        //查看
        function Look(ID) {
            var url = location.pathname.replace(/HandledList.aspx/ig, 'Detail.aspx') + "?ID=" + ID;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '查看',
                width: '800px',
                height: '500px',
                onClose: function () {
                }
            });
        }

        //装箱完成
        function Packing(ID) {
            $.messager.confirm('确认', '请您再次确认是否装箱完成！', function (success) {
                if (success) {
                    $.post('?action=Packing', { ID: ID }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                $('#datagrid').myDatagrid('reload');
                            }
                        });
                    })
                }
            });
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Look(\'' + row.ID + '\')" group >' +
                            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                            '<span class="l-btn-text">查看</span>' +
                            '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                            '</span>' +
                            '</a>';
            buttons += '<a id="btnTemporary" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Packing(\'' + row.ID + '\')" group >' +
                            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                            '<span class="l-btn-text">完成</span>' +
                            '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                            '</span>' +
                            '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                 <li>
                    <span class="lbl">入仓号:</span>
                    <input class="easyui-textbox" id="EntryNumber" style="height: 26px; width: 180px"/>
                    <span class="lbl">公司名称: </span>
                    <input class="easyui-textbox" id="CompanyName" style="height: 26px; width: 180px"/>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,nowrap:false,scrollbarSize:0" title="已处理暂存记录" toolbar="#topBar">
            <thead>
                <tr>
                    <th field="EntryNumber" data-options="align:'center'" style="width: 8%">入仓号</th>
                    <th field="CompanyName" data-options="align:'left'" style="width: 16%">公司名称</th>
                    <th field="ShelveNumber" data-options="align:'center'" style="width: 10%">库位号</th>
                    <th field="WaybillCode" data-options="align:'left'" style="width: 10%">运单号</th>
                    <th field="PackNo" data-options="align:'center'" style="width: 8%">件数</th>
                    <th field="EntryDate" data-options="align:'center'" style="width: 10%">入库日期</th>
                    <th field="Status" data-options="align:'center'" style="width: 10%">状态</th>
                    <th data-options="field:'btnPacking',width:70,formatter:Operation,align:'center'" style="width: 10%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
