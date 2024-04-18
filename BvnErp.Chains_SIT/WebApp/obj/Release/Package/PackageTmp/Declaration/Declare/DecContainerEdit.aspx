<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecContainerEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecContainerEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var ID = getQueryString("DecConID");
            var DeclarationID = getQueryString("DeclarationID");
            var SourceCon = getQueryString("SourceCon");
            var Container = eval('(<%=this.Model.Container%>)');
            var DefaultContainer = eval('(<%=this.Model.DefaultContainer%>)');
            $("#ID").val(ID);
            $("#DeclarationID").val(DeclarationID);
            $("#ContainerMd").combobox({
                data: Container
            });

            if (DefaultContainer != null) {
                $("#ContainerID").textbox("setValue", DefaultContainer.ContainerID);
                $("#ContainerMd").combobox("setValue", DefaultContainer.ContainerMd);
                $("#GoodsNo").textbox("setValue", DefaultContainer.GoodsNo);              
                $("#GoodsContainerWeight").textbox("setValue", DefaultContainer.GoodsContainerWeight);                
                $("input:radio[name='LclFlag'][value='" + DefaultContainer.LclFlag + "']").attr('checked', 'true');
            }

            var AllSelectedGNO = "";
            var SelectedGNO = "";
            $('#orders').myDatagrid({
                fit: false,
                singleSelect: false,
                //autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                nowrap: false,
                onCheck: function (index, row) {
                    SelectedGNO += row.GNo + ",";
                    $("#GoodsNo").textbox("setValue", SelectedGNO);
                },
                onUncheck: function (index, row) {
                    SelectedGNO = SelectedGNO.replace(row.GNo + ",", "");
                    $("#GoodsNo").textbox("setValue", SelectedGNO);
                },
                onSelectAll: function (index, rows) {
                    SelectedGNO = AllSelectedGNO;
                    $("#GoodsNo").textbox("setValue", SelectedGNO);
                },
                onUnselectAll: function (index, rows) {
                    SelectedGNO = "";
                    $("#GoodsNo").textbox("setValue", SelectedGNO);
                },
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; i++) {
                        AllSelectedGNO += data.rows[i].GNo + ",";
                    }
                    if (JSON.stringify(DefaultContainer) != '{}') {
                        if (DefaultContainer.GoodsNo.length > 0) {
                            var goods = DefaultContainer.GoodsNo.split(',');
                            $.each(goods, function (index, val) {
                                for (var i = 0; i < data.rows.length; i++) {
                                    if (data.rows[i].GNo == val) {
                                        $('.datagrid-btable').find("input[type='checkbox']")[i].checked = true;
                                        $('#orders').datagrid('selectRow', i);
                                    }
                                }
                            });
                        }
                    }


                    var heightValue = $("#orders").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#orders").prev().find(".datagrid-body").height(heightValue);
                    $("#orders").prev().height(heightValue);
                    $("#orders").prev().parent().height(heightValue);
                    $("#orders").prev().parent().parent().height(heightValue);


                }
            });

            if (SourceCon == "Search") {
                setDisable();
            } else if (SourceCon == "Add") {
                 $("input:radio[name='LclFlag'][value='1']").attr('checked', 'true');
            } 
        });

        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                //$.messager.alert('提示', '请按提示输入数据！');
                return;
            }

            var weight = $("#GoodsContainerWeight").textbox("getValue");
            if (! /^\d{0,1}(\d{0,18})\.{0,1}(\d{0,5})?$/ig.test(weight)) {
                $("#GoodsContainerWeight").textbox("setValue", 0);
                $.messager.alert('消息', '请输入有效的箱货重量！', 'info');
                return;
            }

            var ContainerID = $("#ContainerID").textbox("getValue");
            var ContainerMd = $("#ContainerMd").combobox("getValue");
            if (ContainerID != "" && ContainerMd != "") {
                var model = {
                    ID: $("#ID").val(),
                    DeclarationID: $("#DeclarationID").val(),
                    ContainerID: $("#ContainerID").textbox("getValue"),
                    ContainerMd: $("#ContainerMd").combobox("getValue"),
                    GoodsNo: $("#GoodsNo").textbox("getValue"),
                    LclFlag: $('input:radio[name="LclFlag"]:checked').val(),
                    GoodsContainerWeight: $("#GoodsContainerWeight").textbox("getValue")
                };

                $.post('?action=Save', model, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function (r) {
                        if (result.success) {
                            ParentSearch();
                            Cancel();
                        } else {

                        }
                    });
                });
            } else {
                $.messager.alert('消息', '请按提示输入数据！', 'info');
            }
        }

        function Cancel() {
            $.myWindow.close();
        }

        function ParentSearch() {
            var ewindow = $.myWindow.getMyWindow("DecContainer2DecContainerEdit");
            ewindow.SearchButton();
        }

        function setDisable() {
            $("#dlg-buttons").css("display", "none");
            $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
            $('input[class*=combobox]').attr('readonly', true).attr('disabled', true);
        }
    </script>
