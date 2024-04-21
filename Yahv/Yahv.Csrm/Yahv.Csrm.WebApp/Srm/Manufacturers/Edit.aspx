<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Manufacturers.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.css" rel="stylesheet">
    <script>
        $(function () {
            var tt = document.getElementById('tt');
            var settings = {
                data: model,
                colHeaders: eval(<%=GetColNames()%>),
                rowHeaders: true,
                manualColumnMove: true,//true/false 当值为true时，列可拖拽移动到指定列
                manualColumnResize: true,//true/false//当值为true时，允许拖动，当为false时禁止拖动
                autoColumnSize: true,//true/false //当值为true且列宽未设置时，自适应列大小
                minRows: 10,//最小行数
                minSpareRows: 2,//最小行空间，不足则添加空行

            }
            var ht = new Handsontable(tt, settings);

            $('.sumbitBtn').click(function () {
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
                    if ($.trim(rowdata[0]) == "" || $.trim(rowdata[1]) == "") {
                        vaildlines.push(linenum);
                    }

                    var obj = {
                        Name: rowdata[0],
                        Agent: (rowdata[1] == "是") ? true : false,
                    };
                    source.push(obj);
                }
                if (vaildlines.length > 0) {
                    $('.vaildtxt').html('请检查必填项在行号：' + vaildlines.join(','));
                }
                else {
                    $('.vaildtxt').html('');
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
                                $.myWindow.close();
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

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div>
            <h2>说明：</h2>
            <p>1.可编辑每列；可从excel复制粘贴。</p>
            <p>2.删除一行数据，可点击行头全选本行后，键盘摁下后退（Backspace）键。 </p>
            <p style="color: red;">3.* 为必填项</p>
            <p style="color: red;">4.是否代理：只能填是/否，其他默认为否</p>
        </div>
        <div id="tt"></div>
        <div style="width: 1px; height: 5px;"></div>
        <div class="resultview" style="display: none;">
            <p>提交结果：</p>
            <p style="color: red;">失败行号：<span class="errorlines"></span> 请检查修正后重新提交。</p>
            <div style="width: 1px; height: 10px;"></div>
        </div>
        <input type="button" value="提交" class="easyui-linkbutton sumbitBtn" style="width: 120px; height: 30px; margin: 10px 0px;" />
    </div>
</asp:Content>
