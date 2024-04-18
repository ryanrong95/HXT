<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Needs.Cbs.WebApp.OriginTariff.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>原产地税则</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
 <%--   <script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '原产地税则';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        var OriginData = eval('(<%=this.Model.OriginData%>)');
        $(function () {
            $("#Origin").combobox({
                data: OriginData
            });
            $('#datagrid').bvgrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                singleSelect: false,
                selectOnCheck: true,
                checkOnSelect: true,
                nowrap: false,
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-options="iconCls:\'icon-edit\'" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-options="iconCls:\'icon-remove\'" onclick="Delete(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '原产地税则新增',
                width: '600px',
                height: '400px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            }).open();
        }

        function Search() {
            var Origin = $('#Origin').combobox('getValue');
            var HSCode = $("#HSCode").val();
            var Name = $("#Name").val();

            $('#datagrid').bvgrid('search', { Origin: Origin, HSCode: HSCode, Name: Name });
        }

        function Reset() {
            $("#Origin").combobox("setValue", "");
            $("#HSCode").textbox("setValue", "");
            $("#Name").textbox("setValue", "");
            Search();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?id=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '原产地税则编辑',
                    url: url,
                    width: '600px',
                    height: '400px',
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        function Delete(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: rowdata.ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        function Del() {
            var docData = $('#datagrid').datagrid('getChecked');
            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {
                arr[i] = docData[i].ID;
            }

            if (arr.length != 0) {
                var jsonString = JSON.stringify(arr);
                $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                    if (success) {
                        $.post('?action=MutilDelete', { IDs: jsonString }, function () {
                            $.messager.alert('删除', '删除成功！');
                            $('#datagrid').datagrid('reload');
                        })
                    }
                });
            } else {
                $.messager.alert('提示', '请选择数据！');
            }
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <a id="btnDel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-remove'" onclick="Del()">删除</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">原产地:</span>
                    <input class="easyui-combobox search" id="Origin" name="Origin" />
                    <span class="lbl">商品编码: </span>
                    <input class="easyui-textbox search" id="HSCode" />
                    <span class="lbl">商品名称: </span>
                    <input class="easyui-textbox search" id="Name" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="原产地税则" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            singleselect="true" fitcolumns="true">
            <thead>
                <tr>
                    <th field="ck" data-options="align:'center',checkbox:true" style="width: 30px">全选</th>
                    <th field="HSCode" data-options="align:'center'" style="width: 50px">商品编码</th>
                    <th field="Name" data-options="align:'left'" style="width: 50px">商品名称</th>
                    <th field="Origin" data-options="align:'center'" style="width: 50px">原产地</th>
                    <th field="Type" data-options="align:'center'" style="width: 50px">征税类型</th>
                    <th field="Rate" data-options="align:'center'" style="width: 50px">税率</th>
                    <th field="StartDate" data-options="align:'center'" style="width: 50px">加征开始日期</th>
                    <th field="EndDate" data-options="align:'center'" style="width: 50px">加征结束日期</th>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 50px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
