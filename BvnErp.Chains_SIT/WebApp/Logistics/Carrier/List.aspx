    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Logistics.Carrier.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>承运商</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '承运商';
        gvSettings.summary = '仅限华芯通业务库房的物流使用的车辆管理'
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            var CarrierTypeData = eval('(<%=this.Model.CarrierTypeData%>)');
            //初始化下拉框
            $('#CarrierType').combobox({
                data: CarrierTypeData,
            });
            //税则列表初始化
            $('#carriersGrid').myDatagrid({
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
            var code = $('#Code').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            var carriertype = $('#CarrierType').combobox('getValue');
            $('#carriersGrid').myDatagrid('search', { Code: code, Name: name, CarrierType:carriertype});
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', null);
            $('#Name').textbox('setValue', null);
            $('#CarrierType').combobox('setValue', null);
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

        //编辑税则
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '650px',
                height: '500px',
                title: '编辑承运商',
                onClose: function () {
                    $('#carriersGrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除承运商成功！');
                        $('#carriersGrid').myDatagrid('reload');
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
                title: '新增承运商',
                onClose: function () {
                    $('#carriersGrid').myDatagrid('reload');
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
                    <span class="lbl">简称: </span>
                    <input class="easyui-textbox search" id="Code" />
                    <span class="lbl">名称: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <span class="lbl">承运商类型: </span>
                    <input class="easyui-combobox search" id="CarrierType" name="CarrierType" data-options="valueField:'TypeValue',textField:'TypeText'"/>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="carriersGrid" data-options="singleSelect:true,fit:true,scrollbarSize:0" title="承运商" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>

                    <th data-options="field:'Code',align:'center'" style="width: 80px;">简称</th>
                    <th data-options="field:'Name',align:'left'" style="width: 100px;">名称</th>
                    <th data-options="field:'QueryMark',align:'left'" style="width: 100px;">查询标记</th>
                    <th data-options="field:'CarrierType',align:'center'" style="width: 80px;">承运商类型</th>
                     <th data-options="field:'ContactName',align:'center'" style="width: 80px;">联系人</th>
                     <th data-options="field:'ContactMobile',align:'center'" style="width: 80px;">联系电话</th>
                     <th data-options="field:'Address',align:'left'" style="width: 180px;">地址</th>
                     <th data-options="field:'Fax',align:'center'" style="width: 80px;">传真</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 100px;">备注</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
