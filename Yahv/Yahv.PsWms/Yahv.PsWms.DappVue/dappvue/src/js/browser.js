/*回调windows事件：name 回调标识; data 数据（字符串或json字符串）*/
function FireEvent(name, data) {

    var event = new MessageEvent(name, { 'view': window, 'bubbles': false, 'cancelable': false, 'data':data ==null ?null : JSON.stringify(data) });
    document.dispatchEvent(event);
} 

export function TemplatePrint(data) {

    FireEvent("TemplatePrint", data);

    if (!!window['_118CCDF89D3505FCA6068E43E2007D3A']) {
        var ___1 = window['_118CCDF89D3505FCA6068E43E2007D3A'];
        window['_118CCDF89D3505FCA6068E43E2007D3A'] = null;
        return ___1;
    }
}
export function FilePrint(data) {

    FireEvent("FilePrint", data);

    if (!!window['_D9A1AA6A9E4AD5CE344FCB710596CFD9']) {
        var ___1 = window['_D9A1AA6A9E4AD5CE344FCB710596CFD9'];
        window['_D9A1AA6A9E4AD5CE344FCB710596CFD9'] = null;
        return ___1;
    }
}
export function Logon(data) {

    FireEvent("Logon", data);

    if (!!window['_6CC549F31D4E87D549992B39024C022E']) {
        var ___1 = window['_6CC549F31D4E87D549992B39024C022E'];
        window['_6CC549F31D4E87D549992B39024C022E'] = null;
        return ___1;
    }
}
export function FormPhoto(data) {

    FireEvent("FormPhoto", data);

    if (!!window['_E120BCDD71C698632C84974967F7EF75']) {
        var ___1 = window['_E120BCDD71C698632C84974967F7EF75'];
        window['_E120BCDD71C698632C84974967F7EF75'] = null;
        return ___1;
    }
}
export function SeletUploadFiles(data) {

    FireEvent("SeletUploadFiles", data);

    if (!!window['_ED27C07AC1A19F15CEDCCE055695C5F9']) {
        var ___1 = window['_ED27C07AC1A19F15CEDCCE055695C5F9'];
        window['_ED27C07AC1A19F15CEDCCE055695C5F9'] = null;
        return ___1;
    }
}
export function FilesProcess(data) {

    FireEvent("FilesProcess", data);

    if (!!window['_FE0C6D8AE932547299ECF27E4FAC50D9']) {
        var ___1 = window['_FE0C6D8AE932547299ECF27E4FAC50D9'];
        window['_FE0C6D8AE932547299ECF27E4FAC50D9'] = null;
        return ___1;
    }
}
export function GetPrinterConfig() {

    FireEvent("GetPrinterConfig", null);

    if (!!window['_BDF1790100179A1165892FACA786E58F']) {
        var ___1 = window['_BDF1790100179A1165892FACA786E58F'];
        window['_BDF1790100179A1165892FACA786E58F'] = null;
        return ___1;
    }
}
export function GetPrinterDictionary() {

    FireEvent("GetPrinterDictionary", null);

    if (!!window['_7405FF8CD16BDF21C09EAA5F6B25E2E8']) {
        var ___1 = window['_7405FF8CD16BDF21C09EAA5F6B25E2E8'];
        window['_7405FF8CD16BDF21C09EAA5F6B25E2E8'] = null;
        return ___1;
    }
}
export function SetPrinterConfig(data) {

    FireEvent("SetPrinterConfig", data);

    if (!!window['_B53F29358291E08D3226FE37836417A0']) {
        var ___1 = window['_B53F29358291E08D3226FE37836417A0'];
        window['_B53F29358291E08D3226FE37836417A0'] = null;
        return ___1;
    }
}
export function GetAllPrinterNames() {

    FireEvent("GetAllPrinterNames", null);

    if (!!window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC']) {
        var ___1 = window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC'];
        window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC'] = null;
        return ___1;
    }
}
export function PrintFaceSheet(data) {

    FireEvent("PrintFaceSheet", data);

    if (!!window['_1969BBE178B3C1587F5EFF7D0B5B4E23']) {
        var ___1 = window['_1969BBE178B3C1587F5EFF7D0B5B4E23'];
        window['_1969BBE178B3C1587F5EFF7D0B5B4E23'] = null;
        return ___1;
    }
}
export function ReprintFaceSheet(data) {

    FireEvent("ReprintFaceSheet", data);

    if (!!window['_2FB0B64E7CB638E41B4D1C21CB8A27E3']) {
        var ___1 = window['_2FB0B64E7CB638E41B4D1C21CB8A27E3'];
        window['_2FB0B64E7CB638E41B4D1C21CB8A27E3'] = null;
        return ___1;
    }
}
export function PrintDeliveryList(data) {

    FireEvent("PrintDeliveryList", data);

    if (!!window['_68ABF579C1D298E41959D6CC684CB3D0']) {
        var ___1 = window['_68ABF579C1D298E41959D6CC684CB3D0'];
        window['_68ABF579C1D298E41959D6CC684CB3D0'] = null;
        return ___1;
    }
}
