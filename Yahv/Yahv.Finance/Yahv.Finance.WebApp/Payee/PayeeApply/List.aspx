<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/fileUploader.js"></script>
    <style>
        form {
            height: 96%;
        }
    </style>
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#s_account').combogrid({
                panelWidth: 500,
                fitColumns: true,
                nowrap: false,
                mode: "local",
                data: model.Accounts,
                idField: 'AccountID',
                textField: 'ShortName',
                multiple: false,
                columns: [[
                    { field: 'ShortName', title: '账户简称', width: 150 },
                    { field: 'CompanyName', title: '公司名称', width: 150 },
                    { field: 'BankName', title: '银行名称', width: 120 },
                    { field: 'Code', title: '银行账号', width: 150 },
                    { field: 'CurrencyDes', title: '币种', width: 100 }
                ]],
            });

            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '新增收款',
                    url: '/Finance/Payee/PayeeApply/Edit.aspx',
                    width: "727",
                    height: "550",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            $("#btnWriteOff").click(function () {
                $.myDialog({
                    title: '收款冲销',
                    url: '/Finance/Payee/PayeeApply/WriteOff.aspx',
                    width: "450",
                    height: "260",
                    isHaveOk: false,
                    isHaveCancel: false,
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            $('#fileUploader').fileUploader({
                type: 'PayeeApply',
                required: true,
                accept: 'application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel,application/vnd.ms-excel'.split(','),
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: false,
                iconCls: "icon-yg-excelImport",
                success: function (data) {
                    ajaxLoading();
                    $.post('?action=addRange', { urls: data[0].CallUrl }, function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            $.messager.alert("提示", result.data, 'info', function () {
                                //$("#tab1").myDatagrid('search', getQuery());
                                location.reload();
                                return false;
                            });
                        } else {
                            if (result.data) {
                                Download(result.data);
                            }

                            $.messager.alert("错误提示", "请您检查导入文件!", "error");
                        }
                    });
                }
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_accountid: $.trim($('#s_account').combogrid("getValue")),
                s_formcode: $.trim($('#s_formcode').textbox('getText')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + row.PayeeLeftID + '\');return false;">查看</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="list(\'' + row.PayeeLeftID + '\');return false;">记录</a> '
                , '</span>'].join('');
        }

        function detail(PayeeLeftID) {
            $.myDialog({
                title: '收款详情',
                url: '/Finance/Payee/PayeeApply/Detail.aspx?PayeeLeftID=' + PayeeLeftID,
                width: "727",
                height: "550",
                isHaveOk: false,
                isHaveCancel: true,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

        function list(PayeeLeftID) {
            $.myDialog({
                title: '核销记录',
                url: '/Finance/Payee/PayeeApply/DetailList.aspx?id=' + PayeeLeftID,
                width: "727",
                height: "550",
                isHaveOk: false,
                isHaveCancel: false,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

        //下载文件
        function Download(url) {
            document.getElementById('file_iframe').src = url;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <iframe id="file_iframe" style="display: none;"></iframe>
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">收款人</td>
                <td style="width: 300px;">
                    <input id="s_account" data-options="editable: true," class="easyui-combogrid" style="width: 200px; height: 22px;" />
                </td>
                <td style="width: 90px;">流水号</td>
                <td>
                    <input id="s_formcode" data-options="prompt:''" style="width: 200px;" class="easyui-textbox" />
                </td>

            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">新增</a>
                    <a id="btnWriteOff" class="easyui-linkbutton" iconcls="icon-yg-assign" style="display: none;">冲销</a>
                </td>
                <td>
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                    <a id="fileUploader" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="收款列表">
        <thead>
            <tr>
                <th data-options="field:'PayeeLeftID',align:'center',width:fixWidth(8)">申请编号</th>
                <th data-options="field:'AccountCatalogName',align:'center',width:fixWidth(8)">收款类型</th>
                <th data-options="field:'FormCode',align:'left',width:fixWidth(10)">流水号</th>
                <th data-options="field:'AccountName',align:'left',width:fixWidth(15)">账户简称</th>
                <th data-options="field:'BankName',align:'left',width:fixWidth(15)">银行名称</th>
                <th data-options="field:'CurrencyDes',align:'left',width:fixWidth(4)">币种</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(5)">金额</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(8)">申请人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(11)">创建日期</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(12)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
