(function () {
  "use strict";

  document.querySelectorAll(".dialogue-demo").forEach(function (root) {
    var lines;
    var body = root.querySelector(".dialogue-body");
    var button = root.querySelector(".dialogue-next");
    var status = root.querySelector(".dialogue-status");
    var lineIndex = 0;
    var charIndex = 0;
    var timer = null;
    var typing = false;

    try {
      lines = JSON.parse(root.getAttribute("data-lines") || "[]");
    } catch (error) {
      lines = [];
    }

    if (!body || !button || status == null || lines.length === 0) {
      return;
    }

    function updateStatus() {
      status.textContent = "对话 " + (lineIndex + 1) + " / " + lines.length;
    }

    function stopTimer() {
      if (timer != null) {
        window.clearInterval(timer);
        timer = null;
      }
    }

    function finishLine() {
      stopTimer();
      typing = false;
      charIndex = lines[lineIndex].length;
      body.textContent = lines[lineIndex];
      button.textContent = lineIndex + 1 < lines.length ? "下一句" : "结束对话";
      updateStatus();
    }

    function startLine() {
      stopTimer();
      typing = true;
      charIndex = 0;
      body.textContent = "";
      button.textContent = "全部显示";
      updateStatus();

      timer = window.setInterval(function () {
        charIndex += 1;
        body.textContent = lines[lineIndex].slice(0, charIndex);

        if (charIndex >= lines[lineIndex].length) {
          finishLine();
        }
      }, 38);
    }

    button.addEventListener("click", function () {
      if (typing) {
        finishLine();
        return;
      }

      if (lineIndex + 1 < lines.length) {
        lineIndex += 1;
        startLine();
        return;
      }

      lineIndex = 0;
      startLine();
    });

    startLine();
  });
})();
