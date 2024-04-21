<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Clients.Edit1" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>

    <style>
        .textbox-label, .textbox-label-after {
            width: 75px;
        }
    </style>
    <script>
        $(function () {
            //$('#cboOrigin').combobox({
            //    url: '?action=Origin',
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    onLoadSuccess: function (data) {
            //        $(this).combobox('select',
            //            model.Entity == null ? (data[0].value) : model.Entity.Origin);
            //    }
            //});

            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select',
                        model.Entity == null || model.Entity.Grade == null ? data[0].value : model.Entity.Grade);
                }
            });
            $('#selType').combobox({
                data: model.AreaType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.AreaType);  //全部
                    }
                }
            });
            $('#selNature').combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Nature);  //全部
                    }
                }
            });

            $("#VipRank").radio({
                name: "radio_vipRank",//input统一的name值
                data: model.VipRank,//数据
                valueField: 'value',//value值
                labelField: 'text',//
                checked: null //选中的值为某一条的valueField
            });

            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#sTips").show();
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,
                        DyjCode: model.Entity.DyjCode,
                        TaxperNumber: model.Entity.TaxperNumber,
                        AdminCode: model.Entity.Enterprise.AdminCode,
                        Corporation: model.Entity.Enterprise.Corporation,
                        RegAddress: model.Entity.Enterprise.RegAddress,
                        TaxperNumber: model.Entity.Enterprise.Uscc
                    });
                $('#txtName').textbox('readonly');
                $("#sgrade").addClass("level" + model.Entity.Grade)
                //$("#sgrade").html(model.Entity.Grade);
                $('#selOrigin').originPlace('setVal', model.Entity.Place)
                $("#VipRank").radio('setCheck', model.Entity.Vip);
                if (model.Entity.Major) {
                    $("#Major").checkbox('check');
                }
            }
            else {
                $(".tr_gradeORvip").hide();
                // $(".isadd").show();
                $('#selCooperType').combobox({
                    data: model.CooperType,
                    valueField: 'value',
                    textField: 'text',
                    panelHeight: 'auto', //自适应
                    multiple: false,
                    required: true,
                    onLoadSuccess: function (data) {
                        if (data.length > 0) {
                            $(this).combobox('select', data[0].value);  //全部
                        }
                    }
                });

            }

            //注册隐藏 VIP
            if (model.Entity == null) {
                $("#Vip").hide();
            }
            $("input", $("#txt_InternalCompany").next("span")).blur(function () {
                var enterprisename = $("#txtName").textbox('getValue');
                var company = $("#txt_InternalCompany").InternalCompany('getVal');
                if (enterprisename && company) {
                    $.post('?action=CheckEnterprise', { clientname: enterprisename, companyid: company }, function (result) {
                        result = eval(result);
                        if (!result.success) {
                            if (result.code == 2) {
                                $("#txt_InternalCompany").InternalCompany('setVal', null);
                                $.messager.alert('提示', result.message);
                            }
                        }
                    })
                }
            })

            //$("input", $("#txtName").next("span")).blur(function () {
            //    var enterprisename = $("#txtName").textbox('getValue');
            //    var company = $("#txt_InternalCompany").InternalCompany('getVal');
            //    console.log($("#txt_InternalCompany").InternalCompany('options'))
            //    if (enterprisename && company) {
            //        $.post('?action=CheckEnterprise', { clientname: enterprisename, companyid: company }, function (result) {
            //            result = eval(result);
            //            if (!result.success) {
            //                if (result.code == 2) {
            //                    $("#txt_InternalCompany").InternalCompany('setVal', null);
            //                    $.messager.alert('提示', result.message);
            //                }
            //            }

            //        })
            //    }
            //})
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
       <%-- <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">--%>
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">客户名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox readonly_style"
                                data-options="prompt:'公司（主体）公司名称,名称要保证全局唯一',required:true,validType:'length[1,75]'" style="width: 350px;">
                        </td>
                    </tr>
                    <%--<tr class="isadd" hidden="hidden">
                        <td>销售公司</td>
                        <td>
                            <input id="txt_InternalCompany" class="easyui-InternalCompany" name="txt_InternalCompany" data-options="required:true,width:350" value="" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 100px">性质</td>
                        <td colspan="3">
                            <select id="selNature" name="Nature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">类型</td>
                        <td colspan="3">
                            <select id="selType" name="Type" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                        </td>
                    </tr>
                    <tr class="tr_gradeORvip">
                        <td style="width: 100px">客户级别</td>
                        <%-- <td colspan="3"><span id="sgrade"></span></td>--%>
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
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td colspan="3">
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 350px;"
                                data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3">
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">法人</td>
                        <td colspan="3">
                            <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">注册地址</td>
                        <td colspan="3">
                            <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">统一社会信用代码</td>
                        <td colspan="3">
                            <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <%-- <tr>
                        <td style="width: 100px">统一社会信用代码</td>
                        <td colspan="3">
                            <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>--%>
                    <%--<tr class="isshow" hidden="hidden">
                        <td>销售类型</td>
                        <td>
                            <select id="selCooperType" name="selCooperType" class="easyui-combobox" data-options="editable:false,required:false" style="width: 350px"></select>

                        </td>
                    </tr>
                    --%>
                    <%--增加vip单选按钮--%>

                    <tr id="Vip">
                        <td style="width: 100px">Vip等级</td>
                        <td colspan="4">
                            <span id="VipRank" style="width: 16px"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td colspan="4">
                            <input id="Major" class="easyui-checkbox" name="Major" /><label for="Major" style="margin-right: 30px">设为重点客户<span class="star" ></span></label>
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <span id="sTips" hidden="hidden" style="color: red">提示：点击保存按钮并保存成功后将被重新审核</span><br />
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
         <%--   </div>
        </div>--%>
    </div>
</asp:Content>
