mergeInto(LibraryManager.library, {
  SendMessageToWeb: function (str) {
    const value = UTF8ToString(str);
    console.log("Got message from Unity: " + value);
    console.log(value);
    window.parent.postMessage(value, '*');
  },
});
