<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Vrs.Companies.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:easyui runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '供应商管理.';
        gvSettings.menu = '公司管理';
        gvSettings.summary = '';
    </script>
    <script>
        var param = "";
        var getAjaxData = function () {
            var data = {
                action: 'data',
                param: param
            };
            return data;
        };
        var btnformatter = function (val, rec) {
            var arry = [];
            arry.push('<button style="cursor:pointer;" onclick="companyedit(\'' + rec.ID + '\');">编辑</button>'
                , '<button style="cursor:pointer;" onclick="del(\'' + rec.ID + '\');">删除</button>')
            return arry.join('|');
        };
        $(function () {
            $('#t_companies').bvgrid({ queryParams: getAjaxData() });
            $('.btn_clear').click(function () {
                location.replace('?');
            });
            $('.btn_search').click(function () {
                var param = {};
                $('.search [name]').each(function () {
                    var value = $(this).val();
                    if (!!value) {
                        param[$(this).attr('name')] = value;
                    }
                });
                $('#t_companies').bvgrid('search', param);

            });

        });
        var del = function (id) {
            $.messager.confirm('删除提示', '确定要删除吗?', function (r) {
                if (r) {
                    $.post("?action=CompanyAbandon", { id: id }, function (data) {
                        if (data) {
                            $.messager.alert('提示', '删除成功');
                            $("#t_companies").bvgrid('reload');
                        }
                    })
                }
            });
        }
        var companyedit = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/Companies\/List.aspx/ig, '/Companies/Edit.aspx') + '?id=' + id, onClose: function () {
                    $("#t_companies").bvgrid('reload');
                }
            }).open();
        };
    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="公司管理">
        <div data-options="region:'north'" style="height: 80px;" title="搜索">
            <table class="liebiao search">
                <tr>
                    <td style="width: 5%">公司名称</td>
                    <td style="width: 15%">
                        <input class="easyui-textbox" name="txtName" style="width: 80%" data-options="prompt:'输入公司名称前几个字符或全部'" /></td>
                    <td>公司类型</td>
                    <td>
                        <select id="_type" name="_type" class="easyui-combobox" style="width: 160px;">
                            <option value="-1">全部</option>
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.ComapnyType)).Cast<NtErp.Vrs.Services.Enums.ComapnyType>())
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
                    <td style="width: 5%">法人代表</td>
                    <td style="width: 15%">
                        <input class="easyui-textbox" name="txtCorporateRepresentative" style="width: 80%" data-options="prompt:'输入法人代表的姓名'" />
                    </td>
                    <td style="width: 5%">公司地址</td>
                    <td style="width: 15%">
                        <input class="easyui-textbox" name="txtAddress" style="width: 80%" data-options="prompt:'输入公司地址前几个字符或全部'" /></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <button type="button" class="btn_search">搜索</button>
                        <button type="button" class="btn_clear">清空</button>
                        <button type="button" onclick="companyedit('')">添加公司</button>
                    </td>
                </tr>
            </table>
        </div>
        <table id="t_companies" class="easyui-datagrid">
            <thead>
                <tr>
                    <%--<th data-options="field:'ID',width:220,align:'center'">公司ID</th>--%>
                    <th data-options="field:'Name',width:150,align:'center'">公司名称</th>
                    <th data-options="field:'Type',width:150,align:'center'">公司类型</th>
                    <th data-options="field:'Code',width:100,align:'center'">纳税人识别号</th>
                    <th data-options="field:'Address',width:200,align:'center'">公司地址</th>
                    <th data-options="field:'RegisteredCapital',width:80,align:'center'">注册资金</th>
                    <th data-options="field:'CorporateRepresentative',width:80,align:'center'">法人代表</th>
                    <%--<th data-options="field:'Summary',width:80,align:'center'">描述</th>--%>
                    <th data-options="field:'CreateDate',width:150,align:'center'">创建时间</th>
                    <th data-options="field:'UpdateDate',width:150,align:'center'">修改时间</th>
                    <th data-options="field:'Btns',formatter:btnformatter,align:'center',width:200">操作</th>
                </tr>
            </thead>
        </table>

    </div>
</body>
</html>
