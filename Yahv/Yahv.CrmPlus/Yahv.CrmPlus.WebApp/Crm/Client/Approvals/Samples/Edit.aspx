<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Samples.Edit" %>

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

             if (!jQuery.isEmptyObject(model.Entity)) {
                $("#ClientName").text(model.Entity.Project.Client.Name);
                $("#Name").text(model.Entity.Project.Name);
                $("#Contact").text(model.Entity.Contact.Name);
                $("#DeliveryDate").text(model.Entity.DeliveryDate.toString("yyyy-MM-dd"));
                $("#Address").text(model.Entity.Address.Context);
            };


        });
            //列表框按钮加载
            function Operation(val, row, index) {
                if (val != undefined && val != null) {
                    if (val.toString().indexOf('<span class="subtotal">') != -1) {
                        return val;
                    }
                }

                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Remove(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                return buttons;
            }

            //操作
            function btnformatter(value, rowData) {
                var arry = ['<span class="easyui-formatted">'];
                arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('</span>');
                return arry.join('');
        }

        function showEditPage(id)
        {
             $.myDialog({
                title: "编辑",
                url:'./SampleItems/Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }


           function approval(result) {
            $("#Result").val(result);
            var tips = result ? "确认审批通过？" : "确认审批不通过？";
            // $("#ProjectCode").textbox({ required: !result });
            $.messager.confirm("操作提示", tips, function (r) {
                if (r) {
                    $('#btnAllowed').click()
                    //$('#form1').submit();
                }
            });
        }
            //function approval(result) {
            //    $("#Result").val(result);
            //    var isValid = $("#form1").form("enableValidation").form("validate");
            //    if (!isValid) {
            //        $.messager.alert('提示', '请按提示输入数据！');
            //        return false;
            //    }
            //    var data = new FormData($('#form1')[0]);
            //    accept();
            //    var rows = $('#dg').datagrid('getRows');
            //    var tips = result ? "确认审批通过？" : "确认审批不通过？";
            //    $.messager.confirm("操作提示", tips, function (r) {
            //        if (r) {
            //            $.post('?action=Approval',
            //                {
            //                    id: model.Entity.ID,
            //                    result: result,
            //                    products: JSON.stringify(rows),

            //                }, function (data) {
            //                    var result = JSON.parse(data);
            //                    if (result.success) {
            //                        top.$.timeouts.alert({
            //                            position: "TC",
            //                            msg: "操作成功！",
            //                            type: "success"
            //                        });
            //                        $.myWindow.close();
            //                    }
            //                    else {
            //                        $.messager.alert('操作提示', '操作失败!', result.message);
            //                    }
            //                });
            //        }
            //    });
            //}


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north',title:'送样信息'" style="height: 253px">
              <table class="liebiao">
            <tr>
                <td>客户名称：</td>
                <td>
                    <label id="ClientName"></label>
                    <td>项目名称：</td>
                <td>
                    <label id="Name"></label>
            </tr>

            <tr>
                <td>联系人：</td>
                <td>
                    <label id="Contact"></label>
                </td>
                <td>寄送日期</td>
                <td>
                    <label id="DeliveryDate"></label>
                </td>
            </tr>

            <tr>
                <td>详细地址：</td>
                <td colspan="3">
                    <label id="Address"></label>
                </td>
            </tr>
        </table>
        </div>
        <div data-options="region:'center',title:'送样明细'">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'">
                <thead>
                    <tr>
                        <th data-options="field:'SpnName',width:150">产品型号</th>
                        <th data-options="field:'Brand',width:150">品牌</th>
                        <th data-options="field:'Quantity',width:60">数量</th>
                        <th data-options="field:'Price',width:60">参考单价</th>
                        <th data-options="field:'Total',width:150">总价</th>
                        <th data-options="field:'SampleTypeDes',width:120">送样类型</th>
                         <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <%-- <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="add()">新增</a>
            </div>--%>
             <input id="Result" runat="server" type="text" hidden="hidden" />
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnAllowed" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click" />
            <a onclick="approval(true);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">通过</a>
            <a onclick="approval(false);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">不通过</a>
        </div>
         
        </div>
    </div>
</asp:Content>
