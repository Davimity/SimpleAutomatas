using System.Text;

namespace SimpleAutomatas.Classes {
    internal class Nfa : Automata {

        #region Variables

            public Dictionary<(State, char), HashSet<State>> Transitions { get; private set; }
            public Dictionary<State, HashSet<State>> EpsilonTransitions { get; private set; }

            private List<State> _traceInitialStates;
            private List<(char, List<State>)> _trace;


        #endregion

        #region Constructors

            public Nfa(State initialState, HashSet<State> states, HashSet<State> finalStates, HashSet<char> alphabet,
                Dictionary<(State, char), HashSet<State>> transitions, Dictionary<State, HashSet<State>> epsilonTransitions)
                : base(initialState, states, finalStates, alphabet) 
            {
                Transitions = transitions;
                EpsilonTransitions = epsilonTransitions;
            }

        #endregion

        #region Public methods

            public void SetTransitions(Dictionary<(State, char), HashSet<State>> transitions) {

                if (transitions.Any(t =>
                        !States.Contains(t.Key.Item1) ||
                        t.Value.Any(t2 => !States.Contains(t2)) ||
                        !Alphabet.Contains(t.Key.Item2))) {
                    throw new ArgumentException("State not found in the automata or symbol not found in the alphabet");
                }

                Transitions = transitions;
            }
            public void SetEpsilonTransitions(Dictionary<State, HashSet<State>> epsilonTransitions) {

                if (epsilonTransitions.Any(t =>
                        !States.Contains(t.Key) ||
                        t.Value.Any(t2 => !States.Contains(t2)))) {
                    throw new ArgumentException("State not found in the automata");
                }

                EpsilonTransitions = epsilonTransitions;
            }

            public void AddTransition(State s1, char symbol, State s2) {
                if (!States.Contains(s1) || !States.Contains(s2)) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                if (!Transitions.ContainsKey((s1, symbol))) Transitions.Add((s1, symbol), [s2]);
                else Transitions[(s1, symbol)].Add(s2);
            }
            public void AddEpsilonTransition(State s1, State s2) {
                if (!States.Contains(s1) || !States.Contains(s2)) throw new ArgumentException("State not found in the automata");

                if (!EpsilonTransitions.ContainsKey(s1)) EpsilonTransitions.Add(s1, [s2]);
                else EpsilonTransitions[s1].Add(s2);
            }

            public void AddTransitions(State s1, char symbol, HashSet<State> s2) {
                if (!States.Contains(s1) || s2.Any(t => !States.Contains(t))) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                Transitions[(s1, symbol)].UnionWith(s2);
            }
            public void AddEpsilonTransitions(State s1, HashSet<State> s2) {
                if (!States.Contains(s1) || s2.Any(t => !States.Contains(t))) throw new ArgumentException("State not found in the automata");

                EpsilonTransitions[s1].UnionWith(s2);
            }

            public void RemoveTransition(State s1, char symbol) {
                if (!States.Contains(s1)) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                Transitions.Remove((s1, symbol));
            }
            public void RemoveEpsilonTransition(State s1) {
                if (!States.Contains(s1)) throw new ArgumentException("State not found in the automata");

                EpsilonTransitions.Remove(s1);
            }

            public void RemoveTransitions(State s1, char symbol, HashSet<State> s2) {
                if (!States.Contains(s1) || s2.Any(t => !States.Contains(t))) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                Transitions[(s1, symbol)].ExceptWith(s2);
            }
            public void RemoveEpsilonTransitions(State s1, HashSet<State> s2) {
                if (!States.Contains(s1) || s2.Any(t => !States.Contains(t))) throw new ArgumentException("State not found in the automata");

                EpsilonTransitions[s1].ExceptWith(s2);
            }

            public override string GetTrace() {
                var sb = new StringBuilder();

                sb.Append($"({string.Join(", ", _traceInitialStates)})");

                foreach (var s in _trace)
                    sb.Append($" - {s.Item1} -> ({string.Join(", ", s.Item2)})");

                return sb.ToString();
            }

            public override bool Accepts(string input) {
                var currentStates = EpsilonClosure([InitialState]);

                _traceInitialStates = [];
                _traceInitialStates.AddRange(currentStates.ToList());

                _trace = [];



                foreach (var symbol in input) {
                    var nextStates = new HashSet<State>();

                    foreach (var state in currentStates) 
                        if (Transitions.ContainsKey((state, symbol))) nextStates.UnionWith(Transitions[(state, symbol)]);
                    
                    currentStates = EpsilonClosure(nextStates);

                    _trace.Add((symbol, currentStates.ToList()));

                    if (currentStates.Count == 0) return false;
                }

                return currentStates.Any(IsStateFinal);
            }

            public override bool Accepts(char[] input) => Accepts(new string(input));

            public override bool Equivalent(Automata a) {
                return false;
            }

            public override Automata Minimize() {
                return new Nfa(InitialState, States, FinalStates, Alphabet, Transitions, EpsilonTransitions);
            }

            public override string ToString() {
            return $"""
                        {base.ToString()}
                        Transitions: {string.Join(", ", Transitions.Select(t => $"({t.Key.Item1}, {t.Key.Item2}) -> {string.Join(", ", t.Value)}"))}
                        Epsilon transitions: {string.Join(", ", EpsilonTransitions.Select(t => $"{t.Key} -> {string.Join(", ", t.Value)}"))}
                        """;
            }

        #endregion

        #region Private methods

        private HashSet<State> EpsilonClosure(HashSet<State> states) {
                var stack = new Stack<State>(states);
                var closure = new HashSet<State>(states);

                while (stack.Count > 0) {
                    var currentState = stack.Pop();

                    if(!EpsilonTransitions.ContainsKey(currentState)) continue;

                    foreach (var epsilonState in EpsilonTransitions[currentState]) {
                        if(!closure.Add(epsilonState)) continue;

                        stack.Push(epsilonState);
                    }
                    
                }

                return closure;
            }

        #endregion
    }
}
