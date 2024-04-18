<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="VoutherMake.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.VoutherMake" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: false,
                fitColumns: false,
                nowrap: false,
            });

            //开票类型选项
            $('#InvoiceType').combobox({
                data: model.InvoiceTypeOption,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });

            //申请人选项
            $('#Apply').combobox({
                data: model.ApplyOption,
                editable: true,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    //grid.myDatagrid('search', getQuery());
                }
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                $('#ClientCode').textbox("setText", "");
                $('#CompanyName').textbox("setText", "");
                $('#InvoiceType').combobox("setValue", "");
                $('#Apply').combobox("setValue", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });

        function MakeAccount() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要做账的数据！');
                return;
            }

            //验证成功
            MaskUtil.mask();//遮挡层
            $.post('?action=MakeAccount', {
                Model: JSON.stringify(data)
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', "生成凭证失败", 'info', function () { });
                }
            });
        }

        function MakeAccountAll() {
            var adminID = $.trim($('#Apply').combobox("getValue"));
            var invoiceType = $.trim($('#InvoiceType').combobox("getValue"));
            var comanyName = $('#ComanyName').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=MakeAccountAll', {
                AdminID: adminID,
                InvoiceType: invoiceType,
                ClientCode: clientCode,
                CompanyName: comanyName,
                StartDate: startDate,
                EndDate: endDate
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成凭证成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', rel.msg, 'info', function () { });
                }
            });
        }
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientCode: $.trim($('#ClientCode').textbox("getText")),
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                InvoiceType: $.trim($('#InvoiceType').combobox("getValue")),
                AdminID: $.trim($('#Apply').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };




    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户编号</td>
                <td style="width: 280px;">
                    <input class="easyui-textbox" id="ClientCode" data-options="width:280,validType:'length[1,50]'" />
                </td>
                <td style="width: 90px;">公司名称</td>
                <td style="width: 280px;">
                    <input class="easyui-textbox" id="CompanyName" data-options="width:280,validType:'length[1,50]'" />
                </td>
                <td style="width: 90px;">开票类型</td>
                <td>
                    <input class="easyui-combobox" id="InvoiceType" data-options="width:280,valueField:'Value',textField:'Text'" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">申请人</td>
                <td style="width: 280px;">
                    <input class="easyui-combobox" id="Apply" data-options="width:280,valueField:'Value',textField:'Text'" />
                </td>
                <td style="width: 90px;">申请日期</td>
                <td style="width: 280px;">
                    <input id="StartDate" data-options="prompt:'',width:123," class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'',width:123," class="easyui-datebox" />
                </td>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccount()">生成凭证</a>
                    <a id="btnMakeAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="MakeAccountAll()">生成全部凭证</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="代仓储开票">
        <thead>
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                <th data-options="field:'InvoiceNoticeID',align:'center'" style="width: 8%;">开票编号</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 6%;">客户编号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 15%;">公司名称</th>
                <th data-options="field:'InvoiceTypeDes',align:'center'" style="width: 8%;">开票类型</th>
                <th data-options="field:'Amount',align:'center'" style="width: 8%;">含税金额</th>
                <th data-options="field:'InvoiceDeliveryTypeDes',align:'center'" style="width: 8%;">交付方式</th>
                <th data-options="field:'WaybillCode',align:'left'" style="width: 10%;">发票运单</th>
                <th data-options="field:'InvoiceNoticeStatusDes',align:'center'" style="width: 7%;">开票状态</th>
                <th data-options="field:'AdminName',align:'center'" style="width: 7%;">申请人</th>
                <th data-options="field:'InvoiceNoticeCreateDateDes',align:'center'" style="width: 8%;">申请日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>
