const webpack = require('webpack');
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

const MFT_URL = process.env['MFT_URL'];
const BASE_URL = process.env.BASE_URL || 'localhost';
const MB_PORT = process.env.METABASE_PORT || '3000';

module.exports = {
  output: {
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
      METABASE_URL: JSON.stringify(`http://${BASE_URL}:${MB_PORT}`),
      METABASE_USER: JSON.stringify(process.env.METABASE_USER || ''),
      METABASE_PASS: JSON.stringify(process.env.METABASE_PASS || '')
    })
  ],
};
