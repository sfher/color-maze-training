(function () {
  "use strict";

  document.querySelectorAll(".economy-demo").forEach(function (demo) {
    var cost = 3;
    var startCoins = 6;
    var maxPurchases = 2;
    var defaultMessage = "购买一次，再让余额不足或升级满级。";
    var coins = startCoins;
    var oxygen = 3;
    var purchases = 0;

    var balance = demo.querySelector("[data-economy-coins]");
    var capacity = demo.querySelector("[data-economy-oxygen]");
    var button = demo.querySelector("[data-economy-buy]");
    var status = demo.querySelector("[data-economy-status]");
    var reset = demo.querySelector("[data-economy-reset]");

    function render(message) {
      balance.textContent = String(coins);
      capacity.textContent = String(oxygen);
      button.disabled = purchases >= maxPurchases;
      button.textContent = purchases >= maxPurchases
        ? "氧气升级已满"
        : "氧气上限 +1 · " + cost + " 金币";
      status.textContent = message || defaultMessage;

      if (purchases >= maxPurchases) {
        button.textContent = "氧气升级已满";
      } else if (coins < cost) {
        button.textContent = "金币不足 · 还差 " + (cost - coins);
      } else {
        button.textContent = "氧气上限 +1 · " + cost + " 金币";
      }
    }

    button.addEventListener("click", function () {
      if (purchases >= maxPurchases) {
        render("升级已满；金币不会被扣除。");
        return;
      }

      if (coins < cost) {
        render("金币不足；这一次没有修改任何数值。");
        return;
      }

      coins -= cost;
      oxygen += 1;
      purchases += 1;
      render("购买成功：先校验，再一次性扣款和升级。");
    });

    reset.addEventListener("click", function () {
      coins = startCoins;
      oxygen = 3;
      purchases = 0;
      render();
    });

    render();
  });
})();
