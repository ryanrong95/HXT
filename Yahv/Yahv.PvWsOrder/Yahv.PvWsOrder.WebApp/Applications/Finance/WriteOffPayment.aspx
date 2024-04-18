<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="WriteOffPayment.aspx.cs" Inherits="Yahv.PvOms.WebApp.Applications.Finance.WriteOffPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        $(function () {
            //页面初始化
            $("#tab1").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                fit: false,
                pagination: false,
                onLoadSuccess: function (data) {//加载完毕后获取所有的checkbox遍历 
                    if (data.rows.length > 0) {
                        //循环判断操作为新增的不能选择 
                        for (var i = 0; i < data.rows.length; i++) {
                            //根据operate让某些行不可选 
                            if (data.rows[i].LeftPrice == data.rows[i].RightPrice) {
                                $("input[type='checkbox']")[i + 1].disabled = true;
                            }
                        }
                    }
                },
                onClickRow: function (rowIndex, rowData) {
                    //加载完毕后获取所有的checkbox遍历 
                    $("input[type='checkbox']").each(function (index, el) {
                        //如果当前的复选框不可选，则不让其选中 
                        if (el.disabled == true) {
                            $("#tab1").datagrid('unselectRow', index - 1);
                        }
                    })
                }
            });
            $("#tab2").myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'data2',
            });
            $("#Payment").combogrid({
                required: true,
                editable: true,
                idField: 'Value',
                textField: 'Text',
                data: model.FlowAccountData,
                columns: [[
                    { field: 'Text', title: '流水号', width: 120 },
                    { field: 'Price', title: '可用金额', width: 60 },
                    { field: 'CurrencyDec', title: '币种', width: 60 },
                ]],
                onChange: function () {
                    var g = $('#Payment').combogrid('grid');	// get datagrid object
                    var res = g.datagrid('getSelected');	// get the selected row
                    if (res != null) {
                        $("#PaymentAvailable").textbox("setValue", res.Price);
                        $("#Price").numberbox("setValue", res.Price);
                    }
                    else {
                        $("#PaymentAvailable").textbox("setValue", "");
                        $("#Price").numberbox("setValue", "");
                    }
                }
            })
            $("#btnWriteOff").click(function () {
                var g = $('#Payment').combogrid('grid');
                var res = g.datagrid('getSelected');
                if (res == null) {
                    $.messager.alert('提示', "请选择一条实付记录。")
                    return;
                }
                var row = $("#tab1").datagrid("getChecked");
                if (row.length == 0) {
                    $.messager.alert('提示', "请勾选需要核销的一条应付账款。")
                    return;
                }
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var price1 = $("#PaymentAvailable").textbox("getValue");
                var price2 = $("#Price").numberbox("getValue");
                if (Number(price2) > Number(price1)) {
                    $.messager.alert('提示', "核销金额不能大于可用余额。")
                    return;
                }

                var data = new FormData();
                //基本信息
                data.append('FormCode', res.Text);
                data.append('Account', res.Account);
                data.append('Bank', res.Bank);
                data.append('PayableID', row[0].PayableID);
                data.append('Payer', row[0].Payer);
                data.append('Payee', row[0].Payee);
                data.append('Currency', row[0].Currency);
                data.append('Price', price2);
                ajaxLoading();
                $.ajax({
                    url: '?action=WriteOff',
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
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        window.location.reload();
                    }
                })
            })
            //提交
            $("#btnSubmit").click(function () {
                var row = $("#tab1").datagrid("getRows");
                for (var i = 0; i < row.length; i++) {
                    if (row[i].LeftPrice != row[i].RightPrice) {
                        $.messager.alert('提示', "第" + (i + 1) + "行，应付账款未核销完成");
                        return;
                    }
                }

                var data = new FormData();
                //基本信息
                data.append('ID', ID);
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
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //初始化
            Init();
        });
    </script>
    <script>
        function Init() {
            if (model.ApplicationData != null) {
                $("#PayerName").textbox("setValue", model.ApplicationData.OutCompanyName);
                $("#PayeeName").textbox("setValue", model.ApplicationData.PayeeName);
            }
        }
    </script>
    <style>
        .lbl {
            width: 100px;
        }

        .datagrid {
            padding-top: 2px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td class="lbl">付款人:</td>
                    <td>
                        <input id="PayerName" class="easyui-textbox" data-options="disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款人:</td>
                    <td>
                        <input id="PayeeName" class="easyui-textbox" data-options="disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">实付记录：</td>
                    <td>
                        <input id="Payment" class="easyui-combogrid" data-options="editable:false" style="width: 250px;height:22px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">可用金额:</td>
                    <td>
                        <input id="PaymentAvailable" class="easyui-textbox" data-options="disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">核销金额:</td>
                    <td>
                        <input id="Price" class="easyui-numberbox" data-options="required:true,min:0,precision:2" style="width: 250px;" />
                        <a id="btnWriteOff" class="easyui-linkbutton" iconcls="icon-ok">核销</a>
                    </td>
                </tr>
            </table>
            <table id="tab1" title="应付账款列表" style="padding-top: 5px">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true"></th>
                        <th data-options="field:'PayableID',width:100">应付ID</th>
                        <th data-options="field:'PayerName',width:150">付款人</th>
                        <th data-options="field:'PayeeName',width:150">收款人</th>
                        <th data-options="field:'CurrencyName',align:'center',width:100">币种</th>
                        <th data-options="field:'LeftPrice',align:'center',width:100">应收金额</th>
                        <th data-options="field:'RightPrice',align:'center',width:100">已核销金额</th>
                    </tr>
                </thead>
            </table>
            <table id="tab2" title="核销记录列表">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true"></th>
                        <th data-options="field:'PayableID',width:100">应付ID</th>
                        <th data-options="field:'PayerName',width:150">付款人</th>
                        <th data-options="field:'PayeeName',width:150">收款人</th>
                        <th data-options="field:'Currency',align:'center',width:100">币种</th>
                        <th data-options="field:'Price',align:'center',width:100">核销金额</th>
                        <th data-options="field:'CreateDate',align:'center',width:100">核销时间</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">完成</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>

