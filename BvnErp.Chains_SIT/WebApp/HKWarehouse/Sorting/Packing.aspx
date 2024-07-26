<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Packing.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.Packing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>装箱</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style>
        table tr td {
            padding-left: 5px
        }
    </style>
</head>
<script type="text/javascript">
    var wrapTypeData = eval('(<%=this.Model.WarpTypeData%>)');
    var CarrierData = eval('(<%=this.Model.CarrierData%>)');
    var WraptypeValue = eval('(<%=this.Model.WraptypeValue%>)');
    var Qty = getQueryString("Quantity");

    $(function () {
        // 初始化包装类型
        $("#PackingType").combogrid({
            idField: "Code",
            textField: "Name",
            data: wrapTypeData,
            fitColumns: true,
            mode: "local",
            columns: [[
                { field: 'Code', title: 'Code', width: 50, align: 'center', sortable: true },
                { field: 'Name', title: 'Name', width: 120, align: 'center' },
            ]],
            onSelect: function () {
                var grid = $("#PackingType").combogrid('grid');
                var row = grid.datagrid('getSelected');
            },
            //设置默认值代码
            onLoadSuccess: function () {
                $("#PackingType").combogrid("setValue", WraptypeValue);
            },

            keyHandler: {
                up: function () { },
                down: function () { },
                enter: function () { },
                query: function (data) {
                    //动态搜索 
                    $("#PackingType").combogrid("grid").datagrid("reload", { 'keyword': data });
                    $("#PackingType").combogrid("setValue", data);
                }
            }
        });
        //承运商
        $('#Carrier').combobox({
            data: CarrierData,
        });
        //是否国际快递
        $('#isExpress').change(function () {
            if (this.checked) {
                $('#Carrier').textbox('textbox').css('background', 'white');
                $('#Carrier').textbox('textbox').attr('readonly', false);
                $('.input').textbox({
                    required: true,
                    disabled: false,
                });

                $("#WaybillCode").combobox({
                    required: true,
                    disabled: false,
                });
            } else {
                $('#Carrier').textbox('textbox').css('background', '#EBEBE4');
                $('#Carrier').textbox('textbox').attr('readonly', true);
                $('.input').textbox({
                    required: false,
                    disabled: true,
                });

                $("#WaybillCode").combobox({
                    required: false,
                    disabled: true,
                });
            }

        });

        init();

        //加载“运单编号”下拉框中的信息
        $.post('?action=WaybillData', { OrderID: getQueryString("OrderID"), }, function (res) {
            var result = JSON.parse(res);
            $("#WaybillCode").combobox({
                valueField : 'WaybillCode',
                textField : 'WaybillCode',
                editable : true,
                required : true,
                mode : 'local',
                data: result.rows,
            });
        });

    });


    function init() {

        //设置系统当前时间
        var curr_time = new Date();
        var str = curr_time.getMonth() + 1 + "/";
        str += curr_time.getDate() + "/";
        str += curr_time.getFullYear() + " ";
        str += curr_time.getHours() + ":";
        str += curr_time.getMinutes() + ":";
        str += curr_time.getSeconds();
        $('#PackingDate').datebox('setValue', str);
        $('#NewPackingDate').datebox('setValue', str);
        $("ArrivalTime").datebox('setValue', str);
        //初始化默认编辑框
        $('#Carrier').textbox('textbox').css('background', '#EBEBE4');
        $('#Carrier').textbox('textbox').attr('readonly', true);
        $('.input').textbox({
            required: false,
            disabled: true,
        });

        $("#WaybillCode").combobox({
            required: false,
            disabled: true,
        });

        //初始化 文本框的值
        $("#Quantity").textbox('setValue', Qty);
    }

    //装箱
    function Packing() {
        var ewindow = $.myWindow.getMyWindow("Sorting2Packing");

        if (!$("#form1").form('validate')) {
            return;
        }
        else {
            //验证正式箱号
            var sumQty = 0;
            var sortings = ewindow.$('#noticeItemGrid').datagrid('getChecked');
            for (var i = 0; i < sortings.length; i++) {
              sumQty+= sortings[i].OrderQuantity
            }
           
            var BoxIndex = $("#BoxIndex").textbox('getValue');
            if (Number(Qty) > Number(sumQty))
            {
                  $.messager.alert("消息", "装箱数量不能大于可装箱数量");
                    return;
            }
            if (BoxIndex.split("-").length == 2) {
                if (sortings.length > 1) {
                    $.messager.alert("消息", "输入箱号为连续箱号，只能勾选一个装箱产品");
                    return;
                }
            }
            if (BoxIndex.split("-").length - 1 > 1 || BoxIndex.substring(0, 1) == '-' || BoxIndex.split("-")[0].substring(0, 3) != 'HXT' || (BoxIndex.split("-").length > 1 && BoxIndex.split("-")[1].substring(0, 3) != 'HXT')) {
                $("#BoxIndex").focus();
                $.messager.alert("消息", "请输入正确的装箱箱号");
                return;
            }
            var arry = BoxIndex.split('-');
            if (Number(arry[0].replace("HXT", "")) < 1 || (BoxIndex.split("-").length > 1 && Number(arry[1].replace("HXT", "")) < 1) || (BoxIndex.split("-").length > 1 && Number(arry[0].replace("HXT", "")) >= Number(arry[1].replace("HXT", "")))) {
                $("#BoxIndex").focus();
                $.messager.alert("消息", "请输入正确的装箱箱号");
                return;
            }

            var entryNoticeID = getQueryString("ID");
            var orderID = getQueryString("OrderID");
            var carrier = $('#Carrier').combobox("getText");

            var data = new FormData($('#form1')[0]);
            data.append("EntryNoticeID", entryNoticeID);
            data.append("OrderID", orderID);
            data.append("Sortings", JSON.stringify(sortings));
            data.append("Carrier", carrier);
            MaskUtil.mask();//遮挡层

            $.ajax({
                url: '?action=PackingBoxIndex',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        })
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {
            });
        }
    }

    //取消
    function Close() {
        $.myWindow.close();
    }
