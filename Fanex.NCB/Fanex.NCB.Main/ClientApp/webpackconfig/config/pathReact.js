var path = require('path');

var projectRoot = path.resolve(__dirname, '../../../');

var cssPaths = {
    build: "wwwroot/css/build"
};

var scriptPaths = {
    build: "wwwroot/js/build",
   
    home: "./sources/pages/home/index.js"    
};
module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};