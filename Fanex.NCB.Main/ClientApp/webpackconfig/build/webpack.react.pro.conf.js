var merge = require('webpack-merge');
var utils = require('./utils');
var baseWebpackConfigGenerator = require('./webpack.react.base.conf');
var CleanWebpackPlugin = require('clean-webpack-plugin');
var pathConfig = require('../config/pathReact');
var environment = "pro";
var baseConfigs = baseWebpackConfigGenerator(environment);

var buildPath = `${pathConfig.paths.build}/${environment}`;
module.exports = merge(baseConfigs,
    utils.setDefinePlugin('process.env.NODE_ENV', 'production'),
    {
        mode: 'production',
        devtool: 'source-map',
        watch: true,
        plugins: [
            new CleanWebpackPlugin([`${buildPath}/`], {
                root: process.cwd()               
            })
        ]
    },
    utils.extractCSS(true)
);