const webpack = require('webpack');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
var pathConfig = require('../config/pathReact');

exports.setDefinePlugin = function(key, value) {
    const env = {};
    env[key] = JSON.stringify(value);

    return {
        plugins: [
            new webpack.DefinePlugin(env)
        ]
    };
};

exports.extractCSS = function(isProduction) {
    var env = isProduction ? 'pro' : 'dev';
    var commonPath = `${pathConfig.paths.build}/${env}/common`;
    return {
        plugins: [
                new MiniCssExtractPlugin({ filename: "[name].chunk.css", chunkFilename: commonPath + '/[id].chunk.css'})
        ],
        module: {
            rules: [
                {
                    test: /\.(sc|c)ss$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                          'css-loader',
                          'sass-loader',
                    ]
                }
            ]
        }
    };
};