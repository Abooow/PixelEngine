using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    public class Animator<T>
    {
        public Animation<T> CurrentAnimation => currentAnimatorState.Animation;
        public T CurrentAnimationValue => currentAnimatorState.Animation.Value;
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

        private AnimatorState<T> currentAnimatorState;
        private List<FloatParameter> parameters;
        private List<AnimatorState<T>> animatorStates;

        private AnimatorState<T> nextState;

        public Animator()
        {
            parameters = new List<FloatParameter>();
            animatorStates = new List<AnimatorState<T>>();
        }

        public void SetCurrentAnimatorState(AnimatorState<T> newState)
        {
            if (currentAnimatorState != null) StopAnimation(currentAnimatorState.Animation);
            currentAnimatorState = newState;
        }

        public void AddState(AnimatorState<T> state)
        {
            if (!animatorStates.Contains(state))
            {
                StopAnimation(state.Animation);
                animatorStates.Add(state);

                if (currentAnimatorState == null)
                {
                    currentAnimatorState = state;
                    currentAnimatorState.Animation.Start();
                }
            }
        }

        public void AddParameter(FloatParameter parameter)
        {
            if (!parameters.Contains(parameter)) parameters.Add(parameter);
        }

        public void CreateConnection(AnimatorState<T> from, AnimatorState<T> to, bool useExitTime, params Condition[] conditions)
        {
            AddState(from);
            AddState(to);

            from.CreateConnection(to, useExitTime, conditions.ToList());
            foreach (Condition condition in conditions)
                AddParameter(condition.Parameter);
        }

        public void Update(float elapsed)
        {
            if (currentAnimatorState == null) return;

            CurrentAnimation.Update(elapsed);

            nextState = currentAnimatorState.FindNextState();
            if (nextState != null)
            {
                StopAnimation(currentAnimatorState.Animation);
                currentAnimatorState = nextState;
                currentAnimatorState.Animation.Start();
            }
        }

        private void StopAnimation(Animation<T> animation)
        {
            animation.Reset();
            animation.Stop();
            animation.Automatic = false;
        }

        #region Params IO
        public float GetFloatValue(string name)
        {
            GetParam(name, out FloatParameter param);

            if (param != null) return param.Value;
            else return -1f;
        }

        public bool GetBoolValue(string name)
        {
            GetParam(name, out FloatParameter param);

            if (param != null) return param.Value == 1f ? true : false;
            else return false;
        }

        public void SetFloat(string name, float value)
        {
            GetParam(name, out FloatParameter param);

            if (param is BoolParameter bParam) bParam.ChangeValue(value);
            else if (param != null) param.Value = value;
        }

        public void SetBool(string name, bool value)
        {
            GetParam(name, out FloatParameter param);

            if (param is BoolParameter bParam) bParam.ChangeValue(value);
            else if (param != null) param.Value = value ? 1f : 0f;
        }

        private void GetParam(string name, out FloatParameter param) => param = parameters.Find((_param) => _param.Name == name);
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

        internal void CreateConnection(AnimatorState<T> to, bool useExitTime, List<Condition> conditions)
        {
            if (connections.Exists((connection) => connection.ConnectedTo == to)) return;

            this.connections.Add(new Connection<T>(this, to, conditions) { HasExitTime = useExitTime });
        }
    }

    public class Connection<T>
    {
        public AnimatorState<T> ConnectedFrom;
        public AnimatorState<T> ConnectedTo;
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
    public class FloatParameter
    {
        public string Name;
        public float Value;

        public FloatParameter(string name)
        {
            Name = name;
        }

        public FloatParameter(string name, float value)
        {
            Value = value;
        }
    }

    public class BoolParameter : FloatParameter
    {
        public BoolParameter(string name) : base(name)
        {
        }

        public BoolParameter(string name, bool value) : base(name)
        {
            Value = value ? 1f : 0f;
        }

        public void ChangeValue(bool value)
        {
            Value = value ? 1f : 0f;
        }

        public void ChangeValue(float value)
        {
            Value = value == 1f ? 1f : 0f;
        }
    }

    public enum ConditionType
    {
        LSS = 0b0001,  // Less than
        QTR = 0b0010,  // Greater than
        NOT = 0b0100, // Is Not
        EQU = 0b1000, // Equals
        LEQ = 0b1001, // Less or Equals than
        GEQ = 0b1010, // Greater or Equals than
        NEQ = 0b1100  // Not Equals
    }

    public class Condition
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

        public Condition(FloatParameter parameter, bool compareTo, ConditionType condition)
        {
            Parameter = parameter;
            CompareTo = compareTo ? 1f : 0f;
            ConditionType = condition;
        }

        public bool ConditionIsMet()
        {
            bool isMet = false;

            if ((ConditionType & ConditionType.LSS) == ConditionType.LSS && !isMet) isMet = Parameter.Value < CompareTo;
            if ((ConditionType & ConditionType.QTR) == ConditionType.QTR && !isMet) isMet = Parameter.Value > CompareTo;
            if ((ConditionType & ConditionType.EQU) == ConditionType.EQU && !isMet) isMet = Parameter.Value == CompareTo;
            if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) isMet = !isMet;

            return isMet;
        }

        public override string ToString()
        {
            string _operator = "";

            bool IsFloatToBool(float value)
            {
                return value == 1f ? true : false;
            }

            if ((ConditionType & ConditionType.NOT) == ConditionType.NOT) _operator += '!';
            if ((ConditionType & ConditionType.LSS) == ConditionType.LSS) _operator += '<';
            if ((ConditionType & ConditionType.QTR) == ConditionType.QTR) _operator += '>';
            if ((ConditionType & ConditionType.EQU) == ConditionType.EQU) _operator += '=';

            if (_operator == "=") _operator += '=';

            if (Parameter is BoolParameter bParam)
                return $"({IsFloatToBool(Parameter.Value)} {_operator} {IsFloatToBool(CompareTo)}) = {ConditionIsMet()}";
            else
                return $"({Parameter.Value} {_operator} {CompareTo}) = {ConditionIsMet()}";
        }
    }
    #endregion
}
