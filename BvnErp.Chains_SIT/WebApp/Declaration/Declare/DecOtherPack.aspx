<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecOtherPack.aspx.cs" Inherits="WebApp.Declaration.Declare.DecOtherPack" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var editIndex = undefined;
        var check = false;
        var OtherPacks = getQueryString("OtherPacks");
        var OtherPacksObject = JSON.parse(OtherPacks);
        var PackSource = getQueryString("PackSource");
        $(function () {
            if (PackSource != "Add") {
                $("#dlg-buttons").css("display", "none");
            }

            $('#otherPack').myDatagrid({
                rownumbers: true,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                singleSelect: false,
                checkOnSelect: false,
                loadFilter: function (data) {
                    if (OtherPacks != '' && OtherPacksObject != null) {
                        $.each(OtherPacksObject, function (index, val) {
                            for (var index = 0; index < data.rows.length; index++) {
                                var row = data.rows[index];
                                if (row.PackType == val.PackType) {
                                    row.PackQty = val.PackQty;
                                }
                            }
                        });
                    }
                    return data;
                },
                onBeforeSelect: function (index, row) {
                    if (!check)
                        return false;
                },
                onBeforeUnselect: function (index, row) {
                    if (!check)
                        return false;
                },
                onCheck: function (index, row) {
                    check = true;
                    $('#otherPack').datagrid('selectRow', index);
                    check = false;
                },
                onUncheck: function (index, row) {
                    check = true;
                    $('#otherPack').datagrid('unselectRow', index);
                    check = false;
                },
                onClickRow: function (index) {
                    if (editIndex != index) {
                        if (endEditing()) {
                            $('#otherPack').datagrid('selectRow', index)
                                .datagrid('beginEdit', index);
                            editIndex = index;
                        } else {
                            $('#otherPack').datagrid('selectRow', editIndex);
                        }
                    }
                    check = false;
                },
                onLoadSuccess: function (data) {
                    if (OtherPacks != '' && OtherPacksObject != null) {
                        $.each(OtherPacksObject, function (index, val) {
                            check = true;
                            for (var i = 0; i < data.rows.length; i++) {
                                if (data.rows[i].PackType == val.PackType) {
                                    $('.datagrid-btable').find("input[type='checkbox']")[i].checked = true;
                                    $('#otherPack').datagrid('selectRow', i);
                                }
                            }
                            check = false;
                        });
                    }
                    
                    var p = $(".datagrid-view2");
                    var rows1 = p.find('tr.datagrid-row td[field!=ck][field!=PackQty]');
                    rows1.unbind('click').bind('click', function(e) {
                        return false;
                    });
                    
                    $("#form1").find(".panel-noscroll").height(400);
                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").height(400);
                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").find(".datagrid-view").height(400);
                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(400);
                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(368);

                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view1").height(400);
                    $("#form1").find(".panel-noscroll").find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view1").find(".datagrid-body").height(368);
                }
            });
        });

        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#otherPack').datagrid('validateRow', editIndex)) {
                var ed = $('#otherPack').datagrid('getEditor', { index: editIndex, field: 'PackNo' });
                $('#otherPack').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
    </script>
    <script>
        function Save() {
            var rows = $('#otherPack').datagrid('getSelections');
            if (rows.length <= 0) {
                $.messager.alert('提示', '请勾选！');
                return;
            }
            //校验，输入了件数，没勾选
            var rowsall = $('#otherPack').datagrid('getRows');
            for (var j = 0; j < rowsall.length; j++) {
                if (rowsall[j].PackQty != undefined && rowsall[j].PackQty != "") {
                    var isSelected = $("#otherPack").parent().find("div.datagrid-cell-check").children("input[type='checkbox']").eq(j).is(':checked')
                    if (!isSelected) {
                        $.messager.alert('提示', '请勾选！');
                        return;
                    }
                }
            }
            //校验，勾选了，没输入件数
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].PackQty == undefined || rows[i].PackQty == "") {
                    $.messager.alert('提示', '请输入正确的包装件数！');
                    return;
                }
            }
            var ewindow = $.myWindow.getMyWindow("DecHeadWindow");
            ewindow.$("#OtherPacks").val(JSON.stringify(rows));
            $.myWindow.close();
        }

        function Cancel() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="display: block; margin-left: 20px;">
            <table id="otherPack" style="width: 450px;">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true" style="width: 20px"></th>
                        <th data-options="field:'PackType',align:'center'" style="width: 120px">包装材料种类代码</th>
                        <th data-options="field:'Name',align:'center'" style="width: 220px">包装材料种类名称</th>
                        <th data-options="field:'PackQty',align:'center',editor:'numberbox'" style="width: 90px">包装件数</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div id="dlg-buttons" style="margin-left: 35%; margin-top: 5px">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
            <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
        </div>
    </form>
</body>
</html>
