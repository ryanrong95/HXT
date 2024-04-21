<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.PartNumbers.Edit" %>

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
                                    missingMessage: '型号必填!',

                                }
                            },
                            {
                                data: 'Manufacturer',
                                type: 'text'
                            }
                ],
                rowHeaders: true,
                manualColumnMove: true,//true/false 当值为true时，列可拖拽移动到指定列
                manualColumnResize: true,//true/false//当值为true时，允许拖动，当为false时禁止拖动
                autoColumnSize: true,//true/false //当值为true且列宽未设置时，自适应列大小
                minRows: 17,//最小行数
                minSpareRows: 2,//最小行空间，不足则添加空行
                filters: true,
                colWidths: [300, 100],
                contextMenu: true,
                contextMenu: ['row_above', 'row_below', '---------', '---------', 'undo', 'redo', '---------', '---------', 'alignment'],
                //--'remove_row',
                language: 'zh-CN',
            }
            var ht = new Handsontable(tt, settings);
            $('#btnSave').click(function () {
                var vaildlines = []; // 必填项缺失项
                var source = []; // 提交的数据
                $('.sumbitBtn').linkbutton('disable');
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
                        vaildlines.push("第" + linenum + "行，型号名称必填！");
                    }
                    var obj = {
                        Name: rowdata[0],
                        Manufacturer: rowdata[1]
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
                            //supplierid: $('#supplierid').val(),
                            source: JSON.stringify(source),
                        }, function (result) {
                            var data = eval(result);
                            if (data.codes == 200) {
                                //$.messager.alert("提示", "提交成功", "info");
                                top.$.timeouts.alert({
                                    position: "CC",
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
                            $('.sumbitBtn').linkbutton('enable');
                        });
                    }

                }

            });

        })
        function Header() {
            var title = [
                '型号<font style="color: red; ">*</font>',
                '品牌']
            return title;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding: 10px 60px 20px 60px;">
        <table style="height: 300px; width: 70px">
            <tr>
                <td>
                    <h2>型号列表：</h2>
                    <p>1.以下编辑每列；可手动添加，可从excel复制粘贴。</p>
                    <p style="color: red;">2.* 为必填项</p>

                </td>
            </tr>
            <tr>
                <td>
                    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="tt" style="width: 100%; height: 250px; overflow-y: scroll; position: relative;"></div>
                </td>
            </tr>

            <tr>
                <td>
                    <div style="width: 100%; height: 150px; overflow-y: scroll; ">
                        <div id="ptf" style="width: 700px; height: auto; padding: 10px;">
                            <div class="vaildtxt" style="color: red;"></div>
                        </div>
                        <div class="resultview" style="display: none;">
                            <p>提交结果：</p>
                            <p style="color: red;">失败行号：<span class="errorlines"></span> 请检查修正后重新提交。</p>
                            <div style="width: 1px; height: 10px;"></div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
