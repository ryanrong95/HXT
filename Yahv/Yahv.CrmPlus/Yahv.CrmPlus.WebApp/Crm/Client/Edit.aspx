<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" Title="客户注册" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            Reset(false,false);
            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    var isUniversity = $('#clientType').fixedCombobx('getValue') == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University %>';
                    Reset(checked, isUniversity)
                }
            });
            loadCombox();
            $("input", $("#Name").next("span")).blur(function () {
                loadData();
            });

        });

         function Reset(isinternatinal, isuniversity) {
            var options1 = {};
            options1['required'] = !isinternatinal && !isuniversity;
            var options2 = {};
             options2['required'] = isinternatinal;
             $("#Uscc").textbox({ required: !isinternatinal });
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

        };
     
        //输入客户名称后加载关联关系，或其他相关数据
        function loadData() {
            var companyName = $.trim($('#Name').textbox("getValue"));
            $.post('?action=GetEnterpriseName', { Name: companyName }, function (data) {

                if (!data.succes && !data.isDraft) {
                    //加载
                    if (!jQuery.isEmptyObject(data.Entity)) {
                        var model = JSON.parse(data.Entity);
                        if (model) {
                            if (model.EnterpriseRegister.IsInternational) {
                                $('#IsInternational').checkbox('check');
                            }
                            $("#Source").combobox("setValue", model.Source);
                            $("#area").combobox("setValue", model.District);
                            $("#clientType").combobox("setValue", model.ClientType);
                            var natureArr = model.EnterpriseRegister.Nature
                            $("#nature").combobox("setValues", natureArr.split(','));
                            $("#industry").combobox("setValue", model.EnterpriseRegister.Industry);
                            $("#website").textbox("setValue", model.EnterpriseRegister.WebSite);
                            $("#Product").textbox("setValue", model.Industry);

                            $("#Uscc").textbox("setValue", model.EnterpriseRegister.Uscc);
                            $("#Corperation").textbox("setValue", model.EnterpriseRegister.Corperation);
                            $("#RegistDate").datebox("setValue", model.EnterpriseRegister.RegistDate);
                            $("#BusinessState").textbox("setValue", model.EnterpriseRegister.BusinessState);
                            $("#RegistCurrency").combobox("setValue", model.EnterpriseRegister.RegistCurrency);
                            $("#RegistFund").textbox("setValue", model.EnterpriseRegister.RegistFund);
                            $("#RegAddress").textbox("setValue", model.EnterpriseRegister.RegAddress);
                            $("#Employees").textbox("setValue", model.EnterpriseRegister.Employees);
                            $("#Place").combobox("setValue", model.Enterprise.Place);
                            $("#Currency").combobox("setValue", model.EnterpriseRegister.Currency);
                            $("#adderss").textbox("setValue", model.EnterpriseRegister.RegAddress);
                            $("#ProfitRate").textbox("setValue", model.ProfitRate);
                        };
                    }
                }
            });

        }

        function loadCombox() {
            $('#license').fileUploader({
                required: true,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#licenseMessge',
                successTarget: '#licenseSuccess',
                multiple: true
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
                required: true,
            });
            $("#Source").fixedCombobx({
                type: "FixedSource",
            });
            $("#area").fixedCombobx({
                type: "FixedArea",
            });
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
           <%-- $("#clientType").fixedCombobx({
                type:"ClientType",
                 onChange: function (m) {
                    if ($("#IsInternational").checkbox('options').checked == true) {
                        InternationRequired(true);
                    } else {
                        if (m =='<%=Yahv.Underly.CrmPlus.ClientType.University.GetHashCode()%>') {
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
            });--%>

          <%--  $("#clientType").combobox({
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
                        if (m =='<%=Yahv.Underly.CrmPlus.ClientType.University.GetHashCode()%>') {
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
                required: false,
                value: '<%=(int)Currency.CNY%>'
            });

            $("#Currency").fixedCombobx({
                type: "Currency",
                required: false,
                value: '<%=(int)Currency.CNY%>'
            });

            $("#Place").fixedCombobx({
                required: true,
                type: "Origin",
                value: '<%=(int)Origin.CHN%>'
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

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao" id="baseInfo">
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
                        <%--<select id="clientType" name="ClientType" class="easyui-combobox" style="width: 200px" data-options=" required:true,editable:false,panelheight:'auto'"></select>--%>
                        <input id="clientType" name="ClientType" style="width: 200px" />
                    </td>
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
                        <%--<select id="RegistCurrency" name="RegistCurrency" class="easyui-combobox" style="width: 200px" data-options="required:false,editable:false,panelheight:'auto'"></select>--%></td>
                    <td>注册资金</td>
                    <td>
                        <input class="easyui-textbox" id="RegistFund" name="RegistFund" style="width: 200px" data-options="required:false,validType:'length[1,50]'" />

                    </td>
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
                        <%--  <select id="Place" name="Place" class="easyui-combobox" style="width: 200px" data-options="required:false,editable:false,panelheight:'auto'"></select>--%></td>
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" style="width: 200px" />
                        <%-- <select id="Currency" name="Currency" class="easyui-combobox" style="width: 200px" data-options=" required:false,editable:false,panelheight:'auto'"></select>--%>
                    </td>
                </tr>
                <tr class="IsInternation">
                    <td>详细地址</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="adderss" name="Address" style="width: 350px" data-options="required:false, validType:'length[1,50]'" /></td>

                </tr>
            </table>
        </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
