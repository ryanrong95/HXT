<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="EditPublic.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Accounts.EditPublic" %>

<%@ Import Namespace="Yahv.Finance.Services.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>

    <script src="/Finance/Content/Scripts/enterprise.js"></script>
    <script>
        $(function () {
            //银行名称
            $('#BankName').combogrid({
                panelWidth: 500,
                fitColumns: true,
                nowrap: false,
                mode: "remote",
                data: model.Banks,
                idField: 'ID',
                textField: 'Name',
                multiple: false,
                required: true,
                editable: true,
                columns: [[
                    { field: 'Name', title: '银行名称', width: 150 },
                    { field: 'EnglishName', title: '英文名称', width: 120 },
                    { field: 'SwiftCode', title: 'SwiftCode', width: 150 }
                ]],
                onChange: function (q) {
                    //不根据ID 自动选择
                    if (q.indexOf('Bank') < 0)
                        doSearch(q, model.Banks, ['Name', 'EnglishName', 'SwiftCode'], $(this));
                }
            });

            //企业
            $('#Name').Enterprise({
                required: true,
                width: 200,
                valueField: 'Name',
                textField: 'Name',
            });

            //银行账户类型
            $('#AccountType').combobox({
                data: model.AccountTypes,
                valueField: "value",
                textField: "text",
                multiple: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.theAccountMapsAccountType != null) {
                        for (var i = 0; i < model.theAccountMapsAccountType.length; i++) {
                            $(this).combobox('select', model.theAccountMapsAccountType[i].AccountTypeID);
                        }
                    }
                },
                formatter: function (row) {
                    var opts = $(this).combobox('options');
                    var isExist = false;
                    for (var i = 0; i < model.theAccountMapsAccountType.length; i++) {
                        if (model.theAccountMapsAccountType[i].AccountTypeID == row[opts.valueField]) {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist) {
                        return '<input type="checkbox" accounttypekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" checked="checked" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    } else {
                        return '<input type="checkbox" accounttypekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    }
                },
                onSelect: function (record) {
                    $('input[accounttypekey="' + record.value + '"]').prop("checked", true);
                },
                onUnselect: function (record) {
                    $('input[accounttypekey="' + record.value + '"]').prop("checked", false);
                },
            });

            //管理类型
            $('#ManageType').combobox({
                data: model.ManageType,
                valueField: "value",
                textField: "text",
                onLoadSuccess: function (data) {
                    if (!model.Data) {
                        $(this).combobox('select', '<%=(int)Yahv.Underly.ManageType.Normal%>');
                    }
                },
            });

            //币种
            $('#Currency').combobox({
                data: model.Currency,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        $(this).combobox('select', model.Data.Currency);
                    }
                },
            });

            //公司名称
            $('#Name').combobox({
                onChange: function (n) {
                    $('#Enterprise').combobox('setValue', n);
                }
            });

            //来源
            $('#Source').combobox({
                data: model.AccountSources,
                valueField: "value",
                textField: "text",
                multiple: false,
                onSelect: function (record) {
                    //标准
                    if ('<%=(int)Yahv.Finance.Services.Enums.AccountSource.Standard%>' == record.value) {
                        $("#BankName").combobox('textbox').validatebox('options').required = true;
                        //$("#Code").textbox('textbox').validatebox('options').required = true;
                        $("#AccountType").combobox('textbox').validatebox('options').required = true;
                        $("#ManageType").combobox('textbox').validatebox('options').required = true;
                        $("#BankAddress").textbox('textbox').validatebox('options').required = true;
                        $("#IsHaveU").combobox('textbox').validatebox('options').required = true;
                        $("#Owner").combobox('textbox').validatebox('options').required = true;
                        $("#GoldStoreID").combobox('textbox').validatebox('options').required = true;
                        $("#Enterprise").combobox('textbox').validatebox('options').required = true;
                        $("#EnterpriseAccountType").combobox('textbox').validatebox('options').required = true;
                    }
                    //简易
                    else if ('<%=(int)Yahv.Finance.Services.Enums.AccountSource.Simple%>' == record.value) {
                        $("#BankName").combobox('textbox').validatebox('options').required = false;
                        //$("#Code").textbox('textbox').validatebox('options').required = false;
                        $("#AccountType").combobox('textbox').validatebox('options').required = false;
                        $("#ManageType").combobox('textbox').validatebox('options').required = false;
                        $("#BankAddress").textbox('textbox').validatebox('options').required = false;
                        $("#IsHaveU").combobox('textbox').validatebox('options').required = false;
                        $("#Owner").combobox('textbox').validatebox('options').required = false;
                        $("#GoldStoreID").combobox('textbox').validatebox('options').required = false;
                        $("#Enterprise").combobox('textbox').validatebox('options').required = false;
                        $("#EnterpriseAccountType").combobox('textbox').validatebox('options').required = false;
                    }

                    $('form').form('validate');
                },
                //onLoadSuccess: function (data) {
                //    if (data.length > 0 && model.Data != null) {
                //        $(this).combobox('select', model.Data.AccountSource);
                //    }
                //},
            });

            //国家及地区
            $('#District').combobox({
                data: model.Districts,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        $(this).combobox('select', model.Data.District);
                    }
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            //有无U盾
            $('#IsHaveU').combobox({
                data: model.IsHaveUs,
                valueField: "value",
                textField: "text"
            });

            //帐户管理人
            $('#Owner').combobox({
                data: model.Admins,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        $(this).combobox('select', model.Data.OwnerID);
                    }
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            //所在金库
            $('#GoldStoreID').combobox({
                data: model.GoldStores,
                valueField: "value",
                textField: "text",
                multiple: false
            });

            //账户类型
            $('#EnterpriseAccountType').combobox({
                data: model.EnterpriseAccountTypes,
                valueField: "value",
                textField: "text",
                multiple: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.theEpAccountType != null) {
                        for (var i = 0; i < model.theEpAccountType.length; i++) {
                            $(this).combobox('select', model.theEpAccountType[i].ID);
                        }
                    }
                },
                formatter: function (row) {
                    var opts = $(this).combobox('options');
                    var isExist = false;

                    if (model.theEpAccountType) {
                        for (var i = 0; i < model.theEpAccountType.length; i++) {
                            if (model.theEpAccountType[i].ID == row[opts.valueField]) {
                                isExist = true;
                                break;
                            }
                        }
                    }

                    if (isExist) {
                        return '<input type="checkbox" accounttypekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" checked="checked" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    } else {
                        return '<input type="checkbox" accounttypekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    }
                },
                onSelect: function (record) {
                    $('input[accounttypekey="' + record.value + '"]').prop("checked", true);
                },
                onUnselect: function (record) {
                    $('input[accounttypekey="' + record.value + '"]').prop("checked", false);
                }
            });

            //账户用途
            $('#AccountPurpose').combobox({
                data: model.AccountPurposes,
                valueField: "value",
                textField: "text",
                multiple: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.theAccountMapsPurpose != null) {
                        for (var i = 0; i < model.theAccountMapsPurpose.length; i++) {
                            $(this).combobox('select', model.theAccountMapsPurpose[i].AccountPurposeID);
                        }
                    }
                },
                formatter: function (row) {
                    var opts = $(this).combobox('options');
                    var isExist = false;
                    for (var i = 0; i < model.theAccountMapsPurpose.length; i++) {
                        if (model.theAccountMapsPurpose[i].AccountPurposeID == row[opts.valueField]) {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist) {
                        return '<input type="checkbox" accountpurposekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" checked="checked" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    } else {
                        return '<input type="checkbox" accountpurposekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    }
                },
                onSelect: function (record) {
                    $('input[accountpurposekey="' + record.value + '"]').prop("checked", true);
                },
                onUnselect: function (record) {
                    $('input[accountpurposekey="' + record.value + '"]').prop("checked", false);
                },
            });

            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData($('form')[0]);
                data.append('Bank', $('#BankName').combogrid('getText'));
                data.append('IsVirtual', $('#IsVirtual').checkbox('options').checked);
                ajaxLoading();
                $.post({
                    url: '?action=Submit&&id=' + getQueryString('AccountID'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialog.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });
                return false;
            });

            if (model.Data) {
                $('form').form('load', model.Data);

                if (model.Data.IsHaveU) {
                    $('#IsHaveU').combobox('select', 1);
                } else {
                    $('#IsHaveU').combobox('select', 2);
                }

                if (model.Data.IsVirtual) {
                    $('#IsVirtual').checkbox({ checked: true });
                }

                //银行账号 不可编辑
                $('#Code').textbox('disable');
                //币种
                $('#Currency').combobox('disable');
            }
        });

        function Close() {
            $.myDialog.close();
        }

        //q为用户输入，data为远程加载的全部数据项，searchList是需要进行模糊搜索的列名的数组，ele是combogrid对象
        //doSearch的思想其实就是，进入方法时将combogrid加载的数据清空，如果用户输入为空则加载全部的数据，输入不为空
        //则对每一个数据项做匹配，将匹配到的数据项加入rows数组，相当于重组数据项，只保留符合筛选条件的数据项，
        //如果筛选后没有数据，则combogrid加载空，有数据则重新加载重组的数据项
        function doSearch(q, data, searchList, ele) {
            ele.combogrid('grid').datagrid('loadData', []);
            if (q == "") {
                ele.combogrid('grid').datagrid('loadData', data);
                return;
            }
            var rows = [];
            $.each(data, function (i, obj) {
                for (var p in searchList) {
                    var v = obj[searchList[p]];
                    if (!!v && v.toString().indexOf(q) >= 0) {
                        rows.push(obj);
                        break;
                    }
                }
            });
            if (rows.length == 0) {
                ele.combogrid('grid').datagrid('loadData', []);
                return;
            }
            ele.combogrid('grid').datagrid('loadData', rows);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="hidden" id='ID' name="ID" />
            <input type="submit" id='btnSubmit' />
        </div>
        <table class="liebiao">
            <tr>
                <td>公司名称</td>
                <td>
                    <input id="Name" class="easyui-Enterprise" name="Name" style="width: 200px;" />
                </td>
                <td>银行名称</td>
                <td>
                    <input id="BankName" name="BankName" class="easyui-combobox" style="width: 200px;" />
                </td>

            </tr>
            <tr>
                <td>银行账号</td>
                <td>
                    <input id="Code" name="Code" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>币种</td>
                <td>
                    <input id="Currency" name="Currency" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>账户简称</td>
                <td>
                    <input id="ShortName" name="ShortName" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>来源</td>
                <td>
                    <select id="Source" name="Source" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;"></select>
                </td>
            </tr>
            <tr>
                <td>类型</td>
                <td>
                    <select id="EnterpriseAccountType" name="EnterpriseAccountType" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;"></select>
                </td>

                <td>管理类型</td>
                <td>
                    <input id="ManageType" name="ManageType" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>关联公司</td>
                <td>
                    <input id="Enterprise" class="easyui-Enterprise" name="Enterprise" data-options="required:true,width:200,valueField: 'Name',textField: 'Name'" value="" />
                </td>
                <td>银行账户类型</td>
                <td>
                    <select id="AccountType" name="AccountType" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;"></select>
                </td>
            </tr>
            <tr>
                <td>SwiftCode</td>
                <td>
                    <input id="SwiftCode" name="SwiftCode" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
                <td>开户行</td>
                <td>
                    <input id="OpeningBank" name="OpeningBank" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
            </tr>
            <tr>
                <td>开户行地址</td>
                <td>
                    <input id="BankAddress" name="BankAddress" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>开户时间</td>
                <td>
                    <input id="OpeningTime" name="OpeningTime" class="easyui-datebox" style="width: 200px;" data-options="editable:false,required:false" />
                </td>
            </tr>
            <tr>
                <td>国家及地区</td>
                <td>
                    <input id="District" name="District" class="easyui-combobox" data-options="editable:true,required:false," style="width: 200px;" />
                </td>
                <td>账户用途</td>
                <td>
                    <select id="AccountPurpose" name="AccountPurpose" class="easyui-combobox" data-options="editable:false,required:false," style="width: 200px;"></select>
                </td>
            </tr>
            <tr>
                <td>有无U盾</td>
                <td>
                    <input id="IsHaveU" name="IsHaveU" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;" />
                </td>
                <td>行号</td>
                <td>
                    <input id="BankNo" name="BankNo" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
            </tr>
            <tr>

                <td>所在金库</td>
                <td>
                    <input id="GoldStoreID" name="GoldStoreID" class="easyui-combobox" data-options="editable:true,required:true," style="width: 200px;" />
                </td>
                <td>是否虚拟账户</td>
                <td>
                    <input id="IsVirtual" name="IsVirtual" class="easyui-checkbox" />
                </td>
            </tr>
            <tr>
                <td>帐户管理人</td>
                <td colspan="3">
                    <input id="Owner" name="Owner" class="easyui-combobox" data-options="editable:true,required:true," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" />
                </td>
            </tr>
        </table>
    </div>
    <%--<div class="dialog-button" style="width: 100%; bottom: 0; margin-top: 164px;">
        <a href="javascript:;" class="l-btn l-btn-small" style="height: 22px;" onclick="Submit()">
            <span class="l-btn-left l-btn-icon-left" style="margin-top: -4px;">
                <span class="l-btn-text">提交</span>
                <span class="l-btn-icon icon-yg-confirm">&nbsp;</span>
            </span>
        </a>
        <a href="javascript:;" class="l-btn l-btn-small" style="height: 22px;" onclick="Close()">
            <span class="l-btn-left l-btn-icon-left" style="margin-top: -4px;">
                <span class="l-btn-text">关闭</span>
                <span class="l-btn-icon icon-yg-cancel">&nbsp;</span>
            </span>
        </a>
    </div>--%>
</asp:Content>
