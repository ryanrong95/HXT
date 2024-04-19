<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Vrs.Invoices.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '供应商管理.';
        gvSettings.menu = '发票管理';
        gvSettings.summary = '';
    </script>
    <script>
        var getAjaxData = function () {
            var data = {
                action: 'data'
            };
            return data;
        };
        $(function () {
            $('#t_Invoice').bvgrid({ queryParams: getAjaxData() });
            $('.btn_search').click(function () {
                var param = {};
                $('.search [name]').each(function () {
                    var value = $(this).val();
                    if (!!value) {
                        param[$(this).attr('name')] = value;
                    }
                });
                $('#t_Invoice').bvgrid('search', param);
            });
            $('.btn_clear').click(function () {
                location.replace('?');
            });
        })
        var btnformatter = function (val, rec) {
            var arry = [];
            arry.push('<button style="cursor:pointer;" onclick="edit(\'' + rec.ID + '\');">编辑</button>'
                , '<button style="cursor:pointer;" onclick="del(\'' + rec.ID + '\');">删除</button>')
            return arry.join('|');
        };
        var requiredformatter = function (val, rec) {
            return rec.Required ? "是" : "否";
        }
        var invoiceadd = function () {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, '/Edit.aspx'),
                onClose: function () {
                    $("#t_Invoice").bvgrid('reload');
                },
                width: '600px',
                height: '500px'
            }).open();
        };
        var edit = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, '/Edit.aspx') + "?id=" + id,
                onClose: function () {
                    $("#t_Invoice").bvgrid('reload');
                },
                width: '600px',
                height: '500px'
            }).open();
        };
        var del = function (id) {
            $.messager.confirm('删除提示', '确定要删除吗?', function (r) {
                if (r) {
                    $.post("?action=InvoiceAbandon", { id: id }, function (data) {
                        if (data) {
                            $.messager.alert('提示', '删除成功');
                            $("#t_Invoice").bvgrid('reload');
                        }
                    })
                }
            }
            )
        };
    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="发票管理">
        <div data-options="region:'north'" style="height: 80px;" title="搜索">
            <table class="liebiao search">
                <tr>
                    <td style="width: 10%">公司名称</td>
                    <td style="width: 40%">
                        <input class="easyui-textbox" name="txtName" style="width: 80%" data-options="prompt:'输入公司名称前几个字符或全部'" /></td>
                    <td style="width: 10%">发票类型</td>
                    <td style="width: 40%">
                        <select id="_type" name="_type" class="easyui-combobox" style="width: 160px;">
                            <option value="-1">全部</option>
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.InvoiceType)).Cast<NtErp.Vrs.Services.Enums.InvoiceType>())
                                {
                            %>
                            <option value="<%=(int)item %>"><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 5%">联系人</td>
                    <td style="width: 15%">
                        <input class="easyui-textbox" name="txtContact" style="width: 80%" data-options="prompt:'输入法人代表的姓名'" />
                    </td>
                    <td colspan="2">
                        <button type="button" class="btn_search">搜索</button>
                        <button type="button" class="btn_clear">清空</button>
                        <button type="button" onclick="invoiceadd('')">添加发票</button>
                    </td>
                </tr>

            </table>
        </div>
        <table id="t_Invoice" class="easyui-datagrid">
            <thead>
                <tr>
                    <th data-options="field:'Required',width:80,align:'center',formatter:requiredformatter">是否需要</th>
                    <th data-options="field:'Company',width:80,align:'center'">公司</th>
                    <th data-options="field:'Code',width:80,align:'center'">税号</th>
                    <th data-options="field:'Type',width:80,align:'center'">发票类型</th>
                    <th data-options="field:'Contact',width:50,align:'center'">联系人</th>
                    <th data-options="field:'Address',width:150,align:'center'">公司地址</th>
                    <th data-options="field:'Postzip',width:50,align:'center'">邮编</th>
                    <th data-options="field:'Bank',width:150,align:'center'">开户行</th>
                    <th data-options="field:'BankAddress',width:150,align:'center'">开户行地址</th>
                    <th data-options="field:'Account',width:130,align:'center'">银行账号</th>
                    <th data-options="field:'SwiftCode',width:60,align:'center'">银行编码</th>
                    <th data-options="field:'btn',width:80,align:'center',formatter:btnformatter">操作</th>
                </tr>
            </thead>
        </table>

    </div>
</body>
</html>
