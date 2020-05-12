﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    public class Animator<T>
    {
        private static T defaultT = default;
        private static Animation<T> defaultAnim = default;

        public Animation<T> CurrentAnimation => currentAnimatorState == null ? defaultAnim : currentAnimatorState.Animation;
        public T CurrentAnimationValue => currentAnimatorState == null ? defaultT : currentAnimatorState.Animation.Value;
        public AnimatorState<T> CurrentAnimatorState
        {
            get => currentAnimatorState;
            set
            {
                if (currentAnimatorState != null) StopAnimation(currentAnimatorState.Animation);
                currentAnimatorState = value;
                currentAnimatorState.Animation.Start();
            }
        }
        public AnimatorState<T> AnyState { get; private set; }

        private AnimatorState<T> currentAnimatorState;
        private List<Param> parameters;
        private List<AnimatorState<T>> animatorStates;

        private AnimatorState<T> nextState;

        public Animator()
        {
            parameters = new List<Param>();
            animatorStates = new List<AnimatorState<T>>();
            AnyState = new AnimatorState<T>("AnyState", null);
            animatorStates.Add(AnyState);
        }

        public void SetCurrentAnimatorState(AnimatorState<T> newState)
        {
            if (currentAnimatorState != null) StopAnimation(currentAnimatorState.Animation);
            currentAnimatorState = newState;
        }

        public void AddState(AnimatorState<T> state)
        {
            if (!animatorStates.Contains(state) && state != AnyState)
            {
                StopAnimation(state.Animation);
                animatorStates.Add(state);

                if (currentAnimatorState == null)
                {
                    currentAnimatorState = state;
                    currentAnimatorState?.Animation.Start();
                }
            }
        }

        public void AddParameter(Param parameter)
        {
            if (!parameters.Contains(parameter)) parameters.Add(parameter);
        }

        public void CreateConnection(AnimatorState<T> from, AnimatorState<T> to, bool useExitTime, bool resetAnimation,  params Condition[] conditions)
        {
            if (from == to) return;
            if (to == AnyState) throw new Exception($"You can not connect \"{from.Name}\" to *AnyState*");

            AddState(from);
            AddState(to);

            from.CreateConnection(to, useExitTime, resetAnimation, conditions.ToList());
            foreach (Condition condition in conditions)
                AddParameter(condition.Parameter);
        }

        public void Update(float elapsed)
        {
            if (currentAnimatorState == null) return;
            
            CurrentAnimation.Update(elapsed);

            nextState = AnyState.FindNextState();
            if (nextState == currentAnimatorState) nextState = null;
            if (nextState == null) nextState = currentAnimatorState.FindNextState();
            if (nextState != null)
            {
                Connection<T> connection = currentAnimatorState.FindConnection(nextState);
                if (connection != null && (connection.ResetAnimation || connection.HasExitTime)) StopAnimation(currentAnimatorState.Animation);
                currentAnimatorState = nextState;
                currentAnimatorState.Animation?.Start();
            }
        }

        private void StopAnimation(Animation<T> animation)
        {
            if (animation == null) return;

            animation.Reset();
            animation.Stop();
            animation.Automatic = false;
        }

        #region Params IO
        //public float GetFloatValue(string name)
        //{
        //    GetParam(name, out Param param);

        //    if (param != null) return param.Value;
        //    else return -1f;
        //}

        //public bool GetBoolValue(string name)
        //{
        //    GetParam(name, out Param param);

        //    if (param != null) return param.Value == 1f ? true : false;
        //    else return false;
        //}

        public void SetFloat(string name, float value)
        {
            GetParam(name, out Param param);

            if (param is FloatParameter bParam) bParam.Value = value;
        }

        public void SetBool(string name, bool value)
        {
            GetParam(name, out Param param);

            if (param is BoolParameter bParam) bParam.Value = value;
        }
        public void SetEnum(string name, Enum value)
        {
            GetParam(name, out Param param);

            if (param is EnumParameter eParam && eParam.Value.Equals(value)) eParam.Value = value;
        }

        public void SetParam<T1>(string name, T1 value) where T1 : Parameter<T1>
        {
            GetParam(name, out Param param);

            if (param is T1 t) t.Value = value;
        }


        private void GetParam(string name, out Param param) => param = parameters.Find((_param) => _param.Name == name);
        #endregion
    }

    public class AnimatorState<T>
    {
        public string Name;
        public Animation<T> Animation;

        private List<Connection<T>> connections;

        public AnimatorState(string name, Animation<T> animation, params Connection<T>[] connections)
        {
            Name = name;
            Animation = animation;
            this.connections = connections.ToList();
        }

        public AnimatorState<T> FindNextState()
        {
            foreach (Connection<T> connection in connections)
            {
                if (connection.AllConditionsTrue())
                {
                    if (connection.HasExitTime && Animation.HaveLoopedOnce)
                        return connection.ConnectedTo;
                    else if (!connection.HasExitTime)
                        return connection.ConnectedTo;
                }
            }

            return null;
        }

        internal void CreateConnection(AnimatorState<T> to, bool useExitTime, bool resetAnimation, List<Condition> conditions)
        {
            if (connections.Exists((connection) => connection.ConnectedTo == to)) return;

            this.connections.Add(new Connection<T>(this, to, conditions) { HasExitTime = useExitTime, ResetAnimation = resetAnimation });
        }

        public Connection<T> FindConnection(AnimatorState<T> other)
        {
            return connections.Find((connection) => connection.ConnectedTo == other);
        }
    }

    public class Connection<T>
    {
        public AnimatorState<T> ConnectedFrom;
        public AnimatorState<T> ConnectedTo;
        public bool ResetAnimation;
        public bool HasExitTime;

        private List<Condition> conditions;

        public Connection(AnimatorState<T> connectedFrom, AnimatorState<T> connectedTo) : this(connectedFrom, connectedTo, new List<Condition>())
        {
        }

        public Connection(AnimatorState<T> connectedFrom, AnimatorState<T> connectedTo, List<Condition> conditions)
        {
            this.conditions = conditions;
            this.ConnectedFrom = connectedFrom;
            this.ConnectedTo = connectedTo;
            ResetAnimation = true;
            HasExitTime = false;
        }

        public void AddCondition(Condition condition)
        {
            if (!conditions.Contains(condition)) return;

            conditions.Add(condition);
        }

        public bool AllConditionsTrue()
        {
            return conditions.TrueForAll((condition) => condition.ConditionIsMet()) || conditions.Count == 0;
        }
    }

    #region Conditions
    public interface Param
    {
        string Name { get; set; }
        bool IsLessThan(object other);
        bool IsMoreThan(object other);
        bool IsEqualTo(object other);
    }

    public interface Parameter<T> : Param
    {
        T Value { get; set; }
    }

    public class FloatParameter : Parameter<float>
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public bool IsEqualTo(object other)
        {
            throw new NotImplementedException();
        }

        public bool IsLessThan(object other)
        {
            throw new NotImplementedException();
        }

        public bool IsMoreThan(object other)
        {
            throw new NotImplementedException();
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
    }

    public class EnumParameter : Parameter<Enum>
    {
        public string Name { get; set; }
        public Enum Value { get; set; }

        public bool IsEqualTo(object other)
        {
            throw new NotImplementedException();
        }

        public bool IsLessThan(object other)
        {
            throw new NotImplementedException();
        }

        public bool IsMoreThan(object other)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum ConditionType
    {
        /// <summary>
        /// Less than
        /// </summary>
        LSS = 0b0001,
        /// <summary>
        /// Greater than
        /// </summary>
        QTR = 0b0010,
        /// <summary>
        /// Is Not
        /// </summary>
        NOT = 0b0100,
        /// <summary>
        /// Equals
        /// </summary>
        EQU = 0b1000,
        /// <summary>
        /// Less or Equals than
        /// </summary>
        LEQ = 0b1001,
        /// <summary>
        /// Greater or Equals than
        /// </summary>
        GEQ = 0b1010,
        /// <summary>
        /// Not Equals
        /// </summary>
        NEQ = 0b1100
    }

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

            return $"({Parameter.ToString()} {_operator} {CompareTo}) = {ConditionIsMet()}";
        }
    }
    //public class FloatParameter
    //{
    //    public string Name;
    //    public float Value;

    //    public FloatParameter(string name)
    //    {
    //        Name = name;
    //    }

    //    public FloatParameter(string name, float value)
    //    {
    //        Value = value;
    //    }
    //}

    //public class BoolParameter : FloatParameter
    //{
    //    public BoolParameter(string name) : base(name)
    //    {
    //    }

    //    public BoolParameter(string name, bool value) : base(name)
    //    {
    //        Value = value ? 1f : 0f;
    //    }

    //    public void ChangeValue(bool value)
    //    {
    //        Value = value ? 1f : 0f;
    //    }

    //    public void ChangeValue(float value)
    //    {
    //        Value = value == 1f ? 1f : 0f;
    //    }
    //}
    //public class Condition
    //{
    //    public FloatParameter Parameter;
    //    public float CompareTo;
    //    public ConditionType ConditionType;

    //    public Condition(FloatParameter parameter, float compareTo, ConditionType condition)
    //    {
    //        Parameter = parameter;
    //        CompareTo = compareTo;
    //        ConditionType = condition;
    //    }

    //    public Condition(FloatParameter parameter, bool compareTo, ConditionType condition)
    //    {
    //        Parameter = parameter;
    //        CompareTo = compareTo ? 1f : 0f;
    //        ConditionType = condition;
    //    }

    //    public bool ConditionIsMet()
    //    {
    //        bool isMet = false;

    //        if ((ConditionType & ConditionType.LSS) == ConditionType.LSS && !isMet) isMet = Parameter.Value < CompareTo;
    //        if ((ConditionType & ConditionType.QTR) == ConditionType.QTR && !isMet) isMet = Parameter.Value > CompareTo;
    //        if ((ConditionType & ConditionType.EQU) == ConditionType.EQU && !isMet) isMet = Parameter.Value == CompareTo;
    //        if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) isMet = !isMet;

    //        return isMet;
    //    }

    //    public override string ToString()
    //    {
    //        string _operator = "";

    //        bool IsFloatToBool(float value)
    //        {
    //            return value == 1f ? true : false;
    //        }

    //        if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) _operator += '!';
    //        if ((ConditionType & ConditionType.LSS) == ConditionType.LSS) _operator += '<';
    //        if ((ConditionType & ConditionType.QTR) == ConditionType.QTR) _operator += '>';
    //        if ((ConditionType & ConditionType.EQU) == ConditionType.EQU) _operator += '=';

    //        if (_operator == "=") _operator += '=';

    //        if (Parameter is BoolParameter bParam)
    //            return $"({IsFloatToBool(Parameter.Value)} {_operator} {IsFloatToBool(CompareTo)}) = {ConditionIsMet()}";
    //        else
    //            return $"({Parameter.Value} {_operator} {CompareTo}) = {ConditionIsMet()}";
    //    }
    //}
    #endregion
}
