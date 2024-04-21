<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Drafts.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {

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
                $("#Name").textbox("setValue", model.Entity.Name);
                $("#Company").combobox("setValue", model.CorCompany);
                if (model.Entity.EnterpriseRegister.IsInternational) {
                    $('#IsInternational').checkbox('check');
                }
                if (model.Conduct =='<%= ConductType.Trade.GetHashCode()%>') {
                    $("input[name='ConductType'][value=1]").attr("checked", true);
                } else {
                    $("input[name='ConductType'][value=2]").attr("checked", true);
                };
                $("#Source").combobox("setValue", model.Entity.Source);
                $("#area").combobox("setValue", model.Entity.District);
                $("#clientType").combobox("setValue", model.Entity.ClientType);
                var natureArr = model.Entity.EnterpriseRegister.Nature
                if (natureArr != null) {
                    $("#nature").combobox("setValues", natureArr.split(','));
                }
                $("#industry").combobox("setValues", model.Entity.EnterpriseRegister.Industry);
                $("#website").textbox("setValue", model.Entity.EnterpriseRegister.WebSite);
                $("#Product").textbox("setValue", model.Entity.Industry);

                $("#Uscc").textbox("setValue", model.Entity.EnterpriseRegister.Uscc);
                $("#Corperation").textbox("setValue", model.Entity.EnterpriseRegister.Corperation);
                $("#RegistDate").datebox("setValue", model.Entity.EnterpriseRegister.RegistDate);
                $("#BusinessState").textbox("setValue", model.Entity.EnterpriseRegister.BusinessState);
                $("#RegistCurrency").combobox("setValue", model.Entity.EnterpriseRegister.RegistCurrency);
                $("#RegistFund").textbox("setValue", model.Entity.EnterpriseRegister.RegistFund);
                $("#RegAddress").textbox("setValue", model.Entity.EnterpriseRegister.RegAddress);
                $("#Employees").textbox("setValue", model.Entity.EnterpriseRegister.Employees);
                $("#Place").fixedCombobx("setValue", model.Entity.Place);
                $("#Currency").fixedCombobx("setValue", model.Entity.EnterpriseRegister.Currency ??'<%=(int)Currency.CNY%>');
                $("#adderss").textbox("setValue", model.Entity.EnterpriseRegister.RegAddress);
            }
        });
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
            $("#Owner").text(model.Owner);
            $('#Company').companyCrmPlus({
                value: model.CorCompany
            });
            //客户来源
            $("#Source").fixedCombobx({
                type: "FixedSource",
            });
            //国别地区
            $("#area").fixedCombobx({
                type: "FixedArea",
            });

            //客户类型
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

            //企业性质
            $("#nature").combobox({
                data: model.EnterpriseNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
            });
            //所属行业
            $("#industry").combobox({
                data: model.Industry,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true
            });
            //注册币种
            $("#RegistCurrency").fixedCombobx({
                type: "Currency",
                value: model.Entity.EnterpriseRegister.RegistCurrency ??'<%=(int)Currency.CNY%>',
                required: false
            });
            //币种
            $("#Currency").fixedCombobx({
                type: "Currency",
                value: model.Entity.EnterpriseRegister.Currency ??'<%=(int)Currency.CNY%>',
                required: false,
            });
            $("#Place").fixedCombobx({
                required: false,
                type: "Origin",
                value: model.Entity.Place ??'<%=(int)Origin.CHN%>'
            });
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


    <script>
        //输入客户名称后加载关联关系，或其他相关数据
        function loadData() {
            var companyName = $.trim($('#Name').textbox("getText"));
            $.post('?action=GetEnterpriseName', { Name: companyName }, function (res) {
                if (!res.succes & !isDraft) {
                    var model = res.Entity;              //加载
                    if (!jQuery.isEmptyObject(model.Entity)) {
                        $("#Name").textbox("setValue", model.Entity.Name);
                        $("#Company").combobox("setValue", model.CorCompany);
                        $('#IsInternational').prop('checked', model.Entity.EnterpriseRegister.IsInternational);
                        if (model.Entity.Conducts[0].ConductType == <%= ConductType.Trade.GetHashCode()%>) {
                            $("input[name='ConductType'][value=1]").attr("checked", true);
                        } else {
                            $("input[name='ConductType'][value=2]").attr("checked", true);
                        };
                        $("#Source").combobox("setValue", model.Entity.Source);
                        $("#area").combobox("setValue", model.Entity.District);
                        $("#clientType").combobox("setValue", model.Entity.ClientType);
                        $("#nature").combobox("setValue", model.Entity.EnterpriseRegister.Nature);
                        $("#industry").combobox("setValue", model.Entity.EnterpriseRegister.Industry);
                        $("#website").textbox("setValue", model.Entity.EnterpriseRegister.WebSite);
                        $("#Product").textbox("setValue", model.Entity.Industry);
                        $("#Uscc").textbox("setValue", model.Entity.EnterpriseRegister.Uscc);
                        $("#Corperation").textbox("setValue", model.Entity.EnterpriseRegister.Corperation);
                        $("#RegistDate").datebox("setValue", model.Entity.EnterpriseRegister.RegistDate);
                        $("#BusinessState").textbox("setValue", model.Entity.EnterpriseRegister.BusinessState);
                        $("#RegistCurrency").combobox("setValue", model.Entity.EnterpriseRegister.RegistCurrency);
                        $("#RegistFund").textbox("setValue", model.Entity.EnterpriseRegister.RegistFund);
                        $("#RegAddress").combobox("setValue", model.Entity.EnterpriseRegister.RegAddress);
                        $("#Employees").textbox("setValue", model.Entity.EnterpriseRegister.Employees);
                        $("#Place").combobox("setValue", model.Entity.Place);
                        $("#Currency").combobox("setValue", model.Entity.Currency);
                        $("#adderss").combobox("setValue", model.Entity.RegAddress);
                    }
                }
            });
        }
    </script>
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
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 200px" data-options="required:true, validType:'length[1,50]'" onblur="loadData()" /></td>
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
                    <input id="Source" name="Source" style="width: 200px" />

                </td>
                <td>国别地区</td>
                <td>
                    <input id="area" name="Area" style="width: 200px" />
                </td>
                <tr>
                    <td>客户类型</td>
                    <td>
                        <input id="clientType" name="ClientType" style="width: 200px" />
                    </td>
                    <td>企业性质</td>
                    <td>
                        <select id="nature" name="Nature" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
                </tr>
            <tr>
                <td>所属行业</td>
                <td>
                    <select id="industry" name="Industry" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
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
            <tr>
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
                        <div id="logoMessge" style="display: inline-block; "></div>
                    </div>
                    <div id="logoSuccess"></div>
                </td>
            </tr>
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
                    <%-- <td>社会统一信用编码</td>
                <td>
                    <input class="easyui-textbox Uscc" name="Uscc" style="width: 200px" data-options="required:true, validType:'length[1,50]'" />
                </td>--%>
                </tr>
                <tr class="domestic">
                    <td>公司成立日期</td>
                    <td>
                        <input class="easyui-datebox" id="RegistDate" name="RegistDate" style="width: 200px" data-options="required:false, editable:false" /></td>
                    <td>经营状态</td>
                    <td>
                        <input class="easyui-textbox" id="BusinessState" name="BusinessState" style="width: 200px" data-options="required:false,validType:'length[1,50]'" /></td>
                    <%--  <td>营业期限</td>
                <td>
                    <input class="easyui-textbox" id="BusinessState1" name="BusinessState" style="width: 200px" data-options="validType:'length[1,50]'" /></td>--%>
                </tr>
                <tr class="domestic">
                    <td>注册币种</td>
                    <td>
                        <input id="RegistCurrency" name="RegistCurrency" style="width: 200px;" />

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
                        <input id="Place" name="Place" style="width: 200px;" />
                    </td>
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" style="width: 200px;" />
                        <%--  <select id="Currency" name="Currency" class="easyui-combobox" style="width: 200px" data-options=" required:false,editable:false,panelheight:'auto'"></select>--%>
                    </td>
                </tr>
                <tr class="IsInternation">
                    <td>详细地址</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="adderss" name="Address" style="width: 350px" data-options="required:false, validType:'length[1,50]'" /></td>
                </tr>
            </table>
        </div>
       <div>
           <uc1:PcFiles runat="server" id="PcFiles" IsPc="false" />
       </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
        </div>


    </div>
</asp:Content>

