<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Init.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Dates.Init" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <meta http-equiv="Access-Control-Allow-Origin" content="*" />

    <script>
        $(function () {
            //年份
            $('#year').combobox({
                data: model.Years,
                textField: "text",
                valueField: "value",
                onSelect: function (record) {
                    $("#new_years_day").textbox("setValue", '');
                    $("#spring_festival").textbox("setValue", '');
                    $("#tomb_sweeping_day").textbox("setValue", '');
                    $("#labour_day").textbox("setValue", '');
                    $("#dragon_boat_festival").textbox("setValue", '');
                    $("#mid_autumn_festival").textbox("setValue", '');
                    $("#national_day").textbox("setValue", '');
                    $("#working_day").textbox("setValue", '');


                    //根据百度接口，获取节假日数据
                    $.get("?action=getHolidays&year=" + record.value, function (data) {
                        var date = JSON.parse(data);
                        var array = [];     //去重（例如：中秋包含在国庆时，中秋不再添加）

                        //判断是否有数据
                        if (date.data[0].holiday) {

                            //此处必须倒序 该接口中秋和国庆有可能重复
                            for (var i = date.data[0].holiday.length - 1; i >= 0 ; i--) {
                                var value = date.data[0].holiday[i];

                                if (value.list) {
                                    $.each(value.list, function (i, v) {
                                        if (array.indexOf(v.date) >= 0) {
                                            return true;
                                        }

                                        if (v.status == '1') {
                                            if (value.name == '元旦') {
                                                var date = $('#new_years_day').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#new_years_day").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '除夕' || value.name == '春节') {
                                                var date = $('#spring_festival').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#spring_festival").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '清明节') {
                                                var date = $('#tomb_sweeping_day').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#tomb_sweeping_day").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '劳动节') {
                                                var date = $('#labour_day').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#labour_day").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '端午节') {
                                                var date = $('#dragon_boat_festival').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#dragon_boat_festival").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '国庆节') {
                                                var date = $('#national_day').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#national_day").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                            else if (value.name == '中秋节') {
                                                var date = $('#mid_autumn_festival').textbox('getValue').split(',');
                                                date.push(v.date);
                                                $("#mid_autumn_festival").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                            }
                                        } else {
                                            var date = $('#working_day').textbox('getValue').split(',');
                                            date.push(v.date);
                                            $("#working_day").textbox("setValue", date.filter(s => $.trim(s).length > 0).join(','));
                                        }

                                        array.push(v.date);
                                    });
                                }
                            }
                        }
                    });
                }

            });

            //班别
            $('#ShiftID').combobox({
                data: model.Schedulings,
                textField: 'text',
                valueField: 'value',
                required: true
            });

            //区域
            $('#RegionID').combobox({
                data: model.Regions,
                textField: 'text',
                valueField: 'value',
                required: true
            });

            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
        });
    </script>
    <script>
        function Init() {
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }
            ajaxLoading();
            var data = new FormData($('#form1')[0]);
            $.post({
                url: '?action=Submit',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    ajaxLoadEnd();
                    console.log(data);
                    if (data.success) {
                        $.messager.alert('操作提示', '初始化成功!', 'info');
                    } else {
                        $.messager.alert('操作提示', data.data, 'error');
                    }
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td style="width: 200px;">年份</td>
                    <td>
                        <input id="year" name="year" class="easyui-combobox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>元旦</td>
                    <td>
                        <input id="new_years_day" name="new_years_day" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>春节</td>
                    <td>
                        <input id="spring_festival" name="spring_festival" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>清明节</td>
                    <td>
                        <input id="tomb_sweeping_day" name="tomb_sweeping_day" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>劳动节</td>
                    <td>
                        <input id="labour_day" name="labour_day" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>端午</td>
                    <td>
                        <input id="dragon_boat_festival" name="dragon_boat_festival" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>中秋</td>
                    <td>
                        <input id="mid_autumn_festival" name="mid_autumn_festival" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>国庆节</td>
                    <td>
                        <input id="national_day" name="national_day" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>调休工作日</td>
                    <td>
                        <input id="working_day" name="working_day" class="easyui-textbox" style="width: 80%;" />
                    </td>
                </tr>
                <tr>
                    <td>区域</td>
                    <td>
                        <input id="RegionID" name="RegionID" class="easyui-combobox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>班别</td>
                    <td>
                        <input id="ShiftID" name="ShiftID" class="easyui-combobox" style="width: 200px;" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btn_Init" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Init()">初始化</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
