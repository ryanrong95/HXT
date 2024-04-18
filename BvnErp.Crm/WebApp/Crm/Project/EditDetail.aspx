<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDetail.aspx.cs" Inherits="WebApp.Crm.Project.EditDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var SaleAdmins = eval('(<%=this.Model.SaleAdmins%>)');
        var Admins = eval('(<%=this.Model.Admins%>)');
        var Status = eval('(<%=this.Model.Status%>)');
        var IsReport = eval('(<%=this.Model.IsReport%>)');
        var Currency = eval('(<%=this.Model.Currency%>)');
        var SampleType = eval('(<%=this.Model.SampleType%>)');
        var Vender = eval('(<%=this.Model.Vender%>)');
        var project = eval(<%=this.Model.Project%>);
        var productitem = eval(<%=this.Model.ProductItem%>);
        var IsSample = eval(('<%=this.Model.IsSample%>').toLowerCase());
        var Files = eval('(<%=this.Model.Files%>)');
        var Enquirymust = ["EnquiryReplyDate", "EnquiryReplyPrice", "EnquiryOriginModel", "EnquiryMOQ", "EnquiryCurrency", "EnquiryValidity"];

        $(function () {
            //产品项状态变更校验
            $("#Status").combobox({
                onChange: function (newValue, oldValue) {
                    if (productitem != null) {
                        var value = Number(newValue);
                        if (value != 0 && (value < productitem.Status || value - productitem.Status > 40)) {
                            alert("1、销售状态变更顺序为：DO->DI-> DW-> MP，不能跨状态变更，不能逆向变更。\n2、其他状态可以直接变为DL");
                            $("#Status").combobox("setValue", productitem.Status);
                        }
                        else {
                            $("#ExpectRate").numberbox("setValue".value);
                        }
                    }
                }
            });

            $("#SaleAdminID").combobox('setValue', SaleAdmins[0].ID);
            $("#AssistantAdiminID").combobox('setValue', SaleAdmins[0].ID);

            //解决特殊字符的问题
            $("#ManufactureID").combobox({
                onChange: function () {
                    var text = escape2Html($(this).combobox('getText'));
                    $(this).combobox('setText', text);
                },
            });

            if (IsSample) {
                $('#SampleOn').prop("checked", true);
                $('#SampleOff').prop("checked", false);

                $('#SampleType').combobox({ required: true });
                $('#SampleQuantity').numberbox({ required: true });
                $('#SamplePrice').numberbox({ required: true });
                $('#SampleDate').datebox({ required: true });
                $('#SampleContactor').textbox({ required: true });
                $('#SamplePhone').textbox({ required: true });
            }
            else {
                $('#SampleOn').prop("checked", false);
                $('#SampleOff').prop("checked", true);

                $('#SampleType').combobox({ required: false });
                $('#SampleQuantity').numberbox({ required: false });
                $('#SamplePrice').numberbox({ required: false });
                $('#SampleDate').datebox({ required: false });
                $('#SampleContactor').textbox({ required: false });
                $('#SamplePhone').textbox({ required: false });
            }

            $('input[name="IsSample"]').change(function () {
                var checkedvalue = $('input[name="IsSample"]:checked').val();

                if (checkedvalue == 'true') {
                    $('#SampleType').combobox({ required: true, editable: false, readonly: false });
                    $('#SampleQuantity').numberbox({ required: true, readonly: false });
                    $('#SamplePrice').numberbox({ required: true, readonly: false });
                    $('#SampleDate').datebox({ required: true, editable: false, readonly: false });
                    $('#SampleContactor').textbox({ required: true, readonly: false });
                    $('#SamplePhone').textbox({ required: true, readonly: false });
                    $("#SampleAddress").textbox({ required: false, readonly: false })
                }
                else {
                    $('#SampleType').combobox({ required: false, readonly: true }).combobox('clear');
                    $('#SampleQuantity').numberbox({ required: false, readonly: true }).numberbox('clear');
                    $('#SamplePrice').numberbox({ required: false, readonly: true }).numberbox('clear');
                    $('#SampleDate').datebox({ required: false, readonly: true }).datebox('clear');
                    $('#SampleContactor').textbox({ required: false, readonly: true }).textbox('clear');
                    $('#SamplePhone').textbox({ required: false, readonly: true }).textbox('clear');
                    $("#SampleAddress").textbox({ required: false, readonly: true }).textbox('clear');
                }
            })

            //客户信息初始赋值
            $("#ClientName").textbox("setValue", project["ClientName"]);
            $("#CompanyName").textbox("setValue", project["CompanyName"]);
            $("#CurrencyName").textbox("setValue", project["CurrencyName"]);
            $("#Name").textbox("setValue", project["Name"]);
            $("#ProductName").textbox("setValue", project["ProductName"]);
            $("#IndustryName").textbox("setValue", project["IndustryName"]);

            if (productitem != null) {
                $("#Summary").textbox("setValue", productitem.Summary);
                $("#ItemName").textbox("setValue", productitem.ItemName);
                $("#ItemOrigin").textbox("setValue", productitem.ItemOrigin);
                $("#ManufactureID").combobox("setValue", productitem.ManufactureID);
                $("#RefUnitQuantity").numberbox("setValue", productitem.RefUnitQuantity);
                $("#RefQuantity").numberbox("setValue", productitem.RefQuantity);
                $("#RefUnitPrice").numberbox("setValue", productitem.RefUnitPrice);
                $("#Status").combobox("setValue", productitem.Status);
                $("#ExpectRate").numberbox("setValue", productitem.ExpectRate);
                $("#ExpectDate").datebox("setValue", productitem.ExpectDate);
                $("#ExpectQuantity").numberbox("setValue", productitem.ExpectQuantity);
                $("#CompeteModel").textbox("setValue", productitem.CompeteModel);
                $("#CompeteManu").textbox("setValue", productitem.CompeteManu);
                $("#CompetePrice").numberbox("setValue", productitem.CompetePrice);
                $("#ExpectTotal").numberbox("setValue", productitem.ExpectTotal);
                $("#SaleAdminID").combobox("setValue", productitem.SaleAdminID);
                $("#AssistantAdiminID").combobox("setValue", productitem.AssistantAdiminID);
                $("#PMAdminID").combobox("setValue", productitem.PMAdminID);
                $("#PurchaseAdminID").combobox("setValue", productitem.PurchaseAdminID);
                $("#FAEAdminID").combobox("setValue", productitem.FAEAdminID);
                $("#IsReport").combobox("setValue", productitem.IsReport);
                $("#ReportDate").datebox("setValue", productitem.ReportDate);
                $("#SampleDate").datebox("setValue", productitem.SampleDate);
                $("#SampleType").combobox("setValue", productitem.SampleType);
                $("#SampleQuantity").numberbox("setValue", productitem.SampleQuantity);
                $("#SamplePrice").numberbox("setValue", productitem.SamplePrice);
                $("#SampleContactor").textbox("setValue", productitem.SampleContactor);
                $("#SamplePhone").textbox("setValue", productitem.SamplePhone);
                $("#SampleAddress").textbox("setValue", productitem.SampleAddress);

            } else {
                $("#Status").combobox({ readonly: true });
                $("#Status").combobox("setValue", "10");
                $("#ExpectRate").numberbox("setValue", "10");
            }
            // 参考单价输入框
            $('#RefUnitPrice').numberbox({
                onChange: function (newValue, oldValue) {
                    var num = $("#ExpectQuantity").numberbox("getValue");
                    $("#ExpectTotal").numberbox("setValue", newValue * 1000 * num);
                }
            });
            // 预计成交量输入框
            $('#ExpectQuantity').numberbox({
                onChange: function (newValue, oldValue) {
                    var price = $("#RefUnitPrice").numberbox("getValue");
                    $("#ExpectTotal").numberbox("setValue", newValue * 1000 * price);
                }
            });
            //文件凭证赋值
            if (Files != null) {
                $.map(Files, function (file) {
                    if (file["Type"] == "100") {
                        $("#tdfile").show();
                        document.getElementById('File').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                    if (file["Type"] == "200") {
                        $("#tdreport").show();
                        document.getElementById('ReportFile').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                    if (file["Type"] == "300") {
                        $("#tdorigin").show();
                        document.getElementById('OriginFile').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                });
            }
            else {
                $("#tdreport").hide(); $("#tdfile").hide(); $("#tdorigin").hide();
            }

            //校验输入框内容
            $("#ManufactureID").combobox("textbox").bind("blur", function () {
                var value = $("#ManufactureID").combobox("getValue");
                var data = $("#ManufactureID").combobox("getData");
                var valuefiled = $("#ManufactureID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ManufactureID").combobox("clear");
                }
            });
            $("#SaleAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#SaleAdminID").combobox("getValue");
                var data = $("#SaleAdminID").combobox("getData");
                var valuefiled = $("#SaleAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#SaleAdminID").combobox("clear");
                }
            });
            $("#AssistantAdiminID").combobox("textbox").bind("blur", function () {
                var value = $("#AssistantAdiminID").combobox("getValue");
                var data = $("#AssistantAdiminID").combobox("getData");
                var valuefiled = $("#AssistantAdiminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#AssistantAdiminID").combobox("clear");
                }
            });
            $("#PurchaseAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#PurchaseAdminID").combobox("getValue");
                var data = $("#PurchaseAdminID").combobox("getData");
                var valuefiled = $("#PurchaseAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#PurchaseAdminID").combobox("clear");
                }
            });
            $("#PMAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#PMAdminID").combobox("getValue");
                var data = $("#PMAdminID").combobox("getData");
                var valuefiled = $("#PMAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#PMAdminID").combobox("clear");
                }
            });
            $("#FAEAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#FAEAdminID").combobox("getValue");
                var data = $("#FAEAdminID").combobox("getData");
                var valuefiled = $("#FAEAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#FAEAdminID").combobox("clear");
                }
            });
        });

        //关闭
        function Close() {
            $.myWindow.close();
            return false;
        };

        //保存校验
        function Save() {

            //校验必填项
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }

            //文件大小检验
            if ($("#FileUpload")[0].files.length > 0 && $("#FileUpload")[0].files[0].size > 4096 * 1024) {
                alert("上传的文件凭证超过4M!请重新选择文件上传!");
                return false;
            }

            var text = ""; var name = "";
            var nodes = $("input[id^='Sample']");
            //nodes.each(function () {
            //    var nodetype = this.classList[0];
            //    var plugin = "";
            //    //获取当前控件类型
            //    plugins.forEach(function (value) {
            //        if ("easyui-" + value == nodetype) {
            //            plugin = value;
            //            return false;
            //        }
            //    });
            //    text = text + $(this)[plugin]("getValue");
            //    if ($(this)[plugin]("getValue") == "" && name == "") {
            //        name = this.getAttribute("textboxname");
            //    }
            //    if (text != "" && name != "") {
            //        return false;
            //    }
            //});

            //if (text != "" && name != "") {
            //    $.messager.alert('提示', name + '栏位应该输入！');
            //    return false;
            //}
            //text = ""; name = "";
            //nodes = $("input[id^='Enquiry']");
            //nodes.each(function () {
            //    var nodetype = this.classList[0];
            //    var plugin = "";
            //    //获取当前控件类型
            //    plugins.forEach(function (value) {
            //        if ("easyui-" + value == nodetype) {
            //            plugin = value;
            //            return false;
            //        }
            //    });
            //    console.log(plugin);
            //    text = text + $(this)[plugin]("getValue");
            //    if ($.easyui.indexOfArray(Enquirymust, this.id) >= 0 && $(this)[plugin]("getValue") == "" && name == "") {
            //        name = this.getAttribute("textboxname");
            //    };
            //    if (text != "" && name != "") {
            //        return false;
            //    }
            //});
            //if (text != "" && name != "") {
            //    $.messager.alert('提示', name + '栏位应该输入！');
            //    return false;
            //}                        
            return true;
        }
    </script>
