<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnUploadList.aspx.cs" Inherits="WebApp.Declaration.Declare.UnUploadList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>上传报关单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />

  <%--  <script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '上传报关单';
        gvSettings.summary = '报关单未上传';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#decheads').myDatagrid({
            });


            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传报关单',
                buttonAlign: 'center',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }
                    var $this = $(this);
                    //验证文件大小
                    if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }
                    //验证文件类型
                    var type = $this.filebox('options').accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var ext = val.substr(val.lastIndexOf(".") + 1);
                    if (type.indexOf(ext.toLowerCase()) < 0) {
                        $this.filebox('setValue', '');
                        $.messager.alert('提示', "请选择" + type + "格式的文件！");
                        return;
                    }
                    MaskUtil.mask();
                    var formData = new FormData($('#form1')[0]);
                    $.ajax({
                        url: '?action=UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message);
                            Search();
                        }
                    });
                }
            });

            $("#uploadFile").next().children("a:first-child").css("border-width", "0").height(26).width(92);

        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="line-height: 30px">
                    <tr>
                        <td colspan="2">
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 90px; height: 26px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">合同号: </td>
                        <td>
                            <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl">订单编号: </td>
                        <td>
                            <input class="easyui-textbox" id="OrderID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>

                    </tr>
                    <tr>
                        <td class="lbl">报关日期: </td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td class="lbl">至</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td>
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="报关单" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 20%;">合同号</th>
                    <th data-options="field:'EntryID',align:'center'" style="width: 10%;">海关编号</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 20%;">订单编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 10%;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 10%;">报关金额</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 20%;">报关日期</th>
                    <th data-options="field:'Status',align:'center'" style="width: 10%;">报关状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
