<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Split.aspx.cs" Inherits="WebApp.PayExchange.Auditing.Split" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>拆分</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script>
        var PaymentTypeData = eval('(<%=this.Model.PaymentTypeData%>)');
        var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
        var ID = getQueryString("ID");
        var SupplierData = eval('(<%=this.Model.SupplierData%>)');

        var bankSensitiveFlag = 1; //银行敏感性标志  1-未检测, 2-已检测,是敏感, 3-已检测,不是敏感

        $(function () {
            //订单信息初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                pagination: false, //启用分页
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: true,
                onClickRow: onClickRow,
            });

            if (PayExchangeApplyData.PaymentType != "") {
                $('#PaymentType').combobox({
                    data: PaymentTypeData,
                    onLoadSuccess: function (data) {
                        if (data) {
                            $('#PaymentType').combobox('setValue', PayExchangeApplyData.PaymentTypeInt);
                        }
                    }
                });
            }
            if (PayExchangeApplyData.ExpectPayDate != "") {
                $('#ExpectPayDate').datebox('setText', PayExchangeApplyData.ExpectPayDate);
            }
            if (PayExchangeApplyData.ABA != "") {
                $('#ABA').textbox('setText', PayExchangeApplyData.ABA);
            }
            if (PayExchangeApplyData.IBAN != "") {
                $('#IBAN').textbox('setText', PayExchangeApplyData.IBAN);
            }
            //初始化供应商下拉框
            $('#SupplierName').combobox({
                data: SupplierData,
                editable: false,
                onChange: function (newValue, oldValue) {
                    var supplierID = '';
                    for (var i = 0; i < SupplierData.length; i++) {
                        if (SupplierData[i].ID == newValue) {
                            supplierID = SupplierData[i].ID;
                            break;
                        }
                    }

                    if (supplierID != null) {
                        //访问后台
                        $("#SupplierID").textbox('setValue', supplierID);
                        $.post('?action=SelectSupplier', { SupplierID: supplierID }, function (result) {
                            var rel = JSON.parse(result);
                            if (rel.success) {
                                var data = rel.data;
                                //更新银行地址数据
                                $('#BankAddress').textbox('setValue', data.BankAddress);
                                var bankdata = data;
                                $('#BankName').combobox({
                                    data: eval(bankdata)
                                });
                                //$('#BankAddress').textbox('setValue', "");
                            }
                            else {
                                $.messager.alert('提示', rel.data);
                            }
                        })
                    }
                }
            });
            //选择银行地址
            $('#BankName').combobox({
                onChange: function () {
                    bankSensitiveFlag = 1; //置为未检测银行是否敏感国家
                    $("#SensitiveTip").hide();

                    var BankID = $(this).combobox('getValue');
                    $("#BankID").textbox('setValue', BankID);
                    $.post('?action=SelectBank', { BankID: BankID }, function (result) {
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            var data = rel.data;
                            $('#BankAddress').textbox('setValue', data.BankAddress);
                            $('#SwiftCode').textbox('setValue', data.SwiftCode);
                        }
                        else {
                            $.messager.alert('提示', rel.data);
                        }
                    })
                }
            });

            $('#ExchangeRate').numberbox({
                onChange: function () {
                    var rate = $('#ExchangeRate').numberbox('getValue');
                    var rmbPrice = Number(PayExchangeApplyData.Price) * Number(rate);
                    $('#RmbPrice').text(rmbPrice + "(RMB)");
                }
            })
            initData();
        });

        function initData() {
            $("#OldApplyID").text(ID);
            $('#ExchangeRate').numberbox('setValue', PayExchangeApplyData.ExchangeRate);
            $('#Price').text(PayExchangeApplyData.Price + "(" + PayExchangeApplyData.Currency + ")");
            $('#RmbPrice').text(PayExchangeApplyData.RmbPrice + "(RMB)");


        }
        //拆分
        function Split() {
            var supplierName = $("#SupplierName").combobox('getText');
            var supplierID = $("#SupplierID").textbox('getValue');
            var bankID = $("#BankID").textbox('getValue');
            var bankName = $("#BankName").combobox('getText');
            var ABA = $("#ABA").textbox('getText');
            var IBAN = $("#IBAN").textbox('getText');
            var paymentType = $("#PaymentType").combobox('getValue');
            var expectPayDate = $("#ExpectPayDate").combobox('getValue');
            var BankAddress = $('#BankAddress').textbox('getValue');
            var SwiftCode = $('#SwiftCode').textbox('getValue');
            var NewExchangeRate = $('#ExchangeRate').numberbox('getValue');
            debugger;
            if (supplierName == "") {
                $.messager.alert('提示', '请选择供应商！');
                return;
            }
            if (bankName == "") {
                $.messager.alert('提示', '请选择银行名称！');
                return;
            }

            if (BankAddress == "") {
                $.messager.alert('提示', "银行地址不能为空，无法拆分，请先维护银行地址！");
                return;
            }
            if (SwiftCode == "") {
                $.messager.alert('提示', "银行代码不能为空，无法拆分，请先维护银行代码！");
                return;
            }
            var selectedRows = $("#datagrid").datagrid("getChecked");
            var Rows = $("#datagrid").datagrid("getRows");
            if (selectedRows.length < 1) {
                $.messager.alert('提示', '请勾选订单编号！');
                return;
            }
            var SplitInfo = [];
            var checkflag = false;
            for (var i = 0; i < selectedRows.length; i++) {

                if (selectedRows[i]["Amount"] == null || selectedRows[i]["Amount"] == "") {
                    checkflag = true;
                    $.messager.alert('提示', '本次申请金额不能为空！');
                    selectedRows[i]["Amount"] = 0;
                    $('#datagrid').datagrid('refreshRow', i);
                }
                if (Number(selectedRows[i]["OldAmount"]) <= Number(selectedRows[i]["Amount"])) {
                    checkflag = true;
                    $.messager.alert("消息", "当前申请金额不能大于本次可申请金额");
                    row["Amount"] = row["OldAmount"];
                    $('#datagrid').datagrid('refreshRow', i);
                }
                if (Number(selectedRows[i]["Amount"]) < 0) {
                    checkflag = true;
                    $.messager.alert("消息", "申请金额不能小于零");
                    row["Amount"] = 0;
                    $('#datagrid').datagrid('refreshRow', i);
                }
                SplitInfo.push({
                    ID: ID,
                    SupplierID: supplierID,
                    BankID: bankID,
                    ABA: ABA,
                    IBAN: IBAN,
                    PaymentType: paymentType,
                    ExpectPayDate: expectPayDate,
                    OrderID: selectedRows[i]["OrderID"],
                    Amount: Number(selectedRows[i]["Amount"]),
                    PaidAmount: Number(selectedRows[i]["OldAmount"]),
                    RowsCount: Rows.length,
                    FileUrl: PayExchangeApplyData.FileUrl,
                    AdvanceMoney: PayExchangeApplyData.AdvanceMoney,
                    NewExchangeRate: NewExchangeRate
                });
            }

            if (checkflag) {
                return;
            }

            $.messager.confirm('确认', '请您确认已勾选订单是否拆分付汇？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=SplitCheck', { Model: JSON.stringify(SplitInfo) }, function (result) {
                        debugger;
                        MaskUtil.unmask();
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                //返回列表页
                                // window.self.close();
                                Return();
                                //var url = location.pathname.replace(/Audit.aspx/ig, 'List.aspx');
                                //window.location = url;
                            }
                        });
                    })
                }
            })
        };
        //返回
        function Return() {
            $.myWindow.close();
        }
        //行编辑
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#datagrid').datagrid('validateRow', editIndex)) {
                $('#datagrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    VerifyQuantity();
                    $('#datagrid').datagrid('selectRow', index);
                    $('#datagrid').datagrid('beginEdit', index);
                    $('#datagrid').datagrid('uncheckRow', index);
                    editIndex = index;
                    var ed = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'Amount' });
                    if (ed) {
                        $(ed.target).textbox({
                            onChange: function (newValue, oldValue) {
                                if (editIndex == undefined)
                                    return;
                                var rows = $("#datagrid").datagrid("getRows");
                                var row = rows[index]
                                var amount = row.Amount;
                                if (newValue < 0) {
                                    $.messager.alert('提示', '申请金额不能小于零');
                                    $(ed.target).textbox("setValue", "0");
                                }
                                if (newValue > row.OldAmount) {
                                    $.messager.alert('提示', '当前申请金额不能大于本次可申请金额');
                                    $(ed.target).textbox("setValue", amount);
                                }
                                $('#datagrid').datagrid('updateRow', {
                                    index: index,
                                    row: {
                                        Amount: newValue
                                    }
                                });
                            }
                        });
                    }
                }
                else {
                    $('#datagrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                VerifyQuantity(index);
                editIndex = undefined;
            }
        }
        function VerifyQuantity(Index) {
            if (Index != null) {
                $('#datagrid').datagrid('acceptChanges');
                $('#datagrid').datagrid('selectRow', Index);
                var row = $('#datagrid').datagrid('getSelected');
                if (Number(row["OldAmount"]) < Number(row["Amount"])) {
                    $.messager.alert("消息", "当前申请金额不能大于本次可申请金额");
                    row["Amount"] = row["OldAmount"];
                    var index = $('#datagrid').datagrid('getRowIndex', row);
                    $('#datagrid').datagrid('refreshRow', index);
                }
                if (Number(row["Amount"]) < 0) {
                    $.messager.alert("消息", "申请金额不能小于零");
                    row["Amount"] = 0;
                    var index = $('#datagrid').datagrid('getRowIndex', row);
                    $('#datagrid').datagrid('refreshRow', index);
                }
                $('#datagrid').datagrid('acceptChanges');
            }
        }
    </script>
