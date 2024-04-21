<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Manufacturers.List" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.css" rel="stylesheet">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/languages/zh-CN.js"></script>
    <script>

        $(function () {
            var tt = document.getElementById('tt');
            var settings = {
                licenseKey: 'non-commercial-and-evaluation',
                data: model,
                colHeaders: Header(),
                columns: [
                            {
                                data: 'Name',
                                type: 'text',
                                options: {
                                    required: true,
                                    missingMessage: '品牌名称必填!',

                                }
                            },
                            {
                                data: 'Agent',
                                type: 'checkbox'
                            }
                ],
                rowHeaders: true,
                // colHeaders: ['品牌名称', '是否代理品牌'],//当值为true时显示列头，当值为数组时，列头为数组的值
                manualColumnMove: true,//true/false 当值为true时，列可拖拽移动到指定列
                manualColumnResize: true,//true/false//当值为true时，允许拖动，当为false时禁止拖动
                autoColumnSize: true,//true/false //当值为true且列宽未设置时，自适应列大小
                minRows: 8,//最小行数
                minSpareRows: 2,//最小行空间，不足则添加空行
                filters: true,
                colWidths: [200, 100],
                contextMenu: ['row_above', 'row_below', '---------', 'remove_row', '---------', 'undo', 'redo', '---------', '---------', 'alignment'],
                language: 'zh-CN',
                beforeRemoveRow: function (index, amount) {
                    var names = [];
                    //封装id成array传入后台
                    if (amount != 0) {
                        for (var i = index; i < amount + index; i++) {
                            var rowdata = ht.getDataAtRow(i);
                            names.push(rowdata[0]);
                        }
                        delExpressCount(names);
                    }
                },
            }
            var ht = new Handsontable(tt, settings);
            $('#btnSave').click(function () {
                var vaildlines = []; // 必填项缺失项
                var source = []; // 提交的数据
                var count = ht.countSourceRows();
                var rows = count - settings.minSpareRows; // 总行-多余行

                for (var i = 0; i < rows; i++) {
                    var linenum = i + 1;
                    // 空行跳过
                    if (ht.isEmptyRow(i)) {
                        continue;
                    }
                    var rowdata = ht.getDataAtRow(i);

                    // 验证必填项
                    if ($.trim(rowdata[0]) == "") {
                        vaildlines.push("第" + linenum + "行，品牌名称必填！");
                    }
                    if ($.trim(rowdata[1]) == "") {
                        rowdata[1] = "FALSE";
                    }
                    else {
                        if ($.trim(rowdata[1]) != "") {
                            var ty = 0;
                            if (rowdata[1] == "TRUE"
                                || rowdata[1]
                                || rowdata[1] == "FALSE"
                                || !rowdata[1]
                                )
                            {
                                ty = 1;
                            }
                            if (ty != 1) {
                                vaildlines.push("第" + linenum + "行，是否代理：必须选中或未选中，粘贴列中值必须为TRUE或FALSE");
                            }
                        }
                    }

                    var obj = {
                        Name: rowdata[0],
                        Agent: rowdata[1],
                    };
                    source.push(obj);
                }
                if (vaildlines.length > 0) {
                    $('#ptf').show();
                    var cou = '请检查列表必填项目：<br />';
                    for (var i = 0; i < vaildlines.length; i++) {
                        cou = cou + vaildlines[i] + '<br />';
                    }
                    $('.vaildtxt').html(cou);
                }
                else {
                    $('.vaildtxt').html('');
                    $('#ptf').hide();
                    if (source.length > 0) {
                        $.post('?action=submit', {
                            supplierid: $('#supplierid').val(),
                            source: JSON.stringify(source),
                        }, function (result) {
                            var data = eval(result);
                            if (data.codes == 200) {
                                //$.messager.alert("提示", "提交成功", "info");
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "提交成功!",
                                    type: "success"
                                });
                                $('#ptf').hide();
                            }
                            else if (data.codes == 600) {
                                $('.resultview').show();
                                $('.errorlines').html(data.message);
                            }
                            else {
                                $.messager.alert("提示", "提交失败 " + data.message, "error");
                            }
                            //$('.sumbitBtn').linkbutton('enable');
                        });
                    }

                }

            });

        })
        function delExpressCount(names) {
            //后台删除
        }
        function Header() {
            var title = [
                '品牌名称<font style="color: red; ">*</font>',
                '是否代理<font style="color: red; ">*</font>']
            return title;
        }
        //function Add() {
        //    $.myWindow({
        //        title: "添加品牌",
        //        url: 'Edit.aspx',
        //        onClose: function () {
        //            hansontable();
        //        },
        //        width: '702',
        //        height: '595',
        //    });
        //    return false;
        //}

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div>
        <h2>说明：</h2>
        <p>1.可编辑每列；可从excel复制粘贴。</p>
        <p>2.删除一行数据，可点击行头全选本行后，键盘摁下后退（Backspace）键。 </p>
        <p style="color: red;">3.* 为必填项</p>
        <p style="color: red;">4.是否代理：可从excel粘贴值为：TRUE可以替换、FALSE不可替换！</p>
    </div>
    <%--<input type="button" value="批量添加" class="easyui-linkbutton" style="width: 120px; height: 30px; margin: 10px 0px;" onclick="Add()" />--%>
    <div id="tt"></div>
    <div style="width: 1px; height: 5px;"></div>

    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>

    <div id="ptf" style="width: 700px; height: auto; padding: 10px;">
        <div class="vaildtxt" style="color: red;"></div>
    </div>


    <div class="resultview" style="display: none;">
        <p>提交结果：</p>
        <p style="color: red;">失败行号：<span class="errorlines"></span> 请检查修正后重新提交。</p>
        <div style="width: 1px; height: 10px;"></div>

    </div>

    <%--<input type="button" value="提交" class="easyui-linkbutton sumbitBtn" style="width: 120px; height: 30px; margin: 10px 0px;" data-options="iconCls:'icon-yg-disabled'" />--%>
</asp:Content>
