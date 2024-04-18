<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '运输批次';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            //下拉框数据初始化
            var carriers = eval('(<%=this.Model.Carriers%>)');
            var cutStatus = eval('(<%=this.Model.CutStatus%>)');
            $('#Carrier').combobox({
                data: carriers
            });
            $('#CutStatus').combobox({
                data: cutStatus
            });

            //运输批次列表初始化
            $('#voyageGrid').myDatagrid({
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
            var voyageNo = $("#VoyageNo").textbox('getValue');
            var carrier = $("#Carrier").combobox('getValue');
            var cutStatus = $("#CutStatus").combobox('getValue');
            $('#voyageGrid').myDatagrid('search', { VoyageNo: voyageNo, Carrier: carrier, CutStatus: cutStatus });
        }

        //重置
        function Reset() {
            $("#VoyageNo").textbox('setValue', "");
            $("#Carrier").combobox('setValue', "");
            $("#CutStatus").combobox('setValue', "");
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '520px',
                height: '410px',
                title: '新增',
                onClose: function () {
                    $('#voyageGrid').myDatagrid('reload');
                }
            });
        }


        //编辑
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '520px',
                height: '410px',
                title: '编辑',
                onClose: function () {
                    $('#voyageGrid').myDatagrid('reload');
                }
            });
        }

        //截单
        function SureCut(id) {
            $.messager.confirm('确认', '请您再次确认是否截单。运输批次号：' + id, function (success) {
                if (success) {
                    $.post('?action=SureCut', { ID: id }, function () {
                        $.messager.alert('消息', '操作成功！');
                        $('#voyageGrid').myDatagrid('reload');
                    })
                }
            });
        }

        //查看
        function View(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?ID=" + id + '&From=Logistics';
            window.location = url;
        }

        //舱单
        function ViewManifest(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'ManifestDetail.aspx') + "?ID=" + id;
            window.location = url;
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                  '<span class =\'l-btn-left l-btn-icon-left\'>' +
                  '<span class="l-btn-text">编辑</span>' +
                  '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            if (row.CutStatusValue == '<%=Needs.Ccs.Services.Enums.CutStatus.UnCutting.GetHashCode()%>') {
                buttons += '<a id="btnCut" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="SureCut(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">截单</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                buttons += '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                  '<span class =\'l-btn-left l-btn-icon-left\'>' +
                  '<span class="l-btn-text">详情</span>' +
                  '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            }
            buttons += '<a id="btnCut" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="ViewManifest(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">舱单</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <table id="table1" style="margin: 5px 0">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" id="VoyageNo" data-options="width:200" />
                    </td>
                    <td class="lbl">承运商：</td>
                    <td>
                        <input class="easyui-combobox" id="Carrier" data-options="width:200,valueField:'Code',textField:'Name',editable:false" />
                    </td>
                    <td class="lbl">截单状态：</td>
                    <td>
                        <input class="easyui-combobox" id="CutStatus" data-options="width:200,valueField:'Key',textField:'Value',editable:false" />
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
        <table id="voyageGrid" class="easyui-datagrid" title="运输批次" data-options="
            nowrap: false,
            border: false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'left'" style="width: 15%">货物运输批次号</th>
                    <th field="Carrier" data-options="align:'left'" style="width: 10%">承运商</th>
                    <th field="HKLicense" data-options="align:'center'" style="width: 10%">车牌号</th>
                    <th field="TransportTime" data-options="align:'center'" style="width: 10%">运输时间</th>
                    <th field="DriverName" data-options="align:'center'" style="width: 10%">驾驶员姓名</th>
                    <th field="VoyageType" data-options="align:'center'" style="width: 7%">运输类型</th>
                    <th field="CutStatus" data-options="align:'center'" style="width: 8%">截单状态</th>
                    <th field="CreateTime" data-options="align:'center'" style="width: 10%">创建日期</th>
                    <th data-options="field:'btnOpt',formatter:Operation,align:'left'" style="width: 20%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
