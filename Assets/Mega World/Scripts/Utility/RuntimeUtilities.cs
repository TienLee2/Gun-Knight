﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEditor;

namespace MegaWorld
{
    using UnityObject = UnityEngine.Object;

    public static class RuntimeUtilities
    {
        #region Reflection

        /// <summary>
        /// Gets all currently available assembly types derived from type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <returns>A list of all currently available assembly types derived from type <typeparamref name="T"/></returns>
#if UNITY_EDITOR
        public static IEnumerable<System.Type> GetAllTypesDerivedFrom<T>()
        {
#if UNITY_2019_2_OR_NEWER
            return UnityEditor.TypeCache.GetTypesDerivedFrom<T>();
#else
            return GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T)));
#endif
        }
#endif
        /// <summary>
        /// Helper method to get the first attribute of type <c>T</c> on a given type.
        /// </summary>
        /// <typeparam name="T">The attribute type to look for</typeparam>
        /// <param name="type">The type to explore</param>
        /// <returns>The attribute found</returns>
        public static T GetAttribute<T>(this System.Type type) where T : Attribute
        {
            Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
            return (T)type.GetCustomAttributes(typeof(T), false)[0];
        }

        public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            Expression body = expr;

            if (body is LambdaExpression)
                body = ((LambdaExpression)body).Body;

            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var fi = (FieldInfo)((MemberExpression)body).Member;
                    return fi.GetCustomAttributes(false).Cast<Attribute>().ToArray();
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }

            return sb.ToString();
        }

        public static void Destroy(UnityObject obj)
        {
            if (obj != null)
            {
                UnityObject.DestroyImmediate(obj);
            }
        }

        #endregion
    }
}