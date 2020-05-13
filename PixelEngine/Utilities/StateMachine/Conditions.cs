using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    #region Interfaces
    public interface Param
    {
        string Name { get; set; }

        void SetValue(object value);
        bool IsLessThan(object other);
        bool IsMoreThan(object other);
        bool IsEqualTo(object other);
        bool LogAnd(object other);
        string AsString();
    }

    public interface Parameter<T> : Param
    {
        T Value { get; set; }
    }
    #endregion

    #region Default Parameters
    public class FloatParameter : Parameter<float>
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public FloatParameter(string name) : this(name, 0f)
        {
        }

        public FloatParameter(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public void SetValue(object value)
        {
            if (value.IsNumeric(out double number)) Value = (float)number;
        }

        public bool IsLessThan(object other)
        {
            if (other == null)
                return false;

            bool isNum = other.IsNumeric(out double number);
            if (isNum) return Value < number;
            return false;
        }

        public bool IsMoreThan(object other)
        {
            if (other == null) return false;

            bool isNum = other.IsNumeric(out double number);
            if (isNum) return Value > number;
            return false;
        }

        public bool IsEqualTo(object other)
        {
            if (other == null) return false;

            bool isNum = other.IsNumeric(out double number);
            if (isNum) return Value == number;
            return false;
        }

        public bool LogAnd(object other)
        {
            return IsEqualTo(other);
        }

        public string AsString()
        {
            return Value.ToString();
        }
    }

    public class BoolParameter : Parameter<bool>
    {
        public string Name { get; set; }
        public bool Value { get; set; }

        public BoolParameter(string name) : this(name, false)
        {
        }

        public BoolParameter(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public void SetValue(object value)
        {
            if (value == null) return;

            bool isNum = value.IsNumeric(out double number);
            if (value is bool vBool) Value = vBool;
            if (isNum) Value = (number == 1) ? true : false;
        }

        public bool IsLessThan(object other)
        {
            if (other == null) return false;

            bool isNum = other.IsNumeric(out double number);
            if (other is bool oBool) return Value ? oBool ? false : false : true;
            if (isNum) return Value ? number == 1 ? false : number == 0 ? false : true : false;
            return false;
        }

        public bool IsMoreThan(object other)
        {
            if (other == null) return false;

            bool isNum = other.IsNumeric(out double number);
            if (other is bool oBool) return Value ? oBool ? false : true : false;
            if (isNum) return Value ? number == 1 ? false : number == 0 ? true : false : false;
            return false;
        }

        public bool IsEqualTo(object other)
        {
            if (other == null) return false;

            bool isNum = other.IsNumeric(out double number);
            if (other is bool oBool) return Value ? oBool ? true : false : !oBool ? true : false;
            if (isNum) return Value ? number == 1 ? true : number == 0 ? false : false : number == 0 ? true : false;
            return false;
        }

        public bool LogAnd(object other)
        {
            return IsEqualTo(other);
        }

        public string AsString()
        {
            return Value.ToString();
        }
    }

    public class EnumParameter : Parameter<Enum>
    {
        public string Name { get; set; }
        public Enum Value { get; set; }

        public EnumParameter(string name, Enum value)
        {
            Name = name;
            Value = value;
        }

        public void SetValue(object value)
        {
            if (value == null) return;

            if (value is Enum vEnum && vEnum.GetType() == Value.GetType()) Value = vEnum;
        }

        public bool IsEqualTo(object other)
        {
            if (other == null) return false;

            if (other is Enum oEnum && oEnum.GetType() == Value.GetType()) return Value.GetHashCode() == other.GetHashCode();
            return false;
        }

        public bool IsLessThan(object other)
        {
            if (other == null)
                return false;

            if (other is Enum oEnum && oEnum.GetType() == Value.GetType()) return Value.GetHashCode() < other.GetHashCode();
            return false;
        }

        public bool IsMoreThan(object other)
        {
            if (other == null)
                return false;

            if (other is Enum oEnum && oEnum.GetType() == Value.GetType()) return Value.GetHashCode() > other.GetHashCode();
            return false;
        }

        public bool LogAnd(object other)
        {
            if (other == null)
                return false;

            if (other is Enum oEnum && oEnum.GetType() == Value.GetType()) return (Value.GetHashCode() & other.GetHashCode()) == Value.GetHashCode();
            return false;
        }

        public string AsString()
        {
            return Value.ToString();
        }

    }
    #endregion

    public class Condition
    {
        public Param Parameter;
        public object CompareTo;
        public ConditionType ConditionType;

        public Condition(Param parameter, object compareTo, ConditionType condition)
        {
            Parameter = parameter;
            CompareTo = compareTo;
            ConditionType = condition;
        }

        public bool ConditionIsMet()
        {
            bool isMet = false;

            if ((ConditionType & ConditionType.AND) == ConditionType.AND && !isMet) isMet = Parameter.LogAnd(CompareTo);
            if ((ConditionType & ConditionType.LSS) == ConditionType.LSS && !isMet) isMet = Parameter.IsLessThan(CompareTo);
            if ((ConditionType & ConditionType.QTR) == ConditionType.QTR && !isMet) isMet = Parameter.IsMoreThan(CompareTo);
            if ((ConditionType & ConditionType.EQU) == ConditionType.EQU && !isMet) isMet = Parameter.IsEqualTo(CompareTo);
            if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) isMet = !isMet;

            return isMet;
        }

        public override string ToString()
        {
            string _operator = "";

            if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) _operator += '!';
            if ((ConditionType & ConditionType.LSS) == ConditionType.LSS) _operator += '<';
            if ((ConditionType & ConditionType.QTR) == ConditionType.QTR) _operator += '>';
            if ((ConditionType & ConditionType.EQU) == ConditionType.EQU) _operator += '=';

            if (_operator == "=") _operator += '=';

            return $"({Parameter.AsString()} {_operator} {CompareTo}) = {ConditionIsMet()}";
        }
    }
}
