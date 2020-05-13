using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    internal class StateMachine<T>
    {
        private static T defaultT = default;

        public MachineState<T> AnyState { get; private set; }

        private List<Param> parameters;
        private List<MachineState<T>> states;
        private MachineState<T> currentState;
        private MachineState<T> nextState;

        public StateMachine()
        {
            AnyState = new MachineState<T>("AnyState", null);
            parameters = new List<Param>();
            states = new List<MachineState<T>>() { AnyState };
        }

        public void SetCurrenState(MachineState<T> newState)
        {
            if (currentState != null)
                currentState = newState;
        }

        public void AddState(MachineState<T> state)
        {
            if (!states.Contains(state) && state != AnyState)
            {
                //StopAnimation(state.Animation);
                states.Add(state);

                if (currentState == null)
                {
                    currentState = state;
                    //currentAnimatorState?.Animation.Start();
                }
            }
        }

        public void AddParameter(Param parameter)
        {
            if (!parameters.Contains(parameter)) parameters.Add(parameter);
        }

        public void CreateConnection(MachineState<T> from, MachineState<T> to, params Condition[] conditions)
        {
            if (from == to) return;
            if (to == AnyState) throw new Exception($"You can not connect \"{from.Name}\" to *AnyState*");

            AddState(from);
            AddState(to);

            //from.CreateConnection(to, conditions.ToList());
            foreach (Condition condition in conditions)
                AddParameter(condition.Parameter);
        }

        #region Params IO
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

            if (param is EnumParameter eParam && eParam.Value.GetType() == value.GetType()) eParam.Value = value;
        }

        public void SetParam(string name, object value)
        {
            GetParam(name, out Param param);

            if (param != null) param.SetValue(value);
        }


        private void GetParam(string name, out Param param) => param = parameters.Find((_param) => _param.Name == name);
        #endregion
    }
}
