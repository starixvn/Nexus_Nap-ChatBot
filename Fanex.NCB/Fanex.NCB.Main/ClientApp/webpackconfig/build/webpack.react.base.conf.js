var pathConfig = require('../config/pathReact');
//var pluginGenerator = require('../config/pluginReact');
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

function generateConfig(environment) {
    var buildPath = `${pathConfig.paths.build}/${environment}/pages`;
    var commonPath = `${pathConfig.paths.build}/${environment}/common`;
    var commonConfigs = {
        resolve: {
            extensions: ['.js', '.jsx']
        },
        entry: {
                     
            [`${buildPath}/newnotification/newnotification`]: pathConfig.paths.newnotification,          
            [`${buildPath}/notificationlist/notificationlist`]: pathConfig.paths.notificationlist,          
            [`${buildPath}/notificationtemplate/notificationtemplate`]: pathConfig.paths.notificationtemplate,          
            [`${buildPath}/chatgroupmapping/chatgroupmapping`]: pathConfig.paths.chatgroupmapping,          
            [`${buildPath}/notificationtype/notificationtype`]: pathConfig.paths.notificationtype,
            [`${buildPath}/home/home`]: pathConfig.paths.home,
            [`${buildPath}/login/login`]: pathConfig.paths.login,
            
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: commonPath + '/[name].chunk.js',
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/               
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg)$/,
                    use: [{
                        loader: 'url-loader',
                        options: { 
                            limit: 4000, // Convert images < 8kb to base64 strings
                            name: '[hash]-[name].[ext]',
                            outputPath: '/wwwroot/assets/images/',
                            publicPath: '/assets/images/'
                        } 
                    }]
                },
                {
                    test: /\.(woff|woff2|eot|ttf)$/,
                    use: [{
                        loader: 'url-loader',
                        options: { 
                            limit: 4000, // Convert images < 8kb to base64 strings
                            name: '[hash]-[name].[ext]',
                            outputPath: '/wwwroot/assets/fonts/',
                            publicPath: '/assets/fonts/'
                        } 
                    }]
                }
            ]
        },
        //plugins: pluginGenerator(environment),
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
                        //test: /(node_modules)/,
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