</head>

<body class="easyui-layout">
    <div class="easyui-panel" data-options="border:false,fit:true">
        <div data-options="region:'center',border:false" style="margin: 5px;">
            <table id="orders" title="商品项" data-options="singleSelect:true,fit:false">
                <thead>
                    <tr>
                        <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                        <th data-options="field:'CaseNo',align:'center'" style="width: 80px;">箱号</th>
                        <th data-options="field:'GNo',align:'center'" style="width: 80px;">备案序号</th>
                        <th data-options="field:'CodeTS',align:'left'" style="width: 100px;">商品编码</th>
                        <th data-options="field:'CiqCode',align:'left'" style="width: 100px;">检验检疫编码</th>
                        <th data-options="field:'GName',align:'left'" style="width: 180px;">商品名称</th>
                        <th data-options="field:'GoodsModel',align:'left'" style="width: 150px;">规格型号</th>
                        <th data-options="field:'GQty',align:'center'" style="width: 80px;">成交数量</th>
                        <th data-options="field:'NetWt',align:'center'" style="width: 80px;">净重</th>
                        <th data-options="field:'GrossWt',align:'center'" style="width: 80px;">毛重</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'center',border:false" style="margin: 5px; margin-left: 150px">
            <table style="border-collapse: separate; border-spacing: 0px 10px;">
                <tr>
                    <td class="lbl">集装箱规格：</td>
                    <td>
                        <input class="easyui-combobox" id="ContainerMd" name="ContainerMd"
                            data-options="valueField:'Value',textField:'Text',required:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">集装箱箱号：</td>
                    <td>
                        <input class="easyui-textbox" id="ContainerID" name="ContainerID" data-options="required:true,validType:'length[1,32]'" style="width: 250px;" />
                        <input type="hidden" id="DeclarationID" />
                        <input type="hidden" id="ID" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">商品项：</td>
                    <td>
                        <input class="easyui-textbox" id="GoodsNo" name="GoodsNo" readonly="true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">拼箱标识：</td>
                    <td>
                        <input type="radio" name="LclFlag" value="1" />是
                        <input type="radio" name="LclFlag" value="0" style="margin-left: 10px" />否                       
                    </td>
                </tr>
                <tr>
                    <td class="lbl">箱货重量：</td>
                    <td>
                        <input class="easyui-textbox" id="GoodsContainerWeight" name="GoodsContainerWeight" style="width: 250px;" />
                    </td>
                </tr>
<%--                <tr>
                    <td class="lbl"></td>
                    <td>
                        <input class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>--%>
            </table>
        </div>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false" style="width: 100%; height: 40px; line-height: 40px; text-align: right; padding-right: 40px; background-color: #F3F3F3; vertical-align: bottom;">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
