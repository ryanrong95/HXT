<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.MyClients.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/CustomAreaCombo.js"></script>
    <script type="text/javascript">
        var customerType = eval(<%=this.Model.CustomerTypeData%>);
        var customerNature = eval(<%=this.Model.CustomerNatureData%>);
        var customerArea = eval(<%=this.Model.CustomerAreaData%>);
        var businessType = eval(<%=this.Model.BusinessTypeData%>);
        var customerLevel = eval(<%=this.Model.CustomerLevelData%>);
        var customerCompanyData = eval(<%=this.Model.CustomerCompanyData%>);
        var currencyData = eval(<%=this.Model.CurrencyData%>);
        var Manufacture = eval(<%=this.Model.Manufacture%>);
        var ProtectLevel = eval(<%=this.Model.ProtectLevel%>);
        var customerStatus = eval(<%=this.Model.CustomerStatus%>);
        var customerData = eval(<%=this.Model.CustomerData%>);
        var DrpCategory = eval(<%=this.Model.DrpCategory %>);
        var AdminTop = eval(<%=this.Model.AdminTop %>);
        var ReIndustry = eval(<%=this.Model.ReIndustry %>);
        var AreaData = eval(<%=this.Model.AreaData %>);
        var files = eval('(<%=this.Model.Files%>)');
        var importantlevel = eval('(<%=this.Model.ImportantLevelData%>)');
        var ClientStatus = eval(<%=this.Model.ClientStaus%>);

        $(function () {
            //只能输入字符和数字
            $("#AdminCode").textbox("textbox").bind("blur", function () {
                var value = $("#AdminCode").textbox("getValue");
                $("#AdminCode").textbox("setValue", value.replace(/[\W]/g, ''));
            });

            //解决特殊字符的问题
            $("#AgentBrand").combobox({
                onChange: function (newValue, oldValue) {
                    var text = escape2Html($(this).combobox('getText'));
                    $(this).combobox('setText', text);
                },
            });

            //币值默认为人民币
            $('#Currency').combobox('setValue', 142);

            //校验客户名称是否重复
            $("#Name").textbox("textbox").bind("blur", function () {
                var name = $("#Name").val().replace(/\s+/g, '');
                if (name == "") {
                    return;
                }
                var url = "http://vpn3.t996.top:8800/users/views/page/";
                url = url + "?page=1&Code=23788432ea2b4c93ba27f8538e410719&Queries=" + name + "&Query_field=FullName&Whether_the_fuzzy=false&format=json";
                $.post('?action=ValidName', { Name: name, Url: url }, function (data) {
                    if (data == "true") {
                        $.messager.alert('温馨提示', "客户已被注册，如需使用，请联系部门经理协调。");
                        $("#Name").textbox("setValue", "");
                    }
                    else if (data == "false") {
                        $.messager.alert('温馨提示', "此客户已经在大赢家中被他人保护；你可以继续提交申请等待审批!");
                    }
                })
            });

            $("#ReIndustry").combobox({
                onChange: function (newValue, oldValue) {
                    IndustryFlier(newValue);
                }
            });

            //总PM录入项
            if (AdminTop.JobType != 800) {
                $('#CustomerLevel').combobox("readonly", true);
                $('#ProtectLevel').combobox("readonly", true);
                $('#ProtectionScope').textbox("readonly", true);
                $('#CreditLimit').textbox("readonly", true);
                $('#CreditPayment').textbox("readonly", true);
            }
            else {
                $('#CustomerLevel').combobox({ required: true });
                $('#ProtectLevel').combobox({ required: true });
            }

            //初始化文件
            if (files.length > 0) {
                document.getElementById('fileName').innerHTML += "<span onclick='show(\"" + files[0].Url +
                    "\")' style='color:Blue'>文件名: " + files[0].Name + "</span></br>";
            }

            //初始化赋值
            if (customerData != null && customerData != "") {
                $("#Name").textbox("setValue", customerData["Name"]);
                $("#AdminCode").textbox("setValue", customerData["AdminCode"]);
                $("#EnterpriseProperty").combobox("setValue", customerData["EnterpriseProperty"]);
                $("#Area").combobox("setValue", customerData["Area"]);
                $("#RegisteredCapital").textbox("setValue", customerData["RegisteredCapital"]);
                $("#Currency").combobox("setValue", customerData["Currency"]);
                $("#EstablishmentDate").datebox("setValue", customerData["EstablishmentDate"]);
                $("#OperatingPeriod").datebox("setValue", customerData["OperatingPeriod"]);
                $("#RegisteredAddress").textbox("setValue", customerData["RegisteredAddress"]);
                $("#OfficeAddress").textbox("setValue", customerData["OfficeAddress"]);
                $("#Site").textbox("setValue", customerData["Site"]);
                $("#BusinessScope").textbox("setValue", customerData["BusinessScope"]);
                $("#CustomerType").combobox("setValue", customerData["CustomerType"]);
                $("#CUSCC").textbox("setValue", customerData["CUSCC"]);
                $("#CustomerLevel").combobox("setValue", customerData["CustomerLevel"]);
                $("#Possessor").textbox("setValue", customerData["Possessor"]);
                $("#CompanyID").combobox("setValue", customerData["CompanyID"]);
                $("#BusinessType").combobox("setValue", customerData["BusinessType"]);
                $("#ProtectLevel").combobox("setValue", customerData["ProtectLevel"]);
                $("#ProtectionScope").textbox("setValue", customerData["ProtectionScope"]);
                $("#CreditLimit").textbox("setValue", customerData["CreditLimit"]);
                $("#CreditPayment").textbox("setValue", customerData["CreditPayment"]);
                $("#CustomerStatus").combobox("setValue", customerData["CustomerStatus"]);
                $("#ExtraPacking").textbox("setValue", customerData["ExtraPacking"]);
                $("#SpecialSupplier").textbox("setValue", customerData["SpecialSupplier"]);
                $("#InformationSource").textbox("setValue", customerData["InformationSource"]);
                $("#Summary").textbox("setValue", customerData["Summary"]);
                $("#ImportantLevel").combobox("setValue", customerData["ImportantLevel"]);
                if (customerData["AgentBrand"] != "" && customerData["AgentBrand"] != undefined) {
                    $("#AgentBrand").combobox("setValues", customerData["AgentBrand"]);
                }
                if (customerData["ReIndustry"] != "" && customerData["ReIndustry"] != undefined) {
                    $("#ReIndustry").combobox("setValues", customerData["ReIndustry"]);
                    IndustryFlier(customerData["ReIndustry"])
                }
                if (customerData["IndustryInvolved"] != "" && customerData["IndustryInvolved"] != undefined) {
                    $("#IndustryInvolved").combotree('setValues', customerData["IndustryInvolved"].split(","));
                }
                if (customerData["AreaID"] != "" && customerData["AreaID"] != undefined && customerData["AreaID"].length > 0) {
                    $("#AreaID").customarea('setValue', customerData["AreaID"]);
                }
                //审批通过后，非总PM角色不可修改
                if (AdminTop.JobType != 800 && ClientStatus == 500) {
                    $('#Name').textbox("readonly", true);
                    $('#CUSCC').textbox("readonly", true);
                    $('#CUSCC').textbox({ required: false });
                    $('#fileImport').filebox("disable");
                }
                else if (document.getElementById("fileName").innerText == "") {
                    $('#fileImport').filebox({ required: true });
                }
                else {
                    $('#fileImport').filebox({ required: false });
                }
            } else {
                $('#fileImport').filebox({ required: true });
                if (AdminTop.CompanyID != "") {
                    $("#CompanyID").combobox("setValue", AdminTop.CompanyID);
                }
            }

           

            //校验输入框内容
            $("#EnterpriseProperty").combobox("textbox").bind("blur", function () {
                var value = $("#EnterpriseProperty").combobox("getValue");
                var data = $("#EnterpriseProperty").combobox("getData");
                var valuefiled = $("#EnterpriseProperty").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#EnterpriseProperty").combobox("clear");
                }
            });
            $("#Area").combobox("textbox").bind("blur", function () {
                var value = $("#Area").combobox("getValue");
                var data = $("#Area").combobox("getData");
                var valuefiled = $("#Area").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Area").combobox("clear");
                }
            });
            $("#Currency").combobox("textbox").bind("blur", function () {
                var value = $("#Currency").combobox("getValue");
                var data = $("#Currency").combobox("getData");
                var valuefiled = $("#Currency").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Currency").combobox("clear");
                }
            });
            $("#CustomerType").combobox("textbox").bind("blur", function () {
                var value = $("#CustomerType").combobox("getValue");
                var data = $("#CustomerType").combobox("getData");
                var valuefiled = $("#CustomerType").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CustomerType").combobox("clear");
                }
            });
            $("#CustomerLevel").combobox("textbox").bind("blur", function () {
                var value = $("#CustomerLevel").combobox("getValue");
                var data = $("#CustomerLevel").combobox("getData");
                var valuefiled = $("#CustomerLevel").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CustomerLevel").combobox("clear");
                }
            });
            $("#AgentBrand").combobox("textbox").bind("blur", function () {
                var values = [];
                $.map($("#AgentBrand").combobox("getValues"), function (value) {
                    var data = $("#AgentBrand").combobox("getData");
                    var valuefiled = $("#AgentBrand").combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index >= 0) {
                        values.push(value);
                    }
                });
                $("#AgentBrand").combobox("setValues", values);
            });
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
            $("#BusinessType").combobox("textbox").bind("blur", function () {
                var value = $("#BusinessType").combobox("getValue");
                var data = $("#BusinessType").combobox("getData");
                var valuefiled = $("#BusinessType").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#BusinessType").combobox("clear");
                }
            });
            $("#ReIndustry").combobox("textbox").bind("blur", function () {
                var values = [];
                $.map($("#ReIndustry").combobox("getValues"), function (value) {
                    var data = $("#ReIndustry").combobox("getData");
                    var valuefiled = $("#ReIndustry").combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index >= 0) {
                        values.push(value);
                    }
                });
                $("#ReIndustry").combobox("setValues", values);
            });
            $("#ProtectLevel").combobox("textbox").bind("blur", function () {
                var value = $("#ProtectLevel").combobox("getValue");
                var data = $("#ProtectLevel").combobox("getData");
                var valuefiled = $("#ProtectLevel").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ProtectLevel").combobox("clear");
                }
            });
            $("#ImportantLevel").combobox("textbox").bind("blur", function () {
                var value = $("#ImportantLevel").combobox("getValue");
                var data = $("#ImportantLevel").combobox("getData");
                var valuefiled = $("#ImportantLevel").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ImportantLevel").combobox("clear");
                }
            });
            $("#CustomerStatus").combobox("textbox").bind("blur", function () {
                var value = $("#CustomerStatus").combobox("getValue");
                var data = $("#CustomerStatus").combobox("getData");
                var valuefiled = $("#CustomerStatus").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CustomerStatus").combobox("clear");
                }
            });
        });


        //根据行业过滤产品
        function IndustryFlier(values) {
            var drpdata = new Array; var i = 0;
            for (var index = 0; index < DrpCategory.length; index++) {
                var data = DrpCategory[index];
                if (values.indexOf(data.id) >= 0) {
                    drpdata[i] = data;
                    i++;
                }
            }
            $("#IndustryInvolved").combotree({ data: drpdata });
            $("#IndustryInvolved").combotree('tree').tree({
                onlyLeafCheck: true,
            });
        }


        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            //营业执照
            var filebox = document.getElementById("filebox_file_id_1");
            if (filebox == null) {
                filebox = document.getElementById("filebox_file_id_2");
            }

            if (filebox.files) {
                var files = filebox.files;
                if (files.length > 0) {
                    if (files[0].size > 4096 * 1024) {
                        alert("上传的文件不得大于4M!");
                        return false;
                    }
                    if (!CheckType(files[0].name)) {
                        alert("请上传符合格式要求的文件！");
                        return false;
                    }
                }
            }

            $('#form1').form('submit', {
                url: window.location.pathname + '?action=Save&id=' + getQueryString("id"),
                success: function (text) {
                    $.myWindow.close();
                }
            });
            return false;
        }


        function closeWin() {
            $.myWindow.close();
        }
    </script>
    <script type="text/javascript">
        //校验文件格式
        function CheckType(name) {
            debugger;
            var isCheck = false;
            var type = [".png", ".jpg", ".pdf",];
            for (var i = 0; i < type.length; i++) {
                if (name.toLowerCase().indexOf(type[i]) >= 0) {
                    isCheck = true;
                    break;
                }
            }
            return isCheck;
        }

        //预览
        function show(url) {
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '预览',
                url: url,
            }).open();
        }
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" method="post" enctype="multipart/form-data">
            <table id="table1" style="width: 780px">
                <tr>
                    <td class="subTiltle">基本信息</td>
                    <td>一经审批确认，不能轻易更改，更改需审批</td>
                </tr>
                <tr>
                    <td class="lbl">客户名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" style="width: 95%"
                            data-options="validType:'length[1,50]',required:true,tipPosition:'bottom'" />
                    </td>
                    <td class="lbl">企业性质</td>
                    <td>
                        <input class="easyui-combobox" id="EnterpriseProperty" name="EnterpriseProperty"
                            data-options="valueField:'value',textField:'text',data: customerNature,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">国别</td>
                    <td>
                        <input class="easyui-combobox" id="Area" name="Area"
                            data-options="valueField:'value',textField:'text',data: customerArea,required:true," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td>注册资本</td>
                    <td>
                        <input class="easyui-numberbox" id="RegisteredCapital" name="RegisteredCapital" style="width: 50%"
                            data-options="validType:'length[1,50]',tipPosition:'bottom',groupSeparator:','" />

                        元
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'value',textField:'text',data: currencyData," style="width: 38%" />
                    </td>
                    <td>成立日期</td>
                    <td>
                        <input class="easyui-datebox" id="EstablishmentDate" name="EstablishmentDate" style="width: 95%" data-options="editable:false" />
                    </td>
                    <td>经营期限</td>
                    <td>
                        <input class="easyui-datebox" id="OperatingPeriod" name="OperatingPeriod" style="width: 95%" data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td>注册地址</td>
                    <td>
                        <input class="easyui-textbox" id="RegisteredAddress" name="RegisteredAddress" style="width: 95%"
                            data-options="validType:'length[1,50]',tipPosition:'bottom'" />
                    </td>
                    <td>办公地址</td>
                    <td>
                        <input class="easyui-textbox" id="OfficeAddress" name="OfficeAddress" style="width: 95%"
                            data-options="validType:'length[1,50]'" />
                    </td>
                    <td>网址</td>
                    <td>
                        <input class="easyui-textbox" id="Site" name="Site" style="width: 95%"
                            data-options="validType:'length[1,100]'" />
                    </td>
                </tr>
                <tr>
                    <td>经营范围</td>
                    <td>
                        <input class="easyui-textbox" id="BusinessScope" name="BusinessScope" style="width: 95%"
                            data-options="validType:'length[1,100]',tipPosition:'bottom'" />
                    </td>
                    <td>社会统一信用代码</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="CUSCC" name="CUSCC" style="width: 98%"
                            data-options="validType:'length[1,18]',tipPosition:'bottom',required:true," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">营业执照上传</td>
                    <td colspan="3">
                        <input class="easyui-filebox" id="fileImport" name="fileImport" style="width: 95%"
                            data-options="buttonText:'选择文件',accept:'.png,.jpg,.pdf,'," />
                    </td>
                    <td colspan="2">
                        <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                            <label id="fileName"></label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <span style="color: red; width: 200px">请上传jpg,png,pdf格式的文件，并且文件大小不大于4M</span>
                    </td>
                </tr>
                <tr style="height: 10px;" />
                <tr>
                    <td class="subTiltle">管理信息</td>
                </tr>
                <tr>
                    <td class="lbl">客户类型</td>
                    <td>
                        <input class="easyui-combobox" id="CustomerType" name="CustomerType"
                            data-options="valueField:'value',textField:'text',data: customerType,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">客户级别</td>
                    <td>
                        <input class="easyui-combobox" id="CustomerLevel" name="CustomerLevel"
                            data-options="valueField:'value',textField:'text',data: customerLevel," style="width: 95%" />
                    </td>
                    <td class="lbl">代理品牌</td>
                    <td>
                        <input class="easyui-combobox" id="AgentBrand" name="AgentBrand"
                            data-options="valueField:'ID',textField:'Name',data: Manufacture,multiple:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">我方合作公司</td>
                    <td>
                        <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                            data-options="valueField:'ID',textField:'Name',data: customerCompanyData,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">业务类型</td>
                    <td>
                        <input class="easyui-combobox" id="BusinessType" name="BusinessType"
                            data-options="valueField:'value',textField:'text',data: businessType,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">所属行业</td>
                    <td>
                        <input class="easyui-combobox" id="ReIndustry" name="ReIndustry"
                            data-options="valueField:'ID',textField:'Name',data: ReIndustry,multiple:true,required:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">主要产品</td>
                    <td>
                        <select id="IndustryInvolved" name="IndustryInvolved" class="easyui-combotree"
                            data-options="valueField: 'id',textField: 'text',data: DrpCategory,multiple:true,required:true" style="width: 95%;">
                        </select>
                    </td>
                    <td class="lbl">保护级别</td>
                    <td>
                        <input class="easyui-combobox" id="ProtectLevel" name="ProtectLevel"
                            data-options="valueField:'value',textField:'text',data: ProtectLevel," style="width: 95%" />
                    </td>
                    <td>保护范围</td>
                    <td>
                        <input class="easyui-textbox" id="ProtectionScope" name="ProtectionScope" style="width: 95%" data-options="validType:'length[1,50]'" />
                    </td>

                </tr>
                <tr>
                    <td>授信额度</td>
                    <td>
                        <input class="easyui-textbox" id="CreditLimit" name="CreditLimit" style="width: 95%" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
                    </td>
                    <td>授信账期</td>
                    <td>
                        <input class="easyui-textbox" id="CreditPayment" name="CreditPayment" style="width: 95%" data-options="validType:'length[1,20]'" />
                    </td>
                    <td>客户状态</td>
                    <td>
                        <input class="easyui-combobox" id="CustomerStatus" name="CustomerStatus"
                            data-options="valueField:'value',textField:'text',data: customerStatus,required:true," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td>特殊包装</td>
                    <td>
                        <input class="easyui-textbox" id="ExtraPacking" name="ExtraPacking" style="width: 95%" data-options="validType:'length[1,50]'" />
                    </td>
                    <td>特殊供应商</td>
                    <td>
                        <input class="easyui-textbox" id="SpecialSupplier" name="SpecialSupplier" style="width: 95%" data-options="validType:'length[1,50]'" />
                    </td>
                    <td>重点客户</td>
                    <td>
                        <input class="easyui-combobox" id="ImportantLevel" name="ImportantLevel"
                            data-options="valueField:'value',textField:'text',data: importantlevel," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td>信息来源</td>
                    <td>
                        <input class="easyui-textbox" id="InformationSource" name="InformationSource" style="width: 95%" data-options="validType:'length[1,50]',tipPosition:'bottom'" />
                    </td>
                    <td>自定义客户编号</td>
                    <td>
                        <input class="easyui-textbox" id="AdminCode" name="AdminCode" style="width: 95%" data-options="validType:'length[1,50]',tipPosition:'bottom'" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">其他备注信息</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Summary" name="Summary" style="width: 95%; height: 80px;"
                            data-options="validType:'length[1,300]',multiline:true,tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">区域</td>
                    <td colspan="5">
                        <input class="easyui-customarea" id="AreaID" name="AreaID" data-options="textField:'text',valueField:'id',data: AreaData,sonsField:'children'" />
                    </td>
                </tr>
            </table>
            <div id="divSave" style="text-align: center">
                <asp:Button ID="btnSumit" data-options="iconCls:'icon-save'" class="easyui-linkbutton" Text="保存" runat="server" OnClientClick="return Save();" />
                <asp:Button ID="btnClose" data-options="iconCls:'icon-save'" class="easyui-linkbutton" Text="取消" runat="server" OnClientClick="return closeWin()" />
            </div>
        </form>
    </div>
</body>
</html>
