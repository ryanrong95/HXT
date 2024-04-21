<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_ns.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Roles.Edit" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        var genSelects = function (rows) { // 生成选择项目
            var name = 'particles_' + current;
            var hidden = $('input[name="' + name + '"]');

            if (hidden.length == 0) {
                hidden = $('<input type="hidden" name="' + name + '" />');
                $('form').append(hidden);
            }

            //var id_selected = $.map($(element).myDatagrid('getChecked'), function (item) {
            //    return item.ID;
            //});
            ////获取没有被选中的，就是我们要隐藏的
            //var unSelects = $.grep($(element).myDatagrid('getRows'), function (item) {
            //    return $.inArray(item.ID, id_selected) == -1;
            //});
            //$.map(unSelects, function (item) {
            //    //item
            //    return item;
            //});

            var json = JSON.stringify(rows);
            hidden.val(json);
        }

        var isTypeCheck = function (sender, index, name) {
            var checked = $(sender).prop('checked');
            var rows = $.currentDataGrid.myDatagrid('getRows');
            rows[index][name] = checked;
            genSelects(rows);

            //是否需要初始化全选checkbox
            if (!$(sender).data("checkAll")) {
                //说明是列表单个点击，需要初始化
                var $divSender = $(sender).parentsUntil("div[id^=particle_]").parent();
                if ($divSender.length > 0) {
                    initChkAll($divSender);
                }
            } else {
                $(sender).removeData("checkAll");
            }
        };

        var isEditformatter = function (value, row, index) {

            if (row.Type.indexOf('input') < 0) {
                return '';
            }

            if (row.IsEdit) {
                return '<input type="checkbox" checked="checked" onclick="isTypeCheck(this, ' + index + ',\'IsEdit\')" />';
            }
            else {
                return '<input type="checkbox"  onclick="isTypeCheck(this,' + index + ',\'IsEdit\')"/>';
            }
        };

        var isShowformatter = function (value, row, index) {
            if (row.IsShow) {
                return '<input type="checkbox" checked="checked" onclick="isTypeCheck(this, ' + index + ',\'IsShow\')" />';
            }
            else {
                return '<input type="checkbox"  onclick="isTypeCheck(this,' + index + ',\'IsShow\')"/>';
            }
        };

    </script>
    <script>
        //当前选中的菜单树节点
        var current = null;
        $(function () {
            if (model) {
                $('#txtName').textbox('readonly', true);
                $('form').form('load', model);
            }

            var menustree = $('#menustree');
            //点击项目就自动选中
            menustree.tree({
                url: '?action=menustree&id=' + (model ? model.ID : ''),
                method: 'get', animate: true, checkbox: true,
                onLoadSuccess: function () {
                    var roots = menustree.tree("getRoots");
                    if (roots && roots.length) {
                        $.each(roots, function (index, item) {
                            menustree.tree("collapseAll", item.target);
                        });
                    }
                },
                onClick: function (node) {
                    current = node.id;
                    //if (!node.url) {
                    //    $("#tab1").myDatagrid('clear');
                    //    return;
                    //}
                    //$("#tab1").myDatagrid({
                    //    pagination: false,
                    //    singleSelect: false,
                    //    queryParams: { url: node.url.toLowerCase() }
                    //});
                    current = node.id;
                    placeParticle(node);

                    //$("#tab1").myDatagrid('search', { url: node.url.toLowerCase() });
                },
                checkbox: function (node) {
                    if (node.attributes && node.attributes.extension) {
                        return false;
                    }
                    return true;
                }
            });

            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };

            var genSelect1s = function (element) { // 生成选择项目

                var name = 'particles_' + current;
                var hidden = $('input[name="' + name + '"]');

                if (hidden.length == 0) {
                    hidden = $('<input type="hidden" name="' + name + '" />');
                    $('form').append(hidden);
                }

                var id_selected = $.map($(element).myDatagrid('getChecked'), function (item) {
                    return item.ID;
                });

                //获取没有被选中的，就是我们要隐藏的
                var unSelects = $.grep($(element).myDatagrid('getRows'), function (item) {
                    return $.inArray(item.ID, id_selected) == -1;
                });


                $.map(unSelects, function (item) {

                    //item

                    return item;
                });


                var json = JSON.stringify(unSelects);
                //alert(json);
                hidden.val(json);
            }


            var placeParticle = function (options) {

                var id = options.id;
                var url = options.url;
                if (url) {
                    url = url.toLowerCase();
                }

                var sender = $('#particle_' + id);

                if (sender.length == 0) {
                    sender = $('#plate').clone();
                    sender.prop('id', 'particle_' + id);
                    sender.show();
                    $('#plate').after(sender);
                    var table = sender.find('table');

                    sender.data('datagrid', table);

                    table.myDatagrid({
                        pagination: false,
                        //singleSelect: false,
                        checkOnSelect: false,
                        selectOnCheck: false,
                        rownumbers: true,
                        fit: true,
                        nowrap: true,
                        striped: true,
                        queryParams: { url: url, roleid: queryString('id') },
                        onBeforeSelect: function (index, row) {
                            //不select行
                            return false;
                        },
                        onLoadSuccess: function (data) {
                            //if (data.rows && data.rows.length > 0) {
                            //    table.myDatagrid('checkAll');
                            //}

                            //var id_alls = $.map(data.rows, function (item) {
                            //    return item.ID;
                            //});


                            //$.each(data.unSelect, function (index, item) {
                            //    var index = $.inArray(item.ID, id_alls);
                            //    table.myDatagrid('uncheckRow', index);
                            //});

                            //$("#tab1").myDatagrid('uncheckRow', 0);

                            initChkAll(sender);
                        },
                        onCheck: function (rowIndex, rowData) {
                            //genSelects(this);
                        },
                        onUncheck: function (rowIndex, rowData) {
                            //genSelects(this);
                        },
                        onCheckAll: function (rows) {
                            //genSelects(this);
                        },
                        onUncheckAll: function (rows) {
                            //genSelects(this);
                        },

                    });
                }

                //当前的 DataGrid 
                $.currentDataGrid = sender.data('datagrid');

                sender.siblings().hide();
                sender.show();
            };

            $('#btnSubmit').click(function () {
                var nodes = menustree.tree('getChecked', ['checked', 'indeterminate']);
                var arry = $.map(nodes, function (item) {
                    return item.id;
                });
                var hidden = $('<input type="hidden" name="menustree" />');
                hidden.val(arry);
                $('form').append(hidden);

                //hidden = $('<input type="hidden" name="particles" />');
                //hidden.val(JSON.stringify(particles));
                //$('form').append(hidden);

                //var rows = $("#tab1").myDatagrid('getChecked');
                //var selects = $.map(rows, function (item) {
                //    return item.ID;
                //});

                //alert(JSON.stringify(rows));

                //return false;

                return true;
            });

        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphFormNorth" runat="server">

    <div class="easyui-panel" data-options="fit:true,border:false">
        <table class="liebiao">
            <tr>
                <td>名称</td>
                <td>
                    <input id="txtName" name="Name" style="width: 200px;" class="easyui-textbox"
                        data-options="prompt:'',required:true,validType:'length[1,75]',isKeydown:true"
                        particle="Name:'编辑名称',jField:'txtName',Type:'easyui-input'" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFormCenter" runat="server">
    <div class="easyui-panel" title="权限选择" data-options="fit:true,border:false">

        <div class="easyui-layout" data-options="fit:true,border:false">
            <div id="p" data-options="region:'west'" style="width: 30%; padding: 10px">
                <ul id="menustree"></ul>
            </div>
            <div id="dataList" data-options="region:'center'">
                <div id="plate" style="display: none; height: 100%; width: 100%;">
                    <table title="颗粒化配置">
                        <thead>
                            <tr>
                                <%--  <th data-options="field:'ck',checkbox:true">显示</th>--%>
                                <th data-options="field:'Name',width:150">名称</th>
                                <th data-options="field:'Type',width:80">类型</th>
                                <th data-options="field:'IsShow',formatter:isShowformatter,width:80">
                                    <input type="checkbox" name="chkAll" onclick="checkAll(this)" />显示</th>
                                <th data-options="field:'IsEdit',formatter:isEditformatter,width:80">编辑</th>

                            </tr>
                        </thead>
                    </table>
                </div>

            </div>
        </div>

    </div>
    <input runat="server" code="" />
    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
    <input name="hParticle" type="hidden" value="/Erm/Erm/Roles/List.aspx" />

    <script>
        $(function () {
            //$('[easyui-input="true"]').toText();
            var arry = 'easyui-input'.split(',');
            for (var index = 0; index < arry.length; index++) {
                $('[' + arry[index] + ']').toText();
            }
        });

        function initChkAll(sender) {
            var $checkbox = sender.find(":checkbox[name!=chkAll]");
            if ($checkbox.length > 0) {
                var checkedLength = $checkbox.filter(":checked").length;
                sender.find(":checkbox[name=chkAll]").prop("checked", checkedLength == $checkbox.length);
            }
        }

        function checkAll(obj) {
            var $table = $(obj).parentsUntil(".datagrid-header").parent().next(".datagrid-body");
            var $checkbox = $table.find(":checkbox[name!=chkAll]");
            $.each($checkbox, function (index, item) {
                $(item)
                    .prop("checked", !$(obj).prop("checked"))
            });
            $.each($checkbox, function (index, item) {
                $(item)
                    .data("checkAll", "1")
                    .click();
            });
        }
    </script>
</asp:Content>
