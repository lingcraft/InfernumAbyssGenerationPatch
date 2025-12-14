#nullable enable
global using static InfernumAbyssGenerationPatch.MyUtils;
global using Microsoft.Xna.Framework;
global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using System.Linq;
global using Terraria;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using Terraria.ObjectData;
global using Terraria.Utilities;
global using Terraria.WorldBuilding;
global using Terraria.GameContent.Generation;
global using static Microsoft.Xna.Framework.MathHelper;
global using static System.MathF;
global using Luminance.Core.Hooking;
global using Luminance.Common.Utilities;
global using MonoMod.Cil;

namespace InfernumAbyssGenerationPatch;

public class MyUtils
{
    internal static Mod? InfernumMod;
    internal static Mod? CalamityMod;
    internal static readonly Dictionary<string, Type> ClassCache = new();
    internal static readonly Dictionary<string, MethodInfo> MethodCache = new();
    internal static readonly Dictionary<string, FieldInfo> FieldCache = new();
    internal static readonly Dictionary<string, PropertyInfo> PropertyCache = new();
    internal static readonly Dictionary<string, ushort> IdCache = new();

    internal const BindingFlags DefaultFlags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    #region 文本

    internal static string GetText(string str, params object[] arg)
    {
        return Language.GetTextValue($"Mods.InfernumAbyssGenerationPatch.{str}", arg);
    }

    #endregion 文本

    #region 反射

    internal static void ClearCache()
    {
        ClassCache.Clear();
        MethodCache.Clear();
        FieldCache.Clear();
        PropertyCache.Clear();
        IdCache.Clear();
    }

    internal static Type? GetClass(string classPath)
    {
        if (ClassCache.TryGetValue(classPath, out var cachedClassInfo))
        {
            return cachedClassInfo;
        }

        var newClassInfo = (classPath.StartsWith("Infernum") ? InfernumMod :
            classPath.StartsWith("Calamity") ? CalamityMod : null)?.Code?.GetType(classPath);
        if (newClassInfo is null)
        {
            return null;
        }

        ClassCache.Add(classPath, newClassInfo);
        return newClassInfo;
    }

    internal static MethodInfo? GetMethod(string classPath, string methodName, Type[]? argTypes = null)
    {
        var methodPath = $"{classPath}.{methodName}";
        if (MethodCache.TryGetValue(methodPath, out var cachedMethodInfo))
        {
            return cachedMethodInfo;
        }

        var newMethodInfo = argTypes is null
            ? GetClass(classPath)?.GetMethod(methodName, DefaultFlags)
            : GetClass(classPath)?.GetMethod(methodName, DefaultFlags, argTypes);
        if (newMethodInfo is null)
        {
            return null;
        }

        MethodCache.Add(methodPath, newMethodInfo);
        return newMethodInfo;
    }

    internal static TResult? InvokeMethod<TResult>(string classPath, string methodName, params object[] args)
    {
        var methodInfo = GetMethod(classPath, methodName, args.Select(arg => arg.GetType()).ToArray());
        var result = methodInfo?.Invoke(null, args);
        if (result is null)
        {
            return default;
        }

        return (TResult)result;
    }

    internal static void InvokeMethod(string classPath, string methodName, params object[] args)
    {
        var methodInfo = GetMethod(classPath, methodName, args.Select(arg => arg.GetType()).ToArray());
        methodInfo?.Invoke(null, args);
    }

    internal static TInstance? InvokeConstructor<TInstance>(string classPath, string? innerClassName, params object[] args)
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

        return (TInstance)instance;
    }

    internal static TInstance? InvokeConstructor<TInstance>(string classPath, params object[] args) =>
        InvokeConstructor<TInstance>(classPath, null, args);

    internal static FieldInfo? GetField(string classPath, string fieldName)
    {
        var fieldPath = $"{classPath}.{fieldName}";
        if (FieldCache.TryGetValue(fieldPath, out var cachedFieldInfo))
        {
            return cachedFieldInfo;
        }

        var newFieldInfo = GetClass(classPath)?.GetField(fieldName, DefaultFlags);
        if (newFieldInfo is null)
        {
            return null;
        }

        FieldCache.Add(fieldPath, newFieldInfo);
        return newFieldInfo;
    }

    internal static PropertyInfo? GetProperty(string classPath, string propertyName)
    {
        var propertyPath = $"{classPath}.{propertyName}";
        if (PropertyCache.TryGetValue(propertyPath, out var cachedPropertyInfo))
        {
            return cachedPropertyInfo;
        }

        var newFieldInfo = GetClass(classPath)?.GetProperty(propertyName, DefaultFlags);
        if (newFieldInfo is null)
        {
            return null;
        }

        PropertyCache.Add(propertyPath, newFieldInfo);
        return newFieldInfo;
    }

    internal static TResult? GetValue<TFieldType, TResult>(string classPath, string fieldName) where TFieldType : MemberInfo
    {
        object? fieldValue;
        if (typeof(TFieldType) == typeof(FieldInfo))
        {
            var field = GetField(classPath, fieldName);
            fieldValue = field?.GetValue(field.IsStatic ? null : GetInstance(classPath));
        }
        else
        {
            var property = GetProperty(classPath, fieldName);
            fieldValue = property?.GetValue(
                property.GetMethod is null || property.GetMethod.IsStatic ? null : GetInstance(classPath)
            );
        }

        if (fieldValue is null)
        {
            return default;
        }

        return (TResult)fieldValue;
    }

    internal static TResult? GetValue<TResult>(string classPath, string fieldName) => GetValue<FieldInfo, TResult>(classPath, fieldName);

    internal static void SetValue<TFieldType>(string classPath, string fieldName, object value) where TFieldType : MemberInfo
    {
        if (typeof(TFieldType) == typeof(FieldInfo))
        {
            var field = GetField(classPath, fieldName);
            field?.SetValue(field.IsStatic ? null : GetInstance(classPath), value);
        }
        else
        {
            var property = GetProperty(classPath, fieldName);
            property?.SetValue(
                property.SetMethod is null || property.SetMethod.IsStatic ? null : GetInstance(classPath), value
            );
        }
    }

    internal static void SetValue(string classPath, string fieldName, object value) => SetValue<FieldInfo>(classPath, fieldName, value);

    internal static object? InvokeModContentMethod(string methodName, string genericClassPath)
    {
        var methodPath = $"{typeof(ModContent).FullName}.{methodName}";
        if (!MethodCache.TryGetValue(methodPath, out var methodInfo))
        {
            methodInfo = typeof(ModContent).GetMethod(methodName);
            if (methodInfo is null)
            {
                return null;
            }

            MethodCache.Add(methodPath, methodInfo);
        }

        var classInfo = GetClass(genericClassPath);
        if (classInfo is null)
        {
            return null;
        }

        var result = methodInfo.MakeGenericMethod(classInfo).Invoke(null, null);
        return result;
    }

    internal static object? GetInstance(string classPath) => InvokeModContentMethod("GetInstance", classPath);
    
    #endregion 反射

    #region 标识

    internal static ushort GetId(string methodName, string classPath)
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

        var result = InvokeModContentMethod(methodName, classPath);
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

    #endregion 标识
}