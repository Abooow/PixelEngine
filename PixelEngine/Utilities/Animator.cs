using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    public class Animator
    {

    }

    public enum ConditionType
    {
        LSS = 0b0001,  // Less than
        QTR = 0b0010,  // Greater than
        NEQ  = 0b0100, // Not Equals
        EQU  = 0b1000, // Equals
        LEQ  = 0b1001, // Less or Equals than
        GEQ  = 0b1010  // Greater or Equals than
    }

    internal class AnimatorState
    {
        public string Name;
        public List<Connection> ConnectedTo;
    }

    internal struct Connection
    {
        private List<Condition> conditions;
        private AnimatorState connectedFrom;
        private AnimatorState connectedTo;
        private bool hasExitTime;

        public Connection(AnimatorState connectedFrom, AnimatorState connectedTo) : this(new List<Condition>(), connectedFrom, connectedTo)
        {
        }

        public Connection(List<Condition> conditions, AnimatorState connectedFrom, AnimatorState connectedTo)
        {
            this.conditions = conditions;
            this.connectedFrom = connectedFrom;
            this.connectedTo = connectedTo;
            hasExitTime = false;
        }
    }

    public class FloatParameter
    {
        public string Name;
        public float Value;

        public FloatParameter(string name)
        {
            Name = name;
        }
    }

    public class BoolParameter : FloatParameter 
    {
        public BoolParameter(string name) : base(name)
        {
        }

        public void ChangeValue(bool value)
        {
            Value = value ? 1f : 0f;
        }
    }

    internal class Condition
    {
        public FloatParameter Parameter;
        public float CompareTo;
        public ConditionType ConditionType;

        public Condition(FloatParameter parameter, float compareTo, ConditionType condition)
        {
            Parameter = parameter;
            CompareTo = compareTo;
            ConditionType = condition;
        }

        public bool ConditionIsMet()
        {
            bool isMet = false;

            if ((ConditionType & ConditionType.LSS) == ConditionType.LSS && !isMet) return Parameter.Value < CompareTo;
            if ((ConditionType & ConditionType.QTR) == ConditionType.QTR && !isMet) return Parameter.Value > CompareTo;
            if ((ConditionType & ConditionType.NEQ) == ConditionType.NEQ && !isMet) return Parameter.Value != CompareTo;
            if ((ConditionType & ConditionType.EQU) == ConditionType.EQU && !isMet) return Parameter.Value == CompareTo;

            return isMet;
        }
    }
}
