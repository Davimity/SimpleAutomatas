namespace SimpleAutomatas.Classes {
    internal class State {

        #region Variables

            private readonly Guid _guid;

            private string _name;
            private string _description;

        #endregion

        #region Constructors

            public State(string name, string description = "") {
                _guid = Guid.NewGuid();

                _name = name;
                _description = description;
            }

        #endregion

        #region Operators

            public static bool operator ==(State a, State b) => a._name == b._name;

            public static bool operator !=(State a, State b) => !(a == b);

        #endregion

        #region Public methods

            public string GetName() => _name;
            public string GetDescription() => _description;

            public void SetDescription(string description) => _description = description;
            public void SetName(string newName) => _name = newName;

            public override string ToString() => _name;
            public override bool Equals(object? obj) {
                if (obj == null || GetType() != obj.GetType()) return false;

                var state = (State)obj;
                return _name == state._name;
            }
            public override int GetHashCode() => _guid.GetHashCode();

        #endregion

    }
}