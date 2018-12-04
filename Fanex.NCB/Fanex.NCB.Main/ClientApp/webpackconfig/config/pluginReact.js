var webpack = require('webpack');
var CleanWebpackPlugin = require('clean-webpack-plugin');
var pathConfig =require('./pathReact');
var cssRootPath = pathConfig.cssPaths.build;

let _buildNumber = 1;
function generatePlugins(environment) {
    var commonPlugins = [
        new CleanWebpackPlugin([cssRootPath], {
            root: process.cwd(),
            exclude: ['fonts', 'images', 'dev', 'pro']
        }),
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
         function () {
             this.plugin('done', function (stats) {
                 setTimeout(function () {
                     console.log('\n ////////// Build version: #' + (_buildNumber++) + ' (' + new Date().toLocaleString() + ').//////////\n');
                 }, 150);
             });
         }
    ];

    return commonPlugins;
}

module.exports = generatePlugins;