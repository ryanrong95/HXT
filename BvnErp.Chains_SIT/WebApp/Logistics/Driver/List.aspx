<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Logistics.Driver.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '司机管理';
        gvSettings.summary = '仅限芯达通业务库房的物流使用的司机管理';
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //列表初始化
            $('#driversGrid').myDatagrid({
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

        //查询
        function Search() {
            var mobile = $('#Mobile').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            var CarrierName = $('#CarrierName').textbox('getValue');
            $('#driversGrid').myDatagrid('search', { Mobile: mobile, Name: name, CarrierName: CarrierName });
        }

        //重置查询条件
        function Reset() {
            $('#Mobile').textbox('setValue', null);
            $('#Name').textbox('setValue', null);
            $('#CarrierName').textbox('setValue', null);
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

        //编辑司机信息
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '650px',
                height: '500px',
                title: '编辑驾驶员信息',
                onClose: function () {
                    $('#driversGrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除司机信息成功！');
                        $('#driversGrid').myDatagrid('reload');
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
                width: '650px',
                height: '500px',
                title: '新增驾驶员',
                onClose: function () {
                    $('#driversGrid').myDatagrid('reload');
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
                    <input class="easyui-textbox search" id="CarrierName" />
                    <span class="lbl">手机号: </span>
                    <input class="easyui-textbox search" id="Mobile" />
                    <span class="lbl">驾驶员姓名: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="driversGrid" data-options="singleSelect:true,fit:true,scrollbarSize:0" title="驾驶员" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 80px;">驾驶员姓名</th>
                    <th data-options="field:'Mobile',align:'left'" style="width: 80px;">大陆手机号</th>
                    <th data-options="field:'HKMobile',align:'left'" style="width: 80px;">香港手机号</th>
                    <th data-options="field:'HSCode',align:'left'" style="width: 80px;">海关编号</th>
                    <th data-options="field:'DriverCardNo',align:'left'" style="width: 80px;">司机卡号</th>
                    <th data-options="field:'PortElecNo',align:'left'" style="width: 80px;">口岸电子编号</th>
                    <th data-options="field:'LaoPaoCode',align:'left'" style="width: 80px;">寮步密码</th>
                    <th data-options="field:'License',align:'left'" style="width: 100px;">证件号码</th>
                    <th data-options="field:'CarrierName',align:'left'" style="width: 100px;">承运商</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
