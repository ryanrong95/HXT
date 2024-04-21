<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsClients.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            // $('#selOrigin').originPlace();
            $('#selNature').combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                required: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        var selected = model.Entity == null ? data[0].value : (model.Entity.Nature == 0 ? data[0].value : model.Entity.Nature);
                        $(this).combobox('select', selected);  //全部
                    }
                }
            });
            $('#selServiceType').combobox({
                data: model.ServiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                required: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        var selected = model.Entity == null ? data[0].value : (model.Entity.ServiceType == 0 ? data[0].value : model.Entity.ServiceType);
                        $(this).combobox('select', selected);  //全部
                    }
                },
                onSelect: function (selected) {
                    switch (selected.value) {
                        case <%=(int)ServiceType.Customs%>:
                            $(".storage").hide();
                            $(".declaretion").show();
                            var required={required:true}
                            $("#txtUscc").textbox(required);
                            $("#txtRegaddress").textbox(required);
                            $("#txtCorporation").textbox(required);
                            
                            break;
                        case <%=(int)ServiceType.Warehouse%>:
                            $(".storage").show();
                            $(".declaretion").hide();
                            var required={required:false}
                            $("#txtUscc").textbox(required);
                            $("#txtRegaddress").textbox(required);
                            $("#txtCorporation").textbox(required);
                            break;
                        case <%=(int)ServiceType.Both%>:
                            $(".storage").hide();
                            $(".declaretion").show();
                            var required={required:true}
                            $("#txtUscc").textbox(required);
                            $("#txtRegaddress").textbox(required);
                            $("#txtCorporation").textbox(required);
                            break;
                        default:
                            $(".storage").hide();
                            $(".declaretion").show();
                            var required={required:true}
                            $("#txtUscc").textbox(required);
                            $("#txtRegaddress").textbox(required);
                            $("#txtCorporation").textbox(required);
                            break;
                    }

                }
            });

            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null || model.Entity.Grade == null ? data[data.length - 1].value : (model.Entity.Grade == 0 ? data[data.length - 1].value : model.Entity.Grade));
                }
            });
            $('#cboIdentity').combobox({
                data: model.Indentify,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                required: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        var selected = model.Entity == null ? data[0].value : (model.Entity.StorageType == 0 ? data[0].value : model.Entity.StorageType);
                        $(this).combobox('select', selected);  //全部
                    }
                }
            });
            $("#radio_EnterType").radio({
                name: "radio_EnterType",//input统一的name值
                data: [{ "value": "XL", "text": "芯达通(XL)" }, { "value": "WL", "text": "恒远(WL)" }],//数据
                valueField: 'value',//value值
                labelField: 'text',//
                checked: "XL" //选中的值为某一条的valueField
            });
            $("input", $("#txtName").next("span")).blur(function () {
                var enterprisename = $("#txtName").val();
                if (!jQuery.isEmptyObject(model.Entity)) {
                    if (enterprisename != model.Entity.Enterprise.Name) {
                        $.post('?action=CheckEnterprise', { Name: enterprisename }, function (success) {
                            if (success) {
                                $.messager.alert('提示', '客户已存在！');
                            }
                        })
                    }
                }
                else {
                    if (enterprisename.length > 0) {
                        $.post('?action=CheckEnterprise', { Name: enterprisename }, function (success) {
                            if (success) {
                                $.messager.alert('提示', '客户已存在！');
                            }
                        })
                    }
                }
            })
            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#sTips").show();
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,//企业名称
                        AdminCode: model.Entity.Enterprise.AdminCode,//管理员编码
                        Corporation: model.Entity.Enterprise.Corporation,//法人
                        RegAddress: model.Entity.Enterprise.RegAddress,//注册地址
                        Uscc: model.Entity.Enterprise.Uscc,//统一社会信用代码
                        EnterCode: model.Entity.EnterCode,//入仓号
                        CustomsCode: model.Entity.CustomsCode,//海关编码
                        Summary: model.Entity.Summary,//备注
                    });
                $('#selOrigin').originPlace('setVal', model.Entity.Place)
                $('#txtName').textbox('readonly');
                $('#txtEnterCode').textbox('readonly');

                if (!jQuery.isEmptyObject(model.Entity.BusinessLicense)) {
                    $('#ImgUpload').fileUpload('setFile', { src: model.Entity.BusinessLicense.Url, name: model.Entity.BusinessLicense.CustomName });
                }
                if (!jQuery.isEmptyObject(model.Entity.Logo)) {
                    $('#LogoUpload').fileUpload('setFile', { src: model.Entity.Logo.Url, name: model.Entity.Logo.CustomName });
                }
                if (model.Entity.EnterCode == null || model.Entity.EnterCode == "") {
                    $(".updTr").hide();
                }
                else {
                    $(".addTr").hide();
                }
            }
            else {
                $(".updTr").hide();
                $(".addTr").show();
                $('#selOrigin').originPlace('setVal', '<%=Origin.CHN.GetOrigin().Code%>')
            }

        })

        function fungeturl() {
            //营业执照上传
            var url = $('#ImgUpload').fileUpload('getFilesUploadAfterPath');
            var file = $('#ImgUpload').fileUpload('files')
            if (file != null) {
                $("#hidurl").val(url)
                $("#hidFormat").val(file[0].type)
                $("#hidName").val(file[0].name)
            }
            //Logo上传上传
            var logourl = $('#LogoUpload').fileUpload('getFilesUploadAfterPath');
            var filelogo = $('#LogoUpload').fileUpload('files')
            if (filelogo != null) {
                $("#hidUrl1").val(logourl)
                $("#hidFormat1").val(filelogo[0].type)
                $("#hidName1").val(filelogo[0].name)
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 800px">
            <div style="padding: 10px 60px 20px 60px;">

                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">业务</td>
                        <td colspan="3">
                            <select id="selServiceType" name="ServiceType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                        </td>
                    </tr>
                    <tr class="storage">
                        <td>客户身份</td>
                        <td>
                            <select id="cboIdentity" name="Identity" class="easyui-combobox" style="width: 350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox readonly_style"
                                data-options="prompt:'公司（主体）公司名称,名称要保证全局唯一',required:true,validType:'length[1,75]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr class="addTr">
                        <td>入仓类型 </td>
                        <td>
                            <span id="radio_EnterType" style="width: 16px"></span>
                        </td>
                    </tr>
                    <tr class="updTr">
                        <td style="width: 100px">入仓号</td>
                        <td colspan="3">
                            <input id="txtEnterCode" name="EnterCode" class="easyui-textbox readonly_style" style="width: 350px;" data-options="readonly:true,required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">性质</td>
                        <td colspan="3">
                            <select id="selNature" name="Nature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                        </td>
                    </tr>

                    <tr class="tr_gradeORvip">
                        <td style="width: 100px">级别</td>
                        <td colspan="3">
                            <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false,panelheight:'auto',required:false" style="width: 350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Origin" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                        </td>
                    </tr>
                    <%-- <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3">
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>--%>
                    <tr class="declaretion">
                        <td style="width: 100px">法人</td>
                        <td colspan="3">
                            <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr class="declaretion">
                        <td style="width: 100px">注册地址</td>
                        <td colspan="3">
                            <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr class="declaretion">
                        <td style="width: 100px">统一社会信用代码</td>
                        <td colspan="3">
                            <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>

                    <tr class="addTr">
                        <td style="width: 100px"><span style="color: red">Vip</span></td>
                        <td>
                            <input id="chbVip" class="easyui-checkbox" name="Vip" />
                        </td>
                    </tr>
                    <tr class="declaretion">
                        <td style="width: 100px">海关编码</td>
                        <td colspan="3">
                            <input id="txtCustomsCode" name="CustomsCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,10]'">
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">备注</td>
                        <td colspan="3">
                            <input id="txtSummary" name="Summary" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,100]'">
                        </td>
                    </tr>
                    <tr class="declaretion">
                        <td style="width: 100px">营业执照</td>
                        <td>
                            <input type="hidden" id="hidurl" runat="server">
                            <input type="hidden" id="hidFormat" runat="server">
                            <input type="hidden" id="hidName" runat="server">
                            <%-- <div style="margin-top: 5px; line-height: 40px;">上传图片（只能上传一张）</div>--%>
                            <a id="ImgUpload" href="#" class="easyui-fileUpload" data-options="{accept:'image/gif,image/png,image/jpeg',limit:1,required:false}"></a>
                        </td>
                    </tr>
                    <tr class="storage">
                        <td style="width: 100px">登记证</td>
                        <td>
                            <input type="hidden" id="hidur2" runat="server">
                            <input type="hidden" id="hidFormat2" runat="server">
                            <input type="hidden" id="hidName2" runat="server">
                            <%-- <div style="margin-top: 5px; line-height: 40px;">上传图片（只能上传一张）</div>--%>
                            <a id="HKRegistration" href="#" class="easyui-fileUpload" data-options="{accept:'image/gif,image/png,image/jpeg',limit:1,required:false}"></a>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">Logo</td>
                        <td>
                            <input type="hidden" id="hidUrl1" runat="server">
                            <input type="hidden" id="hidFormat1" runat="server">
                            <input type="hidden" id="hidName1" runat="server">
                            <%--<div style="margin-top: 5px; line-height: 40px;">上传图片（只能上传一张）</div>--%>
                            <a id="LogoUpload" href="#" class="easyui-fileUpload" data-options="{accept:'image/gif,image/png,image/jpeg',limit:1,required:false}"></a>
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="fungeturl()" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

