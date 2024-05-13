import moment from "moment";
const showDate=function(val) {
    //时间格式转换
    // console.log(val)
    if (val != "") {
      if (val || "") {
        var b = val.split("(")[1];
        var c = b.split(")")[0];
        var result = moment(+c).format("YYYY-MM-DD");
        return result;
      }
    }
  }

  
export {
    showDate
}