using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EFunTech.Sms.Core
{
    public static partial class ObjectCopier
    {
        /*
        // Deep clone
        public static T DeepCopy<T>(this T source)
        {
            // 20151007 Norman, 
            //  不適用 clone model，因為必須加上 [Serializable] 才能使用 DeepCopy
            //  然而加上 [Serializable] 會導致傳入 webapi的 model 初始化錯誤
            //  目前無解法

            if (!typeof(T).IsSerializable)
            {
                // https://msdn.microsoft.com/en-us/library/4abbf6k0(v=vs.110).aspx
                throw new Exception(string.Format("{0} can not be serializable，maybe you need [Serializable] in class", typeof(T).Name));
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        */
        
        /// <summary>
        /// 可以拷貝物件，但是不知道為什麼拷貝Entity會失敗
        /// </summary>
        //public static object DeepCopy(this object obj)
        public static object DeepCopy(object obj)
        {
            // http://stackoverflow.com/questions/2545025/how-to-deep-copy-a-class-without-marking-it-as-serializable
            if (obj == null) return null;

            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {

                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, DeepCopy(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }
    }
}
