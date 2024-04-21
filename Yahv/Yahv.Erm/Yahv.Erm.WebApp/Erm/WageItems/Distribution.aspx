<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_ns.Master" AutoEventWireup="true" CodeBehind="Distribution.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WageItems.Distribution" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model) {
                $('#txtName').textbox('readonly', true);
                $('form').form('load', model);
            }

            var current = null;
            var mytree = $('#tree');
            var flag = true;        //第一次加载数据的时，checkRow不需要走onCheck事件

            //点击项目就自动选中
            mytree.tree({
                url: '?action=tree&id=' + (model ? model.ID : '') + '&selectType=' + $("input[type='radio']:checked").val(),
                method: 'get',
                animate: true,
                checkbox: true,
                onClick: function (node) {
                    current = node.id;
                    $("#tbList").show();
                    $("#s_name").textbox("setValue", "");
                    distStaff(node);
                }
            });

            //分配员工
            var distStaff = function (options) {
                $("#tbList").myDatagrid({
                    pagination: false,
                    singleSelect: false,
                    rownumbers: true,
                    fit: true,
                    queryParams: { url: "?action=data", treeId: options.id, treeType: options.attributes.type, sName: $("#s_name").val() },
                    onLoadSuccess: function (data) {
                        //console.log(data);
                        if (data) {
                            flag = false;
                            $.each(data.rows, function (index, item) {
                                if (item.Selected) {
                                    $("#tbList").myDatagrid('checkRow', index);
                                }
                            });
                            flag = true;
                        }
                    },
                    onCheck: function (rowIndex, rowData) {
                        if (flag) {
                            $.post("?action=bindStaffs", { itemId: model.ID, staffIds: rowData.ID }, function (result) {
                                if (result) {
                                    if (result.code == 200) {
                                        //$.messager.alert("操作提示", "操作成功！", "info", function () {
                                        //    reloadTree();
                                        //});
                                        top.$.timeouts.alert({
                                            position: "TC",
                                            msg: "操作成功!",
                                            type: "success"
                                        });
                                        reloadTree();
                                    } else {
                                        $.messager.alert("操作提示", "操作失败！" + result.message, "error");
                                    }
                                }
                            });
                        }
                        //genSelects(this);
                    },
                    onUncheck: function (rowIndex, rowData) {
                        $.post("?action=unbindStaffs", { itemId: model.ID, staffIds: rowData.ID }, function (result) {
                            if (result) {
                                if (result.code == 200) {
                                    //$.messager.alert("操作提示", "操作成功！", "info", function () {
                                    //    reloadTree();
                                    //});
                                    top.$.timeouts.alert({
                                        position: "TC",
                                        msg: "操作成功!",
                                        type: "success"
                                    });
                                    reloadTree();
                                } else {
                                    $.messager.alert("操作提示", "操作失败！" + result.message, "error");
                                }
                            }
                        });
                    },
                    onCheckAll: function (rows) {
                        var ids = "";
                        $.each(rows, function (index, element) {
                            ids += element.ID + ",";
                        });

                        $.post("?action=bindStaffs", { itemId: model.ID, staffIds: ids }, function (result) {
                            if (result) {
                                if (result.code == 200) {
                                    //$.messager.alert("操作提示", "操作成功！", "info", function () {
                                    //    reloadTree();
                                    //});
                                    top.$.timeouts.alert({
                                        position: "TC",
                                        msg: "操作成功!",
                                        type: "success"
                                    });
                                    reloadTree();
                                } else {
                                    $.messager.alert("操作提示", "操作失败！" + result.message, "error");
                                }
                            }
                        });
                    },
                    onUncheckAll: function (rows) {
                        var ids = "";
                        $.each(rows, function (index, element) {
                            ids += element.ID + ",";
                        });

                        $.post("?action=unbindStaffs", { itemId: model.ID, staffIds: ids }, function (result) {
                            if (result) {
                                if (result.code == 200) {
                                    //$.messager.alert("操作提示", "操作成功！", "info", function () {
                                    //    reloadTree();
                                    //});
                                    top.$.timeouts.alert({
                                        position: "TC",
                                        msg: "操作成功!",
                                        type: "success"
                                    });
                                    reloadTree();
                                } else {
                                    $.messager.alert("操作提示", "操作失败！" + result.message, "error");
                                }
                            }
                        });
                    },
                });
            };

            //根据类型切换树
            $("input[name='rdo_type']").change(function () {
                $("#s_name").textbox('setValue', '');

                reloadTree();

                $("#tbList").myDatagrid("clear");
            });

            //搜索
            $("#btnSearch").click(function () {
                $("#tbList").myDatagrid('search', { url: "?action=data", treeId: current, treeType: $("input[type='radio']:checked").val(), sName: $("input[id='s_name']").val() });
            });

            //加载树
            var reloadTree = function () {
                mytree.tree("options").url = '?action=tree&id=' + (model ? model.ID : '') + '&selectType=' + $("input[type='radio']:checked").val();
                mytree.tree('reload');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphFormNorth" runat="server">
    <div class="easyui-panel" data-options="fit:true,border:false">
        <table class="liebiao">
            <tr>
                <td>工资项名称</td>
                <td>
                    <input id="txtName" name="Name" style="width: 200px;" class="easyui-textbox"
                        data-options="prompt:'',required:true,validType:'length[1,75]'" />
                </td>
                <td>选择类型</td>
                <td>
                    <span class="radioSpan">
                        <input type="radio" name="rdo_type" value="2" checked="checked" />考核岗位
                        &nbsp;&nbsp;&nbsp;
                        <input type="radio" name="rdo_type" value="1" />所属区域
                    </span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFormCenter" runat="server">
    <div class="easyui-panel" title="选择" data-options="fit:true,border:false">
        <div class="easyui-layout" data-options="fit:true,border:false">
            <div id="p" data-options="region:'west'" style="width: 30%; padding: 10px">
                <ul id="tree"></ul>
            </div>
            <div data-options="region:'center'">
                <div id="plate" style="height: 98%; width: 100%;">
                    <div id="topper">
                        <!--搜索按钮-->
                        <table class="liebiao" id="tbSearch">
                            <tr>
                                <td style="width: 90px;">名称</td>
                                <td style="width: 120px;">
                                    <input id="s_name" data-options="prompt:'用户名/编码/大赢家编码',validType:'length[1,75]'" class="easyui-textbox" style="width: 200px" />
                                </td>
                                <td>
                                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <table id="tbList" style="display: none;">
                        <thead>
                            <tr>
                                <th data-options="field:'ck',checkbox:true">选择</th>
                                <th data-options="field:'Code',width:100">编码</th>
                                <th data-options="field:'DyjCode',width:150">大赢家编码</th>
                                <th data-options="field:'Name',width:150">名称</th>
                                <th data-options="field:'WorkCity',width:100">所属地区</th>
                                <th data-options="field:'PostionName',width:120">岗位</th>
                                <th data-options="field:'Status',width:100">状态</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>

    </div>
    <input runat="server" code="" />
</asp:Content>
