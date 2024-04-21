<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Admins.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script>
        window.saleFlag = false;
        window.companyFlag = false;
        $(function () {
            $("#cbo_Sales").combogrid({
                idField: 'ID',
                textField: 'RealName',
                rownumbers: true,
                required: true,
                //data: model.Sales,
                prompt: '请选择销售人员',
                missingMessage: '不能为空',
                //pagination: true,//是否分页
                //pageSize: 10,
                url: "?action=Sales&id=" + model.ClientID,
                method: 'get',
                columns: [[
                    { field: 'ID', title: 'ID', width: 90 },
                    { field: 'RealName', title: '真实姓名', width: 55 },
                    { field: 'UserName', title: '用户名', width: 80 },
                    { field: 'RoleName', title: '角色', width: 75 },
                ]],
                validType: 'validateSale[\'#cbo_Sales\']',
                loadMsg: "加载中...",
                keyHandler: {
                    query: function (keyword) {     //【动态搜索】处理 
                        //设置查询参数  
                        var queryParams = $("#cbo_Sales").combogrid("grid").datagrid('options').queryParams;
                        queryParams.keyword = keyword;
                        $('#cbo_Sales').combogrid("grid").datagrid('options').queryParams = queryParams;
                        //重新加载  
                        $('#cbo_Sales').combogrid("grid").datagrid("reload");
                        $('#cbo_Sales').combogrid("setValue", keyword);
                    }
                },

                onShowPanel: function () {//当下拉面板显示的时候触发。
                    if ($("#cbo_Sales")) {//判断是否初始化了
                        var k = $("#cbo_Sales").combogrid("getText");
                        $("#cbo_Sales").combogrid("grid").datagrid('load', {
                            q: k
                        })
                    }
                },
                onLoadSuccess: function (data) {
                    if (model.Admin) {
                        $("#cbo_Sales").combogrid('grid').datagrid('selectRecord', { ID: model.Admin.ID, RealName: model.Admin.Name });
                        //$("#cbo_Sales").combogrid('setValue', model.CompanyID)
                    }
                }
            });
            //if (model.CompanyID) {
            //    $("txt_InternalCompany").InternalCompany('setVal', model.CompanyID)
            //}

        })
        $("#form1").form(
               {
                   onsubmit: function () {
                       return $('#form1').form('validate') && window.flag;
                   }
               }
           );
        $.extend($.fn.validatebox.defaults.rules, {
            validateSale: {
                validator: function (value, param) {
                    var data = $("#cbo_Sales").combogrid("grid").datagrid('getData').rows;
                    value = $("#cbo_Sales").combobox('getValue');
                    if (data.length) {
                        for (var i = 0; i < data.length; i++) {
                            if (value == data[i].ID) {
                                saleFlag = true;
                                return saleFlag;
                            } else {
                                saleFlag = false;
                            }
                        }
                    }
                    return saleFlag;
                },
                message: '只能选择下拉列表的数据'
            }
        });
        function checkValidate() {
            return $('#form1').form('validate') && window.saleFlag && window.companyFlag;
        }
        function WinClose() {
            $.myWindow.close();
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td style="width: 100px">销售公司</td>
                <td colspan="3">
                    <input id="txt_InternalCompany" class="easyui-InternalCompany" name="txt_InternalCompany" data-options="required:true,width:350" value="" />
                </td>
            </tr>
            <tr>
                <td style="width: 100px">销售</td>
                <td colspan="3">
                    <input id="cbo_Sales" name="Sale" class="easyui-combogrid" style="width: 350px;" data-options="width:350">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">是否默认</td>
                <td colspan="3">
                    <input id="IsDefault" class="easyui-checkbox" name="IsDefault" /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="checkValidate()" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="WinClose()">关闭</a>
                </td>
            </tr>
        </table>

    </div>

</asp:Content>
