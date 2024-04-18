<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancePaymentList.aspx.cs" Inherits="WebApp.Finance.Payment.FinancePaymentList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%-- <script>
        gvSettings.fatherMenu = '收付款管理';
        gvSettings.menu = '付款';
        gvSettings.summary = '财务付款';
    </script>--%>
    <script type="text/javascript">

        var FeeType = eval('(<%=this.Model.FeeType%>)');
        var IsPaymentOperator = '<%=this.Model.IsPaymentOperator%>';

        $(function () {
            //下拉框数据初始化
            $('#FeeType').combobox({
                data: FeeType
            });
            //代理订单列表初始化
            $('#payments').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
                toolbar: '#topBar',
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
            var FeeType = $('#FeeType').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#payments').myDatagrid('search', { FeeType: FeeType, StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            $('#FeeType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //新增财务付款
        function Add() {
            var url = location.pathname.replace(/FinancePaymentList.aspx/ig, 'FinancePaymentAdd.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '800px',
                height: '600px',
                title: '新增付款',
                onClose: function () {
                    $('#payments').myDatagrid('reload');
                }
            });
        }

        //编辑财务付款
        function Edit(id) {
            var url = location.pathname.replace(/FinancePaymentList.aspx/ig, 'FinancePaymentAdd.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '800px',
                height: '600px',
                title: '编辑付款',
                onClose: function () {
                    $('#payments').myDatagrid('reload');
                }
            });
        }

        //删除财务付款
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除财务付款！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#payments').datagrid('reload');
                    })
                }
            });
        }

        //查看订单
        function Look(id) {
            var url = location.pathname.replace(/FinancePaymentList.aspx/ig, 'FinancePaymentDetails.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '800px',
                height: '600px',
                title: '查看付款',
                onClose: function () {

                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            if(row.FeeTypeInt > 10000 || row.FeeType=="付款-货款")
            {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Look(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }
            else {
                //var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                //    '<span class="l-btn-text">编辑</span>' +
                //    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                //    '</span>' +
                //    '</a>';

                var buttons = '';

                //if (IsPaymentOperator == "1") {
                //    buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                //        '<span class="l-btn-text">删除</span>' +
                //        '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                //        '</span>' +
                //        '</a>';
                //}

                return buttons;
            }
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 26px">
                <%--<tr>
                    <td colspan="2">
                        <a id="btnLook" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增付款</a>
                    </td>
                </tr>--%>
                <tr>
                    <td class="lbl">付款类型：</td>
                    <td>
                        <input class="easyui-combobox" id="FeeType" name="FeeType" style="height: 26px; width: 200px"
                            data-options="valueField:'Key',textField:'Value'" />
                    </td>
                    <td class="lbl">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" style="height: 26px; width: 200px" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" style="height: 26px; width: 200px" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="payments" title="财务付款" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 60px">流水号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 60px">收款方</th>
                    <th data-options="field:'FinanceVault',align:'center'" style="width: 40px">付款金库</th>
                    <th data-options="field:'FinanceAccount',align:'center'" style="width: 40px">付款账户</th>
                    <th data-options="field:'FeeType',align:'center'" style="width: 30px">费用类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 40px">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 20px">币种</th>
                    <th data-options="field:'Payer',align:'center'" style="width: 30px">付款人</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 40px">付款日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 60px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
