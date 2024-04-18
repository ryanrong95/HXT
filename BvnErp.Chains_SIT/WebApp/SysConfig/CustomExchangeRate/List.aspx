<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.CustomExchangeRate.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>海关汇率</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '海关汇率';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
                singleSelect: true, fit: true, scrollbarSize: 0, border: false,
                fitcolumns: true,
                toolbar:'#topBar',
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

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?id=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑海关汇率',
                width: '450px',
                height: '220px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        function Search() {
            var Code = $("#Code").val();
            var Name = $("#Name").val();
            $('#datagrid').myDatagrid('search', { Code: Code, Name: Name });
        }

        function Reset() {
            $("#Code").textbox("setValue", "");
            $("#Name").textbox("setValue", "");
            Search();
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=DeleteExchangeRate', { ID: id }, function () {
                        $.messager.alert('消息', '删除海关汇率成功！');
                        $('#datagrid').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增海关汇率',
                width: '450px',
                height: '220px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //function E3()
        //{
        //    formatDateTime: function (val, row) {
        //        var now = new Date(val);
        //        return now.format("yyyy-MM-dd hh:mm:ss");
        //    }

        //}
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
                    <span class="lbl">币种代码: </span>
                    <input class="easyui-textbox search" id="Code" />
                    <span class="lbl">名称: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="海关汇率" >
            <thead>
                <tr>
                    <th  data-options="field:'Code',align:'center'" style="width: 20%;">币种代码</th>
                    <th data-options="field:'Name',align:'center'" style="width: 20%">名称</th>
                    <th  data-options="field:'Rate',align:'center'" style="width: 20%">汇率</th>
                    <th  data-options="field:'UpdateDate',align:'center'" style="width: 20%">更新时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 20%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
