(function () {
  "use strict";

  document.querySelectorAll(".feedback-demo").forEach(function (root) {
    var flash = root.querySelector(".feedback-flash");
    var caption = root.querySelector(".feedback-caption");

    if (!flash) {
      return;
    }

    root.querySelectorAll("[data-feedback]").forEach(function (button) {
      button.addEventListener("click", function () {
        var kind = button.getAttribute("data-feedback");
        flash.classList.remove("damage", "pickup");
        void flash.offsetWidth;
        flash.classList.add(kind);

        if (caption) {
          caption.textContent =
            kind === "damage" ? "DAMAGE · red flash" : "PICKUP · gold flash";
        }
      });
    });
  });
})();
