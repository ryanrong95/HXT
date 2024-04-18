<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Logistics.Vehicle.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
  <%--  <script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '车辆管理';
        gvSettings.summary = '仅限芯达通业务库房的物流使用的车辆管理';
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            var vehicleType = eval('(<%=this.Model.VehicleType%>)');
            //税则列表初始化
            $('#vehiclesGrid').myDatagrid({
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

            //初始化车辆类型
            $("#VehicleType").combobox({
                data: vehicleType
            });
        });

        //查询
        function Search() {
            var license = $('#License').textbox('getValue');
            var carrier = $("#Carrier").textbox('getValue');
            var vehicleType = $('#VehicleType').combobox('getValue');
            $('#vehiclesGrid').myDatagrid('search', { Carrier: carrier, License: license, VehicleType: vehicleType });
        }

        //重置查询条件
        function Reset() {
            $('#Carrier').textbox('setValue', null);
            $('#License').textbox('setValue', null);
            $('#VehicleType').combobox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        //编辑车辆信息
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '600px',
                height: '350px',
                title: '编辑车辆信息',
                onClose: function () {
                    $('#vehiclesGrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除车辆信息成功！');
                        $('#vehiclesGrid').myDatagrid('reload');
                    })
                }
            });
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '600px',
                height: '300px',
                title: '新增车辆',
                onClose: function () {
                    $('#vehiclesGrid').myDatagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">承运商: </span>
                    <input class="easyui-textbox search" id="Carrier" />
                    <span class="lbl">车牌号: </span>
                    <input class="easyui-textbox search" id="License" />
                    <span class="lbl">车辆类型: </span>
                    <input class="easyui-combobox search" id="VehicleType" data-options="valueField:'Key',textField:'Value'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="vehiclesGrid" data-options="singleSelect:true,fit:true,scrollbarSize:0" title="车辆" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'License',align:'left'" style="width: 100px;">车牌号</th>
                    <th data-options="field:'VehicleType',align:'center'" style="width: 80px;">车辆类型</th>
                    <th data-options="field:'HKLicense',align:'left'" style="width: 100px;">香港车牌号</th>
                    <th data-options="field:'Weight',align:'center'" style="width: 80px;">车重(KGS)</th>
                    <th data-options="field:'Size',align:'center'" style="width: 100px;">尺寸</th>
                    <th data-options="field:'CarrierName',align:'center'" style="width: 80px;">承运商</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 180px;">操作</th>
                </tr>
            </thead>
        </table>
    </div
</body>
</html>

