<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Credits.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //校验输入是否为数字
            $("#Price").textbox("textbox").bind('blur', function (data) {
                var reg = /^(\-|\+)?\d+(\.\d+)?$/;
                var text = $("#Price").textbox("getText");
                var value = reg.exec(text);
                if (!value)
                    $("#Price").textbox("setValue", value);
            });

            var getQuery = function () {
                var params = {
                    action: 'data',
                    payerId: getUrlParam("payerId"),
                    payeeId: getUrlParam("payeeId"),
                    bus: getUrlParam("bus"),
                    cata: getUrlParam("cata"),
                    currency: getUrlParam("currency"),
                };

                return params;
            };

            $("#Currency").combobox({
                editable: false,
                data: model.Currencies,
                valueField: 'value',
                textField: 'text',
                panelHeight: '160px',
                required: true
            });

            if (model) {
                $("form").form("load", model);
                initGrid();

                if (getUrlParam("isShow") == 1) {
                    $("#Price").textbox("readonly");
                    $("#trMoney").hide();
                }
            }

            function initGrid() {
                //设置表格
                window.grid = $("#dg").myDatagrid({
                    pagination: false,
                    singleSelect: false,
                    method: 'get',
                    queryParams: getQuery(),
                    fit: true,
                    rownumbers: true
                });
            }
        });

        //获取url中的参数
        function getUrlParam(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
            var r = window.location.search.substr(1).match(reg);  //匹配目标参数
            if (r != null) return unescape(r[2]); return null; //返回参数值
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>客户</td>
                    <td>
                        <input id="Payer" name="Payer" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                    <td>业务</td>
                    <td>
                        <input id="Business" name="Business" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td>分类</td>
                    <td>
                        <input id="Catalog" name="Catalog" class="easyui-textbox" style="width: 200px;"
                            readonly="readonly" />
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" class="easyui-combobox" style="width: 200px;" />

                    </td>
                </tr>
                <tr id="trMoney">
                    <td>金额</td>
                    <td colspan="3">
                        <input id="Price" name="Price" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,75]'" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />

    <!-- 表格 -->
    <table id="dg" style="width: 100%" title="欠款批复记录">
        <thead>
            <tr>
                <th data-options="field: 'Project',width:200">分类</th>
                <th data-options="field: 'Price',width:200">金额</th>
                <th data-options="field: 'CurrencyName',width:200">币种</th>
                <th data-options="field: 'AdminName',width:120">操作人</th>
                <th data-options="field: 'CreateDate',width:150">创建日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>
