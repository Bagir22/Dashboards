const webpack = require('webpack');
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

const MFT_URL = process.env['MFT_URL'] || 'http://localhost:4201';

module.exports = {
  output: {
    publicPath: `auto`,
    uniqueName: 'host',
    scriptType: 'text/javascript',
    crossOriginLoading: 'anonymous'
  },
  experiments: {
    outputModule: true,
  },
  optimization: {
    runtimeChunk: false,
    splitChunks: false,
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'host',
      library: { type: 'module' },
      remotes: {
        'dashboards-mft': `${MFT_URL}/remoteEntry.js`,
      },
      shared: {
        "@angular/core": { singleton: true, strictVersion: false, eager: true },
        "@angular/common": { singleton: true, strictVersion: false, eager: true },
        "@angular/common/http": { singleton: true, strictVersion: false, eager: true },
        "@angular/router": { singleton: true, strictVersion: false, eager: true },
        "rxjs": { singleton: true, strictVersion: false, eager: true },
        "zone.js": { singleton: true, strictVersion: false, eager: true }
      }
    }),
    new webpack.DefinePlugin({
      'process.env.MFT_URL': JSON.stringify(MFT_URL)
    })
  ],
};
