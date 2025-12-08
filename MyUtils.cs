#nullable enable
global using static InfernumAbyssGenerationPatch.MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace InfernumAbyssGenerationPatch;

public class MyUtils
{
    internal static Mod? InfernumMod;
    internal static Mod? CalamityMod;
    internal static Mod? LuminanceMod;
    internal static readonly Dictionary<string, Type> ClassCache = new();
    internal static readonly Dictionary<string, MethodInfo> MethodCache = new();
    internal static readonly Dictionary<string, MemberInfo> FieldCache = new();
    internal static readonly Dictionary<string, ushort> IdCache = new();
    internal const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Static;

    internal static void ClearCache()
    {
        ClassCache.Clear();
        MethodCache.Clear();
        FieldCache.Clear();
        IdCache.Clear();
    }

    internal static Type? GetClass(string classPath)
    {
        if (ClassCache.TryGetValue(classPath, out var cachedClassInfo))
        {
            return cachedClassInfo;
        }

        var modCode = (classPath.StartsWith("Infernum") ? InfernumMod :
            classPath.StartsWith("Calamity") ? CalamityMod : LuminanceMod)?.Code;
        var newClassInfo = modCode?.GetType(classPath);
        if (newClassInfo is null)
        {
            return null;
        }

        ClassCache.Add(classPath, newClassInfo);
        return newClassInfo;
    }

    internal static MethodInfo? GetMethod(string classPath, string methodName,
        Type[]? argTypes = null, BindingFlags bindingFlags = DefaultFlags)
    {
        var methodPath = $"{classPath}.{methodName}";
        if (MethodCache.TryGetValue(methodPath, out var cachedMethodInfo))
        {
            return cachedMethodInfo;
        }

        var classInfo = GetClass(classPath);
        var newMethodInfo = argTypes is null
            ? classInfo?.GetMethod(methodName, bindingFlags)
            : classInfo?.GetMethod(methodName, bindingFlags, argTypes);
        if (newMethodInfo is null)
        {
            return null;
        }

        MethodCache.Add(methodPath, newMethodInfo);
        return newMethodInfo;
    }

    internal static T? InvokeMethod<T>(string classPath, string methodName,
        Type[]? argTypes = null, BindingFlags bindingFlags = DefaultFlags, params object[] args)
    {
        var methodInfo = GetMethod(classPath, methodName, argTypes, bindingFlags);
        var result = methodInfo?.Invoke(null, args);
        if (result is null)
        {
            return default;
        }

        return (T)result;
    }

    internal static T? InvokeMethod<T>(string classPath, string methodName, params object[] args) =>
        InvokeMethod<T>(classPath, methodName, null, DefaultFlags, args);

    internal static T? InvokeMethod<T>(string classPath, string methodName, Type[] argTypes, params object[] args) =>
        InvokeMethod<T>(classPath, methodName, argTypes, DefaultFlags, args);

    internal static T? InvokeConstructor<T>(string classPath, string? innerClassName, params object[] args)
    {
        var classInfo = GetClass(classPath);
        if (innerClassName is not null)
        {
            classInfo = classInfo?.GetNestedType(innerClassName);
        }

        var argTypes = args.Select(arg => arg.GetType()).ToArray();
        var constructor = classInfo?.GetConstructor(argTypes);
        var instance = constructor?.Invoke(args);
        if (instance is null)
        {
            return default;
        }

        return (T)instance;
    }

    internal static T? InvokeConstructor<T>(string classPath, params object[] args) =>
        InvokeConstructor<T>(classPath, null, args);

    internal static T? GetField<T>(string classPath, string fieldName,
        BindingFlags bindingFlags = DefaultFlags) where T : MemberInfo
    {
        var fieldPath = $"{classPath}.{fieldName}";
        if (FieldCache.TryGetValue(fieldPath, out var cachedFieldInfo))
        {
            return (T)cachedFieldInfo;
        }

        var classInfo = GetClass(classPath);
        MemberInfo? newFieldInfo = typeof(T) == typeof(FieldInfo) ? classInfo?.GetField(fieldName, bindingFlags) :
            typeof(T) == typeof(PropertyInfo) ? classInfo?.GetProperty(fieldName, bindingFlags) : null;
        if (newFieldInfo is null)
        {
            return null;
        }

        FieldCache.Add(fieldPath, newFieldInfo);
        return (T)newFieldInfo;
    }

    internal static FieldInfo? GetField(string classPath, string fieldName,
        BindingFlags bindingFlags = DefaultFlags) => GetField<FieldInfo>(classPath, fieldName, bindingFlags);

    internal static TV? GetValue<T, TV>(string classPath, string fieldName,
        BindingFlags bindingFlags = DefaultFlags) where T : MemberInfo
    {
        var fieldInfo = GetField<T>(classPath, fieldName, bindingFlags);
        var fieldValue = typeof(T) == typeof(FieldInfo) ? (fieldInfo as FieldInfo)?.GetValue(null) :
            typeof(T) == typeof(PropertyInfo) ? (fieldInfo as PropertyInfo)?.GetValue(null) : null;
        if (fieldValue is null)
        {
            return default;
        }

        return (TV)fieldValue;
    }

    internal static TV? GetValue<TV>(string classPath, string fieldName,
        BindingFlags bindingFlags = DefaultFlags) => GetValue<FieldInfo, TV>(classPath, fieldName, bindingFlags);

    internal static void SetValue<T>(string classPath, string fieldName, object value,
        BindingFlags bindingFlags = DefaultFlags) where T : MemberInfo
    {
        var fieldInfo = GetField<T>(classPath, fieldName, bindingFlags);
        if (typeof(T) == typeof(FieldInfo))
        {
            (fieldInfo as FieldInfo)?.SetValue(null, value);
        }
        else if (typeof(T) == typeof(PropertyInfo))
        {
            (fieldInfo as PropertyInfo)?.SetValue(null, value);
        }
    }

    internal static void SetValue(string classPath, string fieldName, object value,
        BindingFlags bindingFlags = DefaultFlags) => SetValue<FieldInfo>(classPath, fieldName, value, bindingFlags);

    internal static ushort GetId(string typeName, string classPath)
    {
        if (IdCache.TryGetValue(classPath, out var cachedIdInfo))
        {
            return cachedIdInfo;
        }

        var classInfo = GetClass(classPath);
        if (classInfo is null)
        {
            return 0;
        }

        var methodPath = $"{classPath}.{typeName}";
        if (!MethodCache.TryGetValue(methodPath, out var methodInfo))
        {
            methodInfo = typeof(ModContent).GetMethod(typeName)!;
            MethodCache.Add(methodPath, methodInfo);
        }

        var result = methodInfo.MakeGenericMethod(classInfo).Invoke(null, null);
        if (result is null)
        {
            return 0;
        }

        var newIdInfo = Convert.ToUInt16(result);
        IdCache.Add(classPath, newIdInfo);
        return newIdInfo;
    }

    internal static ushort GetTileId(string classPath) => GetId("TileType", classPath);

    internal static ushort GetWallId(string classPath) => GetId("WallType", classPath);
}