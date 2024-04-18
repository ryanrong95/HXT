<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnUploadTaxFlow.aspx.cs" Inherits="WebApp.Declaration.Declare.UnUploadTaxFlow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>上传海关缴税流水</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />

<%--     <script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '上传缴税流水';
        gvSettings.summary = '报关单未上传';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#decheads').myDatagrid({
                checkOnSelect: false,
                singleSelect: false,
            });


            //注册文件上传的onChange事件
            $('#uploadExcel').filebox({
                buttonText: '上传缴税流水',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择Excel文件',
                accept: ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if ($('#uploadExcel').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=UploadExcel',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message);
                            $('#decheads').myDatagrid('reload');
                        }
                    });
                }
            });
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');

            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                ContrNo: ContrNo,
                EntryID: EntryID,
                OrderID: OrderID,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#EntryID').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            //buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="setNoTaxFlow(\''
            //            + row.ID + '\',\'' + row.ContrNo + '\',\'' + row.EntryID + '\',\'' + row.OrderID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">无缴税流水</span>' +
            //    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

            if (!row.IsHandledTariff) {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="setNoTaxFlow(\''
                        + row.ID + '\',\'' + row.ContrNo + '\',\'' + row.EntryID + '\',\'' + row.OrderID
                        + '\',\'' + <%=Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()%> + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无关税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            } else {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无关税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }

            if (!row.IsHandledExciseTax) {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="setNoTaxFlow(\''
                        + row.ID + '\',\'' + row.ContrNo + '\',\'' + row.EntryID + '\',\'' + row.OrderID
                        + '\',\'' + <%=Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()%> + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无消费税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            } else {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无消费税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }

            if (!row.IsHandledAddedValueTax) {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="setNoTaxFlow(\''
                        + row.ID + '\',\'' + row.ContrNo + '\',\'' + row.EntryID + '\',\'' + row.OrderID
                        + '\',\'' + <%=Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()%> + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无增值税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            } else {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">无增值税</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }

            return buttons;
        }

        //设置无缴费流水
        function setNoTaxFlow(DecHeadID, ContrNo, EntryID, OrderID, handledType) {
            var dataStr = '<br>合同号：' + ContrNo + '<br>海关编号：' + EntryID + '<br>订单编号：' + OrderID;
            var handledTypeDes = "";
            if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()%>') {
                handledTypeDes = "关税";
            } else if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()%>') {
                handledTypeDes = "消费税";
            } else if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()%>') {
                handledTypeDes = "增值税";
            }

            $.messager.confirm('确认', '确定该数据无<span style="color: green;">' + handledTypeDes + '</span>？' + dataStr, function (r) {
	            if (r){
                    $.post('?action=SetNoDecTaxFlow', { DecHeadID: DecHeadID, OrderID: OrderID, HandledType: handledType }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', '提交成功', 'info', function () {
                                $('#decheads').datagrid('reload');
                            });
                        } else {
                            $.messager.alert('错误', result.message, 'error');
                            return;
                        }

                    });
	            }
            });
        }

        //批量设置无税费
        function SetNoTaxBatch(handledType) {
            var rows = $('#decheads').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('消息', '请先勾选报关单！', 'info');
                return;
            }

            var handledTypeDes = "";
            if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()%>') {
                handledTypeDes = "关税";
            } else if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()%>') {
                handledTypeDes = "消费税";
            } else if (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()%>') {
                handledTypeDes = "增值税";
            }

            var params = [];
            var dataStr = '';
            $.each(rows, function (i, val) {
                if ((handledType == '<%=Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()%>' && !val.IsHandledTariff)
                    || (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()%>' && !val.IsHandledExciseTax)
                    || (handledType == '<%=Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()%>' && !val.IsHandledAddedValueTax))
                {
                    params.push({ ID: val.ID, ContrNo: val.ContrNo, EntryID: val.EntryID, OrderID: val.OrderID, HandledType: handledType });
                    dataStr += '<br>合同号：' + val.ContrNo;
                }
            });

            if (params.length < 1) {
                $.messager.alert('错误', '勾选的报关单没有可处理的' + handledTypeDes, 'error');
                return;
            }

            $.messager.confirm('确认', '确定报关单无<span style="color: green;">' + handledTypeDes + '</span>？' + dataStr, function (r) {
                if (r) {
                    $.post('?action=SetNoTaxBatch', { Param: JSON.stringify(params) }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', '提交成功', 'info', function () {
                                $('#decheads').datagrid('reload');
                            });
                        } else {
                            $.messager.alert('错误', result.message, 'error');
                            return;
                        }
                    });
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="line-height: 30px">
                    <tr>
                        <td colspan="4">
                            <%-- <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 90px; height: 26px" />--%>
                            <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 100px; height: 26px" />
                            <span style="padding-left:10px">
                                <a id="noExciseTax" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="SetNoTaxBatch(<%=Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()%>)" style="height:26px;">无消费税</a>
<%--                            <a id="noTariff" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="SetNoTaxBatch(<%=Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()%>)" style="height:26px;">无关税</a>
                            <a id="noAddedValueTax" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="SetNoTaxBatch(<%=Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()%>)" style="height:26px;">无增值税</a>--%>
                            </span>

                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">合同号: </td>
                        <td>
                            <input class="easyui-textbox" id="ContrNo" data-options="validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 10px;">海关编号: </td>
                        <td>
                            <input class="easyui-textbox" id="EntryID" data-options="validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 10px;">订单编号:</td>
                        <td class="lbl">
                            <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                        </td>

                    </tr>
                    <tr>
                        <td class="lbl">报关日期: </td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                        </td>
                        <td class="lbl" style="padding-left: 10px;">至</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                        </td>
                        <td colspan="2" style="padding-left: 10px;">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="报关单" data-options="
            border:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 200px;">合同号</th>
                    <th data-options="field:'EntryID',align:'center'" style="width: 200px;">海关编号</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 200px;">订单编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 150px;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 150px;">报关金额</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 150px;">报关日期</th>
                    <th data-options="field:'Status',align:'center'" style="width: 150px;">报关状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 270px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
