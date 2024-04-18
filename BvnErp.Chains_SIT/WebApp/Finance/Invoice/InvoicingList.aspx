<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoicingList.aspx.cs" Inherits="WebApp.Finance.InvoicingList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开票中</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
<%--    <script>
        gvSettings.fatherMenu = '开票通知';
        gvSettings.menu = '开票中';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var ApplyData = eval('(<%=this.Model.ApplyData%>)');
        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                rownumbers: true,
                singleSelect: false,
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
            var apply = $('#Apply').combobox('getValue');
            var invoiceType = $('#InvoiceType').combobox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {
                Apply: apply,
                InvoiceType: invoiceType,
                CompanyName: companyName,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Apply').combobox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //开票
        function Invoice(id) {
            var url = location.pathname.replace(/InvoicingList.aspx/ig, 'Confirm.aspx?ID=' + id);
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Invoice(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">开票</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //导出Excel
        function Export() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的信息！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            $.post('?action=Export', {
                IDs: JSON.stringify(strIds)
            }, function (result) {
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
                        $('#datagrid').myDatagrid('reload');
                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td colspan="6">
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">开票类型:</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">申请人:</td>
                    <td>
                        <input class="easyui-combobox" id="Apply" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">申请日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
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
        <table id="datagrid" title="开票中" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,border:false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ID',align:'center'" style="width: 12%;">开票编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 8%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 16%;">公司名称</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 8%;">开票类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 8%;">含税金额</th>
                    <th data-options="field:'DeliveryType',align:'center'" style="width: 8%;">交付方式</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">开票状态</th>
                    <th data-options="field:'ApplyName',align:'center'" style="width: 8%;">申请人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">申请日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
