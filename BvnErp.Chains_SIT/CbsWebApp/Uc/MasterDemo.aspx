<%@ Page Language="C#" MasterPageFile="~/Uc/Pages.Master" AutoEventWireup="true" CodeBehind="MasterDemo.aspx.cs" Inherits="Needs.Cbs.WebApp.Uc.MasterDemo" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cbsHead" runat="server">
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //下拉框数据初始化
            var types = eval('(<%=this.Model.Types%>)');
            $('#Type').combobox({
                data: types
            });

            //海关基础数据列表初始化
            $('#datagrid').bvgrid({
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
            var type = $('#Type').combobox('getValue');
            $('#datagrid').bvgrid('search', { Code: code, Type: type });
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', null);
            $('#Type').combobox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            if (row.TypeValue == '<%=Needs.Cbs.Services.Enums.BaseType.CustomMaster.GetHashCode()%>') {
                buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetDefault(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置默认关联</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            }
            return buttons;
        }

        //编辑税则
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '编辑海关基础数据',
                url: url,
                width: '400px',
                height: '250px',
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }

        //设置申报地默认关联
        function SetDefault(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'SetMasterDefault.aspx') + "?Code=" + rowdata.Code;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '设置申报地默认关联',
                    url: url,
                    width: '500px',
                    height: '250px'
                }).open();
            }
        }

        //新增税则
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增海关基础数据',
                url: url,
                width: '400px',
                height: '250px',
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cbsForm" runat="server">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">代码: </span>
                    <input class="easyui-textbox search" id="Code" />
                    <span class="lbl">类型: </span>
                    <input class="easyui-combobox" id="Type" data-options="valueField:'Key',textField:'Value'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fitColumns:true,fit:true,scrollbarSize:0,border:false,toolbar:'#topBar'" title="海关基础数据" class="easyui-datagrid">
            <thead>
                <tr>
                    <th data-options="field:'Code',align:'center'" style="width: 15%;">代码</th>
                    <th data-options="field:'Type',align:'center'" style="width: 15%;">类型</th>
                    <th data-options="field:'Name',align:'left'" style="width: 15%;">中文名称</th>
                    <th data-options="field:'EnglishName',align:'left'" style="width: 15%;">英文名称</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 25%;">摘要备注</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
