const webpack = require('webpack');
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

const MFT_URL = process.env['MFT_URL'] || 'http://localhost:4201';

module.exports = {
  output: {
    //publicPath: 'auto',
    publicPath: `${MFT_URL}/`,
    uniqueName: 'dashboards',
    scriptType: 'text/javascript',
  },
  optimization: {
    runtimeChunk: false,
    splitChunks: false,
  },
  experiments: {
    outputModule: true,
  },
  module: {
    parser: {
      javascript: {
        importMeta: false
      }
    }
  },
  devServer: {
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
      "Access-Control-Allow-Headers": "X-Requested-With, content-type, Authorization"
    }
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'dashboards_mft',
      library: { type: 'module' },
      filename: 'remoteEntry.js',
      exposes: {
        './web-components': './projects/dashboards/src/bootstrap.ts',
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
      'process.env.METABASE_URL': JSON.stringify(process.env.METABASE_URL),
      'process.env.METABASE_DASHBOARD_ID': JSON.stringify(process.env.METABASE_DASHBOARD_ID || '')
    })
  ],
};
