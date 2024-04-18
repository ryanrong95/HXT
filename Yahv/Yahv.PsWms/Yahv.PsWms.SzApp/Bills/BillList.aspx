<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BillList.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.List" %>

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
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Apply(' + index + ');return false;">申请开票</a> '
                , '</span>'].join('');
        }
        //
        function Apply(index) {
            var row = $("#tab1").myDatagrid('getRows')[index];
            var data = new FormData();
            var vouchers = [];
            vouchers.push(row);
            data.append('vouchers', JSON.stringify(vouchers));
            ajaxLoading();
            $.ajax({
                url: '?action=Submit',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    ajaxLoadEnd();
                    var res = eval(res);
                    if (res.success) {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                }
            })
        };
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
                    <input id="dateIndex" class="easyui-numberbox" style="width: 150px" />
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
    <table id="tab1">
        <thead>
            <tr>
                <%-- <th data-options="field:'ck', checkbox:true"></th>--%>
                <th data-options="field:'PayerName',align:'left'" style="width: 200px">客户名称</th>
                <th data-options="field:'Total',align:'center'" style="width: 100px">账单金额</th>
                <th data-options="field:'CutDateIndex',align:'center'" style="width: 100px">账单期号</th>
                <th data-options="field:'ReceiptTotal',align:'center'" style="width: 100px">实收金额</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
