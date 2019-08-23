var path = require('path');

var projectRoot = path.resolve(__dirname, '../../../');

var paths = {
    build: "wwwroot",

    newnotification: "./sources/pages/newnotification/index.js",
    notificationlist: "./sources/pages/notificationlist/index.js",
    notificationtemplate: "./sources/pages/notificationtemplate/index.js",
    chatgroupmapping: "./sources/pages/chatgroupmapping/index.js",
    notificationtype: "./sources/pages/notificationtype/index.js",
    login: "./sources/pages/login/index.js",
    editnotification: "./sources/pages/editnotification/index.jsx"
};
module.exports = {
    projectRoot: projectRoot,
    paths: paths
};