</head>
<body class="e">
    <form id="form1" runat="server" method="post">
        <input type="hidden" runat="server" id="hidID" />
        <table id="table1" style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td style="height: 30px; font-size: 20px;">项目信息</td>
            </tr>
            <tr>
                <td class="lbl">客户</td>
                <td>
                    <input class="easyui-textbox" id="ClientName" name="ClientName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">合作公司</td>
                <td>
                    <input class="easyui-textbox" id="CompanyName" name="CompanyName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-textbox" id="CurrencyName" name="CurrencyName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">项目名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">产品名称</td>
                <td>
                    <input class="easyui-textbox" id="ProductName" name="ProductName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">应用行业</td>
                <td>
                    <input class="easyui-textbox" id="IndustryName" name="IndustryName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td style="height: 30px"></td>
            </tr>
            <tr>
                <td style="height: 30px; font-size: 20px;">用料信息</td>
            </tr>
            <tr>
                <td class="lbl">产品型号</td>
                <td>
                    <input class="easyui-textbox" id="ItemName" name="ItemName" data-options="validType:'length[1,50]',required:true," style="width: 95%" />
                </td>
                <td class="lbl">型号全称</td>
                <td>
                    <input class="easyui-textbox" id="ItemOrigin" name="ItemOrigin" data-options="validType:'length[1,150]'," style="width: 95%" />
                </td>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-combobox" id="ManufactureID" name="ManufactureID"
                        data-options="valueField:'ID',textField:'Name',data: Vender,required:true," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">单机用量</td>
                <td>
                    <input class="easyui-numberbox" id="RefUnitQuantity" name="RefUnitQuantity"
                        data-options="min:0,required:true,validType:'length[1,10]'," style="width: 95%" />
                </td>
                <td class="lbl">项目用量(K)</td>
                <td>
                    <input class="easyui-numberbox" id="RefQuantity" name="RefQuantity"
                        data-options="min:0,required:true,validType:'length[1,10]'," style="width: 95%" />
                </td>
                <td class="lbl">参考单价(CNY)</td>
                <td>
                    <input class="easyui-numberbox" id="RefUnitPrice" name="RefUnitPrice"
                        data-options="min:0,precision:5,required:true,validType:'length[1,15]'," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" name="Status"
                        data-options="valueField:'value',textField:'text',data:Status,required:true,editable:false," style="width: 95%" />
                </td>
                <td class="lbl">预计成交概率(%)</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectRate" name="ExpectRate" data-options="editable:false" style="width: 95%" />
                </td>
                <td class="lbl">预计成交量(K)</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectQuantity" name="ExpectQuantity"
                        data-options="validType:'length[1,10]'," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">预计成交日期</td>
                <td>
                    <input class="easyui-datebox" id="ExpectDate" name="ExpectDate" style="width: 95%" data-options="required:true" />
                </td>
                <td class="lbl">预计成交额</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectTotal" name="ExpectTotal" style="width: 95%" />
                </td>
                <td></td>
                <td>预计成交额计算公式=[预计成交量]*[参考单价]</td>
            </tr>
            <tr>
                <td class="lbl">竞品型号</td>
                <td>
                    <input class="easyui-textbox" id="CompeteModel" name="CompeteModel" data-options="validType:'length[1,50]'" style="width: 95%" />
                </td>
                <td class="lbl">竞品单价</td>
                <td>
                    <input class="easyui-numberbox" id="CompetePrice" name="CompetePrice"
                        data-options="min:0,precision:5,validType:'length[1,15]'" style="width: 95%" />
                </td>
                <td class="lbl">竞品厂商</td>
                <td>
                    <input class="easyui-textbox" id="CompeteManu" name="CompeteManu" data-options="validType:'length[1,50]'" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">凭证</td>
                <td colspan="3">
                    <asp:FileUpload ID="FileUpload" runat="server" />
                </td>
                <td class="lbl" id="tdfile">已上传凭证</td>
                <td>
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="File"></label>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 30px; font-size: 20px;">型号备注信息</td>
            </tr>
            <tr>
                <td class="lbl">型号备注</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,200]'" style="width: 99%" />
                </td>
            </tr>
            <tr>
                <td style="height: 30px; font-size: 20px;">人员信息</td>
            </tr>
            <tr>
                <td class="lbl">销售</td>
                <td>
                    <input class="easyui-combobox" id="SaleAdminID" name="SaleAdminID"
                        data-options="valueField:'ID',textField:'RealName',data:SaleAdmins,required:true," style="width: 95%" />
                </td>
                <td class="lbl">销售助理</td>
                <td>
                    <input class="easyui-combobox" id="AssistantAdiminID" name="AssistantAdiminID"
                        data-options="valueField:'ID',textField:'RealName',data:SaleAdmins," style="width: 95%" />
                </td>
                <td class="lbl">采购助理</td>
                <td>
                    <input class="easyui-combobox" id="PurchaseAdminID" name="PurchaseAdminID"
                        data-options="valueField:'ID',textField:'RealName',data:Admins," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">PM</td>
                <td>
                    <input class="easyui-combobox" id="PMAdminID" name="PMAdminID"
                        data-options="valueField:'ID',textField:'RealName',data:Admins,required:true," style="width: 95%" />
                </td>
                <td class="lbl">FAE</td>
                <td>
                    <input class="easyui-combobox" id="FAEAdminID" name="FAEAdminID"
                        data-options="valueField:'ID',textField:'RealName',data:Admins,required:true," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td style="height: 30px; font-size: 20px;">送样信息</td>
            </tr>
            <tr>
                <td class="lbl">是否送样</td>
                <td>
                    <input type="radio" id="SampleOn" name="IsSample" value="true" />是
                    <input type="radio" id="SampleOff" name="IsSample" value="false" />否
                </td>
            </tr>
            <tr>
                <td class="lbl">送样类型</td>
                <td>
                    <input class="easyui-combobox" id="SampleType" name="送样类型"
                        data-options="valueField:'value',textField:'text',data:SampleType," style="width: 95%" />
                </td>
                <td class="lbl">送样数量</td>
                <td>
                    <input class="easyui-numberbox" id="SampleQuantity" name="送样数量"
                        data-options="min:0,validType:'length[1,10]'," style="width: 95%" />
                </td>
                <td class="lbl">送样单价</td>
                <td>
                    <input class="easyui-numberbox" id="SamplePrice" name="送样单价"
                        data-options="min:0,precision:5,validType:'length[1,15]'," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样时间</td>
                <td>
                    <input class="easyui-datebox" id="SampleDate" name="送样时间" data-options="" style="width: 95%" />
                </td>
                <td class="lbl">送样联系人</td>
                <td>
                    <input class="easyui-textbox" id="SampleContactor" name="送样联系人" data-options="validType:'length[1,50]'" style="width: 95%" />
                </td>
                <td class="lbl">送样联系电话</td>
                <td>
                    <input class="easyui-textbox" id="SamplePhone" name="送样联系电话"
                        data-options="" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样联系地址</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="SampleAddress" name="送样联系地址" data-options="validType:'length[1,200]'" style="width: 99%" />
                </td>
            </tr>

        </table>
        <div id="divSave" style="text-align: center; margin-top: 20px; margin-bottom: 10px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Save();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="return Close();" />
        </div>
    </form>
</body>
</html>
