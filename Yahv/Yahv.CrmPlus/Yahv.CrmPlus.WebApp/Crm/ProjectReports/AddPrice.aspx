<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="AddPrice.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.ProjectReports.AddPrice" %>

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
                onClickRow: onClickRow,
                columns: [[
                    { field: 'StartDate', title: '有效期开始', width: 130, align: 'left', editor: { type: 'datebox', options: { eitable: false } } },
                    { field: 'EndDate', title: '有效期结束', width: 130, align: 'left', editor: { type: 'datebox', options: {} } },
                    {
                        field: 'Currency', title: '币种', width: 100,
                        formatter: function (value) {
                            for (var i = 0; i < model.Currencys.length; i++) {
                                if ((model.Currencys)[i].value == value) return (model.Currencys)[i].text;
                            }
                            return value;
                          
                        },
                        editor: { type: 'combobox', options: { valueField: 'value', textField: 'text', data: model.Currencys, required: true, } },
                        

                    },
                    { field: 'MinQuantity', title: '最小数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: true, min: 0 } } },
                    { field: 'MaxQuantity', title: '最大数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: false, min: 0 } } },
                    {
                        field: 'UnitCostPrice', title: '官网价格', width: 120, align: 'left', editor: {
                            type: 'numberbox', options: {
                                required: true, precision: 7,
                            }
                        }
                    },
                    {
                        field: 'ResalePrice', title: '建议售价', width: 120, align: 'left', editor: {
                            type: 'numberbox', options: {
                                required: true, precision: 7,
                            }
                        }
                    },
                    {
                        field: 'ApprovedPrice', title: '采购价格', width: 120, align: 'left', editor: {
                            type: 'numberbox', options: {
                                required: true, precision: 7,
                            }
                        }
                    },

                    { field: 'ProfitRate', title: '毛利率', width: 120, align: 'left', editor: { type: 'numberbox', options: { required: false, editable: false, min: 0 } } },
                    {
                        field: 'QuoteType', title: '报价类型', width: 100, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < model.QuoteTypes.length; i++) {
                                if ((model.QuoteTypes)[i].value == value) return (model.QuoteTypes)[i].text;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combobox', options: {
                                data: model.QuoteTypes, valueField: "value", textField: "text", required: true,
                            }
                        }
                      
                    },
                    { field: 'Btn', title: '操作', width: 100, align: 'center', formatter: Operation }
                ]],


            });

        })

        function Remove(id) {
            $.messager.confirm('确认', '确认想删除该报价吗？', function (r) {
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

        //提交
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
            data.append('ReportID', model.ID);
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

        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Remove(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

    </script>

    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
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
                $('#dg').datagrid('appendRow', { ProjectStatus: '10' ,Currency:1,MaxQuantity:0});
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex)
                    .datagrid('beginEdit', editIndex);
            }
        }
        function reject() {
            $('#dg').datagrid('rejectChanges');
            editIndex = undefined;
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table id="dg" class="easyui-datagrid" style="width: auto; height: auto">
        </table>
        <div id="tb" style="height: auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="append()">添加</a>
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit()">Remove</a>--%>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">Reject</a>
        </div>
        <div style="text-align: center; padding: 5px">
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">保存</a>
        </div>
    </div>
</asp:Content>
