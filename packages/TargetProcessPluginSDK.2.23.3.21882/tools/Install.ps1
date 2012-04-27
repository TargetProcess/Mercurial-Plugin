param($rootPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("Tp.Integration.Plugin.Common.dll.config").Properties.Item("CopyToOutputDirectory").Value = 2
$project.ProjectItems.Item("Mashups").ProjectItems.Item("ProfileEditor").ProjectItems.Item("ProfileEditor.js").Properties.Item("CopyToOutputDirectory").Value = 2;