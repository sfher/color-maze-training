(function () {
  "use strict";

  function setFeedback(quiz, isCorrect) {
    var feedback = quiz.querySelector(".quiz-feedback");
    var why = quiz.querySelector(".quiz-why");

    if (feedback) {
      feedback.hidden = false;
      feedback.textContent = isCorrect ? "答对了。" : "还差一点，正确答案已标出。";
    }

    if (why) {
      why.hidden = false;
    }
  }

  document.querySelectorAll(".quiz").forEach(function (quiz) {
    var answer = Number(quiz.getAttribute("data-answer"));
    var options = Array.prototype.slice.call(quiz.querySelectorAll(".option"));

    options.forEach(function (option, index) {
      option.addEventListener("click", function () {
        if (quiz.classList.contains("answered")) {
          return;
        }

        quiz.classList.add("answered");
        options.forEach(function (item, itemIndex) {
          item.disabled = true;
          if (itemIndex === answer) {
            item.classList.add("correct");
          }
        });

        if (index !== answer) {
          option.classList.add("incorrect");
        }

        setFeedback(quiz, index === answer);
      });
    });
  });

  document.querySelectorAll(".checklist[data-key]").forEach(function (list) {
    var key = "teach:" + list.getAttribute("data-key");
    var boxes = Array.prototype.slice.call(list.querySelectorAll('input[type="checkbox"]'));
    var progress = document.querySelector(".progress");

    function updateProgress() {
      if (!progress) {
        return;
      }

      var done = boxes.filter(function (box) {
        return box.checked;
      }).length;

      progress.textContent = "已完成 " + done + " / " + boxes.length;
    }

    boxes.forEach(function (box, index) {
      try {
        box.checked = window.localStorage.getItem(key + ":" + index) === "1";
      } catch (error) {
        box.checked = false;
      }

      box.addEventListener("change", function () {
        try {
          window.localStorage.setItem(key + ":" + index, box.checked ? "1" : "0");
        } catch (error) {
          // 本地文件或隐私模式下可能无法写入，忽略即可。
        }
        updateProgress();
      });
    });

    updateProgress();
  });

  function copyText(text) {
    if (navigator.clipboard && window.isSecureContext) {
      return navigator.clipboard.writeText(text);
    }

    return new Promise(function (resolve, reject) {
      var area = document.createElement("textarea");
      area.value = text;
      area.setAttribute("readonly", "");
      area.style.position = "fixed";
      area.style.opacity = "0";
      document.body.appendChild(area);
      area.select();

      try {
        document.execCommand("copy");
        resolve();
      } catch (error) {
        reject(error);
      } finally {
        document.body.removeChild(area);
      }
    });
  }

  document.querySelectorAll(".copy").forEach(function (button) {
    button.addEventListener("click", function () {
      var code = button.closest(".code").querySelector("code");
      if (!code) {
        return;
      }

      copyText(code.innerText).then(function () {
        var original = button.textContent;
        button.textContent = "已复制";
        window.setTimeout(function () {
          button.textContent = original;
        }, 1400);
      }).catch(function () {
        button.textContent = "复制失败，请手动选择";
      });
    });
  });
})();
