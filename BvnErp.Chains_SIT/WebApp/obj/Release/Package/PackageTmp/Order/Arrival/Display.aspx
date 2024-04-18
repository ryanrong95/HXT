<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Order.Arrival.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>拆分报关</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var Order = eval('(<%=this.Model.Order%>)');
        $(function () {
            //产品列表初始化
            $('#products').myDatagrid({
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
            });

            //产品列表初始化
            $('#DivInf').myDatagrid({
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                onLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        //循环判断操作为新增的不能选择
                        for (var i = 0; i < data.rows.length; i++) {
                            //根据isFinanceExamine让某些行不可选                           
                            if (data.rows[i].DecStatus != '未报关') {
                                $("input[type='checkbox']")[i + 1].disabled = true;
                            }
                        }
                    }
                    MergeCells('DivInf', 'BoxIndex', 'BoxIndex,CheckBox,AdminName,PickDate,Status');
                },
                onCheck: function (rowIndex, rowData) {
                    $.each($("#DivInf").datagrid("getRows"), function (index, val) {
                        if (val.BoxIndex == rowData.BoxIndex) {
                            if ($("#DivInf").parent().find("div.datagrid-cell-check").children("input[type='checkbox']").eq(index + 1).is(':checked')) {
                                return;
                            }
                            if ((index + 1) < $("#DivInf").datagrid("getRows").length) {
                                $('#DivInf').datagrid('checkRow', index + 1);
                            }
                        }
                    });
                },
                onUncheck: function (rowIndex, rowData) {
                    $.each($("#DivInf").datagrid("getSelections"), function (index, val) {
                        if (val.BoxIndex == rowData.BoxIndex) {
                            $('#DivInf').datagrid('uncheckRow', index + 1);
                        }
                    });
                },
                onClickRow: function (rowIndex, rowData) {
                    $("input[type='checkbox']").each(function (index, el) {
                        //如果当前的复选框不可选，则不让其选中
                        if (el.disabled == true) {
                            $('#DivInf').datagrid('unselectRow', index - 1);
                        }
                    })
                },
                onSelectAll: function (rowIndex, rowData) {
                    $("input[type='checkbox']").each(function (index, el) {
                        //已封箱 不可选择
                        if (el.disabled == true) {
                            $('#DivInf').datagrid('unselectRow', index - 1);
                        }
                    })
                },
                onCheckAll: function () {
                    $("input[type='checkbox']").each(function (index, el) {
                        //checkbox取消✔样式
                        if (el.disabled == true) {
                            $(el).prop('checked', false);
                        }
                    })
                }
            });


            //订单挂起，无法拆分报关
            if (Order.IsHangUp) {
                $("#hangUpMsg").show();
                $("#btnSplit").hide();
            }

        });

        //返回
        function Return() {
            closeWin();
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

        //拆分生成报关通知
        function Split() {
            var ids = [];
            var rows = $('#DivInf').datagrid('getSelections');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要拆分报关的箱号！');
                return;
            }
            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].SortingID);
            }
            id = ids.join();
            var model = {
                ID: id,
                OrderID: Order.ID,
                Summary: $('#Summary').textbox('getValue')
            };
            $.messager.confirm('确认', '请您再次确认将已勾选箱号拆分报关？', function (success) {
                if (success) {
                    $.post('?action=SplitDeclare', model, function (res) {
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                            if (result.success) {
                                closeWin();
                            } else {

                            }
                        });
                    });
                }
            });

        }

        function closeWin() {
            $.myWindow.close();
        }

        //格式化总价
        function FormatTotalPrice(val, row, index) {
            return parseFloat(val).toFixed(2);
        }
    </script>
</head>
<body>
    <div style="margin-left: 5px; margin-top: 10px">
        <label style="font-size: 16px; font-weight: 600; color: orangered">产品信息</label>
    </div>
    <div style="text-align: center; margin: 5px;">
        <table id="products" data-options="fit:false">
            <thead>
                <tr>
                    <th data-options="field:'Batch',align:'center'" style="width: 50px">批号</th>
                    <th data-options="field:'Name',align:'center'" style="width: 50px">品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 50px">品牌</th>
                    <th data-options="field:'Model',align:'center'" style="width: 50px">型号</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 50px">产地</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                    <th data-options="field:'DeclaredQuantity',align:'center'" style="width: 50px">已申报数量</th>
                    <th data-options="field:'TotalPrice',align:'center',formatter:FormatTotalPrice" style="width: 50px">总价</th>
                    <th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重</th>
                    <th data-options="field:'ProductDeclareStatus',align:'center'" style="width: 50px">申报状态</th>
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
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'BoxIndex',align:'center'" style="width: 50px">箱号</th>
                    <th data-options="field:'Model',align:'center'" style="width: 100px">型号</th>
                    <th data-options="field:'CustomsName',align:'center'" style="width: 80px">品名</th>
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
    <div style="margin-left: 5px; margin-top: 10px">
        <label style="font-size: 16px; font-weight: 600; color: orangered">拆分原因</label>
    </div>
    <div style="text-align: center; margin: 5px;">
        <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,500]',tipPosition:'bottom',multiline:true," style="width: 100%; height: 60px;" />
    </div>
    <div id="hangUpMsg" style="margin-left: 5px; margin-top: 30px; display: none;">
        <label style="font-size: 18px; font-weight: 600; color: red">该订单处于挂起状态，无法拆分报关！</label>
    </div>
    <div style="text-align: center; margin: 15px;">
        <a id="btnSplit" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Split()">拆单报关</a>
        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Return()">取消</a>
    </div>
</body>
</html>
