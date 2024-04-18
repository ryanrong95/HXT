<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNew.aspx.cs" Inherits="WebApp.Order.Fee.AddNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/Customize/charge-standard-1.js?a=<%=this.Model.timestamp%>"></script>
    <script type="text/javascript">
        var chargeStd = null;
        $(function () {
            chargeStd = $("#charge-std").chargeStd({
                itemInputWidth: 80,
            });

            //下拉框数据初始化
            var currData = eval('(<%=this.Model.CurrData%>)');

            //默认数量为1
            $('#Count').textbox('setValue', 1);

            //初始化币种下拉框
            $('#Currency').combobox({
                data: currData,
                onSelect: function (record) {
                    $.post('?action=GetExchangeRate', { Currency: record.Key }, function (data) {
                        if (data != null) {
                            $("#Rate").textbox("setValue", data);
                        } else {
                            $("#Rate").textbox("setValue", null);
                        }
                    })
                }
            });

            $('#Count').textbox({
                onChange: function SetPrice() {
                    //debugger;
                    var price = $('#UnitPrice').textbox('getValue');
                    var Count = $('#Count').textbox('getValue');
                    $('#TotalPrice').textbox('setValue', price * Count);
                }
            });

            $('#UnitPrice').textbox({
                onChange: function SetPrice() {
                    //debugger;
                    var price = $('#UnitPrice').textbox('getValue');
                    var Count = $('#Count').textbox('getValue');
                    $('#TotalPrice').textbox('setValue', price * Count);
                }
            });


            //默认人民币
            $('#Currency').combobox('setValue', 'CNY');
            $('#Rate').textbox('setValue', 1);

            <%--//初始化费用类型下拉框
            $('#Type').combobox({
                onSelect: function (record) {
                    //如果是杂费，则需要填写费用名称
                    if (record.value == '<%=Needs.Ccs.Services.Enums.OrderPremiumType.OtherFee.GetHashCode()%>') {
                        $('#FeeNameTR').show();
                        $('#Name').textbox('textbox').validatebox('options').required = true;
                    } else {
                        $('#FeeNameTR').hide();
                        $('#Name').textbox('textbox').validatebox('options').required = false;
                    }
                    $('#form1').form('enableValidation').form('validate');
                }
            });
            $('#Type').combobox('setValue', null);--%>

            ////隐藏费用名称
            //$('#FeeNameTR').hide();

            //注册上传费用附件filebox的onChange事件
            $('#File').filebox({
                multiple: false,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择费用文件',
                buttonAlign: 'left',
                prompt: '请选择图片或PDF文件，且PDF文件大小不超过3M',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#File').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='File']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var typeArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png", "application/pdf"];

                    if (typeArr.indexOf(file.type) <= -1) {
                        $.messager.alert('提示', '请选择jpg、bmp、jpeg、gif、png、pdf格式的文件！');
                        $('#File').filebox('setValue', null);
                        return;
                    }

                    if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) {
                        $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                        $('#File').filebox('setValue', null);
                    }
                }
            });
        });

        //保存费用
        function Save() {
            var selectedStd = chargeStd.getSelectedStd();
            var chargeSum = chargeStd.getChargeSum();
            var chargeInputs = chargeStd.getChargeInputs();
            var stdCurrency = chargeStd.getStdCurrency();

            if (selectedStd == null || selectedStd == undefined) {
                $.messager.alert('提示', '请选择收费项目');
                return;
            }

            // radioChecked 为 true 的至少有一个
            var isRadioWrong = true;
            for (var i = 0; i < chargeInputs.length; i++) {
                if (chargeInputs[i].radioChecked) {
                    isRadioWrong = false;
                }
            }
            if (isRadioWrong) {
                $.messager.alert('提示', '至少选择一个 ◯');
                return;
            }

            // 检查可编辑的数量输入框, 数值要求大于 0
            var iswrong = false;
            for (var i = 0; i < chargeInputs.length; i++) {
                var chargeInput = chargeInputs[i];
                for (var j = 0; j < chargeInput.Values.length; j++) {
                    var Value = chargeInput.Values[j].Value;
                    var disabled = chargeInput.Values[j].disabled;
                    if (disabled == false && parseFloat(Value) <= 0) {
                        iswrong = true;
                        break;
                    }
                }
            }
            if (iswrong) {
                $.messager.alert('提示', '请输入应填的数量');
                return;
            }

            if (!Valid("form1")) {
                return;
            }

            var orderID = getQueryString('OrderID');
            var formData = new FormData($('#form1')[0]);
            formData.append('OrderID', orderID);

            // 添加要记录的收费标准信息
            formData.append('StandardID', selectedStd.ID);
            formData.append('StandardPrice', chargeSum);
            var standardRemarkModel = {
                SelectedStd: selectedStd,
                ChargeInputs: chargeInputs,
            };
            formData.append('StandardCurrency', stdCurrency);
            formData.append('StandardRemark', JSON.stringify(standardRemarkModel));

            //文件信息
            var file = $("input[name='File']").get(0).files[0];
            if (file == undefined || file == null || file == '') {
                ajaxSubmit(formData);
            } else {
                var fileType = file.type;
                var fileSize = file.size / 1024;
                var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                    photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                        var bl = convertBase64UrlToBlob(base64Codes);
                        formData.set('File', bl, fileName); // 文件对象
                        ajaxSubmit(formData);
                    });
                } else {
                    formData.set('File', file);
                    ajaxSubmit(formData);
                }
            }
        }

        function ajaxSubmit(formData) {
            $.ajax({
                url: '?action=SaveFee',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
    <style>
        #charge-std {
            padding: 5px;
            font-size: 10px;
            text-align: center;
        }

        .charge-std-menu .menu-text span {
            font-size: 8px;
        }

        .charge-std-menu .menu-text {
            font-size: 8px;
        }

        .charge-std-calc-item {
            display: flex;
            margin: 2px 0;
            justify-content: center;
        }

            .charge-std-calc-item label {
                margin: 0 8px 0 2px;
            }
    </style>
</head>
<body class="easyui-layout">
    <div id="content" style="overflow-y: auto; height: 460px;">
        <div>
            <div id="charge-std">
                <div>
                    <label style="margin-right: 5px;">收费项目：</label>
                    <input class="charge-std-input" type="text" data-options="readonly: true,required:true," style="width: 238px;" />
                    <a class="charge-std-btn" data-options="iconCls: 'icon-edit'," style="margin-right: 10px;">选择</a>
                </div>
                <div style="margin-top: 5px;">
                    <label style="margin-right: 18px;">说明：</label>
                    <input class="charge-std-remark" data-options="multiline:true,readonly: true,"
                        style="width: 300px; height: 80px;" />
                </div>
                <div class="charge-std-calc" style="margin-top: 5px;"></div>
                <div>
                    <label style="color: red;">参考总价：</label>
                    <span class="charge-std-result"></span>
                </div>
            </div>
        </div>
        <form id="form1" runat="server" method="post">
            <div style="text-align: center;">
                <table id="editTable" style="margin: 0 auto">
                    <%--<tr>
                        <td>费用类型：</td>
                        <td>
                            <select class="easyui-combobox" id="Type" name="Type" data-options="required:true,width:300,editable:false">
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.DeliveryFee.GetHashCode()%>">送货费</option>
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.ExpressFee.GetHashCode()%>">快递费</option>
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.CustomClearanceFee.GetHashCode()%>">清关费</option>
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.PickUpFee.GetHashCode()%>">提货费</option>
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.ParkingFee.GetHashCode()%>">停车费</option>
                                <option value="<%=Needs.Ccs.Services.Enums.OrderPremiumType.OtherFee.GetHashCode()%>">其他</option>
                            </select>
                        </td>
                    </tr>--%>
                    <tr id="FeeNameTR">

                        <td>备注：</td>
                        <td>
                            <input class="easyui-textbox" id="Name" name="Name" data-options="width:300,validType:'length[1,50]'," />
                        </td>
                    </tr>
                    <tr>
                        <td>数量：</td>
                        <td>
                            <input class="easyui-numberbox" id="Count" name="Count" data-options="min:1,precision:'0',required:true,width:300" />
                        </td>
                    </tr>
                    <tr>
                        <td>单价：</td>
                        <td>
                            <input class="easyui-numberbox" id="UnitPrice" name="UnitPrice" data-options="min:0,precision:'4',required:true,width:300,validType:'length[1,18]'" />
                        </td>
                    </tr>
                    <tr>
                        <td>总价：</td>
                        <td>
                            <input class="easyui-numberbox" id="TotalPrice" name="TotalPrice" data-options="min:0,precision:'4',required:true,width:300" readonly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>币种：</td>
                        <td>
                            <input class="easyui-combobox" id="Currency" name="Currency" data-options="required:true,width:300,valueField:'Key',textField:'Value',limitToList:true,validType:'comboBoxEditValid[\'Currency\']',panelHeight:'100px'" />
                        </td>
                    </tr>
                    <tr>
                        <td>汇率：</td>
                        <td>
                            <input class="easyui-numberbox" id="Rate" name="Rate" data-options="min:0,precision:'4',required:true,width:300" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input class="easyui-filebox" id="File" name="File" data-options="width:370" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnClose" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
