<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FeeList.aspx.cs" Inherits="WebApp.HKWarehouse.Report.FeeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用列表</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%-- <script>
        gvSettings.fatherMenu = '报表';
        gvSettings.menu = '费用统计';
        gvSettings.summary = '综合查询(包含所有订单的费用)';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            //下拉框数据初始化
            var feeType = eval('(<%=this.Model.FeeType%>)');
            $('#FeeType').combobox({
                data: feeType
            });

            $('#datagrid').myDatagrid({
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
                fitColumns:true,
                fit:true,
                //scrollbarSize:0,
                onLoadSuccess: function (data) {
                    //添加合计行
                    $('#datagrid').datagrid('appendRow', {
                        OrderID: '<span class="subtotal">合计：</span>',
                        TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
                        Btn: '<span class="subtotal"></span>',
                    });

                    var rows = $('#datagrid').datagrid('getRows');
                }
            });
        });

        function compute(colName) {
            var rows = $('#datagrid').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(rows[i][colName]);
                }
            }
            return total;
        }

        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var FeeType = $('#FeeType').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagrid').myDatagrid('search', {
                OrderID: OrderID, ClientCode: ClientCode, ClientName: ClientName,
                FeeType: FeeType, StartDate: StartDate, EndDate: EndDate
            });
        }

        //重置
        function Reset() {
            $("#OrderID").textbox('setValue', null);
            $("#ClientCode").textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#FeeType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看
        function View(ID) {
            var url = location.pathname.replace(/FeeList.aspx/ig, '../Fee/Approved/Detail.aspx') + '?FeeID=' + ID;
            self.$.myWindow({
                url: url,
                noheader: false,
                title: '查看费用',
                width: '800px',
                height: '750px',
                onClose: function () {
                }
            });
        }

        //编辑
        function Edit(ID) {
            var url = location.pathname.replace(/FeeList.aspx/ig, '../Fee/FeeEdit.aspx') + '?FeeID=' + ID;
            self.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '编辑费用',
                width: '820px',
                height: '540px',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选费用！', function (success) {
                if (success) {
                    $.post('?action=Delete', {
                        ID: ID,
                    }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                $('#datagrid').myDatagrid('reload');
                            }
                        })
                    })
                }
            })
        }

        //操作
        function Operation(val, row, index) {
            if (val != undefined && val != null) {
                if (val.toString().indexOf('<span class="subtotal">') != -1) {
                    return val;
                }
            }

            var buttons = '';
           
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span></span></a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        //导出Excel
        function ExportExcel() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var FeeType = $('#FeeType').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var param = {
                OrderID: OrderID, ClientCode: ClientCode, ClientName: ClientName,
                FeeType: FeeType, StartDate: StartDate, EndDate: EndDate
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', param, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <%--   <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>--%>
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" />
                    </td>
                    <td class="lbl">客户编号：</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" />
                    </td>
                    <td class="lbl">客户名称：</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">费用类型：</td>
                    <td>
                        <input class="easyui-combobox" id="FeeType" data-options="valueField:'Key',textField:'Value',editable:false" />
                    </td>
                    <td class="lbl">添加时间：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" />
                    </td>
                    <td class="lbl">至：</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="库房费用" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',width:150,align:'center'">订单编号</th>
                    <th data-options="field:'ClientCode',width:130,align:'center'">客户编号</th>
                    <th data-options="field:'ClientName',width:180,align:'left'">客户名称</th>
                    <th data-options="field:'WarehousePremiumType',width:90,align:'left'">费用类型</th>
                    <th data-options="field:'UnitPrice',width:80,align:'center'">单价</th>
                    <th data-options="field:'Count',width:80,align:'center'">数量</th>
                    <th data-options="field:'TotalPrice',width:80,align:'center'">总价</th>
                    <th data-options="field:'Currency',width:80,align:'center'">币种</th>
                    <th data-options="field:'CreateDate',width:130,align:'center'">添加时间</th>
                    <th data-options="field:'AdminName',width:80,align:'center'">添加人</th>
                    <th data-options="field:'Btn',width:140,formatter:Operation,align:'left'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
