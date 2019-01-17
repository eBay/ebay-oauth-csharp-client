dotnet restore
dotnet build

cd Tools
dotnet restore

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../ --assemblies Tests/**/bin/**/*.dll --sources ebay-oauth-csharp-client/**/*.cs

# Reset hits count in case minicover was run for this project
dotnet minicover reset --workdir ../

cd ..

dotnet test --no-build Tests/ebay-oauth-csharp-client-tests.csproj

cd Tools

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir ../

# Create html reports inside folder coverage-html
dotnet minicover htmlreport --workdir ../ --threshold 80

# Print console report
# This command returns failure if the coverage is lower than the threshold
dotnet minicover report --workdir ../ --threshold 80

cd ..
