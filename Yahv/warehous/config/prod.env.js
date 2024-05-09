'use strict'
// const urls="htttp://www.baidu.com"
// const urls=require("./publics.dev")
const urls=require("../static/pubilc.dev")
module.exports = {
  NODE_ENV: '"production"',
  API_ROOT: '"/"',
  TEST_DATA:'"/"',
  // PFWMS_API:'"http://hv.warehouse.b1b.com"',
  PFWMS_API:JSON.stringify(urls.pfwms),
}
