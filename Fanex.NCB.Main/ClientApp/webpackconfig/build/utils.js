const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const pathConfig = require('../config/pathReact');

exports.setDefinePlugin = function (key, value) {
    const env = {};
    env[key] = JSON.stringify(value);

    return {
        plugins: [
            new webpack.DefinePlugin(env)
        ]
    };
};

exports.extractCSS = function (isProduction) {
    const env = isProduction ? 'pro' : 'dev';
    const commonPath = `${pathConfig.paths.build}/${env}/common`;
    return {
        plugins: [
            new MiniCssExtractPlugin({ filename: '[name].chunk.css', chunkFilename: commonPath + '/[name].chunk.css' })
        ],
        module: {
            rules: [
                {
                    test: /\.(sc|c)ss$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader?url=false',
                        'sass-loader'
                    ]
                }
            ]
        }
    };
};