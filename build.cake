#tool "nuget:?package=Cake.FileHelpers"

#load "./build/parameters.cake"

var Parameters = BuildParameters.Load(Context, BuildSystem);


Setup(context => {
    Information($"Running build: {Parameters.Target} {Parameters.Configuration}");
});

Task("Clean-Output-Directory")
    .IsDependentOn("Create-Output-Directory")
    .Does(() => {
        EnsureDirectoryExists(Parameters.OutputPath);
    });
Task("Create-Output-Directory")
    .Does(()=>{
        CleanDirectory(Parameters.OutputPath);
    });
Task("Update-Lib")
    .Does(() => {

    });

Task("Build-Addon")
    .Does(() => {
        MSBuild(Parameters.AddonProjectFile, new MSBuildSettings{
            Configuration = Parameters.Configuration            
        });
    });

Task("Build-Extension")
    .Does(() => {
        MSBuild(Parameters.ExtensionProjectFile, new MSBuildSettings{
            Configuration = Parameters.Configuration            
        });
    });

Task("Build")
    .IsDependentOn("Update-Lib")
    .IsDependentOn("Build-Addon")
    .IsDependentOn("Build-Extension")
    .Does(() =>{
    });    

Task("Clean-ExtensionPackage-Directory")
    .IsDependentOn("Create-ExtensionPackage-Directory")
    .Does(()=>{
        CleanDirectory(Parameters.ExtensionPackagePath);
    });

Task("Create-ExtensionPackage-Directory")
    .Does(() => {
        EnsureDirectoryExists(Parameters.ExtensionPackagePath);
    });

Task("Clean-AddonPackage-Directory")
    .IsDependentOn("Create-AddonPackage-Directory")
    .Does(()=>{
        CleanDirectory(Parameters.AddonPackagePath);
    });

Task("Create-AddonPackage-Directory")
    .Does(() => {
        EnsureDirectoryExists(Parameters.AddonPackagePath);
    });    

Task("Clean-Output-Extension")
    .IsDependentOn("Create-Output-Directory")
    .Does(() => {
        if (FileExists(Parameters.ExtensionArchive))
        {
            DeleteFile(Parameters.ExtensionArchive);
        }
    });
Task("Clean-Output-Addon")
    .IsDependentOn("Create-Output-Directory")
    .Does(() => {
        if (FileExists(Parameters.AddonArchive))
        {
            DeleteFile(Parameters.AddonArchive);
        }
    });

Task("Package-Addon")
    .IsDependentOn("Clean-AddonPackage-Directory")
    .IsDependentOn("Clean-Output-Addon")
    .Does(() => {
        CopyFiles(Parameters.AddonBinaries, Parameters.AddonPackagePath);
        Zip(Parameters.AddonPackagePath, Parameters.AddonArchive);
    });

Task("Package-Extension")
    .IsDependentOn("Clean-ExtensionPackage-Directory")
    .IsDependentOn("Clean-Output-Extension")
    .Does(() => {
        var servicePath = $"{Parameters.ExtensionPackagePath}/services/{Parameters.ExtensionName}";
        EnsureDirectoryExists(servicePath);
        CopyFiles(Parameters.ExtensionBinaries, servicePath);
        Zip(Parameters.ExtensionPackagePath, Parameters.ExtensionArchive);
    });

Task("Package")
    .IsDependentOn("Package-Addon")
    .IsDependentOn("Package-Extension")
    .Does(() => {});

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