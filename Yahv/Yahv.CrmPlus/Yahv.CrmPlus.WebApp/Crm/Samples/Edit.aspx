<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Samples.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
        $(function () {

       
            $('#ClientName').combobox({
                data: model.Clients,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
             
                onChange: function (value) {
                    if (value != null) {
                        $.ajax({
                            type: "POST",
                            data: { ID: value },
                            url: '?action=getInfoByClient',
                            cache: false,
                            dataType: "json",
                            success: function (data) {
                                $('#Name').combobox({
                                    data: data.projects,
                                    valueField: 'value',
                                    textField: 'text',
                                    panelHeight: 'auto', //自适应
                                    multiple: false,
                                    limitToList: true,
                                    collapsible: true,
                                    //OnLoadSuccess: function () {
                                    //    $(this).combobox("setValue", model.Entity.Project.Name);

                                    //},
                                });
                                $('#Contact').combobox({
                                    data: data.contacts,
                                    valueField: 'value',
                                    textField: 'text',
                                    panelHeight: 'auto', //自适应
                                    multiple: false,
                                    limitToList: true,
                                    collapsible: true,

                                });

                                $('#Address').combobox({
                                    data: data.address,
                                    valueField: 'value',
                                    textField: 'text',
                                    panelHeight: 'auto', //自适应
                                    multiple: false,
                                    limitToList: true,
                                    collapsible: true,

                                });
                            }
                        });
                    }
                }
            });

                 if (!jQuery.isEmptyObject(model.Entity)) {

                $('#form1').form('load', {
                    ClientName: model.Entity.Project.EndClientID,
                    Name: model.Entity.Project.Name,
                    Contact: model.Entity.Contact.ID,
                    DeliveryDate: model.Entity.DeliveryDate,
                    Address: model.Entity.Address.ID,

                });
            };


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

            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
                onClickRow: onClickRow,
                columns: [[
                    {
                        field: 'SpnID', title: '产品型号', width: 300, align: 'left', mode: "local",
                        formatter: function (value) {
                            for (var i = 0; i < (model.StandardPartNumber).length; i++) {
                                if (model.StandardPartNumber[i].SpnID == value) return (model.StandardPartNumber)[i].PartNumber;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combogrid', options: {
                                data: model.StandardPartNumber, idField: "SpnID", textField: "PartNumber", required: true,
                                columns: [[
                                    { field: 'SpnID', title: 'ID', width: 10, align: 'left', hidden: 'true' },
                                    { field: 'PartNumber', title: '产品型号', width: 150 },
                                    { field: 'Brand', title: '品牌', width: 200 },
                                ]],

                            }
                        }
                    },
                    { field: 'Quantity', title: '数量', width: 130, align: 'left', editor: { type: 'numberbox', options: { required: true, min: 0 } } },
                    {
                        field: 'Price', title: '参考单价', width: 120, align: 'left', editor: {
                            type: 'numberbox', options: {
                                required: true, precision: 7,
                            }
                        }
                    },
                    {
                        field: 'Total', title: '总价', width: 80, align: 'center',
                        formatter: function (value, row) {
                            if (value != undefined && value != null) {
                                if (value.toString().indexOf('<span class="subtotal">') != -1) {
                                    return value;
                                }
                            }
                            return (row.Price * row.Quantity).toFixed(4);
                        },
                        editor: {
                            type: 'numberbox', options: {
                                min: 0, precision: 4, required: false, editable: false
                            }
                        }
                    },
                    //{ field: 'ProduceQuantity', title: '项目用量', width: 120, align: 'left', editor: { type: 'numberbox', options: { required: true, min: 0 } } },
                    {
                        field: 'SampleType', title: '送样类型', width: 100, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < model.SampleTypes.length; i++) {
                                if ((model.SampleTypes)[i].value == value) return (model.SampleTypes)[i].text;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combobox', options: {
                                data: model.SampleTypes, valueField: "value", textField: "text", required: true,
                            }
                        }
                    },
                    { field: 'Btn', title: '操作', width: 100, align: 'center', formatter: Operation }
                ]],


            });

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


        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            var data = new FormData($('#form1')[0]);
            accept();
            var rows = $('#dg').datagrid('getRows');
            data.append('products', JSON.stringify(rows));
            data.append("ID", model.Entity.ID)

            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    var res = eval(res);
                    if (res.success) {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                }
            }).done(function (res) {
            });

        }

    </script>


    <script>



        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
                //var editors = $('#dg').datagrid('getEditors', rowIndex);
                //var quantity = $(editors[1].target);
                //var price = $(editors[2].target);
                //price.numberbox({
                //    onChange: function () {
                //        AccTotal(quantity, price);
                //    }
                //});

                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index)
                        .datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }
        function append() {
            if (endEditing()) {
                $('#dg').datagrid('appendRow', { ProjectStatus: '10' });
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex)
                    .datagrid('beginEdit', editIndex);
            }
        }
        function removeit() {
            if (editIndex == undefined) { return }
            $('#dg').datagrid('cancelEdit', editIndex)
                .datagrid('deleteRow', editIndex);
            editIndex = undefined;
        }
        function accept() {
            if (endEditing()) {
                $('#dg').datagrid('acceptChanges');
            }
        }
        function reject() {
            $('#dg').datagrid('rejectChanges');
            editIndex = undefined;
        }
    </script>

    <style type="text/css">
        .title {
            font-weight: bold;
            color: #575765;
            height: 20px;
            line-height: 20px;
            background-color: #F5F5F5;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td colspan="4" class="title">
                    <p>送样信息</p>
                </td>
            </tr>
            <tr>
                <td>客户名称：</td>
                <td>
                    <label id="ClientName"></label>
                    <%--<input class="easyui-combobox" id="ClientName" name="ClientName" style="width: 200px" data-options="required:true,validType:'length[1,50]',readonly:true" /></td>--%>
                <td>项目名称：</td>
                <td>
                    <input class="easyui-combobox" id="Name" name="Name" style="width: 200px" data-options="required:true,validType:'length[1,50]'" /></td>
            </tr>

            <tr>
                <td>联系人：</td>
                <td>
                    <input class="easyui-combobox" id="Contact" name="Contact" style="width: 300px;" data-options="required:true, editable:false" />
                    <a id="btnContact" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'"></td>
                <td>寄送日期</td>
                <td>
                    <input class="easyui-datebox" id="DeliveryDate" name="DeliveryDate" style="width: 200px" data-options="required:true, editable:false" />
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

        <div id="sampleItems" style="margin-top: 10px">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto">
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="append()">添加</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">Reject</a>
            </div>
        </div>

        <div style="text-align: center; padding: 5px">
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">保存</a>
        </div>
    </div>
</asp:Content>

