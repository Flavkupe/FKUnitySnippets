mergeInto(LibraryManager.library, {
  SendMessageToWeb: function (str) {
    const value = UTF8ToString(str);
    window.parent.postMessage(value, '*');
  },
});
