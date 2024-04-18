<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ClientStock.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.ClientStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ClientID = getQueryString("ID");
        var EnterCode = getQueryString("EnterCode");
        var currencyData = model.currencyData;

        $(function () {
            //页面初始化
            $("#ClientID").textbox("setValue", EnterCode);
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: false,
                pagination: false,
                fitColumns: true,
                scrollbarSize: 0,
                rownumbers: true,
            });
            $("#Currency").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: currencyData,
                onChange: function () {
                    $('#btnSearch').click();
                }
            })
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#Manufacturer').textbox("setText", "");
                $('#PartNumber').textbox("setText", "");
                $('#DateCode').textbox("setText", "");
                $('#Currency').textbox("setValue", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //申请报关
            $('#btnDeclare').click(function () {
                ToDeclare();
            })
            //申请发货
            $('#btnDelivery').click(function () {
                ToDelivery();
            })
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientID: $.trim($('#ClientID').textbox("getText")),
                Manufacturer: $.trim($('#Manufacturer').textbox("getText")),
                PartNumber: $.trim($('#PartNumber').textbox("getText")),
                DateCode: $.trim($('#DateCode').textbox("getText")),
                Currency: $.trim($('#Currency').textbox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //申请报关
        function ToDeclare() {
            if (!Validation()) {
                return;
            }
            var ids = GetIds();
            $.myWindow({
                title: '新增转报关订单',
                url: 'AddTurnDeclare.aspx?ClientID=' + ClientID + '&EnterCode=' + EnterCode + "&Ids=" + ids,
                //width: 1351,
                //height: 672,
                minWidth: 1200,
                minHeight: 600,
                onClose: function () {
                    grid.myDatagrid('search', getQuery());
                },
            })
        }
        //申请发货
        function ToDelivery() {
            if (!Validation()) {
                return;
            }
            var ids = GetIds();
            $.myWindow({
                title: '新增代发货订单',
                url: 'AddDelivery.aspx?ClientID=' + ClientID + '&EnterCode=' + EnterCode + "&Ids=" + ids,
                //width: 1351,
                //height: 672,
                minWidth: 1200,
                minHeight: 600,
                onClose: function () {
                    grid.myDatagrid('search', getQuery());
                },
            })
        }
        //验证条件
        function Validation() {
            //验证订单项数量(注：合计行)
            var rows = $("#tab1").datagrid("getChecked");
            if (rows.length == 0) {
                $.messager.alert('提示', '请勾选库存产品。');
                return false;
            }
            if (rows.length > 100) {
                $.messager.alert('提示', '勾选数量不能超过100项。');
                return false;
            }
            return true;
        }
        //获取勾选项ID
        function GetIds() {
            var rows = $("#tab1").datagrid("getChecked");
            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });
            return arry.join(",");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户入仓号</td>
                <td>
                    <input id="ClientID" class="easyui-textbox" style="width: 250px" data-options="disabled:true" />
                </td>
                <td style="width: 90px;">型号</td>
                <td>
                    <input id="PartNumber" class="easyui-textbox" style="width: 250px" />
                </td>
                <td style="width: 90px;">品牌</td>
                <td>
                    <input id="Manufacturer" class="easyui-textbox" style="width: 250px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">入库日期</td>
                <td>
                    <input id="StartDate" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" class="easyui-datebox" style="width: 108px" />
                </td>
                <td style="width: 90px;">币种</td>
                <td>
                    <input id="Currency" class="easyui-combobox" style="width: 250px" />
                </td>
                <td style="width: 90px;">批次号</td>
                <td>
                    <input id="DateCode" class="easyui-textbox" style="width: 250px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnDelivery" class="easyui-linkbutton" iconcls="icon-yg-assign">申请发货</a>
                    <a id="btnDeclare" class="easyui-linkbutton" iconcls="icon-yg-assign">申请报关</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:'true'"></th>
                <th data-options="field:'CreateDateStr',align:'center'" style="width: 60px;">入库日期</th>
                <th data-options="field:'Manufacturer',align:'left'" style="width: 100px">品牌</th>
                <th data-options="field:'PartNumber',align:'left'" style="width: 100px;">型号</th>
                <th data-options="field:'DateCode',align:'center'" style="width: 50px">批次号</th>
                <th data-options="field:'OriginDec',align:'center'" style="width: 50px">产地</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 50px;">库存数量</th>
                <th data-options="field:'CurrencyDec',align:'center'" style="width: 50px;">币种</th>
                <th data-options="field:'TotalPrice',align:'center'" style="width: 50px;">总价值</th>
                <%--<th data-options="field:'GrossWeight',align:'center'" style="width: 50px;">毛重(kg)</th>--%>
            </tr>
        </thead>
    </table>
</asp:Content>
