<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchListDetail.aspx.cs" Inherits="WebApp.Declaration.Notice.SearchListDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            var DeclareDetail = eval('(<%=this.Model.DeclareDetail%>)');
            var OrderDetail = eval('(<%=this.Model.OrderDetail%>)');
            $.each(DeclareDetail, function (index, val) {
                $("#div").append($('#templete').html().replace('id="table"', 'id="' + val.ID + '"').replace('id="CreateTime"', 'id="' + val.ID + 'CreateTime"').replace('id="DeclareNO"', 'id="' + val.ID + 'DeclareNO"').replace('id="DeclarePerson"', 'id="' + val.ID + 'DeclarePerson"'));
                $("#" + val.ID + "CreateTime").html(val.CreateTime.replace('T', ' ').substring(0, 10));
                $("#" + val.ID + "DeclareNO").html(val.ID);
                $("#" + val.ID + "DeclarePerson").html(val.InputerID);
                var result = val.List;
                $('#' + val.ID).datagrid({
                    nowrap: false,
                    data: result,
                    onLoadSuccess: function (data) {
                       
                    }
                });  
            });

            $('#table1').datagrid({
                nowrap: false,
                data: OrderDetail,
                onLoadSuccess: function (data) {

                }
            });
         
             //原始PI列表初始化
            $('#pitable').bvgrid({
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#invoiceList').text('合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $(this).datagrid('getPanel');
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });
                }
            });
        });

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }

        //查看文件
        function View(url) {
            var from = getQueryString('From');
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }
        }
    </script>


    <script type="text/template" id="templete">
        <div style="display: block; margin-left: 10%; margin-top: 5%">

            <table>
                <tr>
                    <td>制单时间:</td>
                    <td>
                        <label id="CreateTime"></label>
                        &nbsp&nbsp
                    </td>
                    <td>报关单号:</td>
                    <td>
                        <label id="DeclareNO"></label>
                        &nbsp&nbsp
                    </td>
                    <td>制单员:</td>
                    <td>
                        <label id="DeclarePerson"></label>
                    </td>
                </tr>
            </table>
            <h3 style="color:red;">封箱信息</h3>
            <table id="table" data-options="fit:false" style="width: 95%;">
                <thead>
                    <tr>
                        <th data-options="field:'CodeTS',align:'center'" style="width: 15%">商品编号</th>
                        <th data-options="field:'GName',align:'left'" style="width: 20%">商品名称</th>
                        <th data-options="field:'GoodsModel',align:'left'" style="width: 20%">型号</th>
                        <th data-options="field:'GQty',align:'center'" style="width: 10%">成交数量</th>
                        <th data-options="field:'DeclPrice',align:'center'" style="width: 10%">单价</th>
                        <th data-options="field:'DeclTotal',align:'center'" style="width: 10%">总价</th>
                        <th data-options="field:'OriginCountry',align:'center'" style="width: 8%">原产地</th>
                        <th data-options="field:'CaseNo',align:'center'" style="width: 10%">箱号</th>
                    </tr>
                </thead>
            </table>
        </div>


       
         <div style="display: block; margin-left: 10%; margin-top: 5%">
              <h3 style="color:red;">订单信息</h3>
            <table id="table1" data-options=" nowrap:false,
            fitColumns:true,
            fit:false,
            singleSelect:true," style="width: 95%;">
                <thead>
                    <tr>
                         <th data-options="field:'Name',align:'left'" style="width: 15px;">品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 22px;">型号</th>
                    <th data-options="field:'Quantity',align:'left'" style="width: 10px;">数量</th>
                    <th data-options="field:'TotalPrice',align:'left'" style="width: 10px;">金额</th>
                    <th data-options="field:'Origin',align:'left'" style="width: 8px;">产地</th>
                    </tr>
                </thead>
            </table>
        </div>

     

    </script>

</head>

<body>
    <form id="form1" runat="server">
        <div id="div">
        </div>
        <div style="display: block; margin-left: 8%; margin-top: 10px;">
            <div id="fileContainer" class="easyui-panel" style="width: 30%; border: none;" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                <div class="sub-container" style="display: block; margin-left: 10%; margin-top: 5%;">
                    <div style="margin-bottom: 5px">
                        <span>
                            <img src="../../App_Themes/xp/images/blue-fujian.png" /></span>
                        <span id="invoiceList" style="font-weight: bold">合同发票(INVOICE LIST)</span>
                    </div>
                    <p id="unUpload" style="display: none">未上传</p>
                    <table id="pitable" data-options="queryParams:{ action: 'dataFiles' }">
                        <thead>
                            <tr>
                                <th data-options="field:'img',formatter:ShowImg">图片</th>
                                <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
             <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
                <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
            </div>
        </div>

    </form>

</body>
</html>