</script>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin: 0 auto;">
                <tr>
                    <td>装箱日期：</td>
                    <td>
                        <input class="easyui-datebox" data-options="required:true,height:26,width:200,missingMessage:'请选择装箱日期'" id="PackingDate" name="PackingDate" />
                    </td>
                    <td>箱号：</td>
                    <td>
                        <input class="easyui-textbox" id="BoxIndex" name="BoxIndex" data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入箱号'" />
                    </td>
                </tr>
                <tr>
                    <td>重量：</td>
                    <td>
                        <input class="easyui-textbox" id="Weight" name="Weight" data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入重量'" />
                    </td>
                    <td>数量：</td>
                    <td>
                        <input class="easyui-textbox" id="Quantity" name="Quantity" data-options="editable:false,height:26,width:200" />
                    </td>
                </tr>
                <tr>
                    <td>包装类型：</td>
                    <td>
                        <input class="easyui-combogrid" id="PackingType" name="PackingType" data-options="required:true,height:26,width:200" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <input type="checkbox" name="isExpress" id="isExpress" /><label for="isExpress">是否国际快递</label>
                    </td>
                </tr>
                <tr>
                    <td>承运商：</td>
                    <td>
                        <input class="easyui-combobox input" id="Carrier" 
                            data-options="validType:'length[1,50]',height:26,width:200,tipPosition:'bottom',missingMessage:'请选择快递公司'" />
                        <%-- <input class="easyui-combobox" id="Carrier" name="Carrier"
                            data-options="valueField:'CarrierValue',textField:'CarrierText',required:true,height:26,width:200" />--%>
                    </td>
                    <td>运单编号：</td>
                    <td>
                        <input class="easyui-combobox input" id="WaybillCode" name="WaybillCode" data-options="validType:'length[1,50]',height:26,width:200" />
                    </td>

                </tr>
                <tr>
                    <td>到港日期：</td>
                    <td>
                        <input class="easyui-datebox input" id="ArrivalTime" name="ArrivalTime"
                            data-options="tipPosition:'bottom',height:26,width:200,missingMessage:'请输入到港时间'" />
                    </td>
                </tr>
                <tr>
                    <td>库位号：</td>
                    <td>
                        <input class="easyui-textbox" id="ShelveNumber" name="ShelveNumber" data-options=" required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入库位号'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Packing()">装箱</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
      <%--<a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>--%>
    </div>
</body>
</html>
