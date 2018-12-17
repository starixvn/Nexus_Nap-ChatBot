var path = require('path');

var projectRoot = path.resolve(__dirname, '../../../');

var cssPaths = {
    build: "wwwroot/css/build"
};

var scriptPaths = {
    build: "wwwroot/js/build",
   
    home: "./sources/pages/home/index.js",
    fetchdata: "./sources/pages/fetchdata/index.js",
    newnotification: "./sources/pages/newnotification/index.js",
    notificationlist: "./sources/pages/notificationlist/index.js",
    notificationtemplate: "./sources/pages/notificationtemplate/index.js",
    chatgroupmapping: "./sources/pages/chatgroupmapping/index.js",
    notificationtype: "./sources/pages/notificationtype/index.js"
};
module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};