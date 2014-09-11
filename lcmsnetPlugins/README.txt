When creating a new project be sure to copy this post-build event into it:

if "$(ConfigurationName)" == "PNNLRelease"  goto :release
else goto :end
:release
xcopy /y "$(TargetDir)*.dll" "$(SolutionDir)..\pluginDLLs\"

:end

In order to maintain consistency. Ensure that the proper dependencies are also copied via modification of xcopy target, if necessary.

Also note that with the above post-build event, in order to place the libraries into the pluginDLLs directory for use, the solution must be built with the PNNLRelease configuration, as building in Debug will not copy them to the pluginDLLs folder. This way, we do not risk overwriting a stable plugin with an unverified plugin.

