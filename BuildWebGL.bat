set UNITY_PATH="D:\Unity\Editor\6000.0.29f1\Editor\Unity.exe"
set PROJECT_PATH="D:\Unity\Projects\FKUnitySnippets"
set BUILD_METHOD="WebGLBuildScript.BuildAndRunWebGL"

%UNITY_PATH% -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod %BUILD_METHOD% > build_output.log 2>&1

type build_output.log

pause