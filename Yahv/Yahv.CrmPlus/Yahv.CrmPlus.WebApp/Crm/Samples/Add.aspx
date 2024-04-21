<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Samples.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
        $(function () {
            $("#btnContact").click(function () {
                var enterpriseid = $("#ClientName").combobox("getValue");
                if (!enterpriseid) {
                    $.messager.alert("消息", "请先选择客户");
                    return;
                }
                $.myDialog({
                    title: "新增联系人",
                    url: '../Client/Contacts/Edit.aspx?&id=' + enterpriseid, onClose: function () {
                        window.location.reload();
                    },
                    width: "50%",
                    height: "60%",
                });
            });

            if (!jQuery.isEmptyObject(model.Entity)) {
               // $("#ClientName").text(model.Entity.Project.Client.Name);
                $('#form1').form('load', {
                    Name: model.Entity.Project.Name,
                    Contact: model.Entity.Contact.ID,
                    DeliveryDate: model.Entity.DeliveryDate,
                    Address: model.Entity.Address.ID,

                });
            };



        });

        //新增明细
        function add(id) {
            var id = model.Entity.ID;
            $.myDialog({
                title: "竞品新增",
                url: '../SampleItems/Add.aspx?ProjectProductID=' + id + "&ProjectID=" + model.Entity.ProjectID, onClose: function () {
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
        ////列表框按钮加载
        //function Operation(val, row, index) {
        //    if (val != undefined && val != null) {
        //        if (val.toString().indexOf('<span class="subtotal">') != -1) {
        //            return val;
        //        }
        //    }

        //    var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Remove(\'' + row.ID + '\')" group >' +
        //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
        //        '<span class="l-btn-text">删除</span>' +
        //        '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
        //        '</span>' +
        //        '</a>';
        //    return buttons;
        //}

        //操作
        //function btnformatter(value, rowData) {
        //    var arry = ['<span class="easyui-formatted">'];
        //    arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
        //    arry.push('</span>');
        //    return arry.join('');
        //}


        //function Save() {
        //    var isValid = $("#form1").form("enableValidation").form("validate");
        //    if (!isValid) {
        //        $.messager.alert('提示', '请按提示输入数据！');
        //        return false;
        //    }
        //    var data = new FormData($('#form1')[0]);
        //    accept();
        //    var rows = $('#dg').datagrid('getRows');
        //    data.append('products', JSON.stringify(rows));

        //    $.ajax({
        //        url: '?action=Save',
        //        type: 'POST',
        //        data: data,
        //        dataType: 'JSON',
        //        cache: false,
        //        processData: false,
        //        contentType: false,
        //        success: function (res) {
        //            var res = eval(res);
        //            if (res.success) {
        //                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
        //                $.myWindow.close();
        //            }
        //            else {
        //                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
        //            }
        //        }
        //    }).done(function (res) {
        //    });

        //}


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
        <div data-options="region:'north',title:'送样信息'" style="height: 253px">
            <table class="liebiao" >
                <tr>
                    <td>客户名称：</td>
                    <td>
                        <input class="easyui-combobox" id="ClientName" name="ClientName" style="width: 200px" data-options="required:true,validType:'length[1,50]'" />
                    </td>
                    <td>项目名称：</td>
                    <td>
                        <input class="easyui-combobox" id="Name" name="Name" style="width: 200px" data-options="required:true,validType:'length[1,50]'" /></td>
                </tr>

                <tr>
                    <td>联系人：</td>
                    <td>
                        <input  id="Contact" name="Contact" style="width: 300px;"  />
                        <a id="btnContact" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'"></td>
                    <td>寄送日期</td>
                    <td>
                        <input class="easyui-datebox" id="DeliveryDate" name="DeliveryDate" style="width: 200px" />
                    </td>
                </tr>

                <tr>
                    <td>详细地址：</td>
                    <td colspan="3">
                        <input class="easyui-combobox" id="Address" name="Address"
                            data-options="required:true, multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 700px;" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px; display: none;">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
            </div>
        </div>

        <%
            if (!string.IsNullOrWhiteSpace(Request.QueryString["ID"]))
            {
        %>
        <div data-options="region:'center',title:'送样明细'">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'">
                <thead>
                    <tr>
                        <th data-options="field:'PartNumber',width:150">产品型号</th>
                        <th data-options="field:'Brand',width:150">品牌</th>
                        <th data-options="field:'Quantity',width:60">数量</th>
                        <th data-options="field:'Price',width:60">参考单价</th>
                        <th data-options="field:'ProjectStatusDes',width:150">总价</th>
                        <th data-options="field:'SampleType',width:120">送样类型</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="add()">新增</a>
            </div>
        </div>
        <%
            }
        %>
    </div>
</asp:Content>
