<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.Drafts.Add" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        var editRow = undefined;
        $(function () {
            $(".internation").hide();
            $(".state").show();
            $('#selArea').combobox({
                data: model.Area,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $("#selPlace").fixedCombobx({
                required: false,
                type: "Origin",
                value: '<%=(int)Origin.CHN%>'
            })
            $("#RegistCurrency").fixedCombobx({
                editable: false,
                required: false,
                type: "Currency",
                value: '<%=(int)Currency.CNY%>'
            })
            $("#Currency").fixedCombobx({
                editable: false,
                required: false,
                type: "Currency",
                value: '<%=(int)Currency.CNY%>'
            })
            $("#InvoiceType").fixedCombobx({
                editable: false,
                required: true,
                type: "InvoiceType",
                value: '<%=(int)InvoiceType.Unkonwn%>'
            })
            $("#SupplierType").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierType",
            })
            $("#OrderType").fixedCombobx({
                editable: false,
                required: true,
                type: "OrderType"
            })
            $("#EnterpriseNature").combobox({
                data: model.EnterpriseNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
                required: false,
                editable: true,
                panelheight: 'auto'
            })

            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    seNationalTyle(checked)
                }
            })
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '注册',
                    url: 'Add.aspx',
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })

            $("#SupplierName").textbox('textbox').bind('blur', function (e) {
                var name = $("#SupplierName").textbox('getValue');
                if (name.length) {
                    $.post("?action=Exist", { Name: name }, function (result) {
                        if (result.Exist && result.IsDraft) {
                            $.messager.confirm('确认', '已注册待审批，是否加载并修改？', function (r) {
                                if (r) {
                                    var entity = JSON.parse(result.Entity);
                                    $('#form1').form('load', {
                                        SupplierName: entity.Name,
                                        WorkTime: entity.WorkTime,
                                        WebSite: entity.EnterpriseRegister.WebSite,
                                        Product: entity.Products,
                                        Uscc: entity.EnterpriseRegister.Uscc,
                                        Corperation: entity.EnterpriseRegister.Corperation,
                                        RegistDate: entity.EnterpriseRegister.RegistDate,
                                        RegistCurrency: entity.EnterpriseRegister.RegistCurrency,
                                        RegistFund: entity.EnterpriseRegister.RegistFund,
                                        RegAddress: entity.EnterpriseRegister.RegAddress,
                                        Employees: entity.EnterpriseRegister.Employees,
                                        BusinessState: entity.EnterpriseRegister.BusinessState
                                    });
                                    if (entity.IsInternational) {
                                        $("#IsInternational").checkbox('check');
                                        $("#Address").textbox('setValue', entity.EnterpriseRegister.Address)
                                    }
                                    if (entity.IsFixed) {
                                        $("#IsFixed").checkbox('check');
                                    }
                                    seNationalTyle(entity.IsInternational)
                                }
                                else {
                                    $("#SupplierName").textbox('setValue', "");
                                }
                            })
                        }
                        else if (result.Exist && !result.IsDraft) {
                            $("#SupplierName").textbox('setValue', "");
                            top.$.timeouts.alert({ position: "TC", msg: result.message, type: "error" });
                        }
                        else {
                            return;
                        }
                    });
                }

            });
            var rowNumber = 0;

            $('#Licenses').fileUploader({
                required: true,
                type: 'Licenses',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileLicenseMessge',
                successTarget: '#fileLicenseSuccess',
                multiple: true,
            });
            $('#Logo').fileUploader({
                required: false,
                type: 'EnterpriseLogos',
                accept: 'image/gif,image/jpeg,image/bmp,image/png'.split(','),
                progressbarTarget: '#LogoMessge',
                successTarget: '#fileLogoSuccess',
                multiple: false
            });
        })
        function seNationalTyle(isnational) {
            $("#Address").textbox({ required: isnational });
            $("#Currency").textbox('textbox').validatebox('options').required = isnational;
            $("#selPlace").textbox('textbox').validatebox('options').required = isnational;
            $("#RegAddress").textbox({ required: !isnational });
            $("#Uscc").textbox({ required: !isnational });
            $("#BusinessState").textbox({ required: !isnational });
            $(".internation").show();
            if (isnational) {
                $(".internation").show();
                $(".state").hide();
            }
            else {
                $(".internation").hide();
                $(".state").show();
            }
        }
        function doDelete(index) {
            if (editRow != undefined) {
                $("#myTable").datagrid('cancelEdit', editRow);
                editRow = undefined;
            }

        }
        function doCancel(index) {
            $("#myTable").datagrid('cancelEdit', index);
            editRow = undefined;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="aa" class="easyui-panel" data-options="fit:true" style="padding: 3px 3px 0px 3px;">
        <table class="liebiao" title="基本信息">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>基本信息</p>
                    
                </td>
            </tr>
            <tr>
                <td>供应商名称</td>
                <td colspan="3">
                    <input id="SupplierName" name="SupplierName" class="easyui-textbox" style="width: 400px;" data-options="required:true,validType:'length[1,150]'" /></td>
            </tr>
            <tr>
                <td>国别地区</td>
                <td>
                    <select id="selArea" name="Area" class="easyui-combobox" data-options="required:true,editable:false,panelheight:'auto'" style="width: 200px"></select>
                </td>
                <td>是否国际</td>
                <td>
                    <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsFixed" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>企业性质</td>
                <td>
                    <select id="EnterpriseNature" name="EnterpriseNature" class="easyui-combobox" style="width: 200px"></select></td>
                <td>开票类型</td>
                <td>
                    <select id="InvoiceType" name="InvoiceType" style="width: 200px"></select>
                </td>
            </tr>
            <tr>
                <td>供应商类型</td>
                <td>
                    <select id="SupplierType" name="SupplierType" style="width: 200px"></select></td>
                <td>下单方式</td>
                <td>
                    <select id="OrderType" name="OrderType" style="width: 200px"></select>
                </td>
            </tr>
            <tr>
                <td>工作时间</td>
                <td>
                    <input id="WorkTime" name="WorkTime" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
                <td>是否固定渠道</td>
                <td>
                    <input id="IsFixed" class="easyui-checkbox" name="IsFixed" /><label for="IsFixed" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>网址</td>
                <td colspan="3">
                    <input id="WebSite" name="WebSite" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" />
                </td>
            </tr>
            <tr>
                <td>主要产品</td>
                <td colspan="3">
                    <input id="Product" name="Product" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" />
                </td>
            </tr>
            <tr>
                <td>证照</td>
                <td>
                    <div>
                        <a id="Licenses">上传</a>
                        <div id="fileLicenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileLicenseSuccess"></div>
                    <%-- <input id="Licenses" name="Licenses" class="easyui-filebox" style="width: 200px">--%>
                </td>
                <td>Logo</td>
                <td>
                    <div>
                        <a id="Logo">上传</a>
                        <div id="LogoMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileLogoSuccess"></div>

                    <%-- <input id="Qualification" name="Qualification" class="easyui-filebox" style="width: 200px">--%>
                </td>
            </tr>
            <%-- <tr>
                <td>资质文件</td>
                <td colspan="3">
                    <div>
                        <a id="Qualification">上传</a>
                        <div id="QualificationMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileQualificationSuccess"></div>
                </td>
            </tr>--%>
        </table>

        <table class="liebiao" title="工商信息">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr class="internation">
                <td>国家</td>
                <td>
                    <input id="selPlace" name="Place" style="width: 200px;" />
                </td>
                <td>币种</td>
                <td>
                    <input id="Currency" name="Currency" style="width: 200px;" />
                </td>
            </tr>
            <tr class="internation">
                <td>详细地址</td>
                <td colspan="3">
                    <input id="Address" name="Address" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,200]'" />
                </td>
            </tr>
            <tr class="state">
                <td>统一社会信用代码</td>
                <td>
                    <input id="Uscc" name="Uscc" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,50]'" /></td>
                <td>法人代表</td>
                <td>
                    <input id="Corperation" name="Corperation" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,150]'" /></td>
            </tr>
            <tr class="state">
                <td>公司成立日期</td>
                <td colspan="3">
                    <input id="RegistDate" name="RegistDate" class="easyui-datebox" style="width: 200px;" data-options="required:false" /></td>
            </tr>
            <tr class="state">
                <td>注册币种</td>
                <td>
                    <input id="RegistCurrency" name="RegistCurrency" style="width: 200px;" /></td>
                <td>注册资金</td>
                <td>
                    <input id="RegistFund" name="RegistFund" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>
            <tr class="state">
                <td>注册地址</td>
                <td colspan="3">
                    <input id="RegAddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,200]'" /></td>
            </tr>
            <tr class="state">
                <td>员工人数</td>
                <td>
                    <input id="Employees" name="Employees" class="easyui-numberspinner" style="width: 200px;" data-options="required:false,min:0,precision:0,value:0" /></td>
                <td>经营状态</td>
                <td>
                    <%--  <input id="BusinessState" name="BusinessState" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,50]'" /></td>--%>
                    <input id="BusinessState" name="BusinessState" class="easyui-combobox" style="width: 200px;"
                        data-options="data:[{value:'存续'},{value:'开业',selected:true},{value:'吊销'},{value:'注销'},{value:'迁入'},{value:'迁出'},{value:'停业'},{value:'清算'}],
                    valueField: 'value',
                    textField: 'value',
                    limitToList: false,
                    editable: true,
                    required:true,validType:'length[1,50]'" /></td>
            </tr>
        </table>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>

</asp:Content>
