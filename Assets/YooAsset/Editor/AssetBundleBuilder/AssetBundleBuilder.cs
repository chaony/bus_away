using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
    public class AssetBundleBuilder
    {
        private readonly BuildContext _buildContext = new BuildContext();

        /// <summary>
        /// 构建资源包
        /// </summary>
        public BuildResult Run(BuildParameters buildParameters, List<IBuildTask> buildPipeline, bool enableLog)
        {
            // 检测构建参数是否为空
            if (buildParameters == null)
                throw new Exception($"{nameof(buildParameters)} is null !");

            // 检测构建参数是否为空
            if (buildPipeline.Count == 0)
                throw new Exception($"Build pipeline is empty !");

            // 清空旧数据
            _buildContext.ClearAllContext();

            // 构建参数
            var buildParametersContext = new BuildParametersContext(buildParameters);
            _buildContext.SetContextObject(buildParametersContext);

            // 初始化日志系统
            string logFilePath = $"{buildParametersContext.GetPipelineOutputDirectory()}/buildInfo.log";
            BuildLogger.InitLogger(enableLog, logFilePath);


            //在构建之前，将包输出目录中的文件复制到bundle_original文件夹
            string packageOutputDirectory = buildParametersContext.GetPackageOutputDirectory();
            string backupDirectory = $"{buildParametersContext.Parameters.BuildOutputRoot}/{packageOutputDirectory}/bundle_original";

            // 如果备份目录已存在，则先删除
            if (System.IO.Directory.Exists(backupDirectory))
                System.IO.Directory.Delete(backupDirectory, true);

            // 如果包输出目录存在，则复制到备份目录
            if (System.IO.Directory.Exists(packageOutputDirectory))
                EditorTools.CopyDirectory(packageOutputDirectory, backupDirectory);

            // 执行构建流程
            BuildLogger.Log($"Begin to build package : {buildParameters.PackageName} by {buildParameters.BuildPipeline}");
            var buildResult = BuildRunner.Run(buildPipeline, _buildContext);
            if (buildResult.Success)
            {
                buildResult.OutputPackageDirectory = buildParametersContext.GetPackageOutputDirectory();
                BuildLogger.Log("Resource pipeline build success");
            }
            else
            {
                BuildLogger.Error($"{buildParameters.BuildPipeline} build failed !");
                BuildLogger.Error($"An error occurred in build task {buildResult.FailedTask}");
                BuildLogger.Error(buildResult.ErrorInfo);
            }

            // 关闭日志系统
            BuildLogger.Shuntdown();

            return buildResult;
        }
    }
}