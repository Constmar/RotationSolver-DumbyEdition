﻿using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using RotationSolver.Data;
using System.Diagnostics;

namespace RotationSolver.Helpers;

internal static class RotationHelper
{
    private static readonly Dictionary<Assembly, AssemblyInfo> _assemblyInfos = [];
    private static readonly Dictionary<Assembly, UiString> _assemblyVersionWarnings = [];

    public static List<LoadedAssembly> LoadedCustomRotations { get; } = [];

    private static readonly Version? RsVersion = GetRSBasicVersion(typeof(RotationHelper).Assembly);

    public static UiString GetWarning(this Assembly assembly)
    {
        if (_assemblyVersionWarnings.TryGetValue(assembly, out var info))
        {
            return info;
        }
        return UiString.None;
    }

    private static Version? GetRSBasicVersion(Assembly assembly)
    {
        return assembly.GetReferencedAssemblies().Where(n => n.Name == "RotationSolver.Basic").FirstOrDefault()?.Version;
    }

    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
        if (_assemblyInfos.TryGetValue(assembly, out var info))
        {
            return info;
        }

        var name = assembly.GetName().Name;
        var location = assembly.Location;
        var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        var assemblyInfo = new AssemblyInfo(name, company, location, string.Empty, company, name, DateTime.Now);

        _assemblyInfos[assembly] = assemblyInfo;

        return assemblyInfo;
    }

    public static unsafe Vector4 GetColor(this ICustomRotation rotation)
    {
        if (!rotation.IsEnabled)
        {
            return *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled);
        }

        if (!rotation.IsValid)
        {
            return ImGuiColors.DPSRed;
        }

        if (rotation.IsBeta())
        {
            return ImGuiColors.DalamudOrange;
        }

        return ImGuiColors.DalamudWhite;
    }

    public static bool IsBeta(this ICustomRotation rotation)
    {
        var betaAttribute = rotation.GetType().GetCustomAttribute<BetaRotationAttribute>();
        return betaAttribute != null;
    }

    public static Assembly LoadCustomRotationAssembly(string filePath)
    {
        var directoryInfo = new FileInfo(filePath).Directory;
        var loadContext = new RotationLoadContext(directoryInfo);
        var assembly = loadContext.LoadFromFile(filePath);

        var existingAssembly = GetAssemblyFromPath(filePath);

        var version = GetRSBasicVersion(assembly);
        if (version is not null && RsVersion is not null)
        {
            UiString warning;
            if (version.Major == RsVersion.Major
                || version.Minor == RsVersion.Minor
                || version.Build == RsVersion.Build)
            {
                warning = UiString.None;
            }
            else if (version.Major < RsVersion.Major
                || version.Minor < RsVersion.Minor
                || version.Build < RsVersion.Build)
            {
                warning = UiString.AssemblyLowVersion;
                if (existingAssembly != null)
                {
                    return existingAssembly;
                }
            }
            else
            {
                warning = UiString.AssemblyHighVersion;
            }

            _assemblyVersionWarnings[assembly] = warning;
        }

        var assemblyName = assembly.GetName().Name;
        var author = GetAuthor(filePath, assemblyName);

        var link = assembly.GetCustomAttribute<AssemblyLinkAttribute>();
        var assemblyInfo = new AssemblyInfo(
            assemblyName,
            author,
            filePath,
            link?.Donate,
            link?.UserName,
            link?.Repository,
            DateTime.Now);

        if (existingAssembly != null)
        {
            _assemblyInfos.Remove(existingAssembly);
        }

        _assemblyInfos[assembly] = assemblyInfo;

        var loadedAssembly = new LoadedAssembly(
            filePath,
            File.GetLastWriteTimeUtc(filePath).ToString());

        LoadedCustomRotations.RemoveAll(item => item.FilePath == loadedAssembly.FilePath);
        LoadedCustomRotations.Add(loadedAssembly);

        return assembly;
    }

    private static Assembly? GetAssemblyFromPath(string filePath)
    {
        foreach (var asm in _assemblyInfos)
        {
            if (asm.Value.FilePath == filePath)
            {
                return asm.Key;
            }
        }
        return null;
    }

    private static string GetAuthor(string filePath, string? assemblyName)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);
        return string.IsNullOrWhiteSpace(fileVersionInfo.CompanyName) ? assemblyName ?? string.Empty : fileVersionInfo.CompanyName;
    }
}
