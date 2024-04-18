<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.Receives.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (row["ReceiptTotal"] == null) {
                            row["ReceiptTotal"] = Number("0").toFixed(2);
                        }
                        else {
                            row["ReceiptTotal"] = Number(row["ReceiptTotal"]).toFixed(2);
                        }
                    }
                    return data;
                },
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#clientName').textbox("setText", "");
                $('#dateIndex').numberbox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#clientName').textbox("getText")),
                DateIndex: $.trim($('#dateIndex').numberbox("getText")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Details(' + index + ');return false;">查看</a> '
                , '</span>'].join('');
        }
        //详情
        function Details(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '账单期号',
                minWidth: 1200,
                minHeight: 600,
                url: 'Details.aspx?ID=' + data.ID + '&Index=' + data.CutDateIndex,
                onClose: function () {
                },
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="clientName" data-options="prompt:''," class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">账单期号</td>
                <td>
                    <input id="dateIndex" data-options="prompt:'6位数字',validType:'length[6,6]'" class="easyui-numberbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="应收账单列表">
        <thead>
            <tr>
                <th data-options="field:'PayerName',align:'left'" style="width: 300px">客户名称</th>
                <th data-options="field:'CutDateIndex',align:'left'" style="width: 150px;">账单期号</th>
                <th data-options="field:'Total',align:'left'" style="width: 150px">账单金额</th>
                <th data-options="field:'ReceiptTotal',align:'left'" style="width: 150px">实收金额</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
