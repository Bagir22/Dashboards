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
      // webpack.config.js
      shared: {
        "@angular/core": { singleton: true, strictVersion: false, eager: true },
        "@angular/common": { singleton: true, strictVersion: false, eager: true },
        "@angular/common/http": { singleton: true, strictVersion: false, eager: true },
        "@angular/router": { singleton: true, strictVersion: false, eager: true },
        "@taiga-ui/cdk": { singleton: true, strictVersion: false, eager: true },
        "@taiga-ui/core": { singleton: true, strictVersion: false, eager: true },
        "@taiga-ui/event-plugins": { singleton: true, strictVersion: false, eager: true },
        "@taiga-ui/polymorpheus": { singleton: true, strictVersion: false, eager: true },
        "@ng-web-apis/common": { singleton: true, strictVersion: false, eager: true },
        "@ng-web-apis/platform": { singleton: true, strictVersion: false, eager: true },
        "@ng-web-apis/mutation-observer": { singleton: true, strictVersion: false, eager: true },
        "rxjs": { singleton: true, strictVersion: false, eager: true },
        "zone.js": { singleton: true, strictVersion: false, eager: true },
        "@maskito/core": { singleton: true, strictVersion: false, eager: true },
        "@maskito/kit": { singleton: true, strictVersion: false, eager: true },
        "@maskito/angular": { singleton: true, strictVersion: false, eager: true },

      }

    }),
    new webpack.DefinePlugin({
      'process.env.METABASE_URL': JSON.stringify(process.env.METABASE_URL),
      'process.env.MFT_URL': JSON.stringify(process.env.MFT_URL)
    })
  ],
};
