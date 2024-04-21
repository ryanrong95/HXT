<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Offtime.Add" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        var ID = getQueryString("ID");
        var AdminID = getQueryString("AdminID");
        var BusinessTrip = <%=Yahv.Underly.Enums.LeaveType.BusinessTrip.GetHashCode()%>;
        var OfficialBusiness = <%=Yahv.Underly.Enums.LeaveType.OfficialBusiness.GetHashCode()%>;
        var OtherBusiness = <%=BusinessTripReason.Others.GetHashCode()%>;

        $(function () {
            InitStaff();
            $("#Department").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            $("#Manager").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.ManageData,
            })
            $("#Type").combobox({
                required: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulePrivateType,
                onChange: function () {
                    var value = $("#Type").combobox("getValue");
                    if(value == OfficialBusiness||value==BusinessTrip){
                        $(".normal").css("display","none")
                        $(".tolerance").css("display","table-row")
                        $(".tabs-last").css("display","block")
                    }
                    else{
                        $(".normal").css("display","table-row")
                        $(".tolerance").css("display","none")
                        $(".tabs-last").css("display","none")
                    }
                }
            })
            $("#SwapStaff").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#BusinessReason").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.BusinessTripReason,
                onChange:function(){
                    var business =  $("#BusinessReason").combobox('getValue');
                    if(business==OtherBusiness ){
                        $("#schedule").datagrid("hideColumn", "CompanyName");
                        $("#schedule").datagrid("hideColumn", "Person");
                        $("#schedule").datagrid("hideColumn", "Department");
                        $("#schedule").datagrid("hideColumn", "Position");
                        $("#schedule").datagrid("hideColumn", "Phone");
                    }
                    else{  
                        $("#schedule").datagrid("showColumn", "CompanyName");
                        $("#schedule").datagrid("showColumn", "Person");
                        $("#schedule").datagrid("showColumn", "Department");
                        $("#schedule").datagrid("showColumn", "Position");
                        $("#schedule").datagrid("showColumn", "Phone");
                    }
                    loadData2();
                }
            })
            $("#LoanOrNot").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.LoanOrNot,
            })
            //请假日期
            $("#dg").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadDate',
                onClickRow: onClickRow,
                columns: [[
                    { field: 'Date', title: '请假日期', width: 150, align: 'center' },
                    {
                        field: 'Type', title: '请假时长', width: 150, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < model.DateType.length; i++) {
                                if (model.DateType[i].Value == value) {
                                    return model.DateType[i].Text;
                                }
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: model.DateType, valueField: "Value", textField: "Text", required: true, hasDownArrow: true } }
                    },
                    { field: 'Btn', title: '操作', width: 150, align: 'center', formatter: OperationDate }
                ]],
            });
            //行程安排
            $("#schedule").myDatagrid({
                toolbar: '#scheduletopper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadDate',
                onClickRow: onClickRow2,
                columns: [[
                    { field: 'StartDate', title: '出发日期', width: 120, align: 'center', editor: { type: 'datebox' ,options: { required: true}} },
                    { field: 'StartPlace', title: '出发地点', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]',required: true} } },
                    { field: 'EndDate', title: '到达日期', width: 120, align: 'center', editor: { type: 'datebox' ,options: { required: true}} },
                    { field: 'EndPlace', title: '到达地点', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' ,required: true} } },
                    { field: 'Vehicle', title: '交通工具', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'VehicleCost', title: '预计交通花费', width: 80, align: 'center', editor: { type: 'numberbox', options: { min: 0, precision: 2, } } },
                    { field: 'StayDay', title: '住宿天数', width: 80, align: 'center', editor: { type: 'numberbox', options: { min: 0, precision: 0, } } },
                    { field: 'CompanyName', title: '客户（供应商）名称', width: 120, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Person', title: '拜访人员', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Department', title: '部门名称', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Position', title: '岗位名称', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Phone', title: '联系电话', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,11]' } } },
                    { field: 'BusinessReason', title: '出差事由简述', width: 180, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Btn', title: '操作', width: 80, align: 'center', formatter: OperationSchedule }
                ]],
            });
            //文件信息
            $('#file').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadFile',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationFile }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".file");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            $('#uploadFile').filebox({
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传',
                buttonIcon: 'icon-yg-add',
                width: 58,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadFile', bl, fileName);
                                //上传文件
                                UploadFile(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
                            //上传文件
                            UploadFile(formData);
                        }
                    }
                }
            })
            //添加日期
            $("#btnAddDate").click(function () {
                AddDates();
            })
            //清除日期
            $("#btnClearDate").click(function () {
                var rows = $("#dg").myDatagrid("getRows");
                if (rows.length > 0) {
                    for (var i = rows.length - 1; i >= 0; i--) {
                        $('#dg').datagrid('deleteRow', i);
                    }
                }
            })
            //添加行程
            $("#btnAddSchedule").click(function () {
                AddSchedule();
            })
            //清除行程
            $("#btnClearSchedule").click(function () {
                var rows = $("#schedule").myDatagrid("getRows");
                if (rows.length > 0) {
                    for (var i = rows.length - 1; i >= 0; i--) {
                        $('#schedule').datagrid('deleteRow', i);
                    }
                }
            })
            //提交
            $("#btnSubmit").click(function () {
                if(!Validation()){
                    return;
                }
                endEditing();
                endEditing2();
                var data = new FormData();
                //基本信息
                data.append('Staff', $("#Staff").combobox("getValue"));
                data.append('StaffCode', $("#StaffCode").textbox("getValue"));
                data.append('Manager', $("#Manager").combobox("getValue"));
                data.append('ManagerName', $("#Manager").combobox("getText"));
                data.append('SwapStaff', $("#SwapStaff").combobox("getValue"));
                data.append('WorkContext', $("#WorkContext").textbox("getValue"));
                data.append('Type', $("#Type").combobox("getValue"));
                data.append('Reason', $("#Reason").textbox("getValue"));
                data.append('BusinessReason', $("#BusinessReason").combobox("getValue"));
                data.append('Entourage', $("#Entourage").textbox("getValue"));
                data.append('LoanOrNot', $("#LoanOrNot").combobox("getValue"));
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                //请假日期
                var date = $('#dg').datagrid('getRows');
                var dates = [];
                for (var i = 0; i < date.length; i++) {
                    dates.push(date[i]);
                }
                data.append('dates', JSON.stringify(dates));
                //行程安排
                var schedule = $('#schedule').datagrid('getRows');
                var schedules = [];
                for (var i = 0; i < schedule.length; i++) {
                    schedules.push(schedule[i]);
                }
                data.append('schedules', JSON.stringify(schedules));

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //保存草稿
            $("#btnSave").click(function () {
                if(!Validation()){
                    return;
                }
                endEditing();
                endEditing2();
                var data = new FormData();
                //基本信息
                data.append('Staff', $("#Staff").combobox("getValue"));
                data.append('StaffCode', $("#StaffCode").textbox("getValue"));
                data.append('Manager', $("#Manager").combobox("getValue"));
                data.append('ManagerName', $("#Manager").combobox("getText"));
                data.append('SwapStaff', $("#SwapStaff").combobox("getValue"));
                data.append('WorkContext', $("#WorkContext").textbox("getValue"));
                data.append('Type', $("#Type").combobox("getValue"));
                data.append('Reason', $("#Reason").textbox("getValue"));
                data.append('BusinessReason', $("#BusinessReason").combobox("getValue"));
                data.append('Entourage', $("#Entourage").textbox("getValue"));
                data.append('LoanOrNot', $("#LoanOrNot").combobox("getValue"));
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                //请假日期
                var date = $('#dg').datagrid('getRows');
                var dates = [];
                for (var i = 0; i < date.length; i++) {
                    dates.push(date[i]);
                }
                data.append('dates', JSON.stringify(dates));
                //行程安排
                var schedule = $('#schedule').datagrid('getRows');
                var schedules = [];
                for (var i = 0; i < schedule.length; i++) {
                    schedules.push(schedule[i]);
                }
                data.append('schedules', JSON.stringify(schedules));

                ajaxLoading();
                $.ajax({
                    url: '?action=SaveDraft',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化申请员工
        function InitStaff() {
            if (CheckIsNullOrEmpty(AdminID)) {
                $("#Staff").combobox({
                    required: true,
                    disabled: true,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        var name = $("#Staff").combobox('getValue');
                        $.post('?action=StaffChange', { Name: name }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $("#Department").combobox('setValue', result.data.department);
                                $("#Manager").combobox('setValue', result.data.manager);
                                $("#StaffCode").textbox('setValue', result.data.code)
                                $("#yeardays").text(result.data.YearsDay);
                                $("#offdays").text(result.data.OffDay);
                            }
                            else
                            {
                                $("#Department").combobox('setValue', "");
                                $("#Manager").combobox('setValue', "");
                                $("#StaffCode").textbox('setValue', "")
                                $("#yeardays").text("");
                                $("#offdays").text("");
                            }
                        })
                    }
                })
            }
            else {
                $("#Staff").combobox({
                    required: true,
                    editable: false,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        var name = $("#Staff").combobox('getValue');
                        $.post('?action=StaffChange', { Name: name }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $("#Department").combobox('setValue', result.data.department);
                                $("#Manager").combobox('setValue', result.data.manager);
                                $("#StaffCode").textbox('setValue', result.data.code)
                                $("#yeardays").text(result.data.YearsDay);
                                $("#offdays").text(result.data.OffDay);
                            }
                            else
                            {
                                $("#Department").combobox('setValue', "");
                                $("#Manager").combobox('setValue', "");
                                $("#StaffCode").textbox('setValue', "")
                                $("#yeardays").text("");
                                $("#offdays").text("");
                            }
                        })
                    }
                })
            }
            $("#Staff").combobox('setValue', AdminID);
        }
        //初始化申请
        function Init() {
            $(".tabs-last").css("display","none");
            if (model.ApplicationData != null) {
                $("#Staff").combobox('setValue', model.ApplicationData.ApplicantID);
                $("#Date").datebox('setValue', model.ApplicationData.Date);
                $("#Reason").textbox('setValue', model.ApplicationData.Reason);
            }
            else {
                $("#btnSave").css("display", "inline-block");
            }
        }
    </script>
    <script>
        //提交验证
        function Validation(){
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return false;
            }
            if($("#Manager").combobox("getText")==""){
                top.$.timeouts.alert({ position: "TC", msg: "负责人不能为空", type: "error" });
                return false;
            }
            if($("#SwapStaff").combobox("getValue")==$("#Staff").combobox("getValue")){
                top.$.timeouts.alert({ position: "TC", msg: "申请人和承接人不能相同", type: "error" });
                return false;
            }
            var date = $('#dg').datagrid('getRows');
            if(date.length==0){
                top.$.timeouts.alert({ position: "TC", msg: "请添加请假日期", type: "error" });
                return false;
            }
            return true;
        }
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Template/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteFile(' + index + ')">删除</a>';
        }
        //上传文件
        function UploadFile(formData) {
            $.ajax({
                url: '?action=UploadFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = eval(res.data);
                        for (var i = 0; i < data.length; i++) {
                            $('#file').datagrid('insertRow', {
                                row: {
                                    ID: data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    FileTypeDec: data[i].FileTypeDec,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#file').datagrid('getData');
                        $('#file').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            })
        }
        //删除文件
        function DeleteFile(index) {
            $('#file').datagrid('deleteRow', index);
            var data = $('#file').datagrid('getData');
            $('#file').datagrid('loadData', data);
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
            else {
                endEditing();
                loadData();
            }
        }
        function OperationDate(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //删除行
        function Delete(index) {
            if (editIndex != undefined) {
                $('#dg').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                editIndex = undefined;
            }
            $('#dg').datagrid('deleteRow', index);
            loadData()
        }
        //重新加载数据，作用：刷新列表操作按钮的样式
        function loadData() {
            var data = $('#dg').datagrid('getData');
            $('#dg').datagrid('loadData', data);
        }
        //自动去重复的重新加载数据
        function uniqueLoadData()
        {
            var data = $('#dg').datagrid('getData');
            //自动去重
            var rows =data.rows;
            var r =[];
            for(var i = 0, l = rows.length; i<l; i++){
                for(var j = i + 1; j < l; j++)
                    if(rows[i].Date == rows[j].Date) j == ++i;
                r.push(rows[i]);
            }
            data.rows = r;
            $('#dg').datagrid('loadData', data);
        }
        //添加日期
        function AddDates() {
            var begin = $("#DateStart").datebox("getValue");
            var end = $("#DateEnd").datebox("getValue");
            if (!(CheckIsNullOrEmpty(begin) && CheckIsNullOrEmpty(end))) {
                $.messager.alert('提示', '请选择日期');
                return;
            }
            var ab = begin.split("-");					
            var ae = end.split("-");					
            var db = new Date();					
            db.setUTCFullYear(ab[0], ab[1] - 1, ab[2]);					
            var de = new Date();					
            de.setUTCFullYear(ae[0], ae[1] - 1, ae[2]);					
            var unixDb = db.getTime();					
            var unixDe = de.getTime();
            if (unixDb>unixDe) {
                $.messager.alert('提示', '开始时间不能大于结束时间');
                return;
            }	
            //循环添加数据
            for(var k = unixDb; k <= unixDe;) {
                var date = new Date(parseInt(k));
                var year = date.getFullYear();
                var month = date.getMonth()+1;
                var day = date.getDate();
                var strDate = year+"-"+month+"-"+day;
                $('#dg').datagrid('appendRow', {
                    Date: strDate,
                    Type: 0
                });					
                k = k + 24 * 60 * 60 * 1000;					
            }
            uniqueLoadData();
        }
    </script>
    <script>
        var editIndex2 = undefined;
        function endEditing2() {
            if (editIndex2 == undefined) { return true }
            if ($('#schedule').datagrid('validateRow', editIndex2)) {
                $('#schedule').datagrid('endEdit', editIndex2);
                editIndex2 = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow2(index) {
            if (editIndex2 != index) {
                if (endEditing2()) {
                    $('#schedule').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex2 = index;
                } else {
                    $('#schedule').datagrid('selectRow', editIndex2);
                }
            }
            else {
                endEditing2();
                loadData2();
            }
        }
        function OperationSchedule(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete2(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //删除行
        function Delete2(index) {
            if (editIndex2 != undefined) {
                $('#schedule').datagrid('endEdit', editIndex2).datagrid('cancelEdit', editIndex2);
                editIndex2 = undefined;
            }
            $('#schedule').datagrid('deleteRow', index);
            loadData2()
        }
        function loadData2() {
            var data = $('#schedule').datagrid('getData');
            $('#schedule').datagrid('loadData', data);
        }
        //添加行程
        function AddSchedule(){
            if (endEditing2()){
                $('#schedule').datagrid('appendRow', {
                    VehicleCost:0,
                    StayDay:0,
                });
                editIndex2 = $('#schedule').datagrid('getRows').length - 1;
                $('#schedule').datagrid('selectRow', editIndex2).datagrid('beginEdit', editIndex2);
            }
        }
    </script>
    <style>
        .tabs-header .tabs-panels {
            border: none;
        }

        div.datagrid-wrap {
            border: none;
        }

        .tabs-last {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <div id="topper" style="padding: 5px">
                <input id="DateStart" class="easyui-datebox" style="width: 150px;" />
                <label>到</label>
                <input id="DateEnd" class="easyui-datebox" style="width: 150px;" />
                <a id="btnAddDate" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                <a id="btnClearDate" class="easyui-linkbutton" iconcls="icon-yg-clear">清除</a>
            </div>
            <div id="scheduletopper" style="padding: 5px">
                <a id="btnAddSchedule" class="easyui-linkbutton" iconcls="icon-yg-add">添加日程</a>
                <a id="btnClearSchedule" class="easyui-linkbutton" iconcls="icon-yg-clear">清除</a>
            </div>
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">请假员工：</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 350px;" />
                    </td>
                    <td class="lbl">员工编号：</td>
                    <td>
                        <input id="StaffCode" class="easyui-textbox" style="width: 350px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">员工部门：</td>
                    <td>
                        <input id="Department" class="easyui-combobox" style="width: 350px;" />
                    </td>
                    <td class="lbl">部门负责人：</td>
                    <td>
                        <input id="Manager" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">工作交接：</td>
                    <td colspan="3">
                        <input id="SwapStaff" class="easyui-combobox" style="width: 350px;"
                            data-options="prompt:'承接人'" />
                        <div style="width: 350px; padding-top: 5px">
                            <input id="WorkContext" class="easyui-textbox" style="width: 350px; height: 40px"
                                data-options="prompt:'工作内容描述',multiline:true" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">文件信息：</td>
                    <td colspan="3">
                        <div>
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                        </div>
                        <div class="file" style="width: 500px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">请假类型：</td>
                    <td colspan="3">
                        <input id="Type" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr class="normal">
                    <td class="lbl">请假原因：</td>
                    <td colspan="3">
                        <input id="Reason" class="easyui-textbox" style="width: 350px; height: 40px"
                            data-options="multiline:true" />
                    </td>
                </tr>
                <tr class="tolerance" style="display: none">
                    <td class="lbl">出差事由：</td>
                    <td colspan="3">
                        <input id="BusinessReason" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr class="tolerance" style="display: none">
                    <td class="lbl">随行人员：</td>
                    <td>
                        <input id="Entourage" class="easyui-textbox" style="width: 350px;" />
                    </td>
                    <td class="lbl">是否借款：</td>
                    <td>
                        <input id="LoanOrNot" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
            </table>
            <div id="tt" class="easyui-tabs" style="border: none;">
                <div title="请假日期" style="display: none; border: none;">
                    <table id="dg" title="">
                    </table>
                </div>
                <div title="日程安排" style="display: none; border: none">
                    <table id="schedule" title="" style="display: none">
                    </table>
                </div>
            </div>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: left; margin-left: 5px; margin-top: 8px;">
                <span style="color: blue">剩余带薪假期：</span><span id="yeardays" style="color: red"></span><span style="color: blue">天</span>&nbsp&nbsp&nbsp&nbsp
                <span style="color: blue">剩余调休假期：</span><span id="offdays" style="color: red"></span><span style="color: blue">天</span>
            </div>
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save" style="display: none">保存草稿</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%";>
        <img id="viewfileImg" src="" style="position:relative; zoom:100%; cursor:move;" onMouseEnter="mStart();" onMouseOut="mEnd();"/>
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
