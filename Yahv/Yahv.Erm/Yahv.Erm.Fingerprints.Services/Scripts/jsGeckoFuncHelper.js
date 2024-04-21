function methodName(data) {

    FireEvent("methodName", data);

    if (!!window['guid']) {
        var ___1 = window['guid'];
        window['guid'] = null;
        return ___1;
    }
}