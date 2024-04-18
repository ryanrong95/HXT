<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Allot.aspx.cs" Inherits="WebApp.Plat.Roles.Allot" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分配管理员</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <style>
        .group-item {
            display: block;
            min-width: 100px;
            width: auto;
            float: left;
        }

        .checkbox-inline {
            position: relative;
            display: inline-block;
            padding-left: 20px;
            vertical-align: middle;
            cursor: pointer;
        }

        table td input[type="checkbox"] {
            vertical-align: middle;
            position: relative;
            margin-right: 4px;
        }
    </style>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.menu = '角色管理';
    </script>
    <script>
        $(function () {
            $("input[type='checkbox']").click(function () {
                var id = $(this).attr('id');
                $.post("?action=allot", { id: '<%=this.CurrentRole.ID%>', adminid: id, ischeck: $(this).is(':checked') }, function () { });
            });
        });
    </script>
</head>
<body>
    <div class="easyui-panel" title="权限设置" style="width: 100%; min-height:449px; height: auto; padding: 10px;">
        <div>
            <table class="liebiao">
                <tr>
                    <th>角色名称</th>
                    <td><%=this.CurrentRole.Name %></td>
                </tr>
                <tr>
                    <th style="width: 100px;">角色内用户</th>
                    <td>
                        <%
                            var inners = (this.Inners as IEnumerable<ShowModel>).ToArray();
                            if (inners != null)
                            {
                                foreach (var item in inners)
                                {
                                    var label = item.UserName;
                                    var names = item.RoleNames.ToList();
                                    names.Remove($"[{this.CurrentRole.Name}]");
                                    var roleName = string.Join(",", names);
                                    if (!string.IsNullOrWhiteSpace(roleName))
                                    {
                                        label = label + $" ({roleName})";
                                    }
                        %>
                        <div class="group-item">
                            <label class="checkbox-inline">
                                <input type="checkbox" name="allots" checked="checked" id="<%=item.ID %>" /><%=label %>
                            </label>
                        </div>
                        <%
                                }
                            }
                        %>
                        <div style="clear:both;"></div>
                    </td>
                </tr>
                <tr>
                    <th>角色外用户</th>
                    <td>
                        <%
                            var outers = (this.Outers as IEnumerable<ShowModel>).ToArray();
                            if (outers != null)
                            {
                                foreach (var item in outers)
                                {
                                    var label = item.UserName;
                                    var roleName = string.Join(",", item.RoleNames);
                                    if (!string.IsNullOrWhiteSpace(roleName))
                                    {
                                        label = label + $"({roleName})";
                                    }
                        %>
                        <div class="group-item">
                            <label class="checkbox-inline">
                                <input type="checkbox" name="allots" id="<%=item.ID %>" /><%=label %>
                            </label>
                        </div>
                        <%
                                }
                            }
                        %>
                       
                        <div style="clear:both;"></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</body>
</html>
