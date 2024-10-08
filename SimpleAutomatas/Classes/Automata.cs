using SimpleUtilities.Utilities.Files;
using File = SimpleUtilities.Utilities.Files.File;

namespace SimpleAutomatas.Classes {
    internal abstract class Automata {

        #region Variables

            private readonly Guid _guid;

            public State InitialState { get; private set; }
            public HashSet<State> States { get; }
            public HashSet<State> FinalStates { get; }
            public HashSet<char> Alphabet { get; }

        #endregion

        #region Constructors

            protected Automata(State initialState, HashSet<State> states, HashSet<State> finalStates, HashSet<char> alphabet) {
                _guid = Guid.NewGuid();

                InitialState = initialState;
                States = states;
                FinalStates = finalStates;
                Alphabet = alphabet;
            }

        #endregion

        #region Static methods

            public static Automata ReadFromFile(string path) {
                var lines = File.Read(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

                var statesByName = new Dictionary<string, State>();
                var statesByOrder = new Dictionary<int, State>();
                
                var finalStates = new HashSet<State>();

                var aplhabet = new List<char>();


                //READ ALL STATES

                if (lines[0][0] != '#') throw new ArgumentException("Invalid file format");

                var parts = lines[0].Split(' ');
                var numberStates = int.Parse(parts[0].TrimStart('#'));

                if(numberStates != parts.Length - 1) throw new ArgumentException("Number of states does not match");

                for (var i = 1; i < parts.Length; i++) {
                    if (statesByName.ContainsKey(parts[i])) continue;
                    var s = new State(parts[i]);

                    statesByName.Add(parts[i], s);
                    statesByOrder.Add(i - 1, s);
                }

                //READ FINAL STATES

                if (lines[1][0] != '#') throw new ArgumentException("Invalid file format");

                parts = lines[1].Split(' ');
                var numberFinalStates = int.Parse(parts[0].TrimStart('#'));

                if (numberFinalStates != parts.Length - 1) throw new ArgumentException("Number of final states does not match");

                for (var i = 1; i < parts.Length; i++) {
                    if (!statesByName.ContainsKey(parts[i])) throw new ArgumentException("State not found in the automata");
                    finalStates.Add(statesByName[parts[i]]);
                }

                //READ SYMBOLS OF THE ALPHABET

                if (lines[2][0] != '#') throw new ArgumentException("Invalid file format");

                parts = lines[2].Split(' ');
                var numberSymbols = int.Parse(parts[0].TrimStart('#'));

                if (numberSymbols != parts.Length - 1) throw new ArgumentException("Number of symbols does not match");

                for (var i = 1; i < parts.Length; i++) aplhabet.Add(parts[i][0]);

                //READ TRANSITIONS

                if (lines[3] != "--TABLA DE TRANSICIONES--") throw new ArgumentException("Invalid file format");

                var provTransitions = new Dictionary<(State, char), HashSet<State>>();
                var provEpsilonTransitions = new Dictionary<State, HashSet<State>>();
                var isNfa = false;

                for (var i = 4; i < lines.Length; i++) {
                    parts = lines[i].Split('#');

                    if (parts.Length - 1 != numberSymbols + 1) throw new ArgumentException("Invalid transition format");

                    for (var j = 0; j < parts.Length - 1; j++) {
                        if (string.IsNullOrEmpty(parts[j])) continue;

                        var destStats = parts[j].Split(' ');
                        if (destStats.Length > 1) isNfa = true;

                        //If we are on Epsilon column
                        if (j == parts.Length - 2) {
                            isNfa = true;

                            for (var k = 0; k < destStats.Length; k++) {
                                if (!statesByName.ContainsKey(destStats[k])) throw new ArgumentException("State not found in the automata");

                                var key = statesByOrder[i - 4];

                                if (k == 0) provEpsilonTransitions.Add(key, [statesByName[destStats[k]]]);
                                else provEpsilonTransitions[key].Add(statesByName[destStats[k]]);
                            }

                            continue;
                        }

                        for (var k = 0; k < destStats.Length; k++) {
                            if (!statesByName.ContainsKey(destStats[k])) throw new ArgumentException("State not found in the automata");

                            var key = (statesByOrder[i - 4], aplhabet[j]);

                            if (k == 0) provTransitions.Add(key, [statesByName[destStats[k]]]);
                            else provTransitions[key].Add(statesByName[destStats[k]]);
                        }
                    }
                }

                if(isNfa) {
                    return new Nfa(statesByOrder[0], new HashSet<State>(statesByName.Values.ToList()), finalStates, new HashSet<char>(aplhabet), provTransitions, provEpsilonTransitions);
                }
                else {

                    var definitiveTransitions = new Dictionary<(State, char), State>();

                    foreach (var t in provTransitions){
                        if (t.Value.Count > 1) throw new ArgumentException("Invalid transition for DFA");
                        definitiveTransitions.Add(t.Key, t.Value.First());
                    }

                    return new Dfa(statesByOrder[0], new HashSet<State>(statesByName.Values.ToList()), finalStates, new HashSet<char>(aplhabet), definitiveTransitions);
                }
            }

        #endregion

        #region Public methods

        public void SetInitialState(State state) {
                if (!States.Contains(state)) throw new ArgumentException("State not found in the automata");
                InitialState = state;
            }

            public void AddState(State state) => States.Add(state);
            public void RemoveState(State state) {
                if (state == InitialState) throw new ArgumentException("Cannot remove the initial state");
                States.Remove(state);
            }

            public void AddFinalState(State state) {
                if(!States.Contains(state)) throw new ArgumentException("State not found in the automata");
                FinalStates.Add(state);
            }
            public void RemoveFinalState(State state) => FinalStates.Remove(state);

            public void AddSymbol(char symbol) => Alphabet.Add(symbol);
            public void RemoveSymbol(char symbol) => Alphabet.Remove(symbol);

            public bool IsStateFinal(State state) => FinalStates.Contains(state);

            public override string ToString() {
                return $"""
                        Initial state: {InitialState}
                        States: {string.Join(", ", States)}
                        Final states: {string.Join(", ", FinalStates)}
                        Alphabet: {string.Join(", ", Alphabet)}
                        """;
            }
            public override bool Equals(object? obj) {
                if (obj == null || GetType() != obj.GetType()) return false;

                var automata = (Automata)obj;
                return InitialState == automata.InitialState && States.SetEquals(automata.States) && FinalStates.SetEquals(automata.FinalStates) && Alphabet.SetEquals(automata.Alphabet);
            }
            public override int GetHashCode() => _guid.GetHashCode();

        #endregion

        #region Abstract methods

            public abstract string GetTrace();

            public abstract bool Accepts(string input);
            public abstract bool Accepts(char[] input);

            public abstract bool Equivalent(Automata a);

            public abstract Automata Minimize();

        #endregion
    }
}