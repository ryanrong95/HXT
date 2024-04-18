<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoicedList.aspx.cs" Inherits="WebApp.Order.Query.InvoicedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开票查询</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>   
   <%-- <script>
        gvSettings.fatherMenu = '开票申请';
        gvSettings.menu = '开票申请';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var ApplyData = eval('(<%=this.Model.ApplyData%>)');
        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
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

            //初始化Combobox
            $('#Apply').combobox({
                data: ApplyData,
            })
            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
            })
        });

        //查询
        function Search() {
            //var Apply = $('#Apply').combobox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                //Apply: Apply,
                InvoiceType: InvoiceType,
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                StartDate: StartDate,
                EndDate: EndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            //$('#Apply').combobox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看
        function See(id) {
            if (id) {
                var url = location.pathname.replace(/InvoicedList.aspx/ig, '../../Finance/Invoice/Details.aspx?ID=' + id + '&From=Order');
                window.location = url;
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="See(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            if (row.Status == '待开票') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="CancelApply(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }            
            return buttons;
        }

        function CancelApply(ID) {
            $.messager.confirm('确认', '确认删除该开票通知', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=CancelApply', {InvoiceNoticeID:ID}, function (data) {
                        var Result = JSON.parse(data);
                        MaskUtil.unmask();
                        if (Result.success) {
                            Search();
                            $.messager.alert('提示', Result.message);
                        } else {
                            $.messager.alert('提示', Result.message);
                        }
                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">客户名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">开票类型:</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">申请日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td colspan="2">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="开票申请" data-options="
            fitColumns:true,
            fit:true,
            singleSelect:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center'" style="width: 9%;">申请编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 12%;">公司名称</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 8%;">开票类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 6%;">含税金额</th>
                    <th data-options="field:'DeliveryType',align:'center'" style="width: 6%;">交付方式</th>
                    <th data-options="field:'WaybillCode',align:'left'" style="width: 9%;">发票运单</th>
                    <th data-options="field:'InvoiceNoSummary',align:'left'" style="width: 12%;">发票号</th>
                    <th data-options="field:'Status',align:'center'" style="width: 6%;">开票状态</th>
                    <th data-options="field:'ApplyName',align:'center'" style="width: 6%;">申请人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">申请日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 12%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
