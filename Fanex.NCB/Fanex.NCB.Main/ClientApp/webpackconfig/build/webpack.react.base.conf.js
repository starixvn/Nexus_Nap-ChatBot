var path = require('path');
var alias = require('../config/aliasReact');
var pathConfig = require('../config/pathReact');
var pluginGenerator = require('../config/pluginReact');
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

function generateConfig(environment) {
    var jsPath = `${pathConfig.scriptPaths.build}/${environment}/pages`;
    var cssPath = `${pathConfig.cssPaths.build}/${environment}/pages`;
    var commonConfigs = {
        resolve: {
            alias: alias,
            extensions: ['.js', '.jsx']
        },
        entry: {
            /*js*/            
            [`${jsPath}/home`]: pathConfig.scriptPaths.home,          
            [`${jsPath}/fetchdata`]: pathConfig.scriptPaths.fetchdata,          
            [`${jsPath}/newnotification`]: pathConfig.scriptPaths.newnotification,          
            [`${jsPath}/notificationlist`]: pathConfig.scriptPaths.notificationlist,          
            [`${jsPath}/notificationtemplate`]: pathConfig.scriptPaths.notificationtemplate,          
            [`${jsPath}/chatgroupmapping`]: pathConfig.scriptPaths.chatgroupmapping,          
            [`${jsPath}/notificationtype`]: pathConfig.scriptPaths.notificationtype,
            [`${jsPath}/layout`]: pathConfig.scriptPaths.layout,
            [`${jsPath}/login`]: pathConfig.scriptPaths.login

            /*css*/
            /*[`${cssPath}/vendor`]: pathConfig.cssPaths.layout,*/            
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: jsPath + '/lib/[name].chunk.js',
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/               
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|eot|ttf)$/,
                    use: [{
                        loader: 'url-loader',
                        options: { 
                            limit: 4000, // Convert images < 8kb to base64 strings
                            name: '[hash]-[name].[ext]',
                            outputPath: '/wwwroot/assets/',
                            publicPath: '/assets/'
                        } 
                    }]
                }
            ]
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
            children: true
        },
        node: {
            fs: 'empty'
        },
        externals: [
            { './jszip': 'jszip' },
            { './ods': 'ods' },
            { './cptable': 'var cptable' }
        ],
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
                name: false,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    common: {
                        name: 'common',
                        test: /(node_modules)/,
                        minChunks: 2,
                        priority: -10
                    }
                }
            }
        }
    };

    return commonConfigs;
}

module.exports = generateConfig;