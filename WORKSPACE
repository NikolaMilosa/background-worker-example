workspace(name = "background_worker")

load("@bazel_tools//tools/build_defs/repo:http.bzl", "http_archive")
http_archive(
    name = "rules_dotnet",
    sha256 = "d01b0f44e58224deeb8ac81afe8701385d41b16c8028709d3a4ed5b46f1c48a0",
    strip_prefix = "rules_dotnet-0.14.0",
    url = "https://github.com/bazelbuild/rules_dotnet/releases/download/v0.14.0/rules_dotnet-v0.14.0.tar.gz",
)

load(
    "@rules_dotnet//dotnet:repositories.bzl",
    "dotnet_register_toolchains",
    "rules_dotnet_dependencies",
)

rules_dotnet_dependencies()

# Here you can specify the version of the .NET SDK to use.
dotnet_register_toolchains("dotnet", "7.0.101")

load("@rules_dotnet//dotnet:paket.rules_dotnet_nuget_packages.bzl", "rules_dotnet_nuget_packages")

rules_dotnet_nuget_packages()

load("@rules_dotnet//dotnet:paket.paket2bazel_dependencies.bzl", "paket2bazel_dependencies")

paket2bazel_dependencies()

load("//:paket.main.bzl", "main")

main()