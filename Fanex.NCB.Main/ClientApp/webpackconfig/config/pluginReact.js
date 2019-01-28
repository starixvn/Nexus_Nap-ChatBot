var webpack = require('webpack');
var CleanWebpackPlugin = require('clean-webpack-plugin');
var pathConfig =require('./pathReact');
var buildPath = pathConfig.paths.build;

let _buildNumber = 1;
function generatePlugins(environment) {
    var commonPlugins = [
        new CleanWebpackPlugin([buildPath], {
            root: process.cwd(),
            exclude: ['assets', 'dev', 'pro']
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