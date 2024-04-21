<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Claim.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas.Claim" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/radio.js"></script>
    <script>
        $(function () {
            Reset(false, false);
            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    var IsUneversity = $("#clientType").combobox('getValue') == '<%=(int)Yahv.Underly.CrmPlus.ClientType.University%>';
                    Reset(checked, IsUneversity)
                }
            });
            loadCombox();
            $("#txtHaveConduct").next().hide();
            $("#txtHaveConduct").textbox("setValue", model.HaveConduct)
            showBaseInfo(model.HaveConduct);
        });


        function showBaseInfo(HaveConduct) {
            if (HaveConduct) {
               $(".nonconduct").hide();
                //基本信息和工商信息全非必填
                ConductRequired()
            }
            else {
                $(".nonconduct").show();
            }

        }
        ////是否国际,是否高校的必填项设置和显示
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
            $("#Address").textbox(options2);
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

        //有业务基本信息和工商信息全非不填
        function ConductRequired() {
            var ops = { required: false };
            $("#Place").combobox(ops);
            $("#Address").textbox(ops);
            $("#Currency").combobox(ops);
            $("#Uscc").textbox(ops);
            $("#Corperation").textbox(ops);
            $("#RegistDate").datebox(ops);
            $("#RegistCurrency").combobox(ops);
            $("#RegistFund").textbox(ops);
            $("#RegAddress").textbox(ops);
            $("#BusinessState").textbox(ops);
            $("#Nature").combobox(ops);
            $("#Industry").combobox(ops);

        }

        function loadCombox() {
            $('#license').fileUploader({
                required: false,
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

            $("#ConductType").combobox({
                data: model.Conducts,
                valueField: 'value',
                textField: 'text',
                required: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            });
            $('#Company').companyCrmPlus({
                editable: true,
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

            //企业性质
            $("#Nature").combobox({
                data: model.EnterpriseNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
            });
            //所属行业
            $("#Industry").combobox({
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



</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <input id="txtHaveConduct" class="easyui-textbox" name="HaveConduct" />
        <div class="conduct">
            <table class="liebiao">
                <tr>
                    <td>认领人：</td>
                    <td><%=Yahv.Erp.Current.RealName %></td>
                </tr>
                <tr>
                    <td>客户：</td>
                    <td><%=this.Model.ClientName %></td>
                </tr>
                <tr>
                    <td>业务</td>
                    <td colspan="3" class="auto-style5">
                        <input id="ConductType" name="ConductType" class="easyui-combobox" data-options="required:true, editable:false,panelheight:'auto'" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>我方合作公司</td>
                    <td>
                        <input  id="Company" name="Company"  style="width: 350px"/>
                    </td>
                </tr>

            </table>
        </div>
        <div class="nonconduct">
            <table class="liebiao" id="baseInfo" style="padding: 20px 20px 0px 20px; margin-top: 2px;">
                <tr>
                    <td colspan="4" class="csrmtitle">
                        <p>基本信息</p>
                    </td>
                </tr>
                <tr>
                    <td>是否国际客户</td>
                    <td colspan="3">
                        <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsInternational" style="margin-right: 30px">是</label>
                    </td>
                </tr>
                <tr>
                    <td>客户来源： </td>
                    <td>
                         <input  id="Source" name="Source"  style="width: 200px"/>
                        </td>
                    <td>国别地区</td>
                    <td>
                         <input  id="area" name="Area"  style="width: 200px"/>
                    </td>
                    <tr>
                        <td>客户类型</td>
                        <td>
                             <input  id="clientType" name="ClientType"  style="width: 200px"/></td>
                        <td>企业性质</td>
                        <td>
                            <select id="Nature" name="Nature" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
                    </tr>
                <tr>
                    <td>所属行业</td>
                    <td>
                        <select id="Industry" name="Industry" class="easyui-combobox" style="width: 200px" data-options="required:true,editable:false,panelheight:'auto'"></select></td>
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
                <table class="liebiao" title="csrmtitle" id="businessinfo">
                    <tr>
                        <td colspan="4" style="height: 30px;" class="csrmtitle">
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
                            <input id="Address" class="easyui-textbox" name="Address" style="width: 350px" data-options="required:false, validType:'length[1,50]'" /></td>
                    </tr>
                </table>

            </div>
        </div>


        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <%-- <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>--%>
        </div>
    </div>
</asp:Content>

