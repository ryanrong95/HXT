<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.AutoQuotes.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
        $(function () {
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                fit: true,
                fit: true,
                singleSelect: false,
                queryParams: getQuery()
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除库存');
                    return false;
                }
                var errors = [];
                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                })
                if (arry.length > 0) {
                    $.messager.confirm('确认', '您确认想要删除选中的库存吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { supplierid: model.ID, items: arry.toString() }, function () {
                                //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                                //    grid.myDatagrid('flush');
                                //});
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除成功!",
                                    type: "success"
                                });
                                grid.myDatagrid('flush');
                            });
                        }
                    })
                }
            })
        })
        function Date_formatter(obj) {
            var date = new Date(obj);
            var y = 1900 + date.getYear();
            var m = "0" + (date.getMonth() + 1);
            var d = "0" + date.getDate();
            return strDate = y + "-" + m.substring(m.length - 2, m.length) + "-" + d.substring(d.length - 2, d.length);
        }
    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">详情编辑</a>');
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a>');
            arry.push('</span>');
            return arry.join('');
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                partnumber: $.trim($('#txtPartNumber').textbox("getText")),
                manufacturer: $.trim($('#txtManufacturer').textbox("getText")),
                DateCode: $.trim($('#txtDateCode').textbox("getText")),
                //StartDate: $('#s_startdate').datebox("getValue"),
                //EndDate: $('#s_enddate').datebox("getValue"),
            };
            return params;
        };

        function showEditPage(id) {
            $.myDialog({
                url: 'Edit.aspx?id=' + id + '&supplierid=' + model.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除商品报价吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { supplierid: model.ID, items: id }, function () {
                        grid.myDatagrid('search', getQuery());
                    });
                }
            });
        }

        function showLotEidtPage() {
            $.myWindow({
                title: "自动报价池",
                url: 'LotEdit.aspx?supplierid=' + model.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        //function onSelect1(sd) {
        //    $('#s_enddate').datebox('calendar').calendar({
        //        validator: function (date) {
        //            return sd <= date;
        //        }
        //    });
        //}
        //function onSelect2(ed) {
        //    $('#s_startdate').datebox('calendar').calendar({
        //        validator: function (date) {
        //            return ed >= date;
        //        }
        //    });
        //}
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <%-- <input id="s_name" data-options="prompt:'型号/供应商/品牌/批次/封装/包装/报价人',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 300px" />--%>
                    <td style="width:90px;">型号</td>
                    <td>
                        <input id="txtPartNumber" data-options="prompt:'型号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width:90px;">品牌</td>
                    <td>
                        <input id="txtManufacturer" data-options="prompt:'品牌',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <td style="width:90px;">批次</td>
                    <td>
                        <input id="txtDateCode" data-options="prompt:'批次',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <%-- <td style="width:90px;">有效期</td>
                    <td>
                        <input id="s_startdate" type="text"  class="easyui-datebox" data-options="editable:false,onSelect:onSelect1" />-
                          <input id="s_enddate" type="text"  class="easyui-datebox" data-options="editable:false,onSelect:onSelect2" />
                    </td>--%>
                </tr>
                <tr>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showLotEidtPage()">添加</a>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" data-options="fitColumns:true,border:false">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'Name',width:100">型号</th>
                    <th data-options="field: 'Supplier',width:150">实际供应商名称</th>
                    <th data-options="field: 'Manufacturer',width:120">品牌</th>
                    <th data-options="field: 'DateCode',width:50">批次</th>
                    <th data-options="field: 'PackageCase',width:80">封装</th>
                    <th data-options="field: 'Packaging',width:80">包装</th>
                    <%--<th data-options="field: 'Prices',width:80">阶梯价</th>--%>
                    <th data-options="field: 'UnitPrice',width:80">单价</th>
                    <th data-options="field: 'Quantity',width:50">数量</th>
                    <%--<th data-options="field: 'Admin',width:80">报价人</th>--%>
                    <th data-options="field: 'Deadline',width:80,formatter:Date_formatter">有效期</th>
                    <th data-options="field: 'Btn',formatter:btnformatter,width:200">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
