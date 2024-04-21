<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Projects.PartNumbers.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
            });

            $("#PartNumber").standardPartNumberCrmPlus({
                required: true
            });
      
            
            $("#Currency").fixedCombobx({
                type: "Currency",
                required:true

            });

            $("#ProjectStatus").combobox({
                data: model.ProjectStatus,
                onChange: function (newValue, oldValue ) {
                    if (model.Entity?.ProjectStatus != null && model.Entity.Status=='<%=Yahv.Underly.AuditStatus.Normal.GetHashCode()%>') {
                        var value = Number(newValue);
                        if (value != 500 && (value < model.Entity.ProjectStatus || value - model.Entity.ProjectStatus > 40)) {
                            alert("1、销售状态变更顺序为：DO->DI-> DW-> MP，不能跨状态变更，不能逆向变更。\n2、其他状态可以直接变为DL");
                            $("#ProjectStatus").combobox("setValue", model.Entity.ProjectStatus);
                        }
                    }else{
                        $("#ProjectStatus").combobox("setValue", model.Entity.ProjectStatus);
}
                },
            //  onLoadSuccess:function(){
            //  $(this).combobox("setValue",10)

            //   }
            });

     
            if (!jQuery.isEmptyObject(model.Entity)) {

                $('#form1').form('load', {
                    UnitProduceQuantity: model.Entity.UnitProduceQuantity,
                    ProduceQuantity: model.Entity.ProduceQuantity,
                    Currency: model.Entity.Currency,
                    ProjectStatus: model.Entity.ProjectStatus,
                    ExpectUnitPrice: model.Entity.ExpectUnitPrice,
                    ExpectTotal: model.Entity.ExpectTotal,
                });
            $("#PartNumber").standardPartNumberCrmPlus('setValue',model.Entity.SpnID);
            };
        })

        //新增竞品
        function add(id) {
            var id = model.Entity.ID;
            $.myDialog({
                title: "竞品新增",
                url: '../Compeletes/Add.aspx?ProjectProductID=' + id + "&ProjectID=" + model.Entity.ProjectID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "650px",
                height: "550px",
            });
            return false;
        }
        function Remove(id) {
            $.messager.confirm('确认', '确认想删除该竞品吗？', function (r) {
                if (r) {
                    $.post('?action=disable', { id: id }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "已删除!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作失败!",
                                type: "error"
                            });
                        }
                    });
                }
            })
        }


        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDelete" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Remove(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 253px">
            <table class="liebiao">
                <tr>
                    <td>产品型号</td>
                    <td colspan="3">
                        <input id="PartNumber" name="PartNumber" style="width: 400px;" />
                    </td>
                    <%-- <td>品牌</td>
                    <td>
                        <input id="Brand" name="Brand" class="easyui-combobox" style="width: 250px;" data-options="required:true" /></td>--%>
                </tr>
                <tr>
                    <td>单机用量</td>
                    <td>
                        <input id="UnitProduceQuantity" name="UnitProduceQuantity" class="easyui-numberbox" style="width: 250px;" data-options="required:true,min:0" /></td>
                    <td>项目用量</td>
                    <td>
                        <input id="ProduceQuantity" name="ProduceQuantity" class="easyui-numberbox" style="width: 250px;" data-options="required:false,min:0" />
                    </td>
                </tr>

                <tr>
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" style="width: 250px;" /></td>
                    <td>当前状态</td>
                    <td>
                        <input id="ProjectStatus" name="ProjectStatus" style="width: 250px;" class="easyui-combobox" data-options="required:true" />
                        <%-- <input id="ProjectStatus" name="ProjectStatus" style="width: 250px;"/>--%>
                    </td>

                </tr>
                <tr>
                    <td>参考单价</td>
                    <td>
                        <input id="ExpectUnitPrice" name="ExpectUnitPrice" style="width: 250px;" class="easyui-numberbox" data-options="required:true,min:0" /></td>
                    <td>预计成交量</td>
                    <td>
                        <input class="easyui-numberbox" id="ExpectQuantity" name="ExpectQuantity"
                            data-options="validType:'length[1,10]'," style="width: 250px" />
                </tr>
                <tr>
                    <td>预计成交额</td>
                    <td>
                        <input id="ExpectTotal" name="ExpectTotal" class="easyui-numberbox" style="width: 250px;" data-options="required:false,min:0" /></td>
                    <td colspan="2">预计成交额计算公式=[预计成交量]*[参考单价]</td>
                </tr>

                <%--<tr>
                  <td>凭证</td>
                    <td colspan="3">
                    <div>
                        <a id="voucher">上传</a>
                        <div id="filevoucherMessge" style="display: inline-block; width: 350px;"></div>
                        <div id="filevoucherSuccess"></div>
                    </div>
                    </td>
                  
                </tr>--%>
                <tr>
                    <td>备注</td>
                    <td colspan="3">
                        <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 350px; height: 80px" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                <%-- <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">取消</a>--%>
                <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
            </div>
        </div>
        <%
            if (!string.IsNullOrWhiteSpace(Request.QueryString["ID"]))
            {
        %>
        <div data-options="region:'center'">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="竞品列表">
                <thead>
                    <tr>
                        <th data-options="field:'SpnName',width:200">竞品型号</th>
                        <th data-options="field:'Brand',width:200">竞品品牌</th>
                        <th data-options="field:'UnitPrice',width:160">竞品单价</th>
                        <th data-options="field:'CreateDate',width:150">创建日期</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="add()">新增竞品</a>
            </div>
        </div>
        <%
            }
        %>
    </div>
</asp:Content>
