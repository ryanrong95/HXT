<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fix.szhxd.net/My/Scripts/area.data.js"></script>
    <script src="http://fix.szhxd.net/My/Scripts/areacombo.js"></script>
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script>

        var ID = '<%=this.Model.ID%>';
        var Ranks = eval('(<%=this.Model.ClientRanks%>)');
        var serviceType = eval('(<%=this.Model.ServiceType%>)');
        var storageType = eval('(<%=this.Model.StorageType%>)');
        var ChargeWHType = eval('(<%=this.Model.ChargeWHType%>)');
        var ChargeType = eval('(<%=this.Model.ChargeType%>)');

        var ClientInfoData = null;
        if (ID != '') {
            ClientInfoData = eval('(<%=this.Model.ClientInfoData != null ? this.Model.ClientInfoData:""%>)');
        } else {
            var autoclientCode = '<%=this.Model.AutoclientCode%>';
        }


        //数据初始化
        $(function () {
            if (window.parent.frames.Source == "Add") {
                $("#ChargeWH").combobox({ disabled: true }); //对应元素的禁用
            }

            //debugger;
            $(".storage").hide();
            $(".storage-hk").hide();
            $(".dechead").show()
            $(".contanct").show();
            $("#StorageType").combobox({ required: false })
            $("#certificate").chainsupload({ required: false });
            $("#ContactName").textbox({ required: true });
            $("#CompanyCode").textbox({ required: true });
            $("#Corporate").textbox({ required: true });
            $("#Address").textbox({ required: true });
            $("#ChargeType").combobox({ required: true });
            $("#AmountWH").textbox({ required: true });
            //$("#BusinessLicense").chainsupload({ required: true });
            //验证企业名称格式是否正确
            $.extend($.fn.validatebox.defaults.rules, {
                name_validate: {
                    validator: function (value, param) {
                        if (value.indexOf("reg-") != -1) {
                            return false
                        } else {
                            return true;
                        }


                    },
                    message: '企业名称格式不正确'
                }
            })

            //下拉框数据初始化
            $('#Rank').combobox({
                data: Ranks,
            });

            $('#ChargeType').combobox({
                data: ChargeType,
                onChange: function (m) {
                    debugger;
                    if (m == "<%=Needs.Ccs.Services.Enums.ChargeType.Domestic.GetHashCode()%>") {
                        $(".currencyType").text("RMB");
                    } else {
                        $(".currencyType").text("HKD");
                    }
                }
            });


            //初始化是否收取入仓费下拉框
            $('#ChargeWH').combobox({
                data: ChargeWHType,
                onChange: function (m) {
                    if (m == "<%=Needs.Ccs.Services.Enums.ChargeWHType.Charge.GetHashCode()%>") {
                        $("#ChargeType").combobox({ required: true });
                        $("#AmountWH").textbox({ required: true });
                        $(".chargeStyle").show();
                    } else {

                        $("#ChargeType").combobox({ required: false });
                        $("#AmountWH").textbox({ required: false });
                        $(".chargeStyle").hide();
                    }
                },
                onLoadSuccess: function () {
                    $("#ChargeWH").combobox("setValue", <%=Needs.Ccs.Services.Enums.ChargeWHType.Charge.GetHashCode()%>);
                }
            });

            $("#ServiceType").combobox({
                data: serviceType,
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onChange: function (m) {
                    switch (m) {
                        case '<%=Needs.Ccs.Services.Enums.ServiceType.Unknown.GetHashCode()%>':
                            $(".storage").hide();
                            $("#StorageType").combobox({ required: false });
                            break;
                        case '<%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>':
                            $(".storage").hide();
                            $(".dechead").show();
                            $(".storage-hk").hide();
                            $("#StorageType").combobox({ required: false });
                            $("#certificate").chainsupload({ required: false });
                            $("#ContactName").textbox({ required: true });
                            requriedValid(true);
                            break;
                        case '<%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>':
                            $(".storage").show();
                            $(".dechead").hide();
                            $("#StorageType").combobox({ required: true });
                            break;
                        case '<%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>':
                            $(".storage").show();
                            $(".dechead").show();
                            $("#StorageType").combobox({ required: true });
                            break;
                        default:
                    }

                },
            });
            $("#StorageType").combobox({
                data: storageType,
                required: false,
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                loadFilter: function (data) {
                    if ($("#ServiceType").combobox("getValue") ==<%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>) {
                        var newarr = [];
                        for (var i = 0; i < data.length; i++) {

                            if (data[i].Key != '<%=Needs.Ccs.Services.Enums.StorageType.Person.GetHashCode()%>') {
                                newarr.push(data[i]);

                            }
                        }
                        return newarr;
                    }
                    return data;
                },
                onChange: function (n) {
                    if ($("#ServiceType").combobox("getValue") !=<%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>) {

                        loadhtml(n);
                    }

                },


            });
            //文件上传控件初始化
            $('#BusinessLicense').chainsupload({
                multiple: false,
                validType: ['fileSize[1,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
            });
            $('#certificate').chainsupload({
                multiple: false,
                validType: ['fileSize[1,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
            });
            //初始化表头数据
            if (ClientInfoData != null) {

                if (ClientInfoData['ServiceType'] != null) {
                    $("#ServiceType").combobox("setValue", ClientInfoData['ServiceType'])
                    switch (ClientInfoData['ServiceType']) {
                        case <%=Needs.Ccs.Services.Enums.ServiceType.Unknown.GetHashCode()%>:
                            $(".storage").hide();
                            $("#StorageType").combobox({ required: false });
                            break;
                        case <%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>:
                            $(".storage").hide();
                            $("#StorageType").combobox({ required: false });
                            $("#certificate").chainsupload({ required: false });
                            break;
                        case <%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>:
                            $(".storage").show();
                            $("#StorageType").combobox({ required: true });
                            if (ClientInfoData['StorageType'] != null) {
                                var n = ClientInfoData['StorageType'];
                                loadhtml(n);
                            }
                            break;
                        case <%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>:
                            $(".storage").show();
                            $(".dechead").show();
                            $("#StorageType").combobox({ required: true });
                            if (ClientInfoData['StorageType'] != null) {
                                var n = ClientInfoData['StorageType'];
                                loadhtml(n);
                            }
                            break;
                        default:
                    }
                }


                if (ClientInfoData['ServiceType'] == '<%=Needs.Ccs.Services.Enums.ServiceType.Unknown.GetHashCode()%>') {
                    $("#ServiceType").combobox("setValue", '<%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>')
                } else {

                    $("#ServiceType").combobox("setValue", ClientInfoData['ServiceType'])
                }
                $("#StorageType").combobox("setValue", ClientInfoData['StorageType']);
                $('#ID').textbox('setValue', ClientInfoData['ID']);
                $('#ClientCode').textbox('setValue', ClientInfoData['ClientCode']);
                $('#Rank').combobox('setValue', ClientInfoData['Rank']);
                $('#CompanyName').textbox('setValue', ClientInfoData['CompanyName']);
                $('#CustomsCode').textbox('setValue', ClientInfoData['CustomsCode']);
                $('#CompanyCode').textbox('setValue', ClientInfoData['CompanyCode']);
                $('#Corporate').textbox('setValue', ClientInfoData['Corporate']);
                $('#Address').textbox('setValue', ClientInfoData['Address']);
                $('#ContactName').textbox('setValue', ClientInfoData['ContactName']);
                $('#Mobile').textbox('setValue', ClientInfoData['Mobile']);
                $('#Tel').textbox('setValue', ClientInfoData['Tel']);
                $('#Email').textbox('setValue', ClientInfoData['Email']);
                $('#Fax').textbox('setValue', ClientInfoData['Fax']);
                $('#Summary').textbox('setValue', ClientInfoData['Summary']);
                $('#ChargeWH').combobox('setValue', ClientInfoData['ChargeWH']);
                $('#ChargeType').combobox('setValue', ClientInfoData['ChargeType']);
                $('#AmountWH').numberbox('setValue', ClientInfoData['AmountWH']);



                if (ClientInfoData["ClientNature"] == '<%=Needs.Ccs.Services.Enums.ClientNature.Trade.GetHashCode()%>') {
                    $("input[name='ClientNature'][value=2]").attr("checked", true);
                } else {
                    $("input[name='ClientNature'][value=1]").attr("checked", true);
                };



                $('#BusinessLicense').chainsupload("setValue", ClientInfoData.File);
                $('#certificate').chainsupload("setValue", ClientInfoData.HKBusinessFile);
                // 不可编辑的字段,显示灰色；
                if (ClientInfoData.ClientStatus != '<%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>') {
                    //公司名称状态不是已完善,且包含前缀reg的可以修改
                    var name = ClientInfoData['CompanyName'];
                    if (name.indexOf("reg-") == -1) {
                        $('#CompanyName').textbox('textbox').attr('disabled', true);
                    }

                } else {
                    $('#ClientCode').textbox('textbox').attr('disabled', true);
                    $('#chaeckClientNo').css('display', 'none');
                    $('#CompanyName').textbox('textbox').attr('disabled', true);
                };

            } else {
                $('#ClientCode').textbox('setValue', autoclientCode);
            }


            //保存、申请
            $('.ir-save').on('click', function () {
                //debugger;

                //if (!$("#form1").form('validate')) {
                //    return;
                //}
                var message = RequiredFieldValidator();
                if (message != "OK") {
                    $.messager.alert("消息", message);
                    return;
                }
                //如果名称还有reg- 必须提醒修改公司名称后提交
                var name = $('#CompanyName').textbox('getValue');
                if (name.indexOf("reg-") != -1) {
                    $.messager.alert("消息", "请修改公司名称");
                    return;
                }

                // 新增 验证编号格式
                if (ClientInfoData == null) {

                    var clientNo = $('#ClientCode').textbox('getValue');
                    if (!(/^[H][X][T][0-9]{3}$/.test(clientNo) || /^[H][X][T][0-9]{3}$/.test(clientNo))) {
                        $.messager.alert("消息", "请输入正确的客户编号");
                        return;
                    }
                }



                MaskUtil.mask();//遮挡层
                //验证编号重复
                var id = getQueryString("ID");
                var from = getQueryString('From');
                var clientNo = $('#ClientCode').textbox('getValue');
                $.post('?action=CheckClientNo', { ID: id, ClientNo: clientNo }, function (res) {
                    // MaskUtil.unmask();//关闭遮挡层
                    if (!res) {
                        $.messager.alert('错误', "会员编号已被使用");

                        return;
                    }
                    else {

                        if ($("#ServiceType").combobox('getValue') ==<%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>
                            && ($("#StorageType").combobox('getValue') ==<%=Needs.Ccs.Services.Enums.StorageType.Person.GetHashCode()%>
                            || $("#StorageType").combobox('getValue') ==<%=Needs.Ccs.Services.Enums.StorageType.HKCompany.GetHashCode()%>)
                        ) {
                            fsubmit();
                        } else {
                            MaskUtil.mask();//遮挡层
                            //验证公司名称重复
                            var companyName = $('#CompanyName').textbox('getValue');
                            $.post('?action=CheckCompanyName', { ID: id, CompanyName: companyName }, function (res) {
                                MaskUtil.unmask();//关闭遮挡层
                                var resJson = JSON.parse(res);
                                if (!resJson.success) {
                                    $.messager.alert('错误', resJson.message);

                                    return;
                                }
                                else {
                                    fsubmit();//工商修改
                                }
                            });
                        }
                    }
                });
            });

            //验证客户编号是否重复
            $('#chaeckClientNo').on('click', function () {
                var clientNo = $('#ClientCode').textbox('getValue');
                var id = getQueryString("ID");
                if (!(/^[H][X][T][0-9]{3}$/.test(clientNo) || /^[H][X][T][0-9]{3}$/.test(clientNo))) {
                    $.messager.alert("消息", "请输入正确的客户编号");
                    return;
                }

                //提交后台
                $.post('?action=CheckClientNo', { ID: id, ClientNo: clientNo }, function (res) {
                    if (res) {
                        $.messager.alert('消息', "会员编号可以使用");
                    }
                    else {
                        $.messager.alert('错误', "会员编号已被使用");
                    }
                });
            });

            InitClientPage();
        });
        // 检查公司名称
        function checkCompanyName() {
            var id = getQueryString("ID");
            var companyName = $('#CompanyName').textbox('getValue');
            companyName = companyName.trim();
            $('#CompanyName').textbox('setValue', companyName);
            if (companyName.length <= 0) {
                $.messager.alert('错误', '请填写公司名称');
                return;
            }

            MaskUtil.mask();//遮挡层
            $.post('?action=CheckCompanyName', { ID: id, CompanyName: companyName }, function (res) {
                MaskUtil.unmask();//关闭遮挡层
                var resJson = JSON.parse(res);
                if (!resJson.success) {
                    $.messager.alert('错误', resJson.message);
                } else {
                    $('#CompanyCode').textbox('setValue', resJson.companyinfo.Uscc);
                    $('#Corporate').textbox('setValue', resJson.companyinfo.Corporation);
                    $('#Address').textbox('setValue', resJson.companyinfo.RegAddress);
                    $.messager.alert('正确', resJson.message);
                }
            });
        }

        function requriedValid(required) {

            $("#CompanyCode").textbox({ required: required });
            $("#Corporate").textbox({ required: required });
            $("#Address").textbox({ required: required });
            $("#BusinessLicense").chainsupload({ required: required });
        }
        //必填验证
        function RequiredFieldValidator() {
            //  var message = "";
            if ($("#CompanyName").textbox('getValue') == "") {
                return message = "请填写客户名称";
            }
            if ($("#ClientCode").textbox('getValue') == "") {
                return message = "请填写会员编号";
            }
            if (!$("#Rank").combobox('getValue')) {
                return message = "请填写会员等级";
            }
            if (!$("#ServiceType").combobox('getValue') || $("#ServiceType").combobox('getValue') == "") {
                return message = "请选择业务类型";
            }
            if (!$("#ChargeWH").combobox('getValue')) {
                return message = "请选择是否收取入仓费";
            }
            if ($("#ChargeWH").combobox('getValue') =="<%=Needs.Ccs.Services.Enums.ChargeWHType.Charge.GetHashCode()%>") {

                if (!$("#ChargeType").combobox('getValue')) {
                    return message = "请选择收取方式";
                }
                if (!$("#AmountWH").numberbox("getValue")) {
                    return message = "请输入仓费金额";
                }

            }

            if ($("#ServiceType").combobox('getValue') == <%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>) {
                if ($("#CompanyCode").textbox('getValue') == null || $("#CompanyCode").textbox('getValue') == "") {
                    return message = "请填写统一社会信用代码";
                }
                if ($("#Corporate").textbox('getValue') == null || $("#Corporate").textbox('getValue') == "") {
                    return message = "请填写法人信息";
                }
                if (!$("#Address").textbox("getValue")) {
                    return message = "请填写注册地址";
                }
                if (!$("#ContactName").textbox("getValue")) {
                    return message = "请添加联系人信息";
                }
                if (!$("#Mobile").textbox("getValue")) {
                    return message = "请添加手机号";
                }
                if ($("#spanBL_BusinessLicense input[name='BusinessLicense']")[0].files.length == 0) {
                    if ($("#urlBL_BusinessLicense").html() == undefined || $("#urlBL_BusinessLicense").html() == "") {
                        return message = "请上传营业执照";
                    }
                }
                if ($("#spanBL_BusinessLicense input[name='BusinessLicense']")[0].files[0].size > 1024000) {
                    return message = "文件大小必须小于1M";
                }
            }
            else if ($("#ServiceType").combobox('getValue') == <%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>) {

                var m = $("#StorageType").combobox("getValue");
                return message = StorageValid(m);

            }
            else if ($("#ServiceType").combobox('getValue') == <%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>) {

                if ($("#CompanyCode").textbox('getValue') == null || $("#CompanyCode").textbox('getValue') == "") {
                    return message = "请填写统一社会信用代码";
                }
                if (!$("#Corporate").textbox("getValue")) {
                    return message = "请填写法人信息";
                }
                if (!$("#Address").textbox("getValue")) {
                    return message = "请填写注册地址";
                }
                if ($("#spanBL_BusinessLicense input[name='BusinessLicense']")[0].files.length == 0) {
                    if ($("#urlBL_BusinessLicense").html() == undefined || $("#urlBL_BusinessLicense").html() == "") {
                        return message = "请上传营业执照";
                    }
                }

                if (!$("#Mobile").textbox("getValue")) {
                    return message = "请添加手机号";
                }
                if (!$("#ContactName").textbox("getValue")) {
                    return message = "请添加联系人信息";
                }

                var m = $("#StorageType").combobox("getValue")
                return message = StorageValid(m);
            }

            return "OK";
        }




        function StorageValid(m) {
            if (!$("#StorageType").combobox("getValue")) {
                return message = "请选择仓储类型";
            }
            var m = $("#StorageType").combobox("getValue");
            switch (m) {
                case '<%=Needs.Ccs.Services.Enums.StorageType.Domestic.GetHashCode()%>':
                    if (!$("#Mobile").textbox("getValue")) {
                        return message = "请添加手机号";
                    }
                    if (!$("#CompanyCode").textbox('getValue')) {
                        return flag = "请填写统一社会信用代码";
                    }
                    if (!$("#Corporate").textbox('getValue')) {
                        return flag = "请填写法人信息";
                    }
                    if ($("#Address").textbox("getValue") == null && $("#Address").textbox("getValue") == "") {
                        return flag = "请填写注册地址";
                    }
                    if ($("#spanBL_BusinessLicense input[name='BusinessLicense']")[0].files.length == 0) {
                        if ($("#urlBL_BusinessLicense").html() == undefined || $("#urlBL_BusinessLicense").html() == "") {
                            return message = "请上传营业执照";
                        }
                    }
                    if ($("#ContactName").textbox("getValue") == "") {
                        return flag = "请添加联系人";
                    }
                    if (!$("#Address").textbox("getValue")) {
                        return message = "请填写注册地址";
                    }
                    break;
                case '<%=Needs.Ccs.Services.Enums.StorageType.HKCompany.GetHashCode()%>':
                    if ($("#ContactName").textbox("getValue") == "") {
                        return flag = "请添加联系人信息";
                    }
                    if (!$("#Mobile").textbox("getValue")) {
                        return flag = "请添加手机号";
                    }
                    if ($("#spanBL_certificate input[name='certificate']")[0].files.length == 0) {
                        if ($("#urlBL_certificate").html() == undefined || $("#urlBL_certificate").html() == "") {
                            return message = "请上传登记证";
                        }
                    }

                    break;
                case '<%=Needs.Ccs.Services.Enums.StorageType.Person.GetHashCode()%>':
                    if (!$("#Mobile").textbox("getValue")) {
                        return flag = "请填写手机号";
                    }
                    break;



            }
            return flag = "OK";

        }

        function loadhtml(n) {
            var m = parseInt(n);
            if ($("#ServiceType").combobox("getValue") !=<%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>) {
                switch (m) {
                    case <%=Needs.Ccs.Services.Enums.StorageType.Domestic.GetHashCode()%>:
                        $(".storage-hk").hide();
                        $(".dechead").show();
                        $(".contanct").show();
                        $("#ContactName").textbox({ required: true });
                        $("#CompanyCode").textbox({ required: true });
                        $("#Corporate").textbox({ required: true });
                        $("#Address").textbox({ required: true });
                        $("#BusinessLicense").chainsupload({ required: true });
                        $("#certificate").chainsupload({ required: false });
                        //  requriedValid(true);
                        break;
                    case <%=Needs.Ccs.Services.Enums.StorageType.HKCompany.GetHashCode()%>:
                        $(".storage-hk").show();
                        $(".dechead").hide();
                        $(".contanct").show();
                        $("#ContactName").textbox({ required: true });
                        $("#certificate").chainsupload({ required: true });
                        $("#CompanyCode").textbox({ required: false });
                        $("#Corporate").textbox({ required: false });
                        $("#Address").textbox({ required: false });
                        $("#BusinessLicense").chainsupload({ required: false });
                        // requriedValid(false);
                        break;
                    case <%=Needs.Ccs.Services.Enums.StorageType.Person.GetHashCode()%>:
                        $(".contanct").hide();
                        $(".storage-hk").hide();
                        $(".dechead").hide();
                        $("#ContactName").textbox({ required: false });
                        $("#certificate").chainsupload({ required: false });
                        // requriedValid(false);
                        $("#CompanyCode").textbox({ required: false });
                        $("#Corporate").textbox({ required: false });
                        $("#Address").textbox({ required: false });
                        $("#BusinessLicense").chainsupload({ required: false });
                        break;
                    default:
                }

            } else {
                switch (m) {

                    case <%=Needs.Ccs.Services.Enums.StorageType.Domestic.GetHashCode()%>:
                        $(".storage-hk").hide();
                        $(".dechead").show();
                        $("#certificate").chainsupload({ required: false });
                        $(".contanct").show();
                        $("#ContactName").textbox({ required: true });
                        requriedValid(true);
                        break;
                    case <%=Needs.Ccs.Services.Enums.StorageType.HKCompany.GetHashCode()%>:
                        $(".storage-hk").show();
                        $(".dechead").show();
                        $(".contanct").show();
                        $("#ContactName").textbox({ required: true });
                        $("#certificate").chainsupload({ required: true });
                        requriedValid(true);
                        break;
                    case <%=Needs.Ccs.Services.Enums.StorageType.Person.GetHashCode()%>:
                        $(".storage-hk").hide();
                        $(".dechead").show();
                        $(".contanct").show();
                        $("#ContactName").textbox({ required: true });
                        $("#certificate").chainsupload({ required: false });
                        requriedValid(true);
                        break;
                    default:
                }

            }

        }

        function fsubmit() {
            var from = window.parent.frames.Source;
            var data = new FormData($('#form1')[0]);
            // data.append("ClientNature", $("input[name='ClientNature']:checked").val());
            //data.append("ChargeWH", $("#ChargeWH").combobox('getValue'));
            data.set("ChargeWH", $("#ChargeWH").combobox('getValue'));
            MaskUtil.mask();//遮挡层
            data.append("ClientID", ID);
            $.ajax({
                url: '?action=SaveClientInfo',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    //debugger;
                    MaskUtil.unmask();//关闭遮挡层

                    if (res.success) {
                        $.messager.alert('消息', res.message, '', function () {
                            var url = location.pathname.replace(/Edit.aspx/ig, 'Index.aspx');
                            if (from == "Add" && res.serviceType ==<%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>) {
                                return Return();
                            } else if (from == "Add") {
                                window.parent.location = url + '?Source=Add&ID=' + res.ID;
                            } else if (from = "Approve") {
                                window.parent.location = url + '?Source=Assign&ID=' + res.ID;
                            }
                            //  window.parent.location = url + '?Source=Add&ID=' + res.ID;//(ID == '' ? res.ID : '');
                        });
                    }
                    else {
                        $.messager.alert('错误', "新增会员信息错误");
                    }

                }
            })

        }

        function Return() {
            var source = window.parent.frames.Source;//$('#ClientInfo').data('source');
            var u = "";
            switch (source) {
                case 'Add':
                    u = 'New/List.aspx';
                    break;
                case 'Assign':
                    u = 'Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = 'New/List.aspx';
                    break;
                case 'ApproveView':
                    u = 'Approval/List.aspx';
                    break;
                case 'RiskView':
                    u = 'New/AllList.aspx';
                    break;
                case 'QualifiedView':
                    u = 'Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = 'ServiceManagerView/List.aspx';
                    break;
                default:
                    u = 'View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/Edit.aspx/ig, u);
            window.parent.location = url;
        }

    </script>
    <style>
        .irtbwrite td.lbl {
            font-size: 14px;
            width: 140px;
        }
    </style>
</head>
<body>
    <form id="form1">
        <div style="margin: 8px;">
            <input type="button" name="b1" class="easyui-linkbutton ir-save" value="保存" style="display: none;" title="保存" onclick="fsubmit()" />
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'">保存</a>
            <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
        </div>
        <div>

            <table class="irtbwrite" style="margin: 25px;">
                <tr>
                    <td class="lbl">公司(个人)名称：
                    </td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="CompanyName" name="CompanyName" data-options="required:true,validType:'length[1,150]', tipPosition:'bottom',validType:'name_validate'" />
                        <a href="javascript:void(0);" onclick="checkCompanyName()" style="color: #0081d5; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';">验证公司名称</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">会员编号：</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" name="ClientCode" style="width: 350px;" data-options="required:true,editable:false" />
                        <a href="javascript:void(0);" id="chaeckClientNo" style="display: none; color: #0081d5; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';">验证是否重复</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">会员等级：</td>
                    <td>
                        <input class="easyui-combobox" style="width: 350px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" id="Rank" name="Rank" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">业务类型：</td>
                    <td>
                        <input class="easyui-combobox" style="width: 350px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false," id="ServiceType" name="ServiceType" />

                    </td>
                </tr>
                <tr class="storage">
                    <td class="lbl">仓储客户类型：</td>
                    <td>
                        <input class="easyui-combobox" style="width: 350px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false," id="StorageType" name="StorageType" />

                    </td>
                </tr>

                <tr>
                    <td class="lbl">是否收取入仓费：</td>
                    <td>
                        <input class="easyui-combobox" style="width: 350px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" id="ChargeWH" name="ChargeWH" />
                    </td>
                </tr>

                <tr class="chargeStyle">
                    <td class="lbl">收取方式：</td>
                    <td>
                        <input class="easyui-combobox" style="width: 350px;" data-options="valueField:'Key',textField:'Value',limitToList:true,editable:false" id="ChargeType" name="ChargeType" />
                    </td>
                </tr>
                <tr class="chargeStyle">
                    <td class="lbl">入仓费金额：</td>
                    <td>
                        <input class="easyui-numberbox" style="width: 350px;" data-options="precision:2" id="AmountWH" name="AmountWH" /><label class="currencyType" style="color: red"></label>
                    </td>
                </tr>

                <tr>
                    <td class="lbl">手机号码：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="Mobile" name="Mobile" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">固定电话：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="Tel" name="Tel" data-options="validType:'length[1,50]'" />
                    </td>
                </tr>
                <%--  香港公司Start--%>

                <tr class="storage-hk">
                    <td class="lbl">登记证：</td>
                    <td>
                        <div id="certificate" name="certificate" style="width: 350px"></div>
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">会员类型：</td>
                    <td>
                        <input type="radio" name="ClientNature" value="2" id="Trade" class="radio" checked="checked" /><label for="Trade" style="margin-right: 50px">贸易商</label>
                        <input type="radio" name="ClientNature" value="1" id="terminal" class="radio" /><label for="terminal" style="margin-right: 50px">终端</label>
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl dechead">海关编码：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="CustomsCode" name="CustomsCode" data-options="validType:'length[1,10]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">统一社会信用代码：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="CompanyCode" name="CompanyCode" data-options="required:true,validType:'length[1,18]',tipPosition:'bottom'" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">公司法人：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="Corporate" name="Corporate" data-options="required:true" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">注册地址：</td>
                    <td>
                        <div class="easyui-textbox" style="width: 350px;" id="Address" name="Address" data-options="required:true"></div>
                        <%-- <div class="easyui-textbox" style="width: 350px;" id="Address" name="Address" data-options="required:true"></div>--%>
                    </td>
                </tr>
                <tr class="contanct">
                    <td class="lbl">联系人：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="ContactName" name="ContactName" data-options="required:true,validType:'length[1,150]'" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">电子邮件：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="Email" name="Email" data-options="required:false,validType:'email'" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">传真：</td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="Fax" name="Fax" data-options="validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr class="dechead">
                    <td class="lbl">营业执照：</td>
                    <td>
                        <div id="BusinessLicense" style="width: 350px" data-option="required: true"></div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
