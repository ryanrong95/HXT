<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $(".IsInternation").hide();
            $(".domestic").show();
            InternationRequired(false);
            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    showBaseInfo(checked)
                }
            });
            loadCombox();

           if (!jQuery.isEmptyObject(model.LogoFile)) {
                var msgr = $("#logoSuccess");
                var ul = $("<ul></ul>");
                var li = $("<li><a href='" + model.LogoFile.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + model.LogoFile.CustomName + "</em> </a></li>");
                ul.append(li);
                msgr.html(ul);
            }
            if (!jQuery.isEmptyObject(model.Licenses)) {
                var msgr = $("#licenseSuccess");

                var ul = $("<ul></ul>");
                for (var index = 0; index < model.Licenses.length; index++) {
                    var item = model.Licenses[index];
                    var li = $("<li><a href='" + item.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.FileName + "</em> </a></li>");
                    ul.append(li);
                }
                msgr.html(ul);
            };


        
            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#Name").text(model.Entity.Name);
                if (model.Entity.EnterpriseRegister.IsInternational) {
                    $('#IsInternational').checkbox('check');
                }
                $("#Source").combobox("setValue", model.Entity.Source);
                $("#area").combobox("setValue", model.Entity.District);
                $("#clientType").combobox("setValue", model.Entity.ClientType);
                var natureArr = model.Entity.EnterpriseRegister.Nature
                $("#nature").combobox("setValues", natureArr.split(','));
                $("#industry").combobox("setValue", model.Entity.EnterpriseRegister.Industry);
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
                $("#Place").combobox("setValue", model.Entity.Place);
                $("#Currency").combobox("setValue", model.Entity.EnterpriseRegister.Currency);
                $("#adderss").textbox("setValue", model.Entity.EnterpriseRegister.RegAddress);
                //审批信息
                $("#Grade").combobox("setValue", model.Entity.Grade);
                $("#Vip").combobox("setValue", model.Entity.Vip);
               // $("#ConductGrade").combobox("setValue", model.Entity.Grade);
                if (model.Entity.IsMajor) {
                    $('#IsMajor').checkbox('check');
                }
                if (model.Entity.IsSpecial) {
                    $('#IsSpecial').checkbox('check');
                }
                $("#ProfitRate").textbox("setValue", model.Entity.ProfitRate);
            }

        });

        function showBaseInfo(checked) {
            if (checked) {
                $(".IsInternation").show();
                $(".domestic").hide();

                InternationRequired(true)
            }
            else {
                $(".IsInternation").hide();
                $(".domestic").show();
                InternationRequired(false);
            }

        }

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
            $("#Grade").combobox({ required: true });
           // $("#ConductGrade").combobox({ required: true });
        }

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
            $("#Source").combobox({
                data: model.Sources,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            });

            $("#area").combobox({
                data: model.Areas,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            });

            $("#clientType").combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onChange: function (m) {
                    if ($("#IsInternational").checkbox('options').checked == true) {

                        InternationRequired(true);
                    } else {
                        if (m =='<%=Yahv.Underly.CrmPlus.ClientType.University.GetHashCode()%> ') {
                            $(".domestic").hide();
                            $(".IsInternation").hide();
                            $(".university").show();
                            var options = {};
                            options['required'] = false;
                            $("#Corperation").textbox(options);
                            $("#RegistDate").datebox(options);
                            $("#BusinessState").textbox(options);
                            $("#RegistCurrency").combobox(options);
                            $("#RegistFund").textbox(options);
                            $("#RegAddress").textbox(options);

                        }
                        else {
                            $(".university").hide();
                            $(".IsInternation").hide();
                            $(".domestic").show();
                            var options = {};
                            options['required'] = true;
                            $("#Corperation").textbox(options);
                            $("#RegistDate").datebox(options);
                            $("#BusinessState").textbox(options);
                            $("#RegistCurrency").combobox(options);
                            $("#RegistFund").textbox(options);
                            $("#RegAddress").textbox(options);
                        }
                    }
                },
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);

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
                collapsible: true,
            });

                //币种
            $("#RegistCurrency").fixedCombobx({
                type: "Currency",
                required: false

            });
         
            $("#Currency").fixedCombobx({
                type: "Currency",
                required: false
            });

            $("#Place").fixedCombobx({
                required: false,
                type: "Origin",
                value: model.Entity.Place
            });

            $("#Grade").fixedCombobx({
                required: false,
                type: "ClientGrade",
               // value: model.Entity.ClientGrade
            });
            $("#Vip").fixedCombobx({
                required: false,
                type: "VIPLevel",
                value: model.Entity.Vip
            });


            //注册币种
            //$("#RegistCurrency").combobox({
            //    data: model.Currency,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    limitToList: true,
            //    collapsible: true,
            //});
            //$("#Currency").combobox({
            //    data: model.Currency,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    limitToList: true,
            //    collapsible: true,
            //});
            //$("#Place").combobox({
            //    data: model.Places,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    limitToList: true,
            //    collapsible: true,
            //});

            //$("#Grade").combobox({
            //    data: model.ClientGrade,
            //    valueField: 'value',
            //    textField: 'text',
            //    limitToList: true,
            //    collapsible: true
            //});
            //$("#Vip").combobox({
            //    data: model.Vip,
            //    textField: 'text',
            //    valueField: 'value'
            //});
            //$("#ConductGrade").combobox({ data: model.ConductGrade });
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
        <table class="liebiao" id="baseInfo" style="padding: 10px 10px 0px 10px; margin-top: 2px;">
            <tr>
                <td colspan="4" class="title">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td>客户名称：</td>
                <td>
                    <lable id="Name" name="Name"> </lable>

                    <%-- <input class="easyui-textbox" id="Name" name="Name" style="width: 200px" data-options="required:true, validType:'length[1,50]'"/></td>--%>
                <td>是否国际客户</td>
                <td>
                    <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsInternational" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <%--  <tr>
                <td>业务类型</td>
                <td colspan="3" class="auto-style5">
                    <input type="radio" name="ConductType" value="1" id="Trade" title="贸易" class="radio" checked="checked" /><label for="Trade" style="margin-right: 50px">贸易</label>
                    <input type="radio" name="ConductType" value="2" id="AgentLine" title="代理线" class="radio" /><label for="AgentLine">代理线</label>
                </td>
            </tr>
            <tr>
                <td>我方合作公司：</td>
                <td>
                    <select id="Company" name="Company" class="easyui-combobox" style="width: 200px" data-options="required:true, editable:false,panelheight:'auto'"></select>
                </td>
                <td>客户所有人：</td>
                <td>
                    <label id="Owner" style="width: 200px"></label>
                </td>
            </tr>--%>
            <tr>
                <td>客户来源： </td>
                <td>
                    <select id="Source" name="Source" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
                <td>国别地区</td>
                <td>
                    <select id="area" name="Area" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:false,panelheight:'auto'"></select></td>
                <tr>
                    <td>客户类型</td>
                    <td>
                        <select id="clientType" name="ClientType" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:false,panelheight:'auto'"></select></td>
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
                        data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>
            <tr>
                <td>证照上传</td>
                <td>
                    <div>
                        <a id="license">上传</a>
                        <div id="licenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="licenseSuccess"></div>

                    <td>企业logo</td>
                <td>
                    <div>
                        <a id="logo">上传</a>
                        <div id="logoMessge" style="display: inline-block; width: 300px;"></div>
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
                <tr class="domestic university">
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
                        <select id="RegistCurrency" name="RegistCurrency" class="easyui-combobox" style="width: 200px" data-options="required:false,editable:false,panelheight:'auto'"></select></td>
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
                        <select id="Place" name="Place" class="easyui-combobox" style="width: 200px" data-options="required:false,editable:false,panelheight:'auto'"></select></td>
                    <td>币种</td>
                    <td>
                        <select id="Currency" name="Currency" class="easyui-combobox" style="width: 200px" data-options=" required:false,editable:false,panelheight:'auto'"></select>
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
                        <select id="Grade" name="Grade" class="easyui-combobox" style="width: 200px" data-options="required:true, editable:false,panelheight:'auto'"></select>
                    </td>
                    <td>客户VIP等级</td>
                    <td>
                        <input   id="Vip" name="Vip" style="width: 200px"  />
                       <%-- <select id="Vip" name="Vip" class="easyui-combobox" style="width: 200px" data-options="required:false, editable:false,panelheight:'auto'"></select>--%>

                    </td>
                </tr>
                <tr>
                    <%-- <td>当前业务等级</td>
                    <td>
                        <select id="ConductGrade" name="ConductGrade" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>--%>
                    <td>是否特殊客户</td>
                    <td>
                        <input id="IsSpecial" class="easyui-checkbox" name="IsSpecial" /><label for="IsSpecial" style="margin-right: 30px">是</label></td>
                    <td>是否重点客户</td>
                    <td>
                        <input id="IsMajor" class="easyui-checkbox" name="IsMajor" /><label for="IsMajor" style="margin-right: 30px">是</label></td>
                </tr>
                <tr>
                    <td>限定净毛利率</td>
                    <td colspan="3">
                        <input id="ProfitRate" class="easyui-textbox" name="ProfitRate" style="width: 200px;" /></td>
                </tr>
                <%-- <tr>
                    <td>审批意见：</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="Summary" name="Summary" style="width: 450px; height: 50px" data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" /></td>

                </tr>--%>
            </table>
        </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <%--  <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>--%>
        </div>
    </div>
</asp:Content>
