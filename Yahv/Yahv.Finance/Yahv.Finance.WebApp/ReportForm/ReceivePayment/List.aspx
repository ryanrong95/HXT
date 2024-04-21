<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.ReportForm.ReceivePayment.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        $(function () {
            //默认当天日期
            var date = new Date();
            var today = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();

            $('#s_begin').datebox('setValue', today);
            $('#s_end').datebox('setValue', today);

            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery(),
                onLoadSuccess: function (data) {
                    if (data.total>0) {
                        $('#sp_summary').html(data.rows[0].Summary);
                    }
                }
            });

            //搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#s_goldStore').combobox({
                textField: 'text',
                valueField: 'value',
                data: model.GoldStores,
            });

            //账户
            $('#s_account').combogrid({
                data: model.PayerAccounts,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "ShortName",
                panelWidth: 500,
                mode: "remote",
                columns: [[
                    { field: 'ShortName', title: '账户简称', width: 100, align: 'left' },
                    { field: 'CompanyName', title: '公司名称', width: 100, align: 'left' },
                    { field: 'BankName', title: '银行名称', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    //不根据ID 自动选择
                    if (now.indexOf('Account') < 0)
                        doSearch(now, model.PayerAccounts, ['ShortName', 'CompanyName', 'BankName', 'Code', 'Currency'], $(this));
                }
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_begin: $.trim($('#s_begin').datebox("getText")),
                s_end: $.trim($('#s_end').datebox("getText")),
                s_goldStore: $.trim($('#s_goldStore').combobox('getValue')),
                s_account: $.trim($('#s_account').combogrid('getValue')),
            };
            return params;
        };

        //q为用户输入，data为远程加载的全部数据项，searchList是需要进行模糊搜索的列名的数组，ele是combogrid对象
        //doSearch的思想其实就是，进入方法时将combogrid加载的数据清空，如果用户输入为空则加载全部的数据，输入不为空
        //则对每一个数据项做匹配，将匹配到的数据项加入rows数组，相当于重组数据项，只保留符合筛选条件的数据项，
        //如果筛选后没有数据，则combogrid加载空，有数据则重新加载重组的数据项
        function doSearch(q, data, searchList, ele) {
            ele.combogrid('grid').datagrid('loadData', []);
            if (q == "") {
                ele.combogrid('grid').datagrid('loadData', data);
                return;
            }
            var rows = [];
            $.each(data, function (i, obj) {
                for (var p in searchList) {
                    var v = obj[searchList[p]];
                    if (!!v && v.toString().indexOf(q) >= 0) {
                        rows.push(obj);
                        break;
                    }
                }
            });
            if (rows.length == 0) {
                ele.combogrid('grid').datagrid('loadData', []);
                return;
            }
            ele.combogrid('grid').datagrid('loadData', rows);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">创建日期</td>
                <td style="width: 300px;">
                    <input id="s_begin" type="text" class="easyui-datebox" editable="fasle" />
                    - 
                    <input id="s_end" type="text" class="easyui-datebox" editable="fasle" />
                </td>
                <td style="width: 90px;">金库</td>
                <td style="width: 300px;">
                    <input id="s_goldStore" style="width: 200px;" class="easyui-combobox" data-options="editable:false" />
                </td>
                <td style="width: 90px;">账户</td>
                <td>
                    <select id="s_account" style="width: 200px" class="easyui-combogrid" />
                </td>

            </tr>
            <tr>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                </td>
                <td colspan="4" style="text-align: right">
                    <span id="sp_summary" style="color: red; font-weight: bold;"></span>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="收付款明细">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:fixWidth(10)">创建日期</th>
                <th data-options="field:'GoldStore',align:'center',width:fixWidth(8)">金库</th>
                <th data-options="field:'AccountName',align:'center',width:fixWidth(15)">账户</th>
                <th data-options="field:'EnterpriseName',align:'center',width:fixWidth(15)">公司</th>
                <th data-options="field:'Target',align:'center',width:fixWidth(15)">对方</th>
                <th data-options="field:'AccountMethord',align:'center',width:fixWidth(5)">行为</th>
                <th data-options="field:'Currency',align:'center',width:fixWidth(4)">币种</th>
                <th data-options="field:'Amount',align:'left',width:fixWidth(8)">金额</th>
                <th data-options="field:'Balance',align:'left',width:fixWidth(8)">余额</th>
                <th data-options="field:'CreatorName',align:'center',width:fixWidth(5)">操作人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
