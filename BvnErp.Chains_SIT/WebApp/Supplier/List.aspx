<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Supplier.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            var suplierGrade = eval('(<%=this.Model.SuplierGrade%>)');

            $('#SupplierGrade').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: suplierGrade
            });
            $('#Rank').combobox({
                data: suplierGrade
            });

            //列表初始化
            $('#suppliers').myDatagrid({
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
            var englishName = $.trim($('#EnglishName').textbox('getValue'));
            var clientName = $.trim($('#ClientName').textbox('getValue'));
            var supplierGrade = $('#SupplierGrade').combobox('getValue');
            $('#suppliers').myDatagrid('search', { EnglishName: englishName, ClientName: clientName, SupplierGrade: supplierGrade });
        }

        //重置查询条件
        function Reset() {
            $('#EnglishName').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#SupplierGrade').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditRank(\'' + row.ID + '\',\'' + row.SupplierGradeValue + '\',\'' + row.EnglishName + '\',\'' + row.Summary + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改供应商等级</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //修改会员等级
        function EditRank(id, value, englishName, summary) {
            $("#rank-tip").show();
            $('#Rank').textbox('textbox').validatebox('options').required = true;
            $('#Rank').combobox('setValue', value);
            $('#supplierName').html(englishName);
            $('#summary').textbox('setValue', summary);
            $('#rank-dialog').dialog({
                title: '修改供应商等级',
                width: 450,
                height: 350,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '保存',
                    width: 70,
                    handler: function () {
                        if (!Valid('form1')) {
                            return;
                        }
                        MaskUtil.mask();
                        var rank = $("#Rank").textbox('getValue');
                        rank = rank.trim();
                        var summary = $("#summary").textbox('getValue');
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=EditRank', {
                            ID: id,
                            Rank: rank,
                            Summary: summary
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    //修改成功后刷新datagrid
                                    $("#suppliers").myDatagrid("reload");
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#rank-dialog').window('close');
                    }
                }],
            });
            $('#rank-dialog').window('center');
        }

        //整行关闭一系列弹框
        function NormalClose() {
            $('#rank-dialog').window('close');
            $.myWindow.close();

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <span class="lbl">供应商名称(英文): </span>
            <input class="easyui-textbox search" id="EnglishName" style="width: 200px" />
            <span class="lbl">客户名称:</span>
            <input class="easyui-textbox search" id="ClientName" style="width: 200px" />
            <span class="lbl">供应商等级: </span>
            <input class="easyui-combobox search" id="SupplierGrade" style="width: 200px" />
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px;">
        <table id="suppliers" data-options="singleSelect:true,border:true,fit:true,nowrap:false,scrollbarSize:0" title="供应商" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'EnglishName',align:'left'" style="width: 25%;">供应商名称-英文</th>
                    <th data-options="field:'ChineseName',align:'left'" style="width: 10%;">供应商名称-中文</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'Place',align:'center'" style="width: 10%;">国家/地区</th>
                    <th data-options="field:'SupplierGrade',align:'center'" style="width: 5%;">级别</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 27%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="rank-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false" style="overflow: hidden">
        <form id="form1">
            <div id="rank-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                <div style="height: 30px;">
                    <label>供应商名称：<span id="supplierName"></span></label>
                </div>
                <div style="display: inline-block; height: 30px;">
                    <label>供应商等级：</label>
                </div>
                <div style="margin-top: 3px; display: inline-block; height: 30px;">
                    <input class="easyui-combobox" style="width: 300px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" id="Rank" name="Rank" />
                </div>
                <div style="height: 30px;">
                    <div style="display: inline-block; height: 30px;width:84px">
                        <label>备注：</label>
                    </div>
                    <div style="margin-top: 3px; display: inline-block; ">
                        <input class="easyui-textbox" style="width: 300px;height:100px;" data-options="multiline:true"  id="summary" name="summary" />
                    </div>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
