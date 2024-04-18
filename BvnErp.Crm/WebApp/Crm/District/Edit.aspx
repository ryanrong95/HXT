<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.District.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>人员分配</title>
    <uc:EasyUI2 runat="server" />
    <style type="text/css">
        .right {
        }

            .right table {
                background: #E0ECFF;
                width: 100%;
            }

            .right td {
                background: #eee;
                text-align: center;
                padding: 2px;
            }

            .right td {
                background: #E0ECFF;
            }

                .right td.drop {
                    background: #fafafa;
                    width: 100px;
                }

                .right td.over {
                    background: #FBEC88;
                }

        .item {
            text-align: center;
            border: 1px solid #499B33;
            background: #fafafa;
            width: 300px;
            margin: 3px;
        }

        .assigned {
            border: 1px solid #BC2A4D;
        }
    </style>
    <script type="text/javascript">

        window.getQueryString = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return "";
        }

        //页面初始化
        $(function () {
            $('.left .item').draggable({
                revert: true,
                proxy: 'clone'
            });
            $('.right .item').draggable({
                revert: true,
                //proxy: 'clone'
            });
            $('.right .drop').droppable({
                onDragEnter: function () {
                    $(this).addClass('over');
                },
                onDragLeave: function () {
                    $(this).removeClass('over');
                },
                onDrop: function (e, source) {
                    //重复判断
                    var current = $(source).prop('id');
                    if ($(this).find('[id="' + current + '"]').length > 0) {
                        return;
                    }

                    //经理设置不能超过3人
                    if (this.children.length == 3 && this.id == "managers") {
                        $.messager.alert('提示', '经理不能设置超过三人！');
                        return;
                    }

                    $(this).removeClass('over');
                    if ($(source).hasClass('assigned')) {
                        $(this).append(source);
                    } else {
                        var c = $(source).clone().addClass('assigned');
                        //$(this).empty();
                        $(this).append(c);
                        c.draggable({
                            revert: true
                        });
                        startMenu(c);
                    }
                }
            });

            $(' .right').bind('contextmenu', function (e) { return false; });

            startMenu('.assigned');
        });


        //加载右击按钮菜单
        function startMenu(context) {
            $(context).bind('contextmenu', function (e) {
                e.preventDefault();

                $('#mm').data('current', $(e.target));

                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY,
                });
            });
        }

        //按钮点击触发事件
        function menuHandler(item) {
            var current = $('#mm').data('current');
            if (item.name == 'cancel') {
                current.remove();
            }
            if (item.name == 'clear') {
                current.parent().empty();
            }
        }

        //保存人员修改
        function Save() {
            //获取经理下所有人员控件
            var managers = document.getElementById("managers").getElementsByTagName("div");
            var members = document.getElementById("members").getElementsByTagName("div");
            var managerids = ""; var memberids = "";
            if (managers.length == 0) {
                messager.alert('提示', '请设置该区域经理！');
                return false;
            }
            if (members.length == 0) {
                messager.alert('提示', '请设置该区域组员！');
                return false;
            }
            //循环获取经理Id
            for (var i = 0; i < managers.length; i++) {
                if (i > 0) {
                    managerids += ",";
                }
                managerids += managers[i].id;
            }
            //循环获取员工Id
            for (var i = 0; i < members.length; i++) {
                if (i > 0) {
                    memberids += ",";
                }
                memberids += members[i].id;
            }
            $("#hidManaids").val(managerids);
            $("#hidMemids").val(memberids);
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <input type="hidden" runat="server" id="hidManaids" />
        <input type="hidden" runat="server" id="hidMemids" />
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top; width: 40%;">
                    <div class="left" style="overflow:auto;height:500px;">
                        <div id="Admins" class="easyui-panel drop" title="现有人员" style="width: 100%;" data-options="fit:true">
                            <%
                                var Admins = this.Model.Admins as IEnumerable<NtErp.Crm.Services.Models.AdminTop>;

                                if (Admins != null)
                                {
                                    foreach (var admin in Admins)
                                    {
                            %>
                            <div class="item" id="<%=admin.ID %>"><%=admin.UserName + admin.RealName %></div>
                            <%
                                    }
                                }
                            %>
                        </div>
                    </div>
                </td>
                <td style="vertical-align: top;">
                    <div class="right">
                        <div id="managers" class="easyui-panel drop" title="经理任命区" style="width: 100%; height: 200px;" data-options="iconCls:'icon-save'">
                            <%
                                var managers = this.Model.Managers as IEnumerable<NtErp.Crm.Services.Models.AdminTop>;

                                if (Admins != null)
                                {
                                    foreach (var manager in managers)
                                    {
                            %>
                            <div class="item assigned" id="<%=manager.ID %>"><%=manager.UserName + manager.RealName %></div>
                            <%
                                    }
                                }
                            %>
                        </div>
                        <div id="members" class="easyui-panel drop" title="组员任命区" style="width: 100%; height: 300px;" data-options="iconCls:'icon-save'">
                            <%
                                var members = this.Model.Members as IEnumerable<NtErp.Crm.Services.Models.AdminTop>;

                                if (Admins != null)
                                {
                                    foreach (var member in members)
                                    {
                            %>
                            <div class="item assigned" id="<%=member.ID %>"><%=member.UserName + member.RealName %></div>
                            <%
                                    }
                                }
                            %>
                        </div>
                    </div>
                </td>
            </tr>
            <tfoot>
                <tr>
                    <td colspan="3" style="align-content:center">
                        <asp:Button runat="server" ID="btnSave" Text="保存" OnClick="btnSave_Click" OnClientClick="return Save()" />
                    </td>
                </tr>
            </tfoot>
        </table>
        <div id="mm" class="easyui-menu" style="width: 120px;" data-options="onClick:menuHandler">
            <div data-options="name:'cancel',iconCls:'icon-cancel'">取消任命</div>
            <div class="menu-sep"></div>
            <div data-options="name:'clear',iconCls:'icon-clear'">清空任命</div>
        </div>
    </form>
</body>
</html>
