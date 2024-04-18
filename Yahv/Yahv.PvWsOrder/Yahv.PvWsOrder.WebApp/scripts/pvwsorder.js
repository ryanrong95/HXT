//通用的调用接口获取数据的方法
function getDataFun(url, data, ProcessFun, error) {
    $.ajax({
        url: url,
        type: 'get',
        data: data,
        dataType: 'json',
        success: function (data) {
            if (data.code == "100") {
                if (ProcessFun.noDataFun) {
                    ProcessFun.noDataFun(data);
                }
            } else if (data.code == "200") {
                if (ProcessFun.success) {
                    ProcessFun.success(data);
                }
            } else if (data.code == "300") {
                if (ProcessFun.exceptionFun) {
                    ProcessFun.exceptionFun(data);
                }
                console.log("接口异常");
            }
        },
        error: function (errorMsg) {
            console.log(errorMsg);
            if (error) {
                error(errorMsg);
            }
        }
    });
}

//通用的调用接口提交数据的方法
function postDataFun(url, data, ProcessFun, error) {
    $.ajax({
        url: url,
        type: 'post',
        data: data,
        dataType: 'json',
        crossDomain: true,
        success: function (data) {
            if (data.code == "100") {
                if (ProcessFun.noDataFun) {
                    ProcessFun.noDataFun(data);
                }
            } else if (data.code == "200") {
                if (ProcessFun.success) {
                    ProcessFun.success(data);
                }
            } else if (data.code == "300") {
                if (ProcessFun.exceptionFun) {
                    ProcessFun.exceptionFun(data);
                }
                console.log("接口异常");
            }
        },
        error: function (errorMsg) {
            console.log(errorMsg);
            if (error) {
                error(errorMsg);
            }
        }
    });
}