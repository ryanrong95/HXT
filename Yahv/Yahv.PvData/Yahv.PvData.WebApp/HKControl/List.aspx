<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" Title="香港管制查询" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.HKControl.List" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>

        var getQuery = function () {
            var params = {
                Model: $('#Model').textbox('getText'),
                Brand: $("#Brand").textbox('getText'),
                isControl: $("#isControl").combobox('getValue')
            };
            return params;
        }
        $(function () {
            window.grid = $('#dg').myDatagrid({
                rownumbers: true,
                pagination: true,
                nowrap: false,
                queryParams: getQuery(),
                toolbar: '#topper'
            });

            $('#btnSearch').click(function () {
                window.grid.myDatagrid('search', getQuery());
                return false;
            });

            $('#btnClear').click(function () {
                location.reload();;
                return false;
            });

            //注册上传产品信息filebox的onChange事件
            //$('#uploadExcel').filebox({
            //    onClickButton: function () {
            //        $('#uploadExcel').filebox('setValue', '');
            //    },
            //    onChange: function (e) {
            //        //if ($('#uploadExcel').filebox('getValue') == '') {
            //        //    return;
            //        //}
            //        // var files = $("#uploadExcel").filebox('getValue');
            //        // MaskUtil.mask();
            //        var formData = new FormData();
            //        formData.append('uploadExcel', $("#uploadExcel").filebox("files")[0]);
            //        $.ajax({
            //            url: '?action=ImportHK',
            //            type: 'POST',
            //            data: formData,
            //            dataType: 'JSON',
            //            cache: false,
            //            processData: false,
            //            contentType: false,
            //            success: function (res) {
            //                debugger
            //                alert(1111)
            //                // MaskUtil.unmask();
            //                $.messager.alert('提示', res.message, 'info', function () {
            //                    $('#dg').myDatagrid('reload');
            //                });
            //            }
            //        }).done(function (res) {

            //        });
            //    }
            //});

        })



    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div id="topper">
        <table class="liebiao-compact">
            <tbody>
                <tr>
                    <td style="width: 90px">产品型号:</td>
                    <td>
                        <input id="Model" class="easyui-textbox" />
                    </td>

                    <td style="width: 90px">品牌:</td>
                    <td>
                        <input id="Brand" class="easyui-textbox" />
                    </td>
                    <td style="width: 90px">是否管制:</td>
                    <td>
                        <select id="isControl" class="easyui-combobox" name="isControl" style="width: 200px;">
                            <option value="-100">全部</option>
                            <option value="1">是</option>
                            <option value="0">否</option>

                        </select>
                    </td>
                </tr>
                <tr>

                    <td colspan="8">

                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <%-- <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 103px; height: 26px;"
                            data-options="region:'center',buttonText:'数据导入',buttonIcon:'icon-add',
                                                            accept:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'" />--%>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Model',align:'center'" style="width: 20%">型号</th>
                <th data-options="field:'Brand',align:'center'" style="width: 20%">品牌</th>
                <th data-options="field:'isControl',align:'center'" style="width: 7%">是否管制</th>
                <th data-options="field:'Type',align:'center'" style="width: 7%">类别代码</th>
                <th data-options="field:'Description',align:'center'" style="width: 40%">产品说明描述</th>
                <th data-options="field:'UpdateDate',align:'center'" style="width: 6%">更新时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>
