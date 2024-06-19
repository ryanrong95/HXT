<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Advantages.PartNumbers.List" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.min.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/languages/zh-CN.js"></script>

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.js"></script>
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
                minRows: 10,//最小行数
                minSpareRows: 2,//最小行空间，不足则添加空行
                //stretchH: "last"     //last:延伸最后一列,all:延伸所有列,none默认不延伸。
                colWidths: 130,
                contextMenu: ['row_above', 'row_below', '---------', 'remove_row', '---------', 'undo', 'redo', '---------', '---------', 'alignment'],
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
                        Manufacturer: rowdata[1],
                    };
                    source.push(obj);
                }
                if (vaildlines.length > 0) {
                    $('#pdf').show();
                    var cou = '请检查列表必填项目：<br />';
                    for (var i = 0; i < vaildlines.length; i++) {
                        cou = cou + vaildlines[i] + '<br />';
                    }
                    $('.vaildtxt').html(cou);
                }
                else {
                    $('.vaildtxt').html('');
                    $('#pdf').hide();
                    if (source.length > 0) {
                        $.post('?action=submit', {
                            adminid: $('#adminid').val(),
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
                            }
                            else if (data.codes == 600) {
                                $('#pdf').show();
                                $('.vaildtxt').html(data.message);
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
        <%--$("#btnAdd").click(function ()
            {
                var id='<%=SupplierID %>'
                $.myWindow({
                    title: '优势品牌',
                    url: 'Edit.aspx?id=' + id,
                    onClose: function () {
                        loacation.reload();
                    },
                    width: "50%",
                    height: "80%",
                });
                return false;
            })--%>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false">
        <input type="hidden" id="adminid" value="<%=AdminID %>" />
        <div style="margin: 10px;">
            <h2>说明：</h2>
            <p>1.可编辑每列；可从excel复制粘贴。</p>
            <p>2.删除一行数据，可点击行头全选本行后，键盘摁下后退（Backspace）键。 </p>
            <p style="color: red;">3.* 为必填项</p>
            <p style="color: red;">4.鼠标右击列表时，支持（添加行、删除行等操作）。</p>
        </div>

        <div class="resultview" style="display: none;">
            <p>提交结果：</p>
            <p style="color: red;">失败行号：<span class="errorlines"></span> 请检查修正后重新提交。</p>
        </div>

        <div id="tt"></div>
        <div style="width: 1px; height: 5px;"></div>

        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>
        <div id="ptf" style="width: 700px; height: auto; padding: 10px;">
            <div class="vaildtxt" style="color: red;"></div>
        </div>


        <%--<div style="width: 1px; height: 20px;"></div>--%>
    </div>
</asp:Content>
