#tool "nuget:?package=Cake.FileHelpers"

#load "./build/parameters.cake"

var Parameters = BuildParameters.Load(Context, BuildSystem);


Setup(context => {
    Information($"Running build: {Parameters.Target} {Parameters.Configuration}");
});

Task("Build")
    .IsDependentOn()
    .Does(() => {

        }
    );
Task("Package")
    .Does(() =>{

    });
Task("Prepare-Artifacts")
    .IsDependentOn("Build")
    .IsDependentOn("Package")
    .Does(()=> {
        Information("Preparing Artifacts");
    });    


Task("Default")
    .Does(() => {
        RunTarget("Prepare-Artifacts");
    });

RunTarget(Parameters.Target);