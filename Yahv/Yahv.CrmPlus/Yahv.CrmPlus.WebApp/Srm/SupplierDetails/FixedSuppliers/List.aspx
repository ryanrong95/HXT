<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.FixedSuppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $("#cbbQuoteMethod").fixedCombobx({
                type: 'QuoteMethod'
            })
            $("#FreightPayer").fixedCombobx({
                type: 'FreightPayer'
            })
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', {
                    CutoffTime: model.Entity.CutoffTime,
                    QuoteMethod: model.Entity.QuoteMethod,
                    DeliveryPlace: model.Entity.DeliveryPlace,
                    FreightPayer: model.Entity.FreightPayer,
                    Mop: model.Entity.Mop,
                    WaybillFrom: model.Entity.WaybillFrom,
                    BatchMethod: model.Entity.BatchMethod,
                    DeliveryTime: model.Entity.DeliveryTime
                });
                if (model.Entity.IsOriginPi) {
                    $("#IsOriginPi").checkbox('check');
                }
                if (model.Entity.IsDelegatePay) {
                    $("#IsDelegatePay").checkbox('check');
                }
                if (model.Entity.IsWaybillPi) {
                    $("#IsWaybillPi").checkbox('check');
                }
                if (model.Entity.IsNotcieShiped) {
                    $("#IsNotcieShiped").checkbox('check');
                }
                $("#cbbQuoteMethod").fixedCombobx('setValue', model.Entity.QuoteMethod);
                $("#FreightPayer").fixedCombobx('setValue', model.Entity.FreightPayer)
            }
          
           
            $("#DeliveryPlace").combobox({
                data: model.Origin,
                valueField: 'value',
                textField: 'value',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
                required: false,
                editable: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', jQuery.isEmptyObject(model.Entity) ? data[0].value : model.Entity.DeliveryPlace);
                }
            })
            
            var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: true,
                method: 'get',
                queryParams: getQuery(),
                //fit: true,
                rownumbers: true,
                nowrap: false,
                fitColumns: false
            });
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?enterpriseid=' + model.EnterpriseID,
                    width: '600px',
                    height: '450px',
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })
            $('#PriceRules').fileUploader({
                required: false,
                type: 'PriceRules',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#filePriceRulesMessge',
                successTarget: '#filePriceRulesSuccess',
                multiple: true,
            });
        })
            function edit(isedit) {
                if (isedit) {
                    $("#detail").hide();
                    $("#operate").show();
                }
                else {
                    $("#detail").show();
                    $("#operate").hide();
                }
            }
            function btnformatter(value, rowData) {
                var arry = ['<span class="easyui-formatted">'];
                arry.push('<a id="btnUpdBrand" href="#" particle="Name:\'编辑固定渠道品牌\',jField:\'btnUpdBrand\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="editbrand(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDelBrand" href="#" particle="Name:\'删除固定渠道品牌\',jField:\'btnDelBrand\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
                arry.push('</span>');
                return arry.join('');
            }
            function editbrand(id) {
                $.myDialog({
                    title: '编辑',
                    url: 'Edit.aspx?id=' + id,
                    width: '600px',
                    height: '450px',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            }
            function del(id) {
                $.messager.confirm('确认', '确定要删除吗？', function (r) {
                    if (r) {
                        $.post('?action=del', { id: id }, function (success) {
                            if (success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除成功",
                                    type: "success"
                                });
                                window.grid.myDatagrid('flush');
                            }
                            else {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除失败",
                                    type: "error"
                                });
                            }
                        });
                    }
                })
            }
            function btnISformatter(value, rowdata) {
                return value ? "是" : "否";
            }
            function btnSummaryformatter(value, rowdata) {
                var span = "<span title='" + value + "'>";
                if (value == null) {
                    span += '';
                }
                else if (value.length > 15) {
                    span += value.substring(0, 14) + "......";
                }
                else {
                    span += value;
                }
                span += "</span>"
                return span;
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-panel" data-options="fit:true">
        <div id="operate" hidden="hidden">
            <table class="liebiao">
                <tr>
                    <td>截单时间</td>
                    <td>
                        <input id="textCutoffTime" name="CutoffTime" class="easyui-timespinner" style="width: 200px;" data-options="required:false" />
                    </td>
                    <td>询价议价方式</td>
                    <td>
                        <input id="cbbQuoteMethod" name="QuoteMethod" class="easyui-combobox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>发货地</td>
                    <td>
                        <input id="DeliveryPlace" name="DeliveryPlace" class="easyui-combobox" style="width: 200px;" />
                    </td>
                    <td>运费负担方 </td>
                    <td>
                        <input id="FreightPayer" name="FreightPayer" class="easyui-combobx" style="width: 200px;" data-options="required:false" />
                    </td>

                </tr>
                <tr>
                    <td>最小起订金额:</td>
                    <td>
                        <input id="numMop" name="Mop" class="easyui-numberbox" data-options="min:0,precision:0,value:0" style="width: 200px;" />

                    </td>
                    <td>运单号来源:</td>
                    <td>
                        <input id="WaybillFrom" class="easyui-textbox" name="WaybillFrom" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>货期</td>
                    <td>
                        <input id="textDeliveryTime" name="DeliveryTime" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                    </td>
                    <td>批号确认方式</td>
                    <td>
                        <input id="BatchMethod" name="BatchMethod" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                    </td>
                </tr>
                <tr>
                    <td>发票是否体现产地:</td>
                    <td>
                        <input id="IsOriginPi" class="easyui-checkbox" name="IsOriginPi" />
                    </td>
                    <td>是否接受委托付款:</td>
                    <td>
                        <input id="IsDelegatePay" class="easyui-checkbox" name="IsDelegatePay" />
                    </td>
                </tr>
                <tr>
                    <td>发票是否体现运单号:</td>
                    <td>
                        <input id="IsWaybillPi" class="easyui-checkbox" name="IsWaybillPi" />
                    </td>
                    <td>是否有发货通知单:</td>
                    <td>
                        <input id="IsNotcieShiped" class="easyui-checkbox" name="IsNotcieShiped" />
                    </td>

                </tr>
                <tr>
                    <td>定价规则:</td>
                    <td colspan="3">
                        <div>
                            <a id="PriceRules">上传</a>
                            <div id="filePriceRulesMessge" style="display: inline-block; width: 300px;"></div>
                        </div>
                        <div id="filePriceRulesSuccess"></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div style="text-align: center; padding: 5px">
                            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="edit(false)">取消编辑</a>
                            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                            <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                        </div>

                    </td>
                </tr>
            </table>
            <%----%>
        </div>


        <%
            Yahv.CrmPlus.Service.Models.Origins.FixedSupplier entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.FixedSupplier;

        %>
        <div id="detail">
            <table class="liebiao">
                <tr>
                    <td colspan="4">
                        <a id="btnUpd" class="easyui-linkbutton" particle="Name:'编辑固定供应商信息',jField:'btnUpd'" data-options="iconCls:'icon-yg-edit'" onclick="edit(true)">编辑</a>
                    </td>
                </tr>
                <tr>
                    <td>截单时间</td>
                    <td style="width: 350px">
                        <%=entity?.CutoffTime %>
                    </td>
                    <td>询价议价方式</td>
                    <td style="width: 350px">
                        <%=entity?.QuoteMethod.GetDescription() %>
                    </td>
                </tr>
                <tr>
                    <td>发货地</td>
                    <td>
                        <%=entity?.DeliveryPlace %>
                    </td>
                    <td>运费负担方 </td>
                    <td>
                        <%=entity?.FreightPayer.GetDescription() %>
                    </td>

                </tr>
                <tr>
                    <td>最小起订金额:</td>
                    <td>
                        <%=entity?.Mop %>
                    </td>
                    <td>运单号来源:</td>
                    <td>
                        <%=entity?.WaybillFrom %>
                    </td>
                </tr>
                <tr>
                    <td>货期</td>
                    <td>
                        <%=entity?.DeliveryTime %>
                    </td>
                    <td>批号确认方式</td>
                    <td>
                        <%=entity?.BatchMethod %>
                    </td>
                </tr>
                <tr>
                    <td>发票是否体现产地:</td>
                    <td>
                        <input class="easyui-checkbox" data-options="checked:'<%=entity?.IsOriginPi %>'" />
                    </td>
                    <td>是否接受委托付款:</td>
                    <td>
                        <input class="easyui-checkbox" data-options="checked:'<%=entity?.IsDelegatePay %>'" />
                    </td>
                </tr>
                <tr>
                    <td>发票是否体现运单号:</td>
                    <td>
                        <input class="easyui-checkbox" data-options="checked:'<%=entity?.IsWaybillPi %>'" />
                    </td>
                    <td>是否有发货通知单:</td>
                    <td>
                        <input class="easyui-checkbox" data-options="checked:'<%=entity?.IsNotcieShiped %>'" />
                    </td>

                </tr>

            </table>
            <uc1:PcFiles runat="server" id="PcFiles" IsPc="false" />
           
        </div>
         

        <div id="tb">
            <div>
                <table class="liebiao-compact">
                    <tr class="csrmtitle">
                        <td>品牌信息&nbsp;&nbsp;&nbsp;&nbsp;
                            <a id="btnCreator" particle="Name:'新增固定渠道品牌',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                    </tr>
                </table>
            </div>
        </div>

         <div style="height:300px">
           <table id="dg" style="width: 100%">
                <thead>
                    <tr>
                        <th data-options="field:'Brand',width:280">品牌名称</th>
                        <th data-options="field:'IsProhibited',formatter:btnISformatter,width:120">是否限制出货</th>
                        <th data-options="field:'IsDiscounted',formatter:btnISformatter,width:120">有无折扣</th>
                        <th data-options="field:'IsPromoted',formatter:btnISformatter,width:120">是否推广促销</th>
                        <th data-options="field:'IsAdvantaged',formatter:btnISformatter,width:120">是否优势</th>
                        <th data-options="field:'Summary',formatter:btnSummaryformatter,width:120">备注</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>


</asp:Content>
