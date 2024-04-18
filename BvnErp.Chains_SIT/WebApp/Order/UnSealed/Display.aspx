<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Order.UnSealed.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待封箱信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var Order = eval('(<%=this.Model.Order%>)');
        $(function () {
            //产品列表初始化
            $('#products').myDatagrid({
                nowrap: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
            });

            //产品列表初始化
            $('#DivInf').myDatagrid({
                nowrap: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                onLoadSuccess: function (data) {
                    MergeCells('DivInf', 'BoxIndex', 'BoxIndex,CheckBox,AdminName,PickDate,Status');
                }
            });
        });

        function closeWin() {
            $.myWindow.close();
        }

        /**
        * EasyUI DataGrid根据字段动态合并单元格
        * @param fldList 要合并table的id
        * @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
        */
        function MergeCells(tableID, baseCol, fldList) {
            var dg = $('#' + tableID);
            var fldName = baseCol;
            var RowCount = dg.datagrid("getRows").length;
            var span;
            var PerValue = "";
            var CurValue = "";
            for (row = 0; row <= RowCount; row++) {
                if (row == RowCount) {
                    CurValue = "";
                }
                else {
                    CurValue = dg.datagrid("getRows")[row][fldName];
                }
                if (PerValue == CurValue) {
                    span += 1;
                }
                else {
                    var index = row - span;
                    $.each(fldList.split(","), function (i, val) {
                        dg.datagrid('mergeCells', {
                            index: index,
                            field: val,
                            rowspan: span,
                            colspan: null
                        });
                    });
                    span = 1;
                    PerValue = CurValue;
                }
            }
        }

        //格式化总价
        function FormatTotalPrice(val, row, index) {
            return parseFloat(val).toFixed(2);
        }
    </script>
</head>
<body>
    <div style="margin-left: 5px; margin-top: 10px">
        <label style="font-size: 16px; font-weight: 600; color: orangered">订单产品信息</label>
    </div>
    <div style="text-align: center; margin: 5px;">
        <table id="products" data-options="fit:false">
            <thead>
                <tr>
                    <th data-options="field:'Batch',align:'center'" style="width: 50px">批号</th>
                    <th data-options="field:'Name',align:'left'" style="width: 50px">品名</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 50px">品牌</th>
                    <th data-options="field:'Model',align:'left'" style="width: 50px">型号</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 50px">产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                   <%-- <th data-options="field:'DeclaredQuantity',align:'center'" style="width: 50px">已申报数量</th>--%>
                    <th data-options="field:'TotalPrice',align:'center',formatter:FormatTotalPrice" style="width: 50px">总价</th>
                    <%--<th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重</th>
                    <th data-options="field:'ProductDeclareStatus',align:'center'" style="width: 50px">申报状态</th>--%>
                </tr>
            </thead>
        </table>
    </div>
    <div style="margin-left: 5px; margin-top: 20px">
        <label style="font-size: 16px; font-weight: 600; color: orangered">装箱信息</label>
    </div>
    <div style="text-align: center; margin: 5px;">
        <table id="DivInf" data-options="fit:false,queryParams:{ action: 'dataPackings' }">
            <thead>
                <tr>
                    <th data-options="field:'BoxIndex',align:'center'" style="width: 50px">箱号</th>
                    <th data-options="field:'Model',align:'center'" style="width: 100px">型号</th>
                    <th data-options="field:'CustomsName',align:'left'" style="width: 80px">品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 50px">品牌</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 50px">产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                    <th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重（KG）</th>
                    <th data-options="field:'PickDate',align:'center'" style="width: 50px;">装箱日期</th>
                    <th data-options="field:'Status',align:'center'" style="width: 50px">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
