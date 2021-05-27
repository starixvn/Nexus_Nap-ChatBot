module.exports = {
    setupFilesAfterEnv: ['<rootDir>/setupTests/setupEnzyme.js'],
    moduleNameMapper: {
        '^.+\\.(css|scss)$': 'identity-obj-proxy'
    }
    //verbose: true
};