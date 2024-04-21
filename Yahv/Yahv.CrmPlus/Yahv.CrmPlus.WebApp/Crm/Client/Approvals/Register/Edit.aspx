<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Register.Edit" %>


<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            //if (!jQuery.isEmptyObject(model.LogoFile)) {
            //    var msgr = $("#logoSuccess");
            //    var ul = $("<ul></ul>");
            //    var li = $("<li><a href='" + model.LogoFile.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + model.LogoFile.CustomName + "</em> </a></li>");
            //    ul.append(li);
            //    msgr.html(ul);
            //}
            //if (!jQuery.isEmptyObject(model.Licenses)) {
            //    var msgr = $("#licenseSuccess");

            //    var ul = $("<ul></ul>");
            //    for (var index = 0; index < model.Licenses.length; index++) {
            //        var item = model.Licenses[index];
            //        var li = $("<li><a href='" + item.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.FileName + "</em> </a></li>");
            //        ul.append(li);
            //    }
            //    msgr.html(ul);
            //};

            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    var isUniversity = $('#clientType').fixedCombobx('getValue') == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University %>';
                    Reset(checked, isUniversity)
                }
            });
            loadCombox();

            if (!jQuery.isEmptyObject(model.Entity)) {
                var isinternational = model.Entity.EnterpriseRegister.IsInternational;
                var isuniversity = model.Entity.ClientType == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University %>';
                Reset(isinternational, isuniversity)
                if (model.Entity.EnterpriseRegister.IsInternational) {
                    $('#IsInternational').checkbox('check');
                }
                $.each(model.Conduct, function (index, conduct) {
                    if (conduct.ConductType == '<%= (int)ConductType.Trade%>') {
                        $("input[name='ConductType'][value=1]").attr("checked", true);
                    } else {
                        $("input[name='ConductType'][value=2]").attr("checked", true);
                    }
                });
                $("#area").combobox("setValue", model.Entity.District);
                $("#industry").combobox("setValue", model.Entity.EnterpriseRegister.Industry);
                //$("#clientType").combobox("setValue",model.Entity.ClientType);
                loadform(model.Entity);
            }

            //输入客户名称后加载哪些？企业信息肯定要加载，客户信息是否加载？
            $("#Name").textbox('textbox').bind('blur', function (e) {
                var companyName = $.trim($('#Name').textbox("getText"));
                $.post('?action=GetEnterpriseName', { Name: companyName }, function (model) {
                    if (!jQuery.isEmptyObject(model)) {
                        loadform(entity);
                        $("#Company").combobox("setValue", entity.Relations[0].CompanyID);
                        if (entity.EnterpriseRegister.IsInternational) {
                            $('#IsInternational').checkbox('check');
                        };
                        $("#Source").combobox("setValue", entity.Source);
                        $("#area").combobox("setValue", entity.Enterprise.District);
                        $("#clientType").combobox("setValue", entity.ClientType);
                        $("#nature").combobox("setText", entity.EnterpriseRegister.Nature);
                        $("#industry").combobox("setValue", entity.EnterpriseRegister.Industry);
                        $("#website").textbox("setValue", entity.EnterpriseRegister.WebSite);
                        $("#Product").textbox("setValue", entity.Industry);
                        $("#Uscc").textbox("setValue", entity.EnterpriseRegister.Uscc);
                        $("#Corperation").textbox("setValue", entity.EnterpriseRegister.Corperation);
                        $("#RegistDate").datebox("setValue", entity.EnterpriseRegister.RegistDate);
                        $("#BusinessState").textbox("setValue", entity.EnterpriseRegister.BusinessState);
                        $("#RegistCurrency").combobox("setValue", entity.EnterpriseRegister.RegistCurrency);
                        $("#RegistFund").textbox("setValue", entity.EnterpriseRegister.RegistFund);
                        $("#RegAddress").combobox("setValue", entity.EnterpriseRegister.RegAddress);
                        $("#Employees").textbox("setValue", entity.EnterpriseRegister.Employees);
                        $("#Place").combobox("setValue", entity.Enterprise.Place);
                        $("#Currency").combobox("setValue", entity.Enterprise.Currency);
                        $("#adderss").combobox("setValue", model.Entity.Enterprise.RegAddress);

                    }
                });
            })
        });
        function loadform(entity) {
            $('#form1').form('load', {
                Nature: entity.EnterpriseRegister.Nature,
                ClientType: model.Entity.ClientType,
              //  Area：model.Entity.District,
                //Industry: model.Entity.EnterpriseRegister.Industry
                Name: entity.Name,
                Source: entity.Source,
                WebSite: entity.EnterpriseRegister.WebSite,
                Product: entity.Products,
                Uscc: entity.EnterpriseRegister.Uscc,
                Corperation: entity.EnterpriseRegister.Corperation,
                RegistDate: entity.EnterpriseRegister.RegistDate,
                RegistFund: entity.EnterpriseRegister.RegistFund,
                RegAddress: entity.EnterpriseRegister.RegAddress,
                Employees: entity.EnterpriseRegister.Employees,
                BusinessState: entity.EnterpriseRegister.BusinessState,
                Address: entity.EnterpriseRegister.RegAddress
            });

        }
        //页面显示和必填项重置
      function Reset(isinternatinal, isuniversity) {
            var options1 = {};
            options1['required'] = !isinternatinal && !isuniversity;
            var options2 = {};
            options2['required'] = isinternatinal;
            $("#Uscc").textbox({required:!isinternatinal});
            $("#Corperation").textbox(options1);
            $("#RegistDate").datebox(options1);
            $("#BusinessState").textbox(options1);
            $("#RegistCurrency").combobox(options1);
            $("#RegistFund").textbox(options1);
            $("#RegAddress").textbox(options1);
            $("#adderss").textbox(options2);
            $("#Currency").combobox(options2);
            $("#Place").combobox(options2);
            if (isinternatinal) {
                $(".domestic").hide();
                $(".university").hide();
                $(".IsInternation").show();
            } else {
                $(".IsInternation").hide();
                if (isuniversity) {
                    $(".university").show();
                    $(".domestic").hide()
                } else {
                    $(".university").show();
                    $(".domestic").show()
                }
            }

        }
        //function showBaseInfo(checked) {
        //    if (checked) {
        //        $(".IsInternation").show();
        //        $(".domestic").hide();

        //        InternationRequired(true)
        //    }
        //    else {
        //        $(".IsInternation").hide();
        //        $(".domestic").show();
        //        InternationRequired(false);
        //    }
        //}

        function InternationRequired(isRequired) {
            var options = {};
            options['required'] = isRequired;
            $("#Place").combobox(options);
            $("#adderss").textbox(options);
            $("#Currency").combobox(options);
            if (isRequired) {
                $("#Uscc").textbox({ required: false });
                $("#Corperation").textbox({ required: false });
                $("#RegistDate").datebox({ required: false });
                $("#RegistCurrency").combobox({ required: false });
                $("#RegistFund").textbox({ required: false });
                $("#RegAddress").textbox({ required: false });
                $("#BusinessState").textbox({ required: false });
            } else {
                $("#Uscc").textbox({ required: true });
                $("#Corperation").textbox({ required: true });
                $("#RegistDate").datebox({ required: true });
                $("#RegistCurrency").combobox({ required: true });
                $("#RegistFund").textbox({ required: true });
                $("#RegAddress").textbox({ required: true });
                $("#BusinessState").textbox({ required: true });
            }
            //$("#Grade").combobox({ required: true });
            //$("#ConductGrade").combobox({ required: true }); 
        }


        function loadCombox() {
            $('#license').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf,'.split(','),
                progressbarTarget: '#licenseMessge',
                successTarget: '#licenseSuccess',
                multiple: true,

            });
            $('#logo').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#logoMessge',
                successTarget: '#logoSuccess',
                multiple: false
            });
            $("#Owner").text(model.Owner)
            $('#Company').companyCrmPlus({
                value: model.corCompany
            });
            //客户来源
            $("#Source").fixedCombobx({
                type: "FixedSource",
            });
            //国别地区
            $("#area").fixedCombobx({
                type: "FixedArea",
            });
            //$("#clientType").fixedCombobx({
            //    type: "ClientType",
            //    value: model.Entity.ClientType
            //});
               $("#clientType").fixedCombobx({
                type: "ClientType",
                onChange: function (newvalue, oldvalue) {
                    if ($("#IsInternational").checkbox('options').checked == true) {
                        Reset(true, false);
                    } else {

                        var isUniversity = newvalue == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University %>';
                        var isinternational = false;
                        Reset(isinternational, isUniversity)
                    }
                },
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].value);
                }
            });

           <%-- $("#clientType").combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                onChange: function (newvalue, oldvalue) {
                    if ($("#IsInternational").checkbox('options').checked == true) {
                        Reset(true, false);
                    } else {

                        var isUniversity = newvalue == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University %>';
                        var isinternational = false;
                        Reset(isinternational, isUniversity)
                    }
                },
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].value);
                }
            });--%>

            //企业性质
            $("#nature").combobox({
                data: model.EnterpriseNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
                //value: model.Entity.EnterpriseRegister.Nature
            });
            //所属行业
            $("#industry").combobox({
                data: model.Industry,
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
                //value: model.Entity.EnterpriseRegister.Industry
            });

            debugger;
            //注册币种
            $("#RegistCurrency").fixedCombobx({
                type: "Currency",
                required: false,
                value: model.Entity.EnterpriseRegister.RegistCurrency

            });

            debugger;
            $("#Currency").fixedCombobx({
                type: "Currency",
                required: false,
                value: model.Entity.EnterpriseRegister.RegistCurrency
            });
            $("#Place").fixedCombobx({
                required: false,
                type: "Origin",
                value: model.Entity.Place
            });
            $("#Grade").fixedCombobx({
                required: true,
                type: "ClientGrade",
                value: model.Entity.ClientGrade
            });
            $("#Vip").fixedCombobx({
                type: "VIPLevel",
                value: model.Entity.Vip
            });
            $("#ConductGrade").fixedCombobx({
                required: false,
                type: "ConductGrade",
                value: model.Conduct.Grade
            });

        }

        // 不通过
        function reject() {
            $("#Summary").textbox({ required: true });
            var summary = $("#Summary").textbox("getValue");
            if (summary == "") {
                $.messager.alert("消息", "审批意见不能为空");
                return;
            }
            $.messager.confirm('确认', '您确定要否决该客户吗？', function (r) {
                if (r) {
                    $.post('?action=Reject',
                        {
                            id: model.Entity.ID,
                            Summary: $("#Summary").textbox("getValue"),
                        }, function (data) {
                            if (data.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "审批完成，已否决!",
                                    type: "success"
                                });
                                $.myWindow.close();
                            }
                            else {
                                $.messager.alert('操作提示', '操作失败!', data.message);
                            }
                        });
                }
            })
        }

    </script>

    <style type="text/css">
        .title {
            font-weight: bold;
            color: #575765;
            height: 20px;
            line-height: 20px;
            background-color: #F5F5F5;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao" id="baseInfo" style="padding: 20px 20px 0px 20px; margin-top: 2px;">
            <tr>
                <td colspan="4" class="title">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td>客户名称：</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 200px" data-options="required:true, validType:'length[1,50]'" /></td>
                <td>是否国际客户</td>
                <td>
                    <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsInternational" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>业务类型</td>
                <td colspan="3" class="auto-style5">
                    <input type="radio" name="ConductType" value="1" id="Trade" title="贸易" class="radio" checked="checked" /><label for="Trade" style="margin-right: 50px">贸易</label>
                    <input type="radio" name="ConductType" value="2" id="AgentLine" title="代理线" class="radio" /><label for="AgentLine">代理线</label>
                </td>
            </tr>
            <tr>
                <td>我方合作公司：</td>
                <td>
                    <input id="Company" name="Company" style="width: 200px" />
                </td>
                <td>客户所有人：</td>
                <td>
                    <label id="Owner" style="width: 200px"></label>
                </td>
            </tr>
            <tr>
                <td>客户来源： </td>
                <td>
                     <input  id="Source" name="Source"  style="width:200px"/>
                    <%--<select id="Source" name="Source" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select>--%>

                </td>
                <td>国别地区</td>
                <td>
                    <input  id="area" name="Area"  style="width:200px"/>
                    <%--<select id="area" name="Area" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:false,panelheight:'auto'"></select>--%>

                </td>
                <tr>
                    <td>客户类型</td>
                    <td>
                        <input id="clientType" name="ClientType" style="width: 200px" />
                        <%--<select id="clientType" name="ClientType" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:true,panelheight:'auto'"></select>--%></td>
                    <td>企业性质</td>
                    <td>
                        <select id="nature" name="Nature" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
                </tr>
            <tr>
                <td>所属行业</td>
                <td>
                    <select id="industry" name="Industry" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:false,panelheight:'auto'"></select></td>
                <td>网址</td>
                <td>
                    <input class="easyui-textbox" id="website" name="website" style="width: 200px" data-options="validType:'length[1,50]'" /></td>
            </tr>
            <tr>
                <td>主要产品</td>
                <td colspan="3">
                    <input class="easyui-textbox input" id="Product" name="Product"
                        data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 450px; height: 50px" />
                </td>
            </tr>
          <%--  <tr>
                <td>证照上传</td>
                <td>
                    <div>
                        <a id="license">上传</a>
                        <div id="licenseMessge" style="display: inline-block;"></div>
                    </div>
                    <div id="licenseSuccess"></div>
                    <td>企业logo</td>
                <td>
                    <div>
                        <a id="logo">上传</a>
                        <div id="logoMessge" style="display: inline-block;"></div>
                    </div>
                    <div id="logoSuccess"></div>
                </td>
            </tr>--%>
        </table>

        <div id="div_businessinfo" style="margin-top: 10px">
            <table class="liebiao" title="" id="businessinfo">
                <tr>
                    <td colspan="4" style="height: 30px;" class="title">
                        <p>工商信息</p>
                    </td>
                </tr>
                <tr class="university">
                    <td>社会统一信用编码</td>
                    <td colspan="3">
                        <input class="easyui-textbox Uscc" name="Uscc" id="Uscc" style="width: 350px" data-options="required:true, validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr class="domestic">
                    <td>法人代表</td>
                    <td>
                        <input class="easyui-textbox" id="Corperation" name="Corperation" style="width: 200px" data-options="required:false,validType:'length[1,50]'" />
                    </td>
                    <td>员工人数</td>
                    <td colspan="3">
                        <input class="easyui-numberbox" id="Employees" name="Employees" style="width: 200px" data-options="min:0" /></td>

                </tr>
                <tr class="domestic">
                    <td>公司成立日期</td>
                    <td>
                        <input class="easyui-datebox" id="RegistDate" name="RegistDate" style="width: 200px" data-options="required:false, editable:false" /></td>
                    <td>经营状态</td>
                    <td>
                        <input class="easyui-textbox" id="BusinessState" name="BusinessState" style="width: 200px" data-options="required:false,validType:'length[1,50]'" /></td>
                </tr>
                <tr class="domestic">
                    <td>注册币种</td>
                    <td>
                        <input id="RegistCurrency" name="RegistCurrency" style="width: 200px" />
                    </td>
                    <td>注册资金</td>
                    <td>
                        <input class="easyui-textbox" id="RegistFund" name="RegistFund" style="width: 200px" data-options="required:false,validType:'length[1,50]'" /></td>
                </tr>
                <tr class="domestic">
                    <td>注册地址</td>
                    <td colspan="3">
                        <input id="RegAddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,200]'" /></td>
                </tr>
                <tr class="IsInternation">

                    <td>国家地区</td>
                    <td>
                        <input id="Place" name="Place" style="width: 200px" />

                    </td>
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" style="width: 200px" />
                    </td>
                </tr>
                <tr class="IsInternation">
                    <td>详细地址</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="adderss" name="Address" style="width: 350px" data-options="required:false, validType:'length[1,50]'" /></td>

                </tr>
            </table>
        </div>
        <div id="ApprovalInfo" style="margin-top: 10px">
            <table class="liebiao">
                <tr>
                    <td colspan="4" style="height: 30px;" class="title">
                        <p>审批信息</p>
                    </td>
                </tr>
                <tr>
                    <td>客户等级</td>
                    <td>
                        <input id="Grade" name="Grade" style="width: 200px" />
                    </td>
                    <td>客户VIP等级</td>
                    <td>
                        <input id="Vip" name="Vip" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>当前业务等级</td>
                    <td>
                        <input id="ConductGrade" name="ConductGrade" style="width: 200px" />
                    </td>
                    <td>是否重点客户</td>
                    <td>
                        <input id="IsMajor" class="easyui-checkbox" name="IsMajor" /><label for="IsMajor" style="margin-right: 30px">是</label></td>
                </tr>
                <tr>
                    <td>是否特殊客户</td>
                    <td>
                        <input id="IsSpecial" class="easyui-checkbox" name="IsSpecial" /><label for="IsSpecial" style="margin-right: 30px">是</label></td>
                    <td>限定净毛利率</td>
                    <td>
                        <input id="ProfitRate" class="easyui-textbox" name="ProfitRate" style="width: 200px;" /></td>
                </tr>

                <tr>
                    <td>审批意见：</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="Summary" name="Summary" style="width: 450px; height: 50px" data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" /></td>

                </tr>
            </table>
        </div>
        <uc1:PcFiles runat="server" id="PcFiles"  IsPc="false"/>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" Text="通过" OnClick="btnSubmit_Click" Style="display: none" />
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'" onclick="$('#<%=btnSubmit.ID%>').click();">通过</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="reject()" data-options="iconCls:'icon-yg-approvalPass'">不通过</a>
        </div>
    </div>
</asp:Content>
