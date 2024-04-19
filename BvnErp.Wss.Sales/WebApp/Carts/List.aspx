<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Carts.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>购物车项</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

    <script>
        var getParams = function () {
            var data = {
                action: 'data'
            };
            return data;
        };

        var loadgrid = function () {
            $("#tab1").bvgrid({
                queryParams: getParams(),
                toolbar: '#tbtool',
                fit: true
            });
        };

        $(function () {
            loadgrid();
        });

    </script>
    <script>
        /* 操作按钮 */
        var btnformatter = function (value, row) {
            var arry = [];
            var btns = $('#option-btns');
            /* edit */
            var btn_edit = btns.find('.option-btn-edit');
            btn_edit.attr('onclick', 'edit(\'' + row.UserID + '\',\'' + row.ID + '\');');
            arry.push(btn_edit.prop('outerHTML'));
            return arry.join('');
        };

        var edit = function (uid, id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?uid=' + uid + '&sid=' + id,
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };
        /* 工具栏按钮 */
        var add = function (uid) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?uid=<%=uid%>',
                onClose: function () {
                    loadgrid();
                }
            }).open();
        };
        var reload = function () {
            loadgrid();
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',border:false">
        <table id="tab1" title="购物车列表" data-options="fitColumns:true,border:false,fit:true" class="mygrid">
            <thead>
                <tr>
                    <th g_name="btn" v_title="列表项操作按钮组" data-options="field:'btn',formatter:btnformatter">操作</th>
                    <th data-options="field:'ID'">ID</th>
                    <th data-options="field:'Name'">型号</th>
                    <th data-options="field:'District'">交货地</th>
                    <th data-options="field:'Currency'">币种</th>
                    <th data-options="field:'Price'">单价</th>
                    <th data-options="field:'Quantity'">数量</th>
                    <th data-options="field:'Total'">小计</th>
                    <th data-options="field:'Supplier'">供应商</th>
                    <th data-options="field:'Manufacturer'">品牌</th>
                    <th data-options="field:'Leadtime'">货期(工作日)</th>
                    <th data-options="field:'Summary'">摘要</th>
                    <th data-options="field:'CreateDate'">创建时间</th>
                </tr>
            </thead>
        </table>
        <div id="option-btns" style="display: none;">
            <button style="cursor: pointer;" class="option-btn-edit" v_name="option-btn-edit" v_title="列表项按钮-编辑">编辑</button>
        </div>
        <div id="tbtool" style="height: auto;">
            <a href="javascript:void(0)" v_name="option-btn-add" v_title="按钮-新增" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="add()">新增</a>
            <a href="javascript:void(0)" v_name="option-btn-remove" v_title="按钮-刷新" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="reload()">刷新</a>
        </div>
    </div>
</body>
</html>
