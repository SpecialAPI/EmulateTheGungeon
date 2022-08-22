using System;
using System.Collections.Generic;

public static class GunExt
{

    public static void SetName(this PickupObject item, string text)
    {
        InvokeStaticMTGMethod("GunExt", "SetName", item, text);
    }
    public static void SetShortDescription(this PickupObject item, string text)
    {
        InvokeStaticMTGMethod("GunExt", "SetShortDescription", item, text);
    }
    public static void SetLongDescription(this PickupObject item, string text)
    {
        InvokeStaticMTGMethod("GunExt", "SetLongDescription", item, text);
    }

    public static void UpdateAnimations(this Gun gun, tk2dSpriteCollectionData collection = null)
    {
        InvokeStaticMTGMethod("GunExt", "UpdateAnimations", gun, collection);
    }

    public static string UpdateAnimation(this Gun gun, string name, tk2dSpriteCollectionData collection = null, bool returnToIdle = false)
    {
        return (string)InvokeStaticMTGMethod("GunExt", "UpdateAnimations", gun, name, collection, returnToIdle);
    }

    public static void SetAnimationFPS(this Gun gun, int fps)
    {
        GetMTGType("GunExt").GetMethod("SetAnimationFPS", new Type[] { typeof(Gun), typeof(int) }).Invoke(null, new object[] { gun, fps });
    }
    public static void SetAnimationFPS(this Gun gun, string name, int fps)
    {
        GetMTGType("GunExt").GetMethod("SetAnimationFPS", new Type[] { typeof(Gun), typeof(string), typeof(int) }).Invoke(null, new object[] { gun, name, fps });
    }

    public static Projectile ClonedPrefab(this Projectile orig)
    {
        return orig;
    }
    public static Projectile EnabledClonedPrefab(this Projectile projectile)
    {
        return projectile;
    }

    public static Projectile AddProjectileFrom(this Gun gun, string other, bool cloned = true)
    {
        return (Projectile)InvokeStaticMTGMethod("GunExt", "AddProjectileFrom", gun, other, cloned);
    }
    public static Projectile AddProjectileFrom(this Gun gun, Gun other, bool cloned = true)
    {
        return (Projectile)InvokeStaticMTGMethod("GunExt", "AddProjectileFrom", gun, other, cloned);
    }
    public static Projectile AddProjectile(this Gun gun, Projectile projectile)
    {
        return (Projectile)InvokeStaticMTGMethod("GunExt", "AddProjectile", gun, projectile);
    }

    public static ProjectileModule AddProjectileModuleFrom(this Gun gun, string other, bool cloned = true, bool clonedProjectiles = true)
    {
        return (ProjectileModule)InvokeStaticMTGMethod("GunExt", "AddProjectileModuleFrom", gun, other, cloned, clonedProjectiles);
    }
    public static ProjectileModule AddProjectileModuleFrom(this Gun gun, Gun other, bool cloned = true, bool clonedProjectiles = true)
    {
        return (ProjectileModule)InvokeStaticMTGMethod("GunExt", "AddProjectileModuleFrom", gun, other, cloned, clonedProjectiles);
    }
    public static ProjectileModule AddProjectileModule(this Gun gun, ProjectileModule projectile)
    {
        return (ProjectileModule)InvokeStaticMTGMethod("GunExt", "AddProjectileModule", gun, projectile);
    }

    public static void SetupSprite(this Gun gun, tk2dSpriteCollectionData collection = null, string defaultSprite = null, int fps = 0)
    {
        InvokeStaticMTGMethod("GunExt", "SetupSprite", gun, collection, defaultSprite, fps);
    }

}