{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/master";
    flake-utils.url = "github:numtide/flake-utils";
  };
  outputs = {
    self,
    nixpkgs,
    flake-utils,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {inherit system;};
    in
      with pkgs; {
        devShells.default = mkShell {
          packages = [
            nodejs_24
            dotnetCorePackages.sdk_10_0-bin
          ];
          DOTNET_ROOT = "${dotnetCorePackages.sdk_10_0-bin}";
        };
      });
}
