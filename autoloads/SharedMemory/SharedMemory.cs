using Godot;
using Godot.Collections;

namespace LD48 {
    public class SharedMemory : Node
    {
        private Dictionary<string, object> temporaryData = new Dictionary<string, object>();

        private static SharedMemory globalInstance;
        public static SharedMemory Global {
            get => globalInstance;
        }

        public override void _Ready()
        {
            if (globalInstance == null) {
                globalInstance = this;
            }
        }

        public void StoreTemporaryData<T>(string key, T value) {
            temporaryData[key] = value;
        }

        public T LoadTemporaryData<T>(string key, T defaultValue) {
            if (temporaryData.ContainsKey(key)) {
                return (T)temporaryData[key];
            } else {
                return defaultValue;
            }
        }
    }
}
