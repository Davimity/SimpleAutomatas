using System.Text;

namespace SimpleAutomatas.Classes {
    internal class Dfa : Automata {

        #region Variables

            public Dictionary<(State, char), State> Transitions { get; private set; }
            private List<(char, State)> _trace;

        #endregion

        #region Constructors

            public Dfa(State initialState, HashSet<State> states, HashSet<State> finalStates, HashSet<char> alphabet, 
                Dictionary<(State, char), State> transitions)
                : base(initialState, states, finalStates, alphabet) 
            {
                Transitions = transitions;
                _trace = [];
            }

        #endregion

        #region Public methods

            public void SetTransitions(Dictionary<(State, char), State> transitions) {

                if (transitions.Any(transition =>
                        !States.Contains(transition.Key.Item1) ||
                        !States.Contains(transition.Value) ||
                        !Alphabet.Contains(transition.Key.Item2))) {
                    throw new ArgumentException("State not found in the automata or symbol not found in the alphabet");
                }

                Transitions = transitions;
            }

            public override string GetTrace() {
                var trace = new StringBuilder();
                trace.Append($"({InitialState})");

                foreach (var (symbol, state) in _trace) {
                    trace.Append($" - {symbol} -> ({state})");
                }

                return trace.ToString();
            }

            public void AddTransition(State s1, char symbol, State s2) {
                if(!States.Contains(s1) || !States.Contains(s2)) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                Transitions.Add((s1, symbol), s2);
            }
            public void RemoveTransition(State s1, char symbol) {
                if (!States.Contains(s1)) throw new ArgumentException("State not found in the automata");
                if (!Alphabet.Contains(symbol)) throw new ArgumentException("Symbol not found in the alphabet");

                Transitions.Remove((s1, symbol));
            }

            public override bool Accepts(string input) {
                var currentState = InitialState;

                foreach (var symbol in input) {
                    if (!Alphabet.Contains(symbol)) throw new ArgumentException($"Symbol {symbol} not found on alphabet");

                    if (!Transitions.ContainsKey((currentState, symbol))) return false;
                    currentState = Transitions[(currentState, symbol)];

                    _trace.Add((symbol, currentState));
                }

                return IsStateFinal(currentState);
            }

            public override bool Accepts(char[] input) => Accepts(new string(input));

            public override bool Equivalent(Automata a) {
                return false;
            }

            public override Automata Minimize() {
                return new Dfa(InitialState, States, FinalStates, Alphabet, Transitions);
            }

            public override string ToString() {
                return $"""
                        {base.ToString()}
                        Transitions: {string.Join(", ", Transitions.Select(t => $"({t.Key.Item1}, {t.Key.Item2}) -> {t.Value}"))}
                        """;
            }

        #endregion

    }
}
