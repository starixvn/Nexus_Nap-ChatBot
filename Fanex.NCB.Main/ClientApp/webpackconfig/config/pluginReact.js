const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const pathConfig = require('./pathReact');
const buildPath = pathConfig.paths.build;

let _buildNumber = 1;
function generatePlugins() {
    const commonPlugins = [
        new CleanWebpackPlugin([buildPath], {
            root: process.cwd(),
            exclude: ['assets', 'dev', 'pro']
        }),
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
        function () {
            this.plugin('done', function () {
                setTimeout(function () {
                    console.log(`\n ////////// Build version: #${_buildNumber++} (${new Date().toLocaleString()}).//////////\n`);
                }, 150);
            });
        }
    ];

    return commonPlugins;
}

module.exports = generatePlugins;