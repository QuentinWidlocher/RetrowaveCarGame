using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Helpers
{
    public static class Nullable
    {
        public static void AssertNotNull<T>([NotNull] T? x) where T : class
        {
            if (x == null) throw new Exception();
        }

        public static T ReturnIfNotNull<T>([NotNull] T? x) where T : class
        {
            AssertNotNull(x);
            return x;
        }

        public static bool IsNullOrEmpty(this string instance)
        {
            return string.IsNullOrEmpty(instance);
        }
    }

    public static class SpatialExtentions
    {
        public static void SetOrigin(this Spatial instance, Vector3 origin)
        {
            var t = instance.Transform;
            t.origin = origin;
            instance.Transform = t;
        }

        public static void SetOriginX(this Spatial instance, float x)
        {
            instance.SetOrigin(new Vector3(
                x,
                instance.Transform.origin.y,
                instance.Transform.origin.z
            ));
        }

        public static void SetOriginY(this Spatial instance, float y)
        {
            instance.SetOrigin(new Vector3(
                instance.Transform.origin.x,
                y,
                instance.Transform.origin.z
            ));
        }

        public static void SetOriginZ(this Spatial instance, float z)
        {
            instance.SetOrigin(new Vector3(
                instance.Transform.origin.x,
                instance.Transform.origin.y,
                z
            ));
        }

        public static void SetGlobalOrigin(this Spatial instance, Vector3 origin)
        {
            var t = instance.GlobalTransform;
            t.origin = origin;
            instance.GlobalTransform = t;
        }

        public static void SetGlobalOriginX(this Spatial instance, float x)
        {
            instance.SetGlobalOrigin(new Vector3(
                x,
                instance.GlobalTransform.origin.y,
                instance.GlobalTransform.origin.z
            ));
        }

        public static void SetGlobalOriginY(this Spatial instance, float y)
        {
            instance.SetGlobalOrigin(new Vector3(
                instance.GlobalTransform.origin.x,
                y,
                instance.GlobalTransform.origin.z
            ));
        }

        public static void SetGlobalOriginZ(this Spatial instance, float z)
        {
            instance.SetGlobalOrigin(new Vector3(
                instance.GlobalTransform.origin.x,
                instance.GlobalTransform.origin.y,
                z
            ));
        }

        public static void SetBasis(this Spatial instance, Basis basis)
        {
            var t = instance.Transform;
            t.basis = basis;
            instance.Transform = t;
        }

        public static void SetBasisX(this Spatial instance, Vector3 x)
        {
            instance.SetBasis(new Basis(
                x,
                instance.Transform.basis.y,
                instance.Transform.basis.z
            ));
        }

        public static void SetBasisY(this Spatial instance, Vector3 y)
        {
            instance.SetBasis(new Basis(
                instance.Transform.basis.x,
                y,
                instance.Transform.basis.z
            ));
        }

        public static void SetBasisZ(this Spatial instance, Vector3 z)
        {
            instance.SetBasis(new Basis(
                instance.Transform.basis.x,
                instance.Transform.basis.y,
                z
            ));
        }

        public static void SetGlobalBasis(this Spatial instance, Basis basis)
        {
            var t = instance.GlobalTransform;
            t.basis = basis;
            instance.GlobalTransform = t;
        }

        public static void SetGlobalBasisX(this Spatial instance, Vector3 x)
        {
            instance.SetGlobalBasis(new Basis(
                x,
                instance.GlobalTransform.basis.y,
                instance.GlobalTransform.basis.z
            ));
        }

        public static void SetGlobalBasisY(this Spatial instance, Vector3 y)
        {
            instance.SetGlobalBasis(new Basis(
                instance.GlobalTransform.basis.x,
                y,
                instance.GlobalTransform.basis.z
            ));
        }

        public static void SetGlobalBasisZ(this Spatial instance, Vector3 z)
        {
            instance.SetGlobalBasis(new Basis(
                instance.GlobalTransform.basis.x,
                instance.GlobalTransform.basis.y,
                z
            ));
        }
    }

    public static class TransformExtentions
    {
        public static Transform LookingAtWithY(this Transform instance, Vector3 newY)
        {
            var newInstance = instance;
            newInstance.basis.y = newY;
            newInstance.basis.x = -newInstance.basis.z.Cross(newY);
            newInstance.basis = newInstance.basis.Orthonormalized();
            return newInstance;
        }
    }

    public static class NodeExtentions
    {
        public static IEnumerable<T> GetChildrenWhere<T>(this Node instance, Func<T, bool> predicate)
        {
            return instance.GetChildren().Cast<T>().Where(predicate);
        }
    }

    public static class AnimationPlayerExtentions
    {
        public static Task RunAnimationThen(
            this AnimationPlayer instance,
            string animationToPlay,
            Action thingToDoAfter
        )
        {
            instance.CurrentAnimation = animationToPlay;

            return Task.Run(async () =>
            {
                await Task.Delay(Convert.ToInt32(instance.CurrentAnimationLength * 1000));
                thingToDoAfter();
            });
        }
    }
}