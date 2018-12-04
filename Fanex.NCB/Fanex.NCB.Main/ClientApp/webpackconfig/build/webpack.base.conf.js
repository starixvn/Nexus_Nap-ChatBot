var path = require('path');
var merge = require('webpack-merge');

var pathConfig = require('../config/path');
var alias = require('../config/alias');
var rules = require('../config/rule');
var pluginGenerator = require('../config/plugin');
var exposeLoader = require('expose-loader');
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

function generateConfig(environment) {
    var jsPath = `${pathConfig.scriptPaths.build}/${environment}`;
    var cssPath = `${pathConfig.cssPaths.build}/${environment}`;

    var commonConfigs = {
        resolve: {
            alias: alias,
            modules: [
                path.join(__dirname, "www"),
                "node_modules"
           ]
        },
        entry: {
            [`${jsPath}/pages/home/home`]: pathConfig.scriptPaths.home,
            [`${jsPath}/pages/signin/signin`]: pathConfig.scriptPaths.signin,
            [`${jsPath}/pages/reports/livereport`]: [pathConfig.scriptPaths.livereport],
            
            [`${cssPath}/pages/reports/reportjqgridlayout`]: pathConfig.cssPaths.reportjqgridlayout,
            [`${cssPath}/pages/reports/livereport`]: pathConfig.cssPaths.livereport         
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js"
        },
        module: {
            rules: rules.commonRules
        },
        plugins: pluginGenerator(environment),
        stats: {
            assets: false,
            colors: true,
            version: false,
            hash: false,
            timings: true,
            chunks: false,
            chunkModules: false,
            children: false
        },
        optimization: {
            minimizer: [
             new UglifyJsPlugin({
                    sourceMap: true
                })
            ],
            splitChunks: {
                chunks: 'all',
                minSize: 30000,
                maxSize: 0,
                minChunks: 2,
                maxAsyncRequests: 5,
                maxInitialRequests: 3,
                automaticNameDelimiter: '~',
                name: true,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    common: {
                       name: `${jsPath}/pages/lib/common`,
                       test: /(node_modules|www[\\/]js[\\/]source[\\/]vendors|www[\\/]js[\\/]source[\\/]common)/,
                       minChunks: 2,
                       priority: -10
                    },
                    jqgrid: {
                        name: `${jsPath}/pages/lib/jqgrid`,
                        test: /(www[\\/]js[\\/]source[\\/]lib[\\/]jqgrid)/,
                        minChunks: 2,
                        priority: -15
                    }
                }
            }
        }
    };

    return commonConfigs;
}

module.exports = generateConfig;