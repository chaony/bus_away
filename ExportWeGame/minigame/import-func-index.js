export const importSubFuncIndexList = [];
  export const tableSize = 220637;
  export function setImportGlobal(info) {info["primary"] = info["primary"] || {};info["primary"]["__stack_pointer"] = new WebAssembly.Global({
      value: "i32",
      mutable: true
    });info["primary"]["__stack_end"] = new WebAssembly.Global({
      value: "i32",
      mutable: true
    });info["primary"]["__stack_base"] = new WebAssembly.Global({
      value: "i32",
      mutable: true
    });info["primary"]["global$3"] = new WebAssembly.Global({
      value: "i32",
      mutable: true
    });};
  
  export function setStackPointer(info) {
    // if (info["primary"]["__stack_pointer"]) {
    //   info["primary"]["__stack_pointer"].value = 0;
    // }
  }
  
  export function setImportFunc(info, jCall) {
    
  }
  
  