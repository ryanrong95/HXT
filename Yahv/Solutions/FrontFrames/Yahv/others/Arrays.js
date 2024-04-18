//分组
Array.prototype.groupBy = function (fn) {
    var groups = {};
    var list = this;
    list.forEach(function (o) {
        var group = JSON.stringify(fn(o));
        groups[group] = groups[group] || [];
        groups[group].push(o);
    });
    return groups;
};
//任意相等
Array.prototype.any = function (fn) {
    var list = this;
    for (var index = 0; index < list.length; index++) {
        if (fn(list[index])) {
            return true;
        }
    }
    return false;
};
//条件过滤
Array.prototype.where = function (fn) {
    var list = this;
    var arry = new Array();
    for (var index = 0; index < list.length; index++) {
        if (fn(list[index])) {
            arry.push(list[index]);
        }
    }
    return arry;
};

//排重返回视图
Array.prototype.distinct = function (fn) {
    var arry = new Array();
    var list = this;
    for (var index = 0; index < list.length; index++) {
        var current = fn(list[index]);
        var exist = arry.any(function (item) {
            for (var key in current) {
                if (current[key] != item[key]) {
                    return false;
                }
            }
            return true;
        });

        if (!exist) {
            arry.push(current);
        }
    }
    return arry;
};

//正排序
Array.prototype.orderBy = function (fn) {
    var list = this;
    return list.sort(function (a, b) {
        if (fn(a) > fn(b)) return 1;
        else return -1;
    });
};
Array.prototype.orderByDescending = function (fn) {
    var list = this;
    return list.sort(function (a, b) {
        if (fn(a) < fn(b)) return 1;
        else return -1;
    });
};

