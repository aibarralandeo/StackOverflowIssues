using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var selection = new Class1(1);
            selection.SetEvaluationEnum(typeof(TestEnum));

            var selection2 = new Class2(4);
            selection2.SetParentObject(selection);
            selection2.AddTypeToList(typeof(TestEnum2), (int)TestEnum.Value1);
            selection2.AddTypeToList(typeof(TestEnum3), (int)TestEnum.Value3);
            selection2.Evaluate();
        }
    }

    public static class Demo
    {

        // Extracted from https://stackoverflow.com/a/1556977 and https://stackoverflow.com/a/1556970. Thanks to Mehrdad Afshari and Dave
        public static T? ToEnumSafe<T>(this string s) where T : struct
        {
            return (IsEnum<T>(s) ? (T?)Enum.Parse(typeof(T), s) : null);
        }

        public static T? ToEnumSafe<T>(this int s) where T : struct
        {
            return (IsEnum<T>(s) ? (T?)Enum.Parse(typeof(T), s.ToString()) : null);
        }

        public static bool IsEnum<T>(this string s)
        {
            return Enum.IsDefined(typeof(T), s);
        }

        public static bool IsEnum<T>(this int s)
        {
            return Enum.IsDefined(typeof(T), s);
        }

        public static int EnumToInt<TValue>(this TValue value) where TValue : Enum => (int)(object)value;
    }

    public class Class1 : BaseNestedNode
    {
        public Class1(int param1) : base(param1)
        {
        }

        public Class1(int param1, int param2) : base(param1, param2)
        {
        }
    }

    public class Class2 : BaseNestedNode
    {
        public Class2(int param1) : base(param1)
        {
        }

        public Class2(int param1, int param2) : base(param1, param2)
        {
        }
    }

    public class BaseNestedNode
    {
        private int searchedIndex;
        private int searchedChildIndex;

        private BaseNestedNode parentObject;
        private BaseNestedNode childObject;

        private Enum currentNode;
        private Enum childNode;

        private Dictionary<int, Type> typesList = new Dictionary<int, Type>();
        private Type evaluationEnum;
        private NodeType nodeType;

        public BaseNestedNode(int searchedIndex)
        {
            this.searchedIndex = searchedIndex;
            this.searchedChildIndex = default(int);
        }

        public BaseNestedNode(int searchedIndex, int searchedChildIndex)
        {
            this.searchedIndex = searchedIndex;
            this.searchedChildIndex = searchedChildIndex;
        }

        public BaseNestedNode(BaseNestedNode nodeObject, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Parent:
                    SetParentObject(nodeObject);
                    break;
                case NodeType.Child:
                    SetChildObject(nodeObject);
                    break;
            }
        }

        public void SetEvaluationEnum(Type evaluationType)
        {
            if (evaluationType.IsEnum)
            {
                evaluationEnum = evaluationType;
                //SetCurrentNode(Demo.ToEnumSafe<evaluationEnum>(searchedIndex));
            }
        }

        public void AddTypeToList(Type compareType, int key)
        {
            try
            {
                if (compareType.IsEnum)
                    typesList.Add(key, compareType.GetType());
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void Evaluate()
        {
            if (!IsChild()) return;

            var parentType = parentObject.GetCurrentNode();
            evaluationEnum = typesList.FirstOrDefault(x => x.Key == Demo.EnumToInt(parentType)).Value;

            //var value = EnumerationHelpers.ToEnumSafe<Convert.ChangeType(rrre, evaluationEnum)>(searchedIndex);
        }

        public Type GetEvaluationEnum() => evaluationEnum;

        public int GetSearchedIndex() => searchedIndex;

        public int GetChildIndex() => searchedChildIndex;

        public bool HasChild() => searchedChildIndex > default(int);
        public bool IsChild() => nodeType == NodeType.Child;

        public Enum GetCurrentNode() => currentNode;

        public Enum GetNextNode() => childNode;

        public void SetCurrentNode(Enum node) => currentNode = node;

        public void SetChildNode(Enum node) => childNode = node;

        public void SetParentObject(BaseNestedNode parent)
        {
            if (parentObject != parent)
            {
                parentObject = parent;
                parent.SetChildObject(this);
                this.nodeType = NodeType.Child;
            }
        }
        public void SetChildObject(BaseNestedNode child)
        {
            if (childObject != child)
            {
                childObject = child;
                child.SetParentObject(this);
            }
        }

        public Enum GetParentNode()
        {
            return (parentObject == null) ? null : parentObject.GetCurrentNode();
        }
    }

    public interface INode
    {
        void Initialize();
    }

    public enum NodeType
    {
        Parent = 1,
        Child = 2
    }

    public enum TestEnum
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 3
    }

    public enum TestEnum2
    {
        Value11 = 10,
        Value20 = 11,
        Value50 = 30,
    }

    public enum TestEnum3
    {
        Value5432 = 81290,
        Value31 = 38129,
        Value433 = 832190,
    }
}
