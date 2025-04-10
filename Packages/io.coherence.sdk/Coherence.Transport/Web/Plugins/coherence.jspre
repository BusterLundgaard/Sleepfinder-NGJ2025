/**
 * Initializes the dynCall_* function table lookups.
 * Creates dynCall_* functions using the getWasmTableEntry function if they don't exist.
 * @see https://discussions.unity.com/t/makedyncall-replacing-dyncall-in-unity-6/1543088
 * @returns {void}
*/
function initializeDynCalls() {
  Module.dynCall_vi = Module.dynCall_vi || function (cb, arg1) {
    return getWasmTableEntry(cb)(arg1);
  };
  Module.dynCall_vii = Module.dynCall_vii || function (cb, arg1, arg2) {
    return getWasmTableEntry(cb)(arg1, arg2);
  }
  Module.dynCall_viii = Module.dynCall_viii || function (cb, arg1, arg2, arg3) {
    return getWasmTableEntry(cb)(arg1, arg2, arg3);
  }
  Module.dynCall_viiii = Module.dynCall_viiii || function (cb, arg1, arg2, arg3, arg4) {
    return getWasmTableEntry(cb)(arg1, arg2, arg3, arg4);
  }
}

Module['preRun'].push(function () {
    initializeDynCalls();
});