</head>
<body>
    <div class="easyui-panel" style="width: 100%; height: 100%; border: 0px;">
        <div style="text-align: center; height: 30%; margin: 5px;">
            <table style="width: 100%;" cellspacing="2" cellpadding="0">

                <tr>
                    <td class="lbl">原付汇申请：</td>
                    <td>
                        <label class="lbl" id="OldApplyID"></label>
                    </td>
                    <td class="lbl">付款金额：
                    </td>
                    <td>
                        <label class="lbl" id="Price"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">原付汇汇率:</td>
                    <td>
                        <input type="text" id="ExchangeRate" class="easyui-numberbox" data-options="min:0,precision:6,required:true,border:false,height:26,width:200" />
                    </td>
                    <td class="lbl">应收款：</td>
                    <td>
                        <label class="lbl" id="RmbPrice"></label>
                    </td>
                </tr>
                <tr><td><label style="height: 15px;display: block;"> </label></td></tr>
                <tr>
                    <td class="lbl">供应商名称：</td>
                    <td>
                        <input class="easyui-combobox" id="SupplierName" name="SupplierName"
                            data-options="limitToList:true,required:true,valueField:'ID',textField:'ChineseName',height:26,width:200" />
                    </td>
                    <td class="lbl">银行名称：</td>
                    <td>
                        <input class="easyui-combobox" id="BankName" name="BankName"
                            data-options="limitToList:true,required:true,valueField:'ID',textField:'BankName',height:26,width:200" />
                        <br />
                        <label id="SensitiveTip" style="color: red; display: none;">此银行涉及敏感地区，无法申请付汇</label>
                    </td>

                </tr>
                <tr style="display: none">
                    <td class="lbl">供应商地址：</td>
                    <td>
                        <input class="easyui-combobox" id="SupplierAddress" name="SupplierAddress"
                            data-options="limitToList:true,valueField:'ID',textField:'Name',height:26,width:200" />
                    </td>
                    <td class="lbl">银行代码：</td>
                    <td>
                        <input class="easyui-textbox" id="SwiftCode" name="SwiftCode"
                            data-options="limitToList:true,required:true,height:26,width:200" />
                    </td>
                </tr>
                <tr style="display: none">
                    <td class="lbl">供应商ID：</td>
                    <td>
                        <input class="easyui-textbox" id="SupplierID" name="SupplierID"
                            data-options="limitToList:true,required:true,height:26,width:200" />
                    </td>
                    <td class="lbl">银行ID：</td>
                    <td>
                        <input class="easyui-textbox" id="BankID" name="BankID"
                            data-options="limitToList:true,required:true,height:26,width:200" />
                    </td>
                </tr>
                <tr style="display: none">
                    <td class="lbl">银行地址：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAddress" name="BankAddress"
                            data-options="limitToList:true,required:true,height:26,width:200" />
                    </td>
                    <td class="lbl">银行账号：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAccount" name="BankAccount"
                            data-options="limitToList:true,required:true,height:26,width:200" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">ABA：</td>
                    <td>
                        <input class="easyui-textbox" id="ABA" name="ABA"
                            data-options="height:26,width:200,validType:'length[0,50]'," />
                    </td>
                    <td class="lbl">IBAN：</td>
                    <td>
                        <input class="easyui-textbox" id="IBAN" name="IBAN"
                            data-options="height:26,width:200,validType:'length[0,50]'," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PaymentType" name="PaymentType"
                            data-options="limitToList:true,required:true,valueField:'Key',textField:'Value',height:26,width:200" />
                    </td>
                    <td class="lbl">期望付汇日期：</td>
                    <td>
                        <input class="easyui-datebox" id="ExpectPayDate" name="ExpectPayDate"
                            data-options="height:26,width:200" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="text-align: center; height: 50%; margin: 5px;">
            <table id="datagrid">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true" style="width: 20px"></th>
                        <th data-options="field:'OrderID',width:200,align:'left'">订单编号</th>
                        <th data-options="field:'CreateDate',width: 150,align:'center'">申请时间</th>
                        <th data-options="field:'Currency',width: 130,align:'center'">币种</th>
                        <th data-options="field:'DeclarePrice',width: 150,align:'center'">报关总价</th>
                        <th data-options="field:'PaidPrice',width: 150,align:'center'">已付汇金额</th>
                        <th data-options="field:'OldAmount',width: 150,align:'center', hidden: true">申请金额</th>
                        <th data-options="field:'Amount',width: 150,align:'center',editor:{type:'textbox'}">本次申请金额</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div style="text-align: center; height: 10%; margin: 5px;">
            <div class="sub-container" style="height: 20px;">
                <a id="btnSplit" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cut'" onclick="Split()">拆分</a>
                <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            </div>
        </div>
    </div>
</body>
</html>
