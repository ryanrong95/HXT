<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="UnInvoiceList.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.UnInvoiceList" %>

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

        //导出Excel
        function ExportExcel() {
            var data = $('#tab1').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的信息！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].InvoiceNoticeID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            $.post('?action=ExportExcel', {
                InvoiceNoticeIDs: JSON.stringify(strIds)
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
                        $('#tab1').myDatagrid('reload');
                    }
                });
            })
        }

        //导出Xml
        function ExportXml() {
            var data = $('#tab1').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要导出的开票通知！');
                return;
            }
            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].InvoiceNoticeID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            $.post('?action=ExportXml', {
                InvoiceNoticeIDs: JSON.stringify(strIds)
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
                        $('#tab1').myDatagrid('reload');
                    }
                });
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td colspan="8">
                    <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()">导出Excel</a>
                    <a id="btnExportXml" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportXml()">导出Xml</a>
                </td>
            </tr>
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
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="待开票">
        <thead>
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                <th data-options="field:'InvoiceNoticeID',align:'center'" style="width: 180px;">开票编号</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 100px;">客户编号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 250px;">公司名称</th>
                <th data-options="field:'InvoiceTypeDes',align:'center'" style="width: 110px;">开票类型</th>
                <th data-options="field:'Amount',align:'left'" style="width: 150px;">含税金额</th>
                <th data-options="field:'InvoiceDeliveryTypeDes',align:'center'" style="width: 110px;">交付方式</th>
                <th data-options="field:'InvoiceNoticeStatusDes',align:'center'" style="width: 110px;">开票状态</th>
                <th data-options="field:'AdminName',align:'center'" style="width: 110px;">申请人</th>
                <th data-options="field:'InvoiceNoticeCreateDateDes',align:'center'" style="width: 150px;">申请日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>
