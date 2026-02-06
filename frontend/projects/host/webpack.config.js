const webpack = require('webpack');
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

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
      'process.env.DASHBOARDS': JSON.stringify(process.env.DASHBOARDS),
      'process.env.METABASE_URL': JSON.stringify(process.env.METABASE_URL),
      'process.env.MFT_URL': JSON.stringify(process.env.MFT_URL)
    })
  ],
};
