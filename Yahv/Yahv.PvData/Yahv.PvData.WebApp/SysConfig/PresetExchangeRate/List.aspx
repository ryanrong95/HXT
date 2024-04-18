<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.PresetExchangeRate.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                from: $('#from').combobox('getValue'),
                to: $('#to').combobox('getValue'),
            };
            return params;
        }

        function btn_formatter(value, rec, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block; width:180px;">'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.TypeValue + '\',\'' + rec.DistrictValue + '\',\'' + rec.FromShortName + '\',\'' + rec.ToShortName + '\',' + index + ');return false;">编辑</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteExchangeRate(\'' + rec.TypeValue + '\',\'' + rec.DistrictValue + '\',\'' + rec.FromShortName + '\',\'' + rec.ToShortName + '\',' + index + ');return false;">删除</a>'
                            + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 汇率编辑
        function edit(type, district, from, to, index) {
            $.myDialog({
                title: '汇率编辑',
                url: '/PvData/SysConfig/PresetExchangeRate/Edit.aspx?type=' + type + '&district=' + district + '&from=' + from + '&to=' + to + '&opt=edit',
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }

        // 汇率删除
        function deleteExchangeRate(type, district, from, to, index) {
            $.messager.confirm('操作提示', '您确定要删除该条汇率吗?', function (r) {
                if (r) {
                    $.post('?action=delete',
                {
                    type: type,
                    district: district,
                    from: from,
                    to: to
                }, function (data) {
                    if (data.success) {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: data.data,
                            type: "success"
                        });
                    }
                    window.grid.myDatagrid('flush');
                }, 'json');
                }
            });


            return false;
        }

        $(function () {
            window.grid = $('#tab1').myDatagrid({
                rownumbers: true,
                pagination: true,
                singleSelect: false,
                queryParams: getQuery(),
                toolbar: '#topper'
            });

            $('#from').combobox({
                data: model.ExchangeCurrency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                editable: false,
                onChange: function (newValue, oldValue) {
                }
            });

            $('#from').combobox('select', model.ExchangeCurrency[0].value);

            $('#to').combobox({
                data: model.ExchangeCurrency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                editable: false,
                onChange: function (newValue, oldValue) {
                }
            });
            $('#to').combobox('select', model.ExchangeCurrency[0].value);

            $('#btnSearch').click(function () {
                window.grid.myDatagrid('search', getQuery());
                return false;
            });

            $('#btnClear').click(function () {
                location.replace(location.href);
                return false;
            });

            $('#btnCreate').click(function () {
                $.myDialog({
                    title: '新增汇率',
                    url: '/PvData/SysConfig/PresetExchangeRate/Edit.aspx?opt=create',
                    width: '80%',
                    height: '80%',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
                return false;
            });
        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao-compact">
            <tbody>
                <tr>
                    <td style="width: 90px">从:</td>
                    <td>
                        <select id="from" class="easyui-combobox"></select>
                    </td>

                    <td style="width: 90px">到:</td>
                    <td>
                        <select id="to" class="easyui-combobox"></select>
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreate" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'ck', checkbox:true, width:20"></th>
                <th data-options="field:'btn', formatter:btn_formatter, width: 130">操作</th>
                <th data-options="field:'Type', width:80">汇率类型</th>
                <th data-options="field:'District', width:80">区域</th>
                <th data-options="field:'From', width:80">币种</th>
                <th data-options="field:'To', width:80">兑换币种</th>
                <th data-options="field:'Value', width:100">汇率</th>
                <th data-options="field:'StartDate', width:100">开始时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